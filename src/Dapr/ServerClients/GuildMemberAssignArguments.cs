// <copyright file="GuildMemberAssignArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.ServerClients;

public record GuildMemberAssignArguments(string CharacterName, GuildMemberStatus MemberStatus);