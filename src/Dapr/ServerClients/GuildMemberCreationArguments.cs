// <copyright file="GuildMemberCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a guild member creation.
/// </summary>
/// <param name="GuildId">The guild identifier.</param>
/// <param name="CharacterId">The character identifier.</param>
/// <param name="CharacterName">The character name.</param>
/// <param name="Role">The member role.</param>
/// <param name="ServerId">The server identifier.</param>
public record GuildMemberCreationArguments(uint GuildId, Guid CharacterId, string CharacterName, GuildPosition Role, byte ServerId);