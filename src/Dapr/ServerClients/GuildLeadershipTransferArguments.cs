// <copyright file="GuildLeadershipTransferArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a guild leadership transfer.
/// </summary>
public record GuildLeadershipTransferArguments(uint GuildId, string CharacterName);
