// <copyright file="MoneyDistribution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Splits a money drop between players and hands it over to them.
/// </summary>
/// <remarks>
/// This is the single place where <see cref="Stats.MoneyAmountRate"/> and
/// <see cref="DataModel.Configuration.GameConfiguration.ClampMoneyOnPickup"/> are applied, so that
/// every path - dropped on the ground, added directly, solo or in a party - treats them the same way.
/// </remarks>
internal static class MoneyDistribution
{
    /// <summary>
    /// Splits the money proportionally to the experience each player gained from the kill.
    /// </summary>
    /// <param name="amount">The total amount of money to split.</param>
    /// <param name="experienceShares">The experience gained per player.</param>
    /// <returns>The part of the money which is reserved for each player.</returns>
    public static IReadOnlyList<MoneyShare> CreateShares(uint amount, IReadOnlyList<ExperienceShare> experienceShares)
    {
        if (experienceShares.Count == 0)
        {
            return [];
        }

        if (experienceShares.Count == 1)
        {
            return [new MoneyShare(experienceShares[0].Player, amount)];
        }

        var weights = new long[experienceShares.Count];
        for (int i = 0; i < experienceShares.Count; i++)
        {
            weights[i] = experienceShares[i].Experience;
        }

        var parts = SplitByWeight(amount, weights);
        var shares = new MoneyShare[experienceShares.Count];
        for (int i = 0; i < experienceShares.Count; i++)
        {
            shares[i] = new MoneyShare(experienceShares[i].Player, parts[i]);
        }

        return shares;
    }

    /// <summary>
    /// Splits the money equally, which is used when no per player experience is known - for example
    /// for the fixed money amount of an item box.
    /// </summary>
    /// <param name="amount">The total amount of money to split.</param>
    /// <param name="players">The players to split it between.</param>
    /// <returns>The part of the money which is reserved for each player.</returns>
    public static IReadOnlyList<MoneyShare> CreateEqualShares(uint amount, IReadOnlyList<Player> players)
    {
        if (players.Count == 0)
        {
            return [];
        }

        var parts = SplitByWeight(amount, new long[players.Count]);
        var shares = new MoneyShare[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            shares[i] = new MoneyShare(players[i], parts[i]);
        }

        return shares;
    }

    /// <summary>
    /// Hands the shares over to the players which are still eligible. The shares of players which
    /// are not eligible anymore are re-distributed between the remaining ones.
    /// </summary>
    /// <param name="shares">The shares.</param>
    /// <param name="isEligible">Determines whether a player may still receive their share.</param>
    /// <returns><c>True</c>, if at least one player received money; Otherwise, <c>false</c>.</returns>
    public static bool TryPayShares(IReadOnlyList<MoneyShare> shares, Func<Player, bool> isEligible)
    {
        var eligible = new List<MoneyShare>(shares.Count);
        uint forfeited = 0;
        foreach (var share in shares)
        {
            if (isEligible(share.Player))
            {
                eligible.Add(share);
            }
            else
            {
                forfeited += share.Amount;
            }
        }

        if (eligible.Count == 0)
        {
            return false;
        }

        var extra = new uint[eligible.Count];
        if (forfeited > 0)
        {
            var weights = new long[eligible.Count];
            for (int i = 0; i < eligible.Count; i++)
            {
                weights[i] = eligible[i].Amount;
            }

            extra = SplitByWeight(forfeited, weights);
        }

        var received = false;
        for (int i = 0; i < eligible.Count; i++)
        {
            received |= TryPay(eligible[i].Player, eligible[i].Amount + extra[i]);
        }

        return received;
    }

    /// <summary>
    /// Adds the money to the inventory of the player, applying their <see cref="Stats.MoneyAmountRate"/>
    /// and the configured pick up clamp.
    /// </summary>
    /// <param name="player">The player which should receive the money.</param>
    /// <param name="amount">The amount, before the money rate of the player is applied.</param>
    /// <returns><c>True</c>, if the player received money; Otherwise, <c>false</c>.</returns>
    public static bool TryPay(Player player, uint amount)
    {
        if (amount == 0)
        {
            return false;
        }

        // The rate is applied in double precision: a float multiplication would round money amounts
        // above the ~16.7M the float mantissa can represent exactly, before the cast to long.
        var scaled = (long)(amount * (double)(player.Attributes?[Stats.MoneyAmountRate] ?? 1.0f));
        if (scaled <= 0)
        {
            return false;
        }

        var amountToAdd = (int)Math.Min(scaled, int.MaxValue);
        if (player.GameContext?.Configuration?.ClampMoneyOnPickup ?? false)
        {
            var maximumMoney = player.GameContext?.Configuration?.MaximumInventoryMoney ?? int.MaxValue;
            amountToAdd = (int)Math.Min(amountToAdd, Math.Max(0, maximumMoney - player.Money));
            if (amountToAdd <= 0)
            {
                return false;
            }
        }

        return player.TryAddMoney(amountToAdd);
    }

    /// <summary>
    /// Splits an amount proportionally to the given weights. When all weights are zero, it's split equally.
    /// </summary>
    private static uint[] SplitByWeight(uint amount, IReadOnlyList<long> weights)
    {
        var result = new uint[weights.Count];
        if (weights.Count == 0 || amount == 0)
        {
            return result;
        }

        long totalWeight = 0;
        foreach (var weight in weights)
        {
            totalWeight += Math.Max(0, weight);
        }

        var remainders = new long[weights.Count];
        uint distributed = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            if (totalWeight > 0)
            {
                var numerator = (long)amount * Math.Max(0, weights[i]);
                result[i] = (uint)(numerator / totalWeight);
                remainders[i] = numerator % totalWeight;
            }
            else
            {
                result[i] = amount / (uint)weights.Count;
                remainders[i] = 1;
            }

            distributed += result[i];
        }

        // The integer division loses up to one unit per share. Handing all of it to the same
        // share would systematically favour one player over many kills, so the units go to the
        // shares which were cut the most, one each (largest remainder method).
        for (var rest = amount - distributed; rest > 0; rest--)
        {
            var pick = -1;
            for (int i = 0; i < remainders.Length; i++)
            {
                if (remainders[i] > 0 && (pick < 0 || remainders[i] > remainders[pick]))
                {
                    pick = i;
                }
            }

            if (pick < 0)
            {
                break;
            }

            result[pick]++;
            remainders[pick] = 0;
        }

        return result;
    }
}
