// <copyright file="InventoryRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// Returns an authoritative inventory snapshot after earlier packets on the same connection.
/// </summary>
/// <remarks>
/// Packet ordering is provided by the connection, not by this handler's serialization lock:
/// <see cref="MUnique.OpenMU.Network.PacketPipeReaderBase"/> awaits each packet before reading
/// the next one. <see cref="MUnique.OpenMU.Network.Connection"/> awaits its packet subscribers,
/// and <see cref="MUnique.OpenMU.GameServer.RemoteView.RemotePlayer"/> awaits the packet handler
/// through its persistence-exclusive action. Item handlers therefore finish before a subsequent
/// inventory request on that connection. This does not order unrelated background game actions.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.InventoryRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.InventoryRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("db463aa9-e091-42e4-a926-c68ba24cfdf9")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class InventoryRequestHandlerPlugIn : ISubPacketHandlerPlugIn, ISupportCustomConfiguration<InventoryRequestHandlerConfiguration>, ISupportDefaultCustomConfiguration
{
    private static readonly InventoryRequestHandlerConfiguration DefaultConfiguration = new();
    private readonly ConditionalWeakTable<Player, RefreshState> _refreshes = new();
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryRequestHandlerPlugIn"/> class.
    /// </summary>
    public InventoryRequestHandlerPlugIn()
        : this(TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryRequestHandlerPlugIn"/> class with a clock.
    /// </summary>
    /// <param name="timeProvider">The monotonic clock and delay scheduler.</param>
    internal InventoryRequestHandlerPlugIn(TimeProvider timeProvider) => this._timeProvider = timeProvider;

    /// <inheritdoc />
    public InventoryRequestHandlerConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc />
    public byte Key => InventoryRequest.SubCode;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new InventoryRequestHandlerConfiguration();

    /// <inheritdoc />
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length != InventoryRequest.Length || !CanRefresh(player))
        {
            return ValueTask.CompletedTask;
        }

        var state = this._refreshes.GetValue(player, static _ => new RefreshState());

        // This synchronous lock only protects scheduling; it is never held across an await.
        lock (state)
        {
            if (state.Pending is { Started: false } queued && !queued.Task.IsCompleted)
            {
                return new ValueTask(queued.Task);
            }

            // Requests arriving after serialization starts need a later snapshot. Joining the
            // in-flight write could acknowledge an intervening item action with stale contents.
            var refresh = new Refresh();
            state.Pending = refresh;
            refresh.Task = this.RefreshAsync(player, state, refresh).AsTask();
            return new ValueTask(refresh.Task);
        }
    }

    private static bool CanRefresh(Player player) => player.Inventory is not null
        && (player.PlayerState.CurrentState == PlayerState.EnteredWorld
            || player.PlayerState.CurrentState == PlayerState.NpcDialogOpened);

    private async ValueTask RefreshAsync(Player player, RefreshState state, Refresh refresh)
    {
        using (await state.Serialization.LockAsync().ConfigureAwait(false))
        {
            var interval = this.Configuration?.MinimumInterval ?? DefaultConfiguration.MinimumInterval;
            if (interval <= TimeSpan.Zero)
            {
                interval = DefaultConfiguration.MinimumInterval;
            }

            if (state.LastStarted is { } lastStarted)
            {
                var remaining = interval - this._timeProvider.GetElapsedTime(lastStarted);
                if (remaining > TimeSpan.Zero)
                {
                    await Task.Delay(remaining, this._timeProvider, CancellationToken.None).ConfigureAwait(false);
                }
            }

            lock (state)
            {
                refresh.Started = true;
                state.LastStarted = this._timeProvider.GetTimestamp();
            }

            // A delayed request must not leak a world snapshot after logout.
            if (CanRefresh(player))
            {
                await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
            }
        }
    }

    private sealed class RefreshState
    {
        public AsyncLock Serialization { get; } = new();

        public long? LastStarted { get; set; }

        public Refresh? Pending { get; set; }
    }

    private sealed class Refresh
    {
        public bool Started { get; set; }

        public Task Task { get; set; } = Task.CompletedTask;
    }
}
