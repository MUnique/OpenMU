// <copyright file="MiniGameChangeEventProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tracks and applies the <see cref="MiniGameChangeEvent"/>s of a mini game.
/// It keeps the remaining events, determines the next event, registers kills towards it,
/// and applies triggered events (terrain changes, spawns, messages).
/// </summary>
internal sealed class MiniGameChangeEventProcessor
{
    private readonly MiniGameDefinition _definition;
    private readonly GameMap _map;
    private readonly IMapInitializer _mapInitializer;
    private readonly IEventStateProvider _eventStateProvider;
    private readonly ILogger _logger;
    private readonly Func<LocalizedString, object?[], ValueTask> _announceMessageAsync;
    private readonly Func<MiniGameChangeEvent, ValueTask> _onTerrainChangingAsync;
    private readonly Func<MiniGameChangeEvent, ValueTask> _onTerrainChangedAsync;
    private readonly Func<Func<Player, Task>, ValueTask> _forEachPlayerAsync;
    private readonly List<ChangeEventContext> _remainingEvents = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameChangeEventProcessor"/> class.
    /// </summary>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="map">The map on which the game takes place.</param>
    /// <param name="mapInitializer">The map initializer, which is used to initialize spawns.</param>
    /// <param name="eventStateProvider">The event state provider, handed to the map initializer.</param>
    /// <param name="logger">The logger for this instance.</param>
    /// <param name="announceMessageAsync">Shows a golden center-screen message to all players of the game.</param>
    /// <param name="onTerrainChangingAsync">Called before the map terrain is changing.</param>
    /// <param name="onTerrainChangedAsync">Called when the map terrain changed.</param>
    /// <param name="forEachPlayerAsync">Executes an action for each player of the game.</param>
    public MiniGameChangeEventProcessor(
        MiniGameDefinition definition,
        GameMap map,
        IMapInitializer mapInitializer,
        IEventStateProvider eventStateProvider,
        ILogger logger,
        Func<LocalizedString, object?[], ValueTask> announceMessageAsync,
        Func<MiniGameChangeEvent, ValueTask> onTerrainChangingAsync,
        Func<MiniGameChangeEvent, ValueTask> onTerrainChangedAsync,
        Func<Func<Player, Task>, ValueTask> forEachPlayerAsync)
    {
        this._definition = definition;
        this._map = map;
        this._mapInitializer = mapInitializer;
        this._eventStateProvider = eventStateProvider;
        this._logger = logger;
        this._announceMessageAsync = announceMessageAsync;
        this._onTerrainChangingAsync = onTerrainChangingAsync;
        this._onTerrainChangedAsync = onTerrainChangedAsync;
        this._forEachPlayerAsync = forEachPlayerAsync;
    }

    /// <summary>
    /// Gets the next event which targets should be fulfilled by the players.
    /// </summary>
    public ChangeEventContext? Current { get; private set; }

    /// <summary>
    /// Applies the start events and prepares the remaining events.
    /// </summary>
    /// <param name="playerCount">The number of players which started with the game.</param>
    public async ValueTask InitializeAsync(int playerCount)
    {
        var startEvents = this._definition.ChangeEvents
            .OrderBy(e => e.Index)
            .TakeWhile(e => e is { Index: <= 0, NumberOfKills: 0 })
            .ToList();

        foreach (var changeEvent in startEvents)
        {
            await this.ApplyChangeEventAsync(changeEvent).ConfigureAwait(false);
        }

        this._remainingEvents.AddRange(
            this._definition.ChangeEvents
                .Except(startEvents)
                .Select(e => new ChangeEventContext(e, playerCount)));
        this.UpdateNextEvent();
    }

    /// <summary>
    /// Registers a kill towards the current event and applies the event if its target has been achieved.
    /// </summary>
    /// <param name="killedObject">The killed object.</param>
    public void NotifyKill(IAttackable killedObject)
    {
        if (this.Current is not { } nextEvent)
        {
            return;
        }

        if (this.IsKillValid(killedObject, nextEvent.Definition) && nextEvent.RegisterKill())
        {
            this._remainingEvents.Remove(nextEvent);
            this.UpdateNextEvent();
            _ = Task.Run(() => this.ApplyChangeEventAsync(nextEvent.Definition, killedObject.LastDeath?.KillerName));
        }
    }

    private async Task ApplyChangeEventAsync(MiniGameChangeEvent changeEvent, string? triggeredBy = null)
    {
        try
        {
            if (changeEvent.TerrainChanges.Any())
            {
                await this._onTerrainChangingAsync(changeEvent).ConfigureAwait(false);
                await this.UpdateClientTerrainAsync(changeEvent.TerrainChanges).ConfigureAwait(false);
                this.UpdateServerTerrain(changeEvent.TerrainChanges);
                await this._map.ClearDropsOnInvalidTerrainAsync().ConfigureAwait(false);
                await this._onTerrainChangedAsync(changeEvent).ConfigureAwait(false);
            }

            if (changeEvent.SpawnArea is { } spawnArea)
            {
                for (int i = 0; i < spawnArea.Quantity; i++)
                {
                    await this._mapInitializer.InitializeSpawnAsync(i, this._map, spawnArea, this._eventStateProvider).ConfigureAwait(false);
                }
            }

            if (changeEvent.Message is { } message)
            {
                await this._announceMessageAsync(message, [triggeredBy]).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected exception at change event {changeEvent}: {ex}", changeEvent.Description, ex);
        }
    }

    private void UpdateServerTerrain(ICollection<MiniGameTerrainChange> changes)
    {
        foreach (var change in changes)
        {
            var isSafezone = change.TerrainAttribute is TerrainAttributeType.Safezone;
            var map = isSafezone ? this._map.Terrain.SafezoneMap : this._map.Terrain.WalkMap;
            var targetValue = isSafezone ? change.SetTerrainAttribute : !change.SetTerrainAttribute;
            for (var x = change.StartX; x <= change.EndX; x++)
            {
                for (var y = change.StartY; y <= change.EndY; y++)
                {
                    map[x, y] = targetValue;
                    this._map.Terrain.UpdateAiGridValue(x, y);
                }
            }
        }
    }

    private async ValueTask UpdateClientTerrainAsync(ICollection<MiniGameTerrainChange> changes)
    {
        var groupedChanges = changes
            .Where(c => c.IsClientUpdateRequired)
            .GroupBy(
                c => (c.SetTerrainAttribute, c.TerrainAttribute),
                c => (c.StartX, c.StartY, c.EndX, c.EndY))
            .Select(g => (g.Key, Areas: g.ToList()))
            .ToList();

        await this._forEachPlayerAsync(async player =>
        {
            foreach (var group in groupedChanges)
            {
                await player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p => p.ChangeAttributesAsync(group.Key.TerrainAttribute, group.Key.SetTerrainAttribute, group.Areas)).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    private void UpdateNextEvent()
    {
        this.Current = this._remainingEvents.Count == 0
            ? null
            : this._remainingEvents.MinBy(e => e.Definition.Index);
    }

    private bool IsKillValid(IAttackable killedObject, MiniGameChangeEvent definition)
    {
        if (definition.MinimumTargetLevel.HasValue && killedObject.Attributes[Stats.Level] < definition.MinimumTargetLevel)
        {
            return false;
        }

        if (definition.Target == KillTarget.AnyMonster && killedObject is not Monster)
        {
            return false;
        }

        if (definition.Target == KillTarget.Specific
            && (killedObject is not NonPlayerCharacter npc || npc.Definition.Number != definition.TargetDefinition?.Number))
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// The context of the <see cref="MiniGameChangeEvent"/>.
/// </summary>
public sealed class ChangeEventContext
{
    private int _actualKills;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeEventContext"/> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="playerCount">The player count.</param>
    public ChangeEventContext(MiniGameChangeEvent definition, int playerCount)
    {
        this.Definition = definition;
        this.RequiredKills = definition.NumberOfKills;
        if (this.Definition.MultiplyKillsByPlayers)
        {
            this.RequiredKills *= playerCount;
        }
    }

    /// <summary>
    /// Gets the definition of the change event.
    /// </summary>
    public MiniGameChangeEvent Definition { get; }

    /// <summary>
    /// Gets the required kills.
    /// </summary>
    public int RequiredKills { get; }

    /// <summary>
    /// Gets the actual kills.
    /// </summary>
    public int ActualKills => this._actualKills;

    /// <summary>
    /// Registers a kill and returns if the target has been achieved.
    /// </summary>
    /// <returns>True, if the target has been achieved just right now.</returns>
    public bool RegisterKill()
    {
        if (this._actualKills == this.RequiredKills)
        {
            // Already achieved.
            return false;
        }

        return Interlocked.Increment(ref this._actualKills) == this.RequiredKills;
    }
}
