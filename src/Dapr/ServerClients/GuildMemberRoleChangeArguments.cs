// <copyright file="GuildMemberRoleChangeArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a guild member role change.
/// </summary>
/// <param name="GuildId">The guild identifier.</param>
/// <param name="CharacterId">The character identifier.</param>
/// <param name="NewRole">The new role.</param>
public record GuildMemberRoleChangeArguments(uint GuildId, Guid CharacterId, GuildPosition NewRole);