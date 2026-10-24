// <copyright file="DiscordHostedChannelPermissionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using global::Discord;
using MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// Tests the permission overwrites of the users in the hosted channel of a guild.
/// </summary>
[TestFixture]
public class DiscordHostedChannelPermissionsTest
{
    private const ulong Master = 1;
    private const ulong Member = 2;
    private const ulong Kicked = 3;
    private const ulong NewMember = 4;

    /// <summary>
    /// Tests that kicked members are removed, new members are added, and the guild master can moderate.
    /// </summary>
    [Test]
    public void MembersAndGuildMasterGetTheirPermissions()
    {
        var current = new Dictionary<ulong, OverwritePermissions>
        {
            [Master] = DiscordHostedChannelPermissions.ModeratorPermissions,
            [Member] = DiscordHostedChannelPermissions.MemberPermissions,
            [Kicked] = DiscordHostedChannelPermissions.MemberPermissions,
        };

        var permissions = DiscordHostedChannelPermissions.Create(current, new HashSet<ulong> { Master, Member, NewMember }, Master, 100);

        Assert.That(permissions.Removals, Is.EqualTo(new[] { Kicked }));
        Assert.That(permissions.Changes, Is.EqualTo(new[] { (NewMember, DiscordHostedChannelPermissions.MemberPermissions) }));
        Assert.That(permissions.NotFitting, Is.Empty);
        Assert.That(DiscordHostedChannelPermissions.ModeratorPermissions.ManageMessages, Is.EqualTo(PermValue.Allow));
        Assert.That(DiscordHostedChannelPermissions.MemberPermissions.ManageMessages, Is.EqualTo(PermValue.Inherit));
    }

    /// <summary>
    /// Tests that the moderation follows a change of the guild master, and that the previous one stays a member.
    /// </summary>
    [Test]
    public void ModerationFollowsTheGuildMaster()
    {
        var current = new Dictionary<ulong, OverwritePermissions>
        {
            [Master] = DiscordHostedChannelPermissions.ModeratorPermissions,
            [Member] = DiscordHostedChannelPermissions.MemberPermissions,
        };

        var permissions = DiscordHostedChannelPermissions.Create(current, new HashSet<ulong> { Master, Member }, Member, 100);

        Assert.That(permissions.Removals, Is.Empty);
        Assert.That(permissions.Changes, Is.EquivalentTo(new[]
        {
            (Master, DiscordHostedChannelPermissions.MemberPermissions),
            (Member, DiscordHostedChannelPermissions.ModeratorPermissions),
        }));

        var withoutMaster = DiscordHostedChannelPermissions.Create(current, new HashSet<ulong> { Master, Member }, 0, 100);
        Assert.That(withoutMaster.Changes, Is.EqualTo(new[] { (Master, DiscordHostedChannelPermissions.MemberPermissions) }), "an unlinked guild master gives no one the moderation");
    }

    /// <summary>
    /// Tests that overwrites which were changed by hand, e.g. a denied permission, are set back.
    /// </summary>
    [Test]
    public void ChangedOverwritesAreSetBack()
    {
        var current = new Dictionary<ulong, OverwritePermissions>
        {
            [Member] = new(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny),
        };

        var permissions = DiscordHostedChannelPermissions.Create(current, new HashSet<ulong> { Member }, 0, 100);

        Assert.That(permissions.Changes, Is.EqualTo(new[] { (Member, DiscordHostedChannelPermissions.MemberPermissions) }));
    }

    /// <summary>
    /// Tests that a full channel adds the guild master first, and reports the members who don't fit.
    /// </summary>
    [Test]
    public void FullChannelAddsTheGuildMasterFirst()
    {
        var current = new Dictionary<ulong, OverwritePermissions> { [Member] = DiscordHostedChannelPermissions.MemberPermissions };

        var permissions = DiscordHostedChannelPermissions.Create(current, new HashSet<ulong> { Member, NewMember, 9 }, 9, 2);

        Assert.That(permissions.Changes, Is.EqualTo(new[] { (9UL, DiscordHostedChannelPermissions.ModeratorPermissions) }));
        Assert.That(permissions.NotFitting, Is.EqualTo(new[] { NewMember }));
    }
}
