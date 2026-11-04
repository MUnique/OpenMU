// <copyright file="DiscordRoleState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The state of a role of the layout on a Discord server.
/// </summary>
public enum DiscordRoleState
{
    /// <summary>
    /// The bot can give the role to users.
    /// </summary>
    Assignable,

    /// <summary>
    /// The role doesn't exist on the Discord server.
    /// </summary>
    Missing,

    /// <summary>
    /// The role is above the highest role of the bot in the role list, so the bot can't give it to users.
    /// </summary>
    AboveBot,
}
