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
    /// Gets the monster number of Castle Siege gates.
    /// </summary>
    public static short MonsterNumber { get; } = 277;

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
        this.SetDefense(
            this.Context.Configuration.GetUpgrades(MonsterNumber, CastleSiegeUpgradeType.Defense)?.GetValue(this.DefenseLevel)
            ?? throw new InvalidOperationException($"Castle Siege gate defense level {this.DefenseLevel} is not configured."));
        this.SetMaximumHealth(
            this.Context.Configuration.GetUpgrades(MonsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(this.LifeLevel)
            ?? throw new InvalidOperationException($"Castle Siege gate life level {this.LifeLevel} is not configured."),
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
        this.Context.NpcController.BlockGateTerrain(this.CurrentMap, area);
        this.IsClosed = true;
        await this.NotifyStateAsync(true, [area], []).ConfigureAwait(false);
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
        var blockedAreas = this.Context.NpcController.ReleaseGateTerrain(this.CurrentMap, area);
        this.IsClosed = false;
        await this.NotifyStateAsync(false, [area], blockedAreas).ConfigureAwait(false);
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
        // The shipped map defines every gate horizontally. Its spawn X is the width center, while spawn Y is the
        // lower edge of the two-tile blocked area; visual direction does not describe the collision rectangle.
        const int gateWidth = 6;
        const int gateHeight = 2;
        var startX = Math.Clamp(this.Position.X - (gateWidth / 2), byte.MinValue, byte.MaxValue - gateWidth + 1);
        var startY = Math.Clamp((int)this.Position.Y, byte.MinValue, byte.MaxValue - gateHeight + 1);
        return ((byte)startX, (byte)startY, (byte)(startX + gateWidth - 1), (byte)(startY + gateHeight - 1));
    }

    private ValueTask NotifyStateAsync(
        bool setBlocked,
        IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> areas,
        IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> blockedAreas)
    {
        return this.Context.ForEachSiegePlayerAsync(async player =>
        {
            await NotifyTerrainChangeAsync(player, setBlocked, areas).ConfigureAwait(false);
            if (blockedAreas.Count > 0)
            {
                await NotifyTerrainChangeAsync(player, true, blockedAreas).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                    view => view.ShowGateStateAsync(!this.IsClosed, this.Id))
                .ConfigureAwait(false);
        });
    }
}
