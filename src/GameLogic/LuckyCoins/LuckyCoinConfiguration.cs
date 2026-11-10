// <copyright file="LuckyCoinConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LuckyCoins;

/// <summary>
/// The configuration of the lucky coin exchange at the NPC Delgado.
/// </summary>
public class LuckyCoinConfiguration
{
    /// <summary>
    /// Gets or sets the possible rewards of the exchange. When a player exchanges coins,
    /// one of the rewards for that count of coins is chosen randomly.
    /// </summary>
    /// <remarks>
    /// The rewards of the original game are server data which isn't known, so there are none by default.
    /// Without a reward for a count of coins, the exchange of that count isn't possible.
    /// </remarks>
    public IList<LuckyCoinReward> Rewards { get; set; } = new List<LuckyCoinReward>();
}
