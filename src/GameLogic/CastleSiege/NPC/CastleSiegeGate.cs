// <copyright file="CastleSiegeGate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An attackable Castle Siege gate.
/// </summary>
public sealed class CastleSiegeGate : CastleSiegeAttackableNpc
{
    /// <summary>
    /// The monster number of Castle Siege gates.
    /// </summary>
    public const short MonsterNumber = 277;

    private readonly Dictionary<Point, bool> _originalTerrain = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeGate"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the gate is spawned.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The gate intelligence.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plugin manager.</param>
    public CastleSiegeGate(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeContext context,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeGateIntelligence intelligence,
        IDropGenerator dropGenerator,
        PlugInManager plugInManager)
        : base(spawnInfo, stats, map, context, runtime, intelligence, dropGenerator, plugInManager)
    {
    }

    /// <summary>
    /// Gets the defense upgrade level.
    /// </summary>
    public byte DefenseLevel => this.State.DefenseLevel;

    /// <summary>
    /// Gets the life upgrade level.
    /// </summary>
    public byte LifeLevel => this.State.LifeLevel;

    /// <summary>
    /// Gets a value indicating whether the gate is closed and blocking terrain.
    /// </summary>
    public bool IsClosed { get; private set; }

    /// <inheritdoc />
    public override void ApplyPersistedUpgrades(bool preserveMissingHealth)
    {
        this.SetDefense(GetValue(this.Context.Configuration.GateDefenseUpgrades, this.DefenseLevel));
        this.SetMaximumHealth(
            GetValue(this.Context.Configuration.GateLifeUpgrades, this.LifeLevel),
            preserveMissingHealth);
    }

    /// <summary>
    /// Closes the gate and blocks its six-by-two tile area.
    /// </summary>
    /// <returns>A task that represents the asynchronous close operation.</returns>
    public async ValueTask CloseAsync()
    {
        if (this.IsClosed || !this.IsAlive)
        {
            return;
        }

        var area = this.GetTerrainArea();
        for (var x = area.StartX; x <= area.EndX; x++)
        {
            for (var y = area.StartY; y <= area.EndY; y++)
            {
                var point = new Point(x, y);
                this._originalTerrain.TryAdd(point, this.CurrentMap.Terrain.WalkMap[x, y]);
                this.CurrentMap.Terrain.WalkMap[x, y] = false;
                this.CurrentMap.Terrain.UpdateAiGridValue(x, y);
            }
        }

        this.IsClosed = true;
        await this.NotifyTerrainChangeAsync(true, [area]).ConfigureAwait(false);
        await this.NotifyGateStateAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Opens the gate and restores the terrain which existed before it was closed.
    /// </summary>
    /// <returns>A task that represents the asynchronous open operation.</returns>
    public async ValueTask OpenAsync()
    {
        if (!this.IsClosed)
        {
            return;
        }

        var area = this.GetTerrainArea();
        var originallyBlockedAreas = this._originalTerrain
            .Where(entry => !entry.Value)
            .Select(entry => (entry.Key.X, entry.Key.Y, entry.Key.X, entry.Key.Y))
            .ToList();
        foreach (var (point, wasWalkable) in this._originalTerrain)
        {
            this.CurrentMap.Terrain.WalkMap[point.X, point.Y] = wasWalkable;
            this.CurrentMap.Terrain.UpdateAiGridValue(point.X, point.Y);
        }

        this._originalTerrain.Clear();
        this.IsClosed = false;
        await this.NotifyTerrainChangeAsync(false, [area]).ConfigureAwait(false);
        if (originallyBlockedAreas.Count > 0)
        {
            await this.NotifyTerrainChangeAsync(true, originallyBlockedAreas).ConfigureAwait(false);
        }

        await this.NotifyGateStateAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the current gate and terrain state to a player entering the Castle Siege map.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>A task that represents the asynchronous synchronization.</returns>
    internal async ValueTask SynchronizeStateAsync(Player player)
    {
        if (this.IsClosed)
        {
            await NotifyTerrainChangeAsync(player, true, [this.GetTerrainArea()]).ConfigureAwait(false);
        }

        await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                view => view.ShowGateStateAsync(!this.IsClosed, this.Id))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnDeathAsync(IAttacker attacker)
    {
        await this.OpenAsync().ConfigureAwait(false);
        await base.OnDeathAsync(attacker).ConfigureAwait(false);
    }

    private static int GetValue(IEnumerable<CastleSiegeUpgradeDefinition> definitions, byte level)
    {
        return definitions.FirstOrDefault(definition => definition.Level == level)?.Value
               ?? throw new InvalidOperationException($"Castle Siege gate upgrade level {level} is not configured.");
    }

    private static async ValueTask NotifyTerrainChangeAsync(
        Player player,
        bool setBlocked,
        IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> areas)
    {
        await player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(
                view => view.ChangeAttributesAsync(TerrainAttributeType.Blocked, setBlocked, areas))
            .ConfigureAwait(false);
    }

    private (byte StartX, byte StartY, byte EndX, byte EndY) GetTerrainArea()
    {
        const int gateWidth = 6;
        const int gateHeight = 2;
        var startX = Math.Clamp(this.Position.X - (gateWidth / 2), byte.MinValue, byte.MaxValue - gateWidth + 1);
        var startY = Math.Clamp((int)this.Position.Y, byte.MinValue, byte.MaxValue - gateHeight + 1);
        return ((byte)startX, (byte)startY, (byte)(startX + gateWidth - 1), (byte)(startY + gateHeight - 1));
    }

    private async ValueTask NotifyTerrainChangeAsync(
        bool setBlocked,
        IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> areas)
    {
        var players = await this.Context.GameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players.Where(player => player.CurrentMap == this.CurrentMap))
        {
            await NotifyTerrainChangeAsync(player, setBlocked, areas).ConfigureAwait(false);
        }
    }

    private async ValueTask NotifyGateStateAsync()
    {
        var players = await this.Context.GameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players.Where(player => player.CurrentMap == this.CurrentMap))
        {
            await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                    view => view.ShowGateStateAsync(!this.IsClosed, this.Id))
                .ConfigureAwait(false);
        }
    }
}
