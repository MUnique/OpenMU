// <copyright file="DiscordRoleHierarchy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// Determines if the bot can give a role, by the order of the roles on a Discord server.
/// </summary>
/// <remarks>
/// The bot can only give roles which are below its highest role. Roles are ordered by their position;
/// roles with the same position, which Discord allows, are ordered by their identifier: the older role,
/// with the lower identifier, is the higher one.
/// </remarks>
public static class DiscordRoleHierarchy
{
    /// <summary>
    /// Gets the state of a role.
    /// </summary>
    /// <param name="roles">All roles of the Discord server.</param>
    /// <param name="botRoleIds">The identifiers of the roles of the bot.</param>
    /// <param name="roleName">The name of the role.</param>
    /// <returns>The state of the role.</returns>
    public static DiscordRoleState GetState(IReadOnlyCollection<(ulong Id, string Name, int Position)> roles, IReadOnlyCollection<ulong> botRoleIds, string? roleName)
    {
        var target = roles.FirstOrDefault(role => string.Equals(role.Name, roleName, StringComparison.OrdinalIgnoreCase));
        if (target.Name is null)
        {
            return DiscordRoleState.Missing;
        }

        var botRoles = roles.Where(role => botRoleIds.Contains(role.Id)).ToList();
        return botRoles.Any(botRole => IsHigher(botRole, target)) ? DiscordRoleState.Assignable : DiscordRoleState.AboveBot;
    }

    private static bool IsHigher((ulong Id, string Name, int Position) role, (ulong Id, string Name, int Position) other)
    {
        return role.Position > other.Position || (role.Position == other.Position && role.Id < other.Id);
    }
}
