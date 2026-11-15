// <copyright file="InventoryRequestHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.PlugIns;
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

    /// <summary>
    /// A burst awaits one fresh delayed response, while another player is not throttled.
    /// </summary>
    [Test]
    public async ValueTask BurstRequestsShareFreshSnapshotAndDoNotThrottleOtherPlayersAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var other = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!);
        view.Invocations.Clear();
        var otherView = Mock.Get(other.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!);
        otherView.Invocations.Clear();
        var (clock, timer) = CreateClock();
        var handler = CreateHandler(clock.Object);
        ((ISupportCustomConfiguration<InventoryRequestHandlerConfiguration>)handler).Configuration = new() { MinimumInterval = TimeSpan.FromSeconds(1) };
        var snapshots = new List<byte[]>();
        view.Setup(v => v.UpdateInventoryListAsync()).Callback(() => snapshots.Add(player.Inventory!.Items.Select(i => i.ItemSlot).ToArray())).Returns(ValueTask.CompletedTask);
        await handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).ConfigureAwait(false);
        var burst = Enumerable.Range(0, 100).Select(_ => handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).AsTask()).ToArray();
        Assert.That(burst.All(task => !task.IsCompleted), Is.True);
        view.Verify(v => v.UpdateInventoryListAsync(), Times.Once);
        await handler.HandlePacketAsync(other, new byte[InventoryRequest.Length]).ConfigureAwait(false);
        otherView.Verify(v => v.UpdateInventoryListAsync(), Times.Once);
        var item = new Persistence.BasicModel.Item { Definition = new ItemDefinition { Width = 1, Height = 1 }, Durability = 1 };
        await player.Inventory!.AddItemAsync(20, item).ConfigureAwait(false);
        var scheduled = await timer.Task.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        Assert.That(scheduled.Delay, Is.EqualTo(TimeSpan.FromSeconds(1)));
        clock.Setup(c => c.GetTimestamp()).Returns(TimeSpan.TicksPerSecond);
        scheduled.Callback(scheduled.State);
        await Task.WhenAll(burst).WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        view.Verify(v => v.UpdateInventoryListAsync(), Times.Exactly(2));
        Assert.That(snapshots.Last(), Does.Contain((byte)20), "coalesced requests must see changes made while waiting");
        clock.Setup(c => c.GetTimestamp()).Returns(2 * TimeSpan.TicksPerSecond);
        await handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).ConfigureAwait(false);
        view.Verify(v => v.UpdateInventoryListAsync(), Times.Exactly(3));
    }

    /// <summary>
    /// A request after writing starts must receive a later snapshot, not join stale output.
    /// </summary>
    [Test]
    public async ValueTask RequestDuringSerializationWaitsForAnotherSnapshotAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!);
        view.Invocations.Clear();
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        view.Setup(v => v.UpdateInventoryListAsync()).Returns(() => ++calls == 1 ? new ValueTask(gate.Task) : ValueTask.CompletedTask);
        var (clock, timer) = CreateClock();
        var handler = CreateHandler(clock.Object);
        var first = handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).AsTask();
        var second = handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).AsTask();
        var third = handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).AsTask();
        Assert.That(second, Is.Not.SameAs(first));
        Assert.That(third, Is.SameAs(second));
        gate.SetResult();
        var scheduled = await timer.Task.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        scheduled.Callback(scheduled.State);
        await Task.WhenAll(first, second, third).WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        Assert.That(calls, Is.EqualTo(2));
    }

    /// <summary>
    /// Logout while waiting suppresses the deferred world snapshot.
    /// </summary>
    [Test]
    public async ValueTask DeferredSnapshotIsNotSentAfterLogoutAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()!);
        view.Invocations.Clear();
        var (clock, timer) = CreateClock();
        var handler = CreateHandler(clock.Object);
        await handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).ConfigureAwait(false);
        var pending = handler.HandlePacketAsync(player, new byte[InventoryRequest.Length]).AsTask();
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false), Is.True);
        var scheduled = await timer.Task.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        scheduled.Callback(scheduled.State);
        await pending.ConfigureAwait(false);
        view.Verify(v => v.UpdateInventoryListAsync(), Times.Once);
    }

    /// <summary>
    /// Creates the actual packet handler, optionally with a controlled delay scheduler.
    /// </summary>
    /// <param name="clock">The clock to use, or the system clock.</param>
    /// <returns>The packet handler.</returns>
    internal static ISubPacketHandlerPlugIn CreateHandler(TimeProvider? clock = null)
    {
        return clock is null
            ? new InventoryRequestHandlerPlugIn()
            : new InventoryRequestHandlerPlugIn(clock);
    }

    private static (Mock<TimeProvider> Clock, TaskCompletionSource<(TimerCallback Callback, object? State, TimeSpan Delay)> Timer) CreateClock()
    {
        var clock = new Mock<TimeProvider>();
        clock.SetupGet(c => c.TimestampFrequency).Returns(TimeSpan.TicksPerSecond);
        clock.Setup(c => c.GetTimestamp()).Returns(0);
        var created = new TaskCompletionSource<(TimerCallback, object?, TimeSpan)>(TaskCreationOptions.RunContinuationsAsynchronously);
        clock.Setup(c => c.CreateTimer(It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Returns((TimerCallback callback, object? state, TimeSpan due, TimeSpan _) =>
            {
                created.TrySetResult((callback, state, due));
                return Mock.Of<ITimer>();
            });
        return (clock, created);
    }
}
