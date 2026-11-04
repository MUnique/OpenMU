// <copyright file="DiscordPendingRoleRevocation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A pending removal of the role of linked players.
/// </summary>
/// <param name="UserId">The identifier of the Discord user.</param>
/// <param name="GuildId">The identifier of the Discord server, in which the role is removed; <c>null</c>, if it wasn't known, then it's the current server of the bot.</param>
/// <param name="RoleName">The name of the role, which the user got; <c>null</c> for the role of linked players of the current layout.</param>
internal readonly record struct DiscordPendingRoleRevocation(ulong UserId, ulong? GuildId, string? RoleName = null);
