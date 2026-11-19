// <copyright file="LogoutDuringTradeFinishTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// A player logs out while its trade partner completes their trade. Either the trade completes or it's cancelled,
/// but the logout never restores the inventory backup or the offered money while the completion moves them, so every
/// item exists exactly once and the money is conserved.
/// </summary>
[TestFixture]
public class LogoutDuringTradeFinishTests
{
    /// <summary>
    /// Both players offered an item and money, the owner pressed its trade button. Then the owner logs out and the
    /// peer presses its button. The completion is held at its button-state view to the owner (after it moved the
    /// items and money, before it ends the trade states), and the owner's logout either passed its map removal before
    /// (<paramref name="logoutFirst"/>) or starts while the completion is held.
    /// </summary>
    /// <param name="logoutFirst">Whether the logout passes its map removal before the peer presses its button.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task LogoutDuringTradeFinishConservesItemsAndMoneyAsync(bool logoutFirst)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? peer = null;
        var removalRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finishRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var logout = Task.CompletedTask;
        var press = Task.CompletedTask;
        var order = new List<string>();
        Exception? finishError = null;
        try
        {
            peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var p = peer;
            owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
            var character = owner.SelectedCharacter!;
            character.Name = "FinishA";
            Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
            owner.Money = 200;
            p.Money = 300;
            var y = await AddItemAsync(owner).ConfigureAwait(false);
            var x = await AddItemAsync(p).ConfigureAwait(false);

            // The completion calls this view of the owner after it moved the items and money and before it ends the trade.
            var finishArmed = false;
            var finishEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock.Get(owner.ViewPlugIns.GetPlugIn<IChangeTradeButtonStatePlugIn>()!)
                .Setup(v => v.ChangeTradeButtonStateAsync(TradeButtonState.Checked))
                .Returns(() => finishArmed ? HoldAsync(finishEntered, finishRelease) : ValueTask.CompletedTask);

            // The logout's map removal happens after its respawn handling and before it restores the inventory.
            var removalArmed = false;
            var removalEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            owner.CurrentMap!.ObjectRemoved += async removal =>
            {
                if (ReferenceEquals(removal.Object, owner) && removalArmed)
                {
                    removalEntered.TrySetResult();
                    await removalRelease.Task.ConfigureAwait(false);
                }
            };

            Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, p)).ConfigureAwait(false), Is.True);
            await RunAsync(p, () => new TradeAcceptAction().HandleTradeAcceptAsync(p, true)).ConfigureAwait(false);
            await RunAsync(owner, async () =>
            {
                await new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                await new TradeMoneyAction().TradeMoneyAsync(owner, 60).ConfigureAwait(false);
            }).ConfigureAwait(false);
            await RunAsync(p, async () =>
            {
                await new MoveItemAction().MoveItemAsync(p, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                await new TradeMoneyAction().TradeMoneyAsync(p, 50).ConfigureAwait(false);
            }).ConfigureAwait(false);
            await RunAsync(owner, () => new TradeButtonAction().TradeButtonChangedAsync(owner, TradeButtonState.Checked)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
            Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            finishArmed = true;

            if (logoutFirst)
            {
                removalArmed = true;
                logout = RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection));
                var reached = await Task.WhenAny(removalEntered.Task, logout).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(reached, Is.SameAs(removalEntered.Task), "setup: the logout reaches its map removal");
                order.Add("logout held at map removal");
                press = RunAsync(p, () => new TradeButtonAction().TradeButtonChangedAsync(p, TradeButtonState.Checked));
                var finishHeld = await WaitFinishAsync(finishEntered, press, order).ConfigureAwait(false);
                removalRelease.TrySetResult();
                order.Add("logout released");
                if (finishHeld)
                {
                    await ObserveRestoreAsync(owner, order).ConfigureAwait(false);
                }
            }
            else
            {
                press = RunAsync(p, () => new TradeButtonAction().TradeButtonChangedAsync(p, TradeButtonState.Checked));
                var finishHeld = await WaitFinishAsync(finishEntered, press, order).ConfigureAwait(false);
                logout = RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection));
                order.Add("logout started");
                if (finishHeld)
                {
                    await ObserveRestoreAsync(owner, order).ConfigureAwait(false);
                }
            }

            finishRelease.TrySetResult();
            try
            {
                await press.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not TimeoutException)
            {
                finishError = ex;
                order.Add($"button action threw {ex.GetType().Name}: {ex.Message}");
            }

            await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            order.Add($"logout completed, owner {owner.PlayerState.CurrentState}, peer {p.PlayerState.CurrentState}");
            TestContext.Out.WriteLine(string.Join(" -> ", order));

            // The peer may still hold its half of the trade; it leaves it with its ordinary cancel.
            if (p.PlayerState.CurrentState == PlayerState.TradeOpened || p.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
            {
                await RunAsync(p, () => new TradeCancelAction().CancelTradeAsync(p)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated), string.Join(" -> ", order));
            await RunAsync(owner, () => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
            await RunAsync(owner, () => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);

            var players = new[] { owner, p };
            var completed = p.Inventory!.Items.Contains(y) && owner.Inventory!.Items.Contains(x);
            var cancelled = owner.Inventory!.Items.Contains(y) && p.Inventory.Items.Contains(x);
            TestContext.Out.WriteLine($"Counts X {Count(players, x)}, Y {Count(players, y)}; money owner {owner.Money}, peer {p.Money}; completed {completed}, cancelled {cancelled}");
            Assert.Multiple(() =>
            {
                Assert.That(finishError, Is.Null, "the completion of the trade doesn't fail");
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(Count(players, y), Is.EqualTo(1), "the owner's item exists once");
                Assert.That(Count(players, x), Is.EqualTo(1), "the peer's item exists once");
                Assert.That(owner.Money + p.Money, Is.EqualTo(500), "money is conserved");
                Assert.That(completed || cancelled, Is.True, "both items belong to one outcome of the trade");
                if (completed)
                {
                    Assert.That(owner.Money, Is.EqualTo(190));
                    Assert.That(p.Money, Is.EqualTo(310));
                }
                else if (cancelled)
                {
                    Assert.That(owner.Money, Is.EqualTo(200));
                    Assert.That(p.Money, Is.EqualTo(300));
                }
                else
                {
                    // The assertion above already reports an inconsistent item outcome.
                }

                foreach (var player in players)
                {
                    Assert.That(player.TradingPartner, Is.Null);
                    Assert.That(player.BackupInventory, Is.Null);
                    Assert.That(player.TemporaryStorage?.Items ?? Enumerable.Empty<Item>(), Is.Empty);
                }

                Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            });
        }
        finally
        {
            removalRelease.TrySetResult();
            finishRelease.TrySetResult();
            try
            {
                await Task.WhenAll(
                    press.ContinueWith(_ => { }, TaskScheduler.Default),
                    logout.ContinueWith(_ => { }, TaskScheduler.Default)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                try
                {
                    await owner.DisposeAsync().ConfigureAwait(false);
                }
                finally
                {
                    if (peer is not null)
                    {
                        await peer.DisposeAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }

    private static async ValueTask HoldAsync(TaskCompletionSource entered, TaskCompletionSource release)
    {
        entered.TrySetResult();
        await release.Task.ConfigureAwait(false);
    }

    /// <summary>Waits until the completion is held, or until the button action returned without completing.</summary>
    private static async Task<bool> WaitFinishAsync(TaskCompletionSource finishEntered, Task press, List<string> order)
    {
        var first = await Task.WhenAny(finishEntered.Task, press).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        var held = ReferenceEquals(first, finishEntered.Task);
        order.Add(held ? "completion held after moving items and money" : "button action returned without a completion");
        return held;
    }

    /// <summary>
    /// The logout restores the owner's inventory backup by clearing it; this is the actual state change which shows
    /// that the restore ran while the completion was held. If it doesn't happen within the bound, the logout waits for
    /// the completion (the run records that; it can't see where the logout waits).
    /// </summary>
    private static async Task ObserveRestoreAsync(Player owner, List<string> order)
    {
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (owner.BackupInventory is not null && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        order.Add(owner.BackupInventory is null
            ? "logout restored the backup while the completion was held"
            : "logout didn't restore the backup within 2 s while the completion was held");
    }

    private static int Count(IEnumerable<Player> players, Item item)
        => players.Sum(p => (p.Inventory?.Items.Count(i => ReferenceEquals(i, item)) ?? 0)
            + (p.TemporaryStorage?.Items.Count(i => ReferenceEquals(i, item)) ?? 0));

    private static async Task<Item> AddItemAsync(Player owner)
    {
        var item = owner.PersistenceContext.CreateNew<Item>();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await owner.Inventory!.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
        return item;
    }

    private static Task RunAsync(Player player, Func<ValueTask> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Task<T> RunAsync<T>(Player player, Func<ValueTask<T>> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));
}
