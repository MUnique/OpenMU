// <copyright file="GuildStatistics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// Statistics about the guilds, e.g. for the dashboard of the admin panel.
/// </summary>
/// <param name="GuildCount">The number of guilds.</param>
/// <param name="AllianceCount">The number of alliances, i.e. the guilds which are the master of an alliance.</param>
public sealed record GuildStatistics(int GuildCount, int AllianceCount);
