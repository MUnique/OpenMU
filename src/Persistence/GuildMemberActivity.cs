// <copyright file="GuildMemberActivity.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The activity of a guild member: when the account of the character was last seen in the game.
/// </summary>
/// <param name="GuildId">The identifier of the guild.</param>
/// <param name="GuildName">The name of the guild.</param>
/// <param name="CharacterId">The identifier of the character.</param>
/// <param name="CharacterName">The name of the character.</param>
/// <param name="Position">The position of the character in the guild.</param>
/// <param name="LastSeen">The time when the account of the character was last seen in the game.</param>
public sealed record GuildMemberActivity(Guid GuildId, string GuildName, Guid CharacterId, string CharacterName, GuildPosition Position, DateTime LastSeen);
