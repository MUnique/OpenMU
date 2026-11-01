// <copyright file="CashShopCoinExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using MUnique.OpenMU.Persistence;

/// <summary>
/// Extensions for the cash shop coins of an account.
/// </summary>
public static class CashShopCoinExtensions
{
    /// <summary>
    /// Gets the balance of a coin type of the account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="coinType">The coin type.</param>
    /// <returns>The balance.</returns>
    public static int GetCashShopCoins(this Account account, CashShopCoinType coinType)
    {
        return coinType switch
        {
            CashShopCoinType.WCoinC => account.WCoinC,
            CashShopCoinType.WCoinP => account.WCoinP,
            CashShopCoinType.GoblinPoints => account.GoblinPoints,
            _ => throw new ArgumentOutOfRangeException(nameof(coinType), coinType, null),
        };
    }

    /// <summary>
    /// Sets the balance of a coin type of the account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="coinType">The coin type.</param>
    /// <param name="value">The new balance.</param>
    public static void SetCashShopCoins(this Account account, CashShopCoinType coinType, int value)
    {
        switch (coinType)
        {
            case CashShopCoinType.WCoinC:
                account.WCoinC = value;
                break;
            case CashShopCoinType.WCoinP:
                account.WCoinP = value;
                break;
            case CashShopCoinType.GoblinPoints:
                account.GoblinPoints = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(coinType), coinType, null);
        }
    }

    /// <summary>
    /// Applies the pending coin grants to the balances of the account of the player,
    /// and marks them as applied in the same save.
    /// </summary>
    /// <remarks>
    /// A negative grant takes coins, but the balance doesn't go below zero.
    /// If the save fails, the changes stay in the context of the player and are committed by its next save.
    /// </remarks>
    /// <param name="player">The player.</param>
    /// <returns>A value task which completes when the grants are applied.</returns>
    public static async ValueTask ApplyPendingCashShopCoinGrantsAsync(this Player player)
    {
        if (player.Account is not { } account)
        {
            return;
        }

        await player.RunPersistenceExclusiveAsync(async () =>
        {
            var grants = await player.PersistenceContext.GetPendingCashShopCoinGrantsAsync(account.GetId()).ConfigureAwait(false);
            if (grants.Count == 0)
            {
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var grant in grants)
            {
                if (!Enum.IsDefined(grant.CoinType))
                {
                    // It stays pending, so it doesn't block the other grants and can be corrected in the database.
                    player.Logger.LogWarning("The coin grant {grant} of player {player} has the unknown coin type {coinType}; it's skipped.", grant.GetId(), player, grant.CoinType);
                    continue;
                }

                var balance = (long)account.GetCashShopCoins(grant.CoinType) + grant.Amount;
                account.SetCashShopCoins(grant.CoinType, (int)Math.Clamp(balance, 0, int.MaxValue));
                grant.AppliedAt = now;
                player.Logger.LogInformation(
                    "Applied the grant of {amount} {coinType} to the account of player {player}, reason: {reason}.",
                    grant.Amount,
                    grant.CoinType,
                    player,
                    grant.Reason);
            }

            await player.SaveProgressAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
