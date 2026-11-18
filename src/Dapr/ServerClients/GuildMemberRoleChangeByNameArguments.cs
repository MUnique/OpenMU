// <copyright file="GuildMemberRoleChangeByNameArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a guild member role change by character name.
/// </summary>
/// <param name="GuildId">The guild identifier.</param>
/// <param name="CharacterName">The character name.</param>
/// <param name="NewRole">The new role.</param>
/// <param name="MasterTotalLevel">The master total level.</param>
public record GuildMemberRoleChangeByNameArguments(uint GuildId, string CharacterName, GuildPosition NewRole, int MasterTotalLevel);
