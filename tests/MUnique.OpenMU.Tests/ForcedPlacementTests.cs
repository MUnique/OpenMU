// <copyright file="ForcedPlacementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>Forced placement claims map change before detachment and preserves the current character/death across awaits.</summary>
[TestFixture]
public class ForcedPlacementTests
{
    /// <summary>Both protocol branches preserve rollback, new lethal ownership and teardown while placement is pending.</summary>
    /// <param name="legacy">Whether the client completes directly with its respawn packet.</param>
    /// <param name="stage">The production asynchronous boundary being held.</param>
    [TestCase(false, "ordinary")]
    [TestCase(true, "ordinary")]
    [TestCase(false, "cleanup")]
    [TestCase(true, "cleanup")]
    [TestCase(false, "remove")]
    [TestCase(true, "remove")]
    [TestCase(false, "packet")]
    [TestCase(true, "packet")]
    [TestCase(false, "remove-teardown")]
    [TestCase(true, "remove-teardown")]
    [TestCase(false, "remove-reselect")]
    [TestCase(true, "remove-reselect")]
    [TestCase(false, "teardown")]
    [TestCase(true, "teardown")]
    [TestCase(false, "reselect")]
    [TestCase(true, "reselect")]
    [TestCase(false, "compete")]
    [TestCase(true, "compete")]
    public async Task PlacementPreservesLifeAndInteractionAsync(bool legacy, string stage)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, legacy);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        peer.Attributes![Stats.CurrentHealth] = 100;
        target.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        target.Money = 200;
        var character = target.SelectedCharacter!;
        character.Name = "Placement";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var source = target.CurrentMap!;
        var definition = new Mock<GameMapDefinition>();
        definition.SetupAllProperties();
        definition.Setup(m => m.DropItemGroups).Returns(source.Definition.DropItemGroups);
        definition.Setup(m => m.MonsterSpawns).Returns(source.Definition.MonsterSpawns);
        definition.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        definition.Object.Number = 1;
        definition.Object.TerrainData = source.Definition.TerrainData;
        target.GameContext.Configuration.Maps.Add(definition.Object);
        var destination = (await target.GameContext.GetMapAsync(1).ConfigureAwait(false))!;
        var gate = new ExitGate { Map = definition.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true };
        definition.Object.ExitGates.Add(gate);
        source.Definition.SafezoneMap = definition.Object;
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Object.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await target.Inventory!.AddItemAsync(20, item.Object).ConfigureAwait(false), Is.True);
        Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, target)).ConfigureAwait(false), Is.True);
        await target.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(target, true)).ConfigureAwait(false);
        await target.RunPersistenceExclusiveAsync(async () =>
        {
            await new MoveItemAction().MoveItemAsync(target, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
            await new TradeMoneyAction().TradeMoneyAsync(target, 60).ConfigureAwait(false);
        }).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var packets = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        async ValueTask PauseAsync(string boundary)
        {
            if (stage == boundary || (boundary == "packet" && stage is "teardown" or "reselect" or "compete")
                || (boundary == "remove" && stage is "remove-teardown" or "remove-reselect"))
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        }

        Mock.Get(target.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(() => PauseAsync("cleanup"));
        source.ObjectRemoved += async args =>
        {
            if (ReferenceEquals(args.Object, target))
            {
                await PauseAsync("remove").ConfigureAwait(false);
            }
        };
        async ValueTask PacketAsync()
        {
            Interlocked.Increment(ref packets);
            await PauseAsync("packet").ConfigureAwait(false);
        }

        if (legacy)
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(PacketAsync);
        }
        else
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }

        var placement = Task.Run(async () => await peer.RunPersistenceExclusiveAsync(() => target.RespawnAtAsync(gate)).ConfigureAwait(false));
        DeathInformation? owned = null;
        try
        {
            if (stage != "ordinary")
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                if (stage is "cleanup" or "remove")
                {
                    await Task.Run(async () => await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    owned = target.LastDeath;
                }
                else if (stage == "packet")
                {
                    // Ordinary hits reject an off-map player; a real instant hazard may still reserve this life.
                    await Task.Run(async () => await target.KillInstantlyAsync().ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    owned = target.LastDeath;
                    Assert.That(deaths, Is.Zero, "Death is deferred without making its caller wait for future Ready.");
                }
                else if (stage is "teardown" or "remove-teardown")
                {
                    await target.DisconnectAsync().ConfigureAwait(false);
                }
                else if (stage is "reselect" or "remove-reselect")
                {
                    var oldAttributes = target.Attributes;
                    await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                    await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                    await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                    Assert.That(target.Attributes, Is.Not.SameAs(oldAttributes));
                }
                else
                {
                    await target.RespawnAtAsync(new ExitGate { Map = source.Definition, X1 = 2, X2 = 2, Y1 = 2, Y2 = 2 }).ConfigureAwait(false);
                    Assert.That(character.CurrentMap, Is.SameAs(definition.Object));
                    Assert.That(packets, Is.EqualTo(1), "A competing generic placement must refuse without disturbing the original stream.");
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                }
            }

            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!legacy && stage is not ("cleanup" or "teardown" or "remove-teardown"))
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            Mock.Get(peer.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            Assert.Multiple(() =>
            {
                if (stage is "cleanup" or "remove-reselect")
                {
                    Assert.That(source.GetPlayers(), Does.Contain(target), "An abandoned placement must leave the current life on its map.");
                }
                else
                {
                    Assert.That(source.GetPlayers(), Does.Not.Contain(target));
                }
                Assert.That(peer.TradingPartner, Is.Null);
                if (stage is not ("teardown" or "remove-teardown"))
                {
                    Assert.That(target.TradingPartner, Is.Null);
                    Assert.That(target.BackupInventory, Is.Null);
                    Assert.That(target.TradingMoney, Is.Zero);
                    Assert.That(target.Money, Is.EqualTo(200));
                    Assert.That(target.Inventory!.Items.Count(i => ReferenceEquals(i, item.Object)), Is.EqualTo(1));
                }

                if (stage is "cleanup" or "remove" or "packet")
                {
                    Assert.That(target.LastDeath, Is.SameAs(owned));
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                    Assert.That(target.IsAlive, Is.False);
                    Assert.That(deaths, Is.EqualTo(1));
                }
                else if (stage is "teardown" or "remove-teardown")
                {
                    Assert.That(destination.GetPlayers(), Does.Not.Contain(target));
                    Assert.That(target.SelectedCharacter, Is.Null);
                }
                else
                {
                    var expectedMap = stage == "remove-reselect" ? source : destination;
                    Assert.That(target.CurrentMap, Is.SameAs(expectedMap));
                    Assert.That(expectedMap.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1));
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(target.IsAlive, Is.True);
                }
            });
        }
        finally
        {
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Refusing a competing placement cannot drop an already elapsed original death timer.</summary>
    /// <param name="legacy">Whether the original placement completes server-side or through Ready.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CompetingPlacementRetainsDueDeathAsync(bool legacy)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, legacy);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var source = target.CurrentMap!;
        var definition = new Mock<GameMapDefinition>();
        definition.SetupAllProperties();
        definition.Setup(m => m.DropItemGroups).Returns(source.Definition.DropItemGroups);
        definition.Setup(m => m.MonsterSpawns).Returns(source.Definition.MonsterSpawns);
        definition.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        definition.Object.Number = 1;
        definition.Object.TerrainData = source.Definition.TerrainData;
        peer.GameContext.Configuration.Maps.Add(definition.Object);
        var gate = new ExitGate { Map = definition.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true };
        definition.Object.ExitGates.Add(gate);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var restored = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var packets = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        async ValueTask PacketAsync()
        {
            if (Interlocked.Increment(ref packets) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
            else
            {
                restored.TrySetResult();
            }
        }

        if (legacy)
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(PacketAsync);
        }
        else
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }

        await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
        var owned = target.LastDeath;
        var placement = Task.Run(async () => await target.RespawnAtAsync(gate).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Delay(3500).ConfigureAwait(false);
            await target.RespawnAtAsync(new ExitGate { Map = source.Definition, X1 = 2, X2 = 2, Y1 = 2, Y2 = 2 }).ConfigureAwait(false);
            Assert.That(packets, Is.EqualTo(1));
            Assert.That(target.GetPlacementDeathSnapshot(), Is.EqualTo((owned, true)));
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.Zero);
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!legacy)
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            await restored.Task.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            if (!legacy)
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.LastDeath, Is.SameAs(owned));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(target.Attributes[Stats.MaximumHealth]));
            });
        }
        finally
        {
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static byte[] CreateReadyPacket()
    {
        var packet = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef(packet);
        return packet;
    }

    private sealed class PlacementPlayer(IGameContext context, bool legacy) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new PlacementViews(legacy);
    }

    private sealed class PlacementViews(bool legacy) : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _mocks = new();

        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn
            => !legacy && typeof(T) == typeof(IRespawnAfterDeathPlugIn) ? null : this._mocks.GetPlugIn<T>();
    }
}
