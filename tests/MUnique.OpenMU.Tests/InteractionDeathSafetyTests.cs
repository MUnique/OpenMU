// <copyright file="InteractionDeathSafetyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A death closes the trade, party and NPC interactions of the dying player, so that it reaches the dead state and
/// respawns once. Closing them keeps every item and the money where it belongs.
/// </summary>
[TestFixture]
public class InteractionDeathSafetyTests
{
    /// <summary>
    /// Items which a closed crafting dialog left in the temporary storage (because inventory and vault are full)
    /// are not cleared when a death cancels a pending trade request.
    /// </summary>
    /// <param name="ownerRequests">Whether the dying player is the requester of the trade.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeathCancellingTradeRequestKeepsCraftingLeftoversAsync(bool ownerRequests)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        try
        {
            owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
            owner.Attributes![Stats.CurrentHealth] = 10;
            owner.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = owner.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
            var inventory = owner.Inventory!;
            var temporary = owner.TemporaryStorage!;
            var definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true, Durability = 9 };
            var ingredient = owner.PersistenceContext.CreateNew<Item>();
            ingredient.Definition = definition;
            ingredient.Level = 3;
            ingredient.Durability = 7;
            var deaths = 0;
            owner.Died += (_, _) => Interlocked.Increment(ref deaths);
            var npc = CreateCraftingNpc(owner);
            await owner.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(owner, npc)).ConfigureAwait(false);
            Assert.That(await inventory.AddItemAsync(20, ingredient).ConfigureAwait(false), Is.True);
            await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.ChaosMachine)).ConfigureAwait(false);
            await FillAsync(owner, inventory, definition).ConfigureAwait(false);
            await FillAsync(owner, new Storage(InventoryConstants.WarehouseSize, SetUpVault(owner)), definition).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(owner)).ConfigureAwait(false);
            Assert.That(temporary.Items, Is.EquivalentTo(new[] { ingredient }), "Inventory and vault are full, so the leftover stays in the tray.");
            owner.Money = 1234;
            var ids = inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order().ToArray();

            var requested = ownerRequests
                ? await owner.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(owner, peer)).ConfigureAwait(false)
                : await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false);
            Assert.That(requested, Is.True);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));

            await peer.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order(), Is.EqualTo(ids));
                Assert.That(temporary.Items, Does.Contain(ingredient));
                Assert.That(ingredient.Level, Is.EqualTo(3));
                Assert.That(ingredient.Durability, Is.EqualTo(7));
                Assert.That(owner.Money, Is.EqualTo(1234));
            });

            await AssertTimerRespawnsOnceAndCanBeHitAsync(owner, peer).ConfigureAwait(false);
            Assert.That(deaths, Is.EqualTo(1));
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A death in an open trade restores the offered items and money of both traders exactly once, without
    /// duplicating them, and the dying player respawns once.
    /// </summary>
    /// <param name="buttonPressed">Whether the dying player already pressed the trade button.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeathInOpenTradeRestoresBothTradersAsync(bool buttonPressed)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        try
        {
            owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
            owner.Attributes![Stats.CurrentHealth] = 10;
            owner.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = owner.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
            var definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true, Durability = 9 };
            var ownerItem = CreateItem(owner, definition);
            var peerItem = CreateItem(peer, definition);
            Assert.That(await owner.Inventory!.AddItemAsync(20, ownerItem).ConfigureAwait(false), Is.True);
            Assert.That(await peer.Inventory!.AddItemAsync(20, peerItem).ConfigureAwait(false), Is.True);
            owner.Money = 1000;
            peer.Money = 2000;
            var deaths = 0;
            owner.Died += (_, _) => Interlocked.Increment(ref deaths);
            Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false), Is.True);
            await owner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(owner, true)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
            await peer.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(peer, 20, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(owner, 60)).ConfigureAwait(false);
            await peer.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(peer, 70)).ConfigureAwait(false);
            Assert.That(owner.TemporaryStorage!.Items, Does.Contain(ownerItem));
            if (buttonPressed)
            {
                await owner.RunPersistenceExclusiveAsync(() => new TradeButtonAction().TradeButtonChangedAsync(owner, TradeButtonState.Checked)).ConfigureAwait(false);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
            }

            await peer.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
                Assert.That(owner.TemporaryStorage!.Items, Is.Empty);
                Assert.That(peer.TemporaryStorage!.Items, Is.Empty);
                Assert.That(owner.Inventory.Items.Count(i => i.GetId() == ownerItem.GetId()), Is.EqualTo(1));
                Assert.That(peer.Inventory.Items.Count(i => i.GetId() == peerItem.GetId()), Is.EqualTo(1));
                Assert.That(owner.Inventory.Items.Any(i => i.GetId() == peerItem.GetId()), Is.False);
                Assert.That(peer.Inventory.Items.Any(i => i.GetId() == ownerItem.GetId()), Is.False);
                Assert.That(owner.Money, Is.EqualTo(1000));
                Assert.That(peer.Money, Is.EqualTo(2000));
                Assert.That(owner.TradingMoney, Is.Zero);
                Assert.That(peer.TradingMoney, Is.Zero);
            });

            await AssertTimerRespawnsOnceAndCanBeHitAsync(owner, peer).ConfigureAwait(false);
            Assert.That(deaths, Is.EqualTo(1));
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// While a lethal hit is reserved but its death not yet entered, repeated trade, party and NPC requests are
    /// refused, so that the death closes a bounded set of interactions, reaches the dead state and respawns once.
    /// </summary>
    [Test]
    public async Task RequestsDuringReservedDeathAreRefusedAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? attacker = null;
        Player? peer = null;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var lethal = Task.CompletedTask;
        try
        {
            attacker = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            owner.Attributes![Stats.CurrentHealth] = 10;
            owner.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = owner.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
            var npc = CreateCraftingNpc(owner);
            var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var holds = 0;
            Mock.Get(owner.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Setup(v => v.ShowHitAsync(owner, It.IsAny<HitInfo>())).Returns(async () =>
            {
                if (Interlocked.Increment(ref holds) == 1)
                {
                    entered.TrySetResult();
                    await release.Task.ConfigureAwait(false);
                }
            });
            var deaths = 0;
            owner.Died += (_, _) => Interlocked.Increment(ref deaths);

            // The attacker holds only its own packet funnel; the requests use the funnels of the peer and the owner.
            var lethalAttacker = attacker;
            lethal = Task.Run(async () => await lethalAttacker.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(lethalAttacker, 10)).ConfigureAwait(false));
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(owner.IsAlive, Is.False);
            var requestPeer = peer;
            for (var attempt = 0; attempt < 8; attempt++)
            {
                Assert.That(await requestPeer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(requestPeer, owner)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.False);
                Assert.That(await owner.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(owner, requestPeer)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.False);
                await requestPeer.RunPersistenceExclusiveAsync(() => new PartyRequestAction().HandlePartyRequestAsync(requestPeer, owner)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await owner.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(owner, npc)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(requestPeer.TradingPartner, Is.Null);
                Assert.That(owner.LastPartyRequester, Is.Null);
                Assert.That(owner.OpenedNpc, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(requestPeer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            });

            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(deaths, Is.EqualTo(1));
            });
            await AssertTimerRespawnsOnceAndCanBeHitAsync(owner, requestPeer).ConfigureAwait(false);
            Assert.That(deaths, Is.EqualTo(1));
        }
        finally
        {
            release.TrySetResult();
            try
            {
                await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                await owner.DisposeAsync().ConfigureAwait(false);
                if (attacker is not null)
                {
                    await attacker.DisposeAsync().ConfigureAwait(false);
                }

                if (peer is not null)
                {
                    await peer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// A player who logs out while dead selects a new life: the timer of the old death doesn't respawn it, and the
    /// new life takes damage. A character which was saved with zero health enters alive and accepts one lethal hit.
    /// </summary>
    [Test]
    public async Task LogoutFromDeathSelectsHittableLifeAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        try
        {
            var character = owner.SelectedCharacter!;
            character.Name = "Victim";
            Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
            owner.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = owner.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
            owner.Attributes![Stats.CurrentHealth] = 10;
            var deaths = 0;
            owner.Died += (_, _) => Interlocked.Increment(ref deaths);
            var respawnView = Mock.Get(owner.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!);
            int Respawns() => respawnView.Invocations.Count(i => i.Method.Name == nameof(IRespawnAfterDeathPlugIn.RespawnAsync));
            await peer.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            var oldDeath = owner.LastDeath;
            await owner.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.IsAlive, Is.True);
            });

            // The old death's timer (three seconds) is abandoned with the logout; it must not respawn the new life.
            await Task.Delay(TimeSpan.FromSeconds(4)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(Respawns(), Is.Zero);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.IsAlive, Is.True);
            });

            var health = owner.Attributes![Stats.CurrentHealth];
            TestContext.Out.WriteLine($"Health of the selected life: {health}");
            await peer.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(peer, 1)).ConfigureAwait(false);
            if (health > 1)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(owner.Attributes[Stats.CurrentHealth], Is.EqualTo(health - 1));
                    Assert.That(owner.IsAlive, Is.True);
                    Assert.That(deaths, Is.EqualTo(1));
                });
            }
            else
            {
                // Zero or one health left: the hit is accepted and owns exactly one new death.
                Assert.Multiple(() =>
                {
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                    Assert.That(owner.LastDeath, Is.Not.Null.And.Not.SameAs(oldDeath));
                    Assert.That(deaths, Is.EqualTo(2));
                });
                await AssertTimerRespawnsOnceAndCanBeHitAsync(owner, peer).ConfigureAwait(false);
                Assert.That(deaths, Is.EqualTo(2));
            }
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Waits for the original respawn timer of the death, which revives the player through exactly one respawn,
    /// and checks that the new life takes damage again.
    /// </summary>
    private static async Task AssertTimerRespawnsOnceAndCanBeHitAsync(Player owner, Player attacker)
    {
        var respawnView = Mock.Get(owner.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!);
        int Respawns() => respawnView.Invocations.Count(i => i.Method.Name == nameof(IRespawnAfterDeathPlugIn.RespawnAsync));
        Assert.That(Respawns(), Is.Zero, "The respawn follows the timer of the death.");

        // The normal timer respawns after three seconds; the bound leaves room for a loaded host.
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!(owner.IsAlive && owner.PlayerState.CurrentState == PlayerState.EnteredWorld) && DateTime.UtcNow < deadline)
        {
            await Task.Delay(25).ConfigureAwait(false);
        }

        Assert.Multiple(() =>
        {
            Assert.That(owner.IsAlive, Is.True);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(Respawns(), Is.EqualTo(1));
        });
        var health = owner.Attributes![Stats.CurrentHealth];
        Assert.That(health, Is.GreaterThan(1));
        await attacker.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(attacker, 1)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(owner.Attributes[Stats.CurrentHealth], Is.EqualTo(health - 1));
            Assert.That(Respawns(), Is.EqualTo(1));
        });
    }

    private static Item CreateItem(Player owner, ItemDefinition definition)
    {
        var item = owner.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Durability = 7;
        return item;
    }

    private static async ValueTask FillAsync(Player owner, IStorage storage, ItemDefinition definition)
    {
        while (await storage.AddItemAsync(CreateItem(owner, definition)).ConfigureAwait(false))
        {
            // Adds items until there is no space left.
        }
    }

    private static ItemStorage SetUpVault(Player player)
    {
        var vaultMock = new Mock<ItemStorage>();
        vaultMock.SetupAllProperties();
        vaultMock.Setup(v => v.Items).Returns(new List<Item>());
        var vault = vaultMock.Object;
        Mock.Get(player.Account!).SetupProperty(account => account.Vault, vault);
        return vault;
    }

    private static NonPlayerCharacter CreateCraftingNpc(Player owner)
    {
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        var crafting = new ItemCrafting { SimpleCraftingSettings = owner.PersistenceContext.CreateNew<SimpleCraftingSettings>() };
        definition.Setup(d => d.ItemCraftings).Returns(new List<ItemCrafting> { crafting });
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        definition.Object.Number = 238;
        return new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, owner.CurrentMap!);
    }
}
