// <copyright file="DiscordRoleChange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A change of the link of a Discord user, after which the bot updates the role of linked players.
/// </summary>
/// <param name="UserId">The identifier of the Discord user.</param>
/// <param name="IsLinked">A value indicating whether the user is linked now.</param>
public sealed record DiscordRoleChange(ulong UserId, bool IsLinked);
