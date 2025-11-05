// <copyright file="GuildRelationshipArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for guild relationship operations (alliance or hostility).
/// </summary>
/// <param name="RequestingGuildId">The ID of the guild requesting the relationship.</param>
/// <param name="TargetGuildId">The ID of the target guild.</param>
public record GuildRelationshipArguments(uint RequestingGuildId, uint TargetGuildId);
