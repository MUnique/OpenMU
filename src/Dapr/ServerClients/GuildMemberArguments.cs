// <copyright file="GuildMemberArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a guild member action.
/// </summary>
public record GuildMemberArguments(uint GuildId, string PlayerName);