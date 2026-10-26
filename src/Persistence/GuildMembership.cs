// <copyright file="GuildMembership.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The membership of a character in a guild.
/// </summary>
/// <param name="CharacterId">The identifier of the character.</param>
/// <param name="GuildId">The identifier of the guild.</param>
/// <param name="Position">The position of the character in the guild.</param>
/// <param name="AllianceMasterGuildId">The identifier of the master guild of the alliance of the guild, if it's a member of an alliance.</param>
public sealed record GuildMembership(Guid CharacterId, Guid GuildId, GuildPosition Position, Guid? AllianceMasterGuildId);
