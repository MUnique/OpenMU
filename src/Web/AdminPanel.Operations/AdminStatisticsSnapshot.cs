// <copyright file="AdminStatisticsSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.Persistence;

/// <summary>Dashboard statistics shared by the admin panel and agents.</summary>
/// <param name="Summary">The scope of the statistics.</param>
/// <param name="NewAccountDays">The number of days used for new accounts.</param>
/// <param name="Accounts">The account and character counts.</param>
/// <param name="Guilds">The guild and alliance counts.</param>
/// <param name="OnlinePlayerHistory">The available online player samples.</param>
public sealed record AdminStatisticsSnapshot(string Summary, int NewAccountDays, AccountStatistics Accounts, GuildStatistics Guilds, IReadOnlyList<PlayerCountSample> OnlinePlayerHistory);
