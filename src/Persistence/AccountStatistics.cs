// <copyright file="AccountStatistics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// Statistics about the player accounts, e.g. for the dashboard of the admin panel.
/// </summary>
/// <remarks>
/// Template and bot accounts are not included, since they are not created by players.
/// </remarks>
/// <param name="AccountCount">The number of accounts.</param>
/// <param name="NewAccountCount">The number of accounts which were registered since the requested point in time.</param>
/// <param name="BannedAccountCount">The number of accounts which are banned, permanently or temporarily.</param>
/// <param name="CharacterCount">The number of characters of the accounts.</param>
public sealed record AccountStatistics(int AccountCount, int NewAccountCount, int BannedAccountCount, int CharacterCount);
