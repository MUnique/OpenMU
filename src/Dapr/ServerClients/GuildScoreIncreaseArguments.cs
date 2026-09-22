// <copyright file="GuildScoreIncreaseArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for increasing a guild's score.
/// </summary>
public record GuildScoreIncreaseArguments(uint GuildId, int Amount);
