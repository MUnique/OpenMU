// <copyright file="InventoryRequestHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Verifies the authoritative inventory refresh request and its state boundary.
/// </summary>
[TestFixture]
public class InventoryRequestHandlerTests
{
    /// <summary>
    /// World and NPC interactions can refresh inventory without changing contents.
    /// </summary>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask ActivePlayerReceivesSnapshotAsync(bool npcDialog)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!).Invocations.Clear();
        if (npcDialog)
        {
            Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false), Is.True);
        }

        var handler = CreateHandler();
        Assert.That(handler.Key, Is.EqualTo(InventoryRequest.SubCode));
        Assert.That(handler.IsEncryptionExpected, Is.True);
        await handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!).Verify(p => p.UpdateInventoryListAsync(), Times.Once);
    }

    /// <summary>
    /// A malformed request or a player in character selection cannot refresh world inventory.
    /// </summary>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask InactiveOrMalformedRequestIsIgnoredAsync(bool characterSelection)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!).Invocations.Clear();
        if (characterSelection)
        {
            Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false), Is.True);
        }

        await CreateHandler().HandlePacketAsync(player, new byte[characterSelection ? InventoryRequest.Length : InventoryRequest.Length - 1]).ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!).Verify(p => p.UpdateInventoryListAsync(), Times.Never);
    }

    private static ISubPacketHandlerPlugIn CreateHandler()
    {
        var type = typeof(IPacketHandlerPlugIn).Assembly.GetType("MUnique.OpenMU.GameServer.MessageHandler.Character.InventoryRequestHandlerPlugIn", throwOnError: true)!;
        return (ISubPacketHandlerPlugIn)Activator.CreateInstance(type)!;
    }
}
