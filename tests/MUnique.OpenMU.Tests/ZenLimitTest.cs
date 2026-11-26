// <copyright file="ZenLimitTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// Tests that receiving zen can't exceed the money limit, or wrap the money to a negative value.
/// </summary>
[TestFixture]
internal class ZenLimitTest
{
    private const int Almost = int.MaxValue - 1_000;

    /// <summary>
    /// Tests that a trade is cancelled, and nobody loses or gains zen, when one side would exceed the limit.
    /// </summary>
    /// <param name="requesterIsFull">If <c>true</c>, the requester is the one near the limit; otherwise the accepter.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask TradeWhichExceedsTheLimitIsCancelledAsync(bool requesterIsFull)
    {
        var (requester, accepter) = await OpenTradeAsync(requesterIsFull ? Almost : 20_000_000, requesterIsFull ? 20_000_000 : Almost).ConfigureAwait(false);
        var receiver = requesterIsFull ? requester : accepter;
        var giver = requesterIsFull ? accepter : requester;
        await new TradeMoneyAction().TradeMoneyAsync(giver, 3_000_000).ConfigureAwait(false);

        await FinishAsync(requester, accepter).ConfigureAwait(false);

        Assert.That(receiver.Money, Is.EqualTo(Almost));
        Assert.That(giver.Money, Is.EqualTo(20_000_000));
        Assert.That(requester.TradingMoney, Is.Zero);
        Assert.That(accepter.TradingMoney, Is.Zero);
        Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

    /// <summary>
    /// Tests that money which was gained after the zen was put into the trade is considered, too.
    /// </summary>
    [Test]
    public async ValueTask MoneyGainedDuringTheTradeIsConsideredAsync()
    {
        var (requester, accepter) = await OpenTradeAsync().ConfigureAwait(false);
        await new TradeMoneyAction().TradeMoneyAsync(requester, 3_000_000).ConfigureAwait(false);
        accepter.Money = Almost;

        await FinishAsync(requester, accepter).ConfigureAwait(false);

        Assert.That(accepter.Money, Is.GreaterThan(0));
        Assert.That(requester.Money, Is.EqualTo(20_000_000));
    }

    /// <summary>
    /// Tests that a trade which reaches exactly the limit still works.
    /// </summary>
    [Test]
    public async ValueTask TradeUpToTheLimitSucceedsAsync()
    {
        var (requester, accepter) = await OpenTradeAsync().ConfigureAwait(false);
        accepter.Money = int.MaxValue - 3_000_000;
        await new TradeMoneyAction().TradeMoneyAsync(requester, 3_000_000).ConfigureAwait(false);

        await FinishAsync(requester, accepter).ConfigureAwait(false);

        Assert.That(accepter.Money, Is.EqualTo(int.MaxValue));
        Assert.That(requester.Money, Is.EqualTo(17_000_000));
    }

    /// <summary>
    /// Tests that the amount of the trade window isn't lost, when it can't be given back to change it.
    /// </summary>
    [Test]
    public async ValueTask ChangingTheAmountKeepsTheWindowWhenItDoesNotFitBackAsync()
    {
        var (requester, _) = await OpenTradeAsync().ConfigureAwait(false);
        var action = new TradeMoneyAction();
        await action.TradeMoneyAsync(requester, 3_000_000).ConfigureAwait(false);
        requester.Money = Almost;

        await action.TradeMoneyAsync(requester, 1_000).ConfigureAwait(false);

        Assert.That(requester.TradingMoney, Is.EqualTo(3_000_000));
        Assert.That(requester.Money, Is.EqualTo(Almost));
    }

    /// <summary>
    /// Tests the common way of adding money (npc sale, quest, event reward, siege tribute, pickup).
    /// </summary>
    /// <param name="limit">The configured money limit.</param>
    [TestCase(int.MaxValue)]
    [TestCase(2_000_000_000)]
    public async ValueTask AddingMoneyNeverExceedsTheLimitAsync(int limit)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = limit;
        player.Money = limit - 10;

        Assert.That(player.TryAddMoney(11), Is.False);
        Assert.That(player.TryAddMoney(int.MaxValue), Is.False);
        Assert.That(player.Money, Is.EqualTo(limit - 10));
        Assert.That(player.TryAddMoney(10), Is.True);
        Assert.That(player.Money, Is.EqualTo(limit));
    }

    /// <summary>
    /// Tests that a sale in a player store is refused, when the seller can't take the price.
    /// </summary>
    [Test]
    public async ValueTask PlayerStoreSaleNeverExceedsTheLimitAsync()
    {
        var (buyer, seller) = await OpenTradeAsync().ConfigureAwait(false);
        seller.Money = Almost;
        var purchase = new ShopPurchase(buyer, seller, new ShopPrice(3_000_000), buyer.PersistenceContext);

        var result = await new ZenShopCurrencyExchange().TryPayAsync(purchase).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(ShopPaymentResult.SellerOverflow));
        Assert.That(seller.Money, Is.EqualTo(Almost));
        Assert.That(buyer.Money, Is.EqualTo(20_000_000));
    }

    private static async ValueTask<(Player Requester, Player Accepter)> OpenTradeAsync(int requesterMoney = 20_000_000, int accepterMoney = 20_000_000)
    {
        var requester = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var accepter = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        requester.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        accepter.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        requester.Money = requesterMoney;
        accepter.Money = accepterMoney;
        await new TradeRequestAction().RequestTradeAsync(requester, accepter).ConfigureAwait(false);
        await new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true).ConfigureAwait(false);
        return (requester, accepter);
    }

    private static async ValueTask FinishAsync(Player requester, Player accepter)
    {
        var handler = new TradeButtonAction();
        await handler.TradeButtonChangedAsync(requester, TradeButtonState.Checked).ConfigureAwait(false);
        await handler.TradeButtonChangedAsync(accepter, TradeButtonState.Checked).ConfigureAwait(false);
    }
}
