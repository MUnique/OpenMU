// <copyright file="DiscordHostedChannelPermissions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using global::Discord;

/// <summary>
/// The changes of the permission overwrites of the users in a hosted channel of a guild or alliance:
/// The linked members can see and write, and the linked guild master can also moderate the messages, only in this channel.
/// </summary>
/// <param name="Removals">The users whose overwrites are removed, because they aren't linked members anymore, e.g. after a kick.</param>
/// <param name="Changes">The users whose overwrites are added or changed, e.g. for a new member or a new guild master.</param>
/// <param name="NotFitting">The linked members who don't get an overwrite, because the channel has no free overwrite left.</param>
internal sealed record DiscordHostedChannelPermissions(
    IReadOnlyList<ulong> Removals,
    IReadOnlyList<(ulong UserId, OverwritePermissions Permissions)> Changes,
    IReadOnlyList<ulong> NotFitting)
{
    /// <summary>
    /// The permissions of the linked members.
    /// </summary>
    internal static readonly OverwritePermissions MemberPermissions = new(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, readMessageHistory: PermValue.Allow);

    /// <summary>
    /// The permissions of the linked guild master.
    /// </summary>
    internal static readonly OverwritePermissions ModeratorPermissions = new(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, readMessageHistory: PermValue.Allow, manageMessages: PermValue.Allow);

    /// <summary>
    /// Gets a value indicating whether the overwrites have to be changed.
    /// </summary>
    internal bool HasChanges => this.Removals.Count > 0 || this.Changes.Count > 0;

    /// <summary>
    /// Determines the changes.
    /// </summary>
    /// <param name="current">The current overwrites of the users, without the one of the bot.</param>
    /// <param name="linkedMembers">The Discord users who are linked to members of the guild or alliance.</param>
    /// <param name="moderatorId">The Discord user who is linked to the guild master; or 0, if there is none.</param>
    /// <param name="freeOverwrites">The number of overwrites which are available for the users, e.g. the maximum without the ones of roles and of the bot.</param>
    /// <returns>The changes.</returns>
    internal static DiscordHostedChannelPermissions Create(IReadOnlyDictionary<ulong, OverwritePermissions> current, IReadOnlySet<ulong> linkedMembers, ulong moderatorId, int freeOverwrites)
    {
        var removals = current.Keys.Where(userId => !linkedMembers.Contains(userId)).Order().ToList();
        var remaining = current.Where(overwrite => linkedMembers.Contains(overwrite.Key)).ToList();
        var changes = remaining
            .Where(overwrite => !IsEqual(overwrite.Value, GetPermissions(overwrite.Key, moderatorId)))
            .Select(overwrite => (overwrite.Key, GetPermissions(overwrite.Key, moderatorId)))
            .ToList();

        // The guild master first, so that a full channel still gets its moderator.
        var missing = linkedMembers.Where(userId => !current.ContainsKey(userId)).OrderBy(userId => userId != moderatorId).ThenBy(userId => userId).ToList();
        var free = Math.Max(freeOverwrites - remaining.Count, 0);
        changes.AddRange(missing.Take(free).Select(userId => (userId, GetPermissions(userId, moderatorId))));
        return new DiscordHostedChannelPermissions(removals, changes, missing.Skip(free).ToList());
    }

    /// <summary>
    /// Creates all overwrites of the channel after the changes. They replace the current ones in one request,
    /// so that a failure changes nothing, removals don't need to look up the users, and the limit of overwrites holds.
    /// </summary>
    /// <param name="current">The current overwrites of the channel, also of the roles and of the bot.</param>
    /// <param name="botUserId">The identifier of the bot user, whose overwrite is kept.</param>
    /// <param name="addableUserIds">The users who don't have an overwrite yet and can get one, because they are members of the Discord server.</param>
    /// <returns>The overwrites.</returns>
    internal IReadOnlyList<Overwrite> Apply(IEnumerable<Overwrite> current, ulong botUserId, IReadOnlySet<ulong> addableUserIds)
    {
        var removals = this.Removals.ToHashSet();
        var changes = this.Changes.ToDictionary(change => change.UserId, change => change.Permissions);
        var result = new List<Overwrite>();
        foreach (var overwrite in current)
        {
            if (overwrite.TargetType != PermissionTarget.User || overwrite.TargetId == botUserId)
            {
                result.Add(overwrite);
            }
            else if (!removals.Contains(overwrite.TargetId))
            {
                result.Add(changes.Remove(overwrite.TargetId, out var changed) ? new Overwrite(overwrite.TargetId, PermissionTarget.User, changed) : overwrite);
            }
            else
            {
                // It's removed.
            }
        }

        // The remaining changes are the new users.
        result.AddRange(changes.Where(change => addableUserIds.Contains(change.Key)).Select(change => new Overwrite(change.Key, PermissionTarget.User, change.Value)));
        return result;
    }

    private static OverwritePermissions GetPermissions(ulong userId, ulong moderatorId) => userId == moderatorId ? ModeratorPermissions : MemberPermissions;

    private static bool IsEqual(OverwritePermissions left, OverwritePermissions right) => left.AllowValue == right.AllowValue && left.DenyValue == right.DenyValue;
}
