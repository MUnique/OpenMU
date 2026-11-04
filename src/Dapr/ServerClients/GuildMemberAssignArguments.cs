// <copyright file="GuildMemberAssignArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a guild member assignment.
/// </summary>
public record GuildMemberAssignArguments(string CharacterName, GuildMemberStatus MemberStatus);