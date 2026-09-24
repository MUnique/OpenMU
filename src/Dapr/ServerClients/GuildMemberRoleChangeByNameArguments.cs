// <copyright file="GuildMemberRoleChangeByNameArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a guild member role change by character name.
/// </summary>
public record GuildMemberRoleChangeByNameArguments(uint GuildId, string CharacterName, GuildPosition NewRole);
