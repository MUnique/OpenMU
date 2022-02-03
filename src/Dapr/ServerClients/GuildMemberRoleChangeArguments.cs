// <copyright file="GuildMemberRoleChangeArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.ServerClients;

public record GuildMemberRoleChangeArguments(uint GuildId, Guid CharacterId, GuildPosition NewRole);