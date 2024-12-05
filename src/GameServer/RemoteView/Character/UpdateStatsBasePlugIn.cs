// <copyright file="UpdateStatsBasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using UpdateAction = Func<RemotePlayer, ValueTask>;

/// <summary>
/// The default implementation of the <see cref="IUpdateStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
public abstract class UpdateStatsBasePlugIn : Disposable, IUpdateStatsPlugIn
{
    private static readonly int SendDelayMs = 16;

    private static readonly ConcurrentDictionary<
        FrozenDictionary<AttributeDefinition, UpdateAction>,
        FrozenDictionary<UpdateAction, int>> ActionIndexMappings = new();

    private readonly RemotePlayer _player;

    private readonly FrozenDictionary<UpdateAction, int> _actionIndexMapping;

    private readonly FrozenDictionary<AttributeDefinition, UpdateAction> _changeActions;

    private readonly AutoResetEvent[] _resetEvents;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsBasePlugIn" /> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="changeActions">The change actions.</param>
    protected UpdateStatsBasePlugIn(RemotePlayer player, FrozenDictionary<AttributeDefinition, UpdateAction> changeActions)
    {
        this._player = player;
        this._changeActions = changeActions;
        this._actionIndexMapping = GetActionIndexMapping(changeActions);
        this._resetEvents = new AutoResetEvent[changeActions.Count];
        for (int i = 0; i < this._resetEvents.Length; i++)
        {
            this._resetEvents[i] = new AutoResetEvent(true);
        }
    }

    /// <inheritdoc />
    public async ValueTask UpdateStatsAsync(AttributeDefinition attribute, float value)
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        if (this._changeActions.TryGetValue(attribute, out var action))
        {
            _ = this.SendDelayedUpdateAsync(action);
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        foreach (var are in this._resetEvents)
        {
            are.Dispose();
        }

        base.Dispose(disposing);
    }

    private async ValueTask SendDelayedUpdateAsync(UpdateAction action)
    {
        var autoResetEvent = this._resetEvents[this._actionIndexMapping[action]];
        if (!autoResetEvent.WaitOne(0))
        {
            // we're sending an update already.
            return;
        }

        try
        {
            await Task.Delay(SendDelayMs).ConfigureAwait(false);
            await action(this._player).ConfigureAwait(false);
        }
        finally
        {
            autoResetEvent.Set();
        }
    }

    private static FrozenDictionary<UpdateAction, int> GetActionIndexMapping(FrozenDictionary<AttributeDefinition, UpdateAction> changeActions)
    {
        return ActionIndexMappings.GetOrAdd(changeActions, CreateNewIndexDictionary);

        FrozenDictionary<UpdateAction, int> CreateNewIndexDictionary(FrozenDictionary<AttributeDefinition, UpdateAction> dict)
        {
            return dict.Values.Distinct().Index().ToFrozenDictionary(tuple => tuple.Item, tuple => tuple.Index);
        }
    }
}