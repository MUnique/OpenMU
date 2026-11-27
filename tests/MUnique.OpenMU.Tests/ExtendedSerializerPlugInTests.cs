// <copyright file="ExtendedSerializerPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the extension points of the extended item and appearance serializers.
/// </summary>
[TestFixture]
public class ExtendedSerializerPlugInTests
{
    private static readonly ClientVersion Version = new(106, 3, ClientLanguage.Invariant);

    /// <summary>
    /// The serializers which are created for a player call the plugins, which may change and extend the data.
    /// </summary>
    [Test]
    public void SerializersOfAPlayerCallThePlugIns()
    {
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer(Version);
        var manager = player.GameContext.PlugInManager;
        manager.RegisterPlugIn<IItemSerializer, ItemSerializerExtended>();
        manager.RegisterPlugIn<IAppearanceSerializer, AppearanceSerializerExtended>();
        manager.RegisterPlugIn<IExtendedItemSerializedPlugIn, AppendingPlugIn>();
        manager.RegisterPlugIn<IExtendedAppearanceSerializedPlugIn, MarkingPlugIn>();
        (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer((IGameServerContext)player.GameContext, Version);

        var itemSerializer = player.ViewPlugIns.GetPlugIn<IItemSerializer>()!;
        var appearanceSerializer = player.ViewPlugIns.GetPlugIn<IAppearanceSerializer>()!;
        using var context = new InMemoryPersistenceContextProvider().CreateNewContext();
        var item = context.CreateNew<Item>();
        item.Definition = context.CreateNew<ItemDefinition>();

        var data = new byte[itemSerializer.NeededSpace];
        var plain = new byte[itemSerializer.NeededSpace];
        var plainLength = new ItemSerializerExtended().SerializeItem(plain, item);
        var length = itemSerializer.SerializeItem(data, item);

        Assert.That(length, Is.EqualTo(plainLength + 1));
        Assert.That(data[plainLength], Is.EqualTo(0x42));

        var appearance = new Mock<IAppearanceData>();
        appearance.Setup(a => a.EquippedItems).Returns(Array.Empty<ItemAppearance>());
        var appearanceBytes = new byte[appearanceSerializer.NeededSpace];
        appearanceSerializer.WriteAppearanceData(appearanceBytes, appearance.Object, false);

        Assert.That(appearanceBytes[0], Is.EqualTo(0x42));
    }

    /// <summary>
    /// Without plugins, the data is not changed.
    /// </summary>
    [Test]
    public void SerializersWithoutPlugInsKeepTheData()
    {
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer(Version);
        using var context = new InMemoryPersistenceContextProvider().CreateNewContext();
        var item = context.CreateNew<Item>();
        item.Definition = context.CreateNew<ItemDefinition>();
        var expected = new byte[15];
        var actual = new byte[15];

        var expectedLength = new ItemSerializerExtended().SerializeItem(expected, item);
        var actualLength = new ItemSerializerExtended(player).SerializeItem(actual, item);

        Assert.That(actualLength, Is.EqualTo(expectedLength));
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>Appends one byte to the item.</summary>
    [PlugIn]
    [Guid("2A9C4E61-5D3B-4F87-B0A2-9E6C1D8F3B74")]
    private sealed class AppendingPlugIn : IExtendedItemSerializedPlugIn
    {
        public void ItemSerialized(Span<byte> data, Item item, ExtendedItemSerializedArgs args)
        {
            ArgumentNullException.ThrowIfNull(item);
            data[args.Length++] = 0x42;
        }
    }

    /// <summary>Marks the first byte of the appearance.</summary>
    [PlugIn]
    [Guid("7F2B8D15-6C4A-4E93-A1D7-3B5E0C9F6A28")]
    private sealed class MarkingPlugIn : IExtendedAppearanceSerializedPlugIn
    {
        public void AppearanceSerialized(Span<byte> data, IAppearanceData appearance)
        {
            ArgumentNullException.ThrowIfNull(appearance);
            data[0] = 0x42;
        }
    }
}
