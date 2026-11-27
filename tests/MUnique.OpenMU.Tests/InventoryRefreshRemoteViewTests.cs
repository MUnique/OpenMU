// <copyright file="InventoryRefreshRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Inventory;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests full inventory response framing and contents after NPC item transfers.
/// </summary>
[TestFixture]
public class InventoryRefreshRemoteViewTests
{
    /// <summary>
    /// The requested packet contains only authoritative inventory slots, excluding items in
    /// the NPC grid, then includes returned items at their new slot in both wire formats.
    /// </summary>
    /// <param name="extended">Whether to use the variable-length item format.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask SnapshotAfterNpcMovesEncodesCurrentSlotsAndItemsAsync(bool extended)
    {
        var (player, output) = await CreatePlayerAsync(extended).ConfigureAwait(false);
        var source = new BasicModel.Item
        {
            Definition = new BasicModel.ItemDefinition { Number = 1, Group = 0, Width = 1, Height = 1, Durability = 20 },
            Level = 2,
            Durability = 9,
        };
        var keeper = new BasicModel.Item
        {
            Definition = new BasicModel.ItemDefinition { Number = 3, Group = 0, Width = 1, Height = 1, Durability = 20 },
            Level = 4,
            Durability = 17,
        };
        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);
        await player.Inventory.AddItemAsync(30, keeper).ConfigureAwait(false);
        var npc = new Mock<MonsterDefinition>();
        npc.Setup(n => n.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>());
        npc.Object.NpcWindow = NpcWindow.CombineLuckyItem;
        player.OpenedNpc = new NonPlayerCharacter(null!, npc.Object, null!);
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false), Is.True);
        var move = new MoveItemAction();
        await move.MoveItemAsync(player, 20, Storages.Inventory, 0, Storages.LuckyItemTrade).ConfigureAwait(false);
        Assert.That(player.TemporaryStorage!.GetItem(0), Is.SameAs(source));
        output.SetLength(0);
        output.Position = 0;
        var handler = InventoryRequestHandlerTests.CreateHandler();
        await handler.HandlePacketAsync(player, new byte[] { 0xC3, 4, 0xF3, 0x10 }).ConfigureAwait(false);
        var bytes = output.ToArray();
        var itemSize = extended ? 5 : 12;
        AssertSnapshot(bytes, 1, itemSize);
        Assert.That(bytes[6], Is.EqualTo(30));
        AssertItem(bytes.AsSpan(7, itemSize), extended, 3, 4, 17);

        await move.MoveItemAsync(player, 0, Storages.LuckyItemRefinery, 22, Storages.Inventory).ConfigureAwait(false);
        output.SetLength(0);
        output.Position = 0;
        await handler.HandlePacketAsync(player, new byte[] { 0xC3, 4, 0xF3, 0x10 }).ConfigureAwait(false);
        bytes = output.ToArray();
        AssertSnapshot(bytes, 2, itemSize);
        Assert.That(bytes[6], Is.EqualTo(22));
        AssertItem(bytes.AsSpan(7, itemSize), extended, 1, 2, 9);
        Assert.That(bytes[7 + itemSize], Is.EqualTo(30));
        AssertItem(bytes.AsSpan(8 + itemSize, itemSize), extended, 3, 4, 17);
    }

    private static void AssertSnapshot(byte[] bytes, byte count, int itemSize)
    {
        Assert.That(bytes.Length, Is.EqualTo(6 + (count * (1 + itemSize))), "no reserved item padding is sent");
        Assert.That(bytes.Take(5), Is.EqualTo(new byte[] { 0xC4, 0, (byte)bytes.Length, 0xF3, 0x10 }));
        CharacterInventory packet = bytes.AsMemory();
        Assert.That(packet.Header.Length, Is.EqualTo(bytes.Length));
        Assert.That(packet.ItemCount, Is.EqualTo(count));
    }

    private static void AssertItem(ReadOnlySpan<byte> bytes, bool extended, byte number, byte level, byte durability)
    {
        if (extended)
        {
            Assert.That(bytes.ToArray(), Is.EqualTo(new byte[] { 0, number, level, durability, 0 }));
        }
        else
        {
            Assert.That(bytes[0], Is.EqualTo(number));
            Assert.That(bytes[1], Is.EqualTo(level << 3));
            Assert.That(bytes[2], Is.EqualTo(durability));
        }
    }

    private static async ValueTask<(RemotePlayer Player, MemoryStream Output)> CreatePlayerAsync(bool extended)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.RegisterPlugIn<IViewPlugIn, ItemSerializer>();
        manager.RegisterPlugIn<IViewPlugIn, ItemSerializerExtended>();
        manager.RegisterPlugIn<IViewPlugIn, UpdateInventoryListPlugIn>();
        var maps = new MapInitializer(configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var context = new GameServerContext(
            new BasicModel.GameServerDefinition { GameConfiguration = configuration, ServerConfiguration = new BasicModel.GameServerConfiguration() },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(),
            maps,
            NullLoggerFactory.Instance,
            manager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        maps.PlugInManager = context.PlugInManager;
        maps.PathFinderPool = context.PathFinderPool;
        var version = extended ? new ClientVersion(106, 3, ClientLanguage.Invariant) : new ClientVersion(6, 3, ClientLanguage.English);
        var (player, output) = PlayerTestHelper.CreateRemotePlayer(context, version);
        await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(context)).ConfigureAwait(false);
        return (player, output);
    }
}
