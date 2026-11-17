// <copyright file="GuildPlayerLeftGameArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a notification about a game-leaving guild member.
/// </summary>
/// <param name="GuildId">The guild identifier.</param>
/// <param name="GuildMemberId">The guild member character identifier.</param>
/// <param name="ServerId">The server identifier.</param>
public record GuildPlayerLeftGameArguments(uint GuildId, Guid GuildMemberId, byte ServerId);