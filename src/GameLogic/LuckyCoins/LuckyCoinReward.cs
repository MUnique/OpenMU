// <copyright file="LuckyCoinReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LuckyCoins;

/// <summary>
/// A possible reward of the lucky coin exchange at the NPC Delgado.
/// </summary>
public class LuckyCoinReward
{
    /// <summary>
    /// Gets or sets the count of the lucky coins which are exchanged for this reward.
    /// The game client offers to exchange 10, 20 or 30 coins.
    /// </summary>
    public int CoinCount { get; set; }

    /// <summary>
    /// Gets or sets the group of the reward item.
    /// </summary>
    public byte ItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the number of the reward item.
    /// </summary>
    public short ItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the level of the reward item.
    /// </summary>
    public byte ItemLevel { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.CoinCount} coins: {this.ItemGroup}/{this.ItemNumber} +{this.ItemLevel}";
    }
}
