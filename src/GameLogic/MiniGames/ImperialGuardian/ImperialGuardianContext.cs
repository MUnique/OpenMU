// <copyright file="ImperialGuardianContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The context of an imperial guardian event game.
/// </summary>
/// <remarks>
/// The event takes place on the map of the current day of the week and consists of several zones.
/// Each zone starts with a standby time, in which the players gather at its entrance. Then its monsters
/// appear, which have to be killed within a limited time. The gates of a zone block the way. The gate at
/// the entrance can be attacked as soon as the monsters appear, the other gates and the statues when all
/// monsters are killed. The event is completed when the monsters of the last zone are killed, and fails
/// when the time of a zone is over or when all players left.
/// </remarks>
public sealed class ImperialGuardianContext : MiniGameContext
{
    /// <summary>
    /// The duration of the countdown between the entering phase and the start of the game.
    /// </summary>
    private static readonly TimeSpan CountdownDuration = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(1);

    private readonly IGameContext _gameContext;
    private readonly ImperialGuardianEventDefinition _definition;
    private readonly ImperialGuardianWeather _weather;

    /// <summary>
    /// The monsters of the current zone, which have to be killed. Gates, statues and traps aren't included.
    /// </summary>
    private readonly ConcurrentDictionary<Monster, byte> _zoneMonsters = new();

    private readonly ConcurrentDictionary<ImperialGuardianGate, byte> _gates = new();

    private int _zone;
    private ImperialGuardianTimerType _timerType = ImperialGuardianTimerType.Standby;
    private DateTime _timerEndsAtUtc;
    private ImperialGuardianMonsterScaling? _monsterScaling;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public ImperialGuardianContext(
        MiniGameMapKey key,
        MiniGameDefinition definition,
        IGameContext gameContext,
        IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._gameContext = gameContext;

        // The definition is resolved once, so that a configuration change doesn't affect a running game.
        this._definition = ImperialGuardianFeaturePlugIn.GetEventDefinition(gameContext);
        this.Day = definition.GameLevel;
        this.ZoneCount = Math.Max(1, this.Map.Definition.MonsterSpawns
            .Where(spawn => spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart && spawn.WaveNumber / 10 == this.Day)
            .Select(spawn => (spawn.WaveNumber % 10) + 1)
            .DefaultIfEmpty(0)
            .Max());
        this._weather = (ImperialGuardianWeather)Rand.NextInt(0, 4);
        this._timerEndsAtUtc = this.EnterEndsAtUtc.Add(CountdownDuration);

        _ = Task.Run(() => this.RunTimerLoopAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <summary>
    /// Gets the day of the week (1 = monday, ..., 7 = sunday), for which the game takes place.
    /// </summary>
    public byte Day { get; }

    /// <summary>
    /// Gets the number of zones of the game.
    /// </summary>
    public int ZoneCount { get; }

    /// <summary>
    /// Gets the current zone, starting at 0.
    /// </summary>
    public int Zone => Volatile.Read(ref this._zone);

    /// <summary>
    /// Gets the number of the monsters of the current zone, which are still alive.
    /// </summary>
    public int RemainingMonsterCount => this._zoneMonsters.Keys.Count(monster => monster.IsAlive);

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        var playerLevel = players
            .Select(player => (int)((player.Attributes?[Stats.Level] ?? 0) + (player.Attributes?[Stats.MasterLevel] ?? 0)))
            .DefaultIfEmpty(0)
            .Max();
        this._monsterScaling = this._definition.GetMonsterScaling(playerLevel);

        _ = Task.Run(() => this.RunZonesAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc />
    protected override void OnMonsterDied(object? sender, DeathInformation e)
    {
        base.OnMonsterDied(sender, e);
        if (sender is Monster monster && this.Day != ImperialGuardianEventDefinition.Sunday)
        {
            _ = this.DropFragmentsAsync(monster, e);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnObjectAddedToMapAsync((GameMap Map, ILocateable Object) args)
    {
        await base.OnObjectAddedToMapAsync(args).ConfigureAwait(false);
        if (args.Object is not Player player)
        {
            return;
        }

        var blockedAreas = this._gates.Keys.Where(gate => gate.IsBlocking && gate.IsAlive).Select(gate => gate.GetBlockedArea()).ToList();
        if (blockedAreas.Count > 0)
        {
            await player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p => p.ChangeAttributesAsync(TerrainAttributeType.Blocked, true, blockedAreas)).ConfigureAwait(false);
        }

        await this.ShowZoneAsync(player).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        await base.GameEndedAsync(finishers).ConfigureAwait(false);
        this._gates.Clear();
        this._zoneMonsters.Clear();
    }

    private static void Multiply(AttackableNpcBase npc, AttributeDefinition attribute, float multiplier)
    {
        if (Math.Abs(multiplier - 1) > 0.001f)
        {
            npc.Attributes.AddElement(new SimpleElement(multiplier, AggregateType.Multiplicate), attribute);
        }
    }

    private async Task RunTimerLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.SpawnGatesAsync(0).ConfigureAwait(false);
            using var timer = new PeriodicTimer(TimerInterval);
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                var remaining = this._timerEndsAtUtc - DateTime.UtcNow;
                var type = this._timerType;
                var monsterCount = this.RemainingMonsterCount;
                await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p =>
                    p.ShowTimerAsync(type, remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero, monsterCount)).AsTask()).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in the timer loop.", this);
        }
    }

    private async Task RunZonesAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (var zone = 0; zone < this.ZoneCount; zone++)
            {
                if (zone > 0)
                {
                    Volatile.Write(ref this._zone, zone);
                    await this.SpawnGatesAsync(zone).ConfigureAwait(false);
                    this.SetTimer(ImperialGuardianTimerType.Standby, this._definition.StandbyDuration);
                    await this.ForEachPlayerAsync(player => this.ShowZoneAsync(player).AsTask()).ConfigureAwait(false);
                    await this.DelayWithSkipAsync(this._definition.StandbyDuration, cancellationToken).ConfigureAwait(false);
                }

                this.SetTimer(ImperialGuardianTimerType.TimeAttack, this._definition.ZoneDuration);
                this.SetGatesAttackable(this._definition.EarlyAttackableGateNumbers);
                await this.SpawnMonstersAsync(zone).ConfigureAwait(false);

                var isLastZone = zone == this.ZoneCount - 1;
                if (!await this.WaitUntilZoneIsClearedAsync(isLastZone, cancellationToken).ConfigureAwait(false))
                {
                    await this.FailAsync().ConfigureAwait(false);
                    return;
                }

                this.SetGatesAttackable(this._definition.LateAttackableGateNumbers);
                if (isLastZone)
                {
                    await this.CompleteAsync().ConfigureAwait(false);
                    return;
                }

                this.Logger.LogDebug("{context}: Zone {zone} cleared.", this, zone);
                await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p =>
                    p.ShowResultAsync(ImperialGuardianResult.ZoneCleared, 0)).AsTask()).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error during the zones.", this);
        }
    }

    /// <summary>
    /// Waits until the monsters of the zone are killed.
    /// </summary>
    /// <returns><c>true</c>, if the monsters were killed in time; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> WaitUntilZoneIsClearedAsync(bool isLastZone, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimerInterval);
        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            if (this.PlayerCount == 0)
            {
                return false;
            }

            var isCleared = this.RemainingMonsterCount == 0;
            var isTimeOver = DateTime.UtcNow >= this._timerEndsAtUtc;
            if (isCleared && (isLastZone || this._definition.StartNextZoneWhenCleared || isTimeOver))
            {
                return true;
            }

            if (isTimeOver)
            {
                return false;
            }
        }

        return false;
    }

    private async ValueTask CompleteAsync()
    {
        this.Logger.LogInformation("{context}: The event has been completed.", this);
        this.SetTimer(ImperialGuardianTimerType.LootTime, this.Definition.ExitDuration);
        await this.ForEachPlayerAsync(async player =>
        {
            var playerLevel = (int)((player.Attributes?[Stats.Level] ?? 0) + (player.Attributes?[Stats.MasterLevel] ?? 0));
            var experience = this._definition.GetExperienceReward(playerLevel, this.Day);
            if (experience > 0)
            {
                await player.AddExperienceAsync(experience, null).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p => p.ShowResultAsync(ImperialGuardianResult.Success, experience)).ConfigureAwait(false);
        }).ConfigureAwait(false);

        this.FinishEvent();
    }

    private async ValueTask FailAsync()
    {
        this.Logger.LogInformation("{context}: The event failed in zone {zone}.", this, this.Zone);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p =>
            p.ShowResultAsync(ImperialGuardianResult.Failed, 0)).AsTask()).ConfigureAwait(false);
        this.FinishEvent();
    }

    private void SetTimer(ImperialGuardianTimerType type, TimeSpan duration)
    {
        this._timerType = type;
        this._timerEndsAtUtc = DateTime.UtcNow.Add(duration);
    }

    private ValueTask ShowZoneAsync(Player player)
    {
        var remaining = this._timerEndsAtUtc - DateTime.UtcNow;
        return player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p =>
            p.ShowEnterResultAsync(ImperialGuardianEnterResult.Success, this.Day, this.Zone, this._weather, remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero));
    }

    private void SetGatesAttackable(ICollection<short> monsterNumbers)
    {
        foreach (var gate in this._gates.Keys.Where(gate => monsterNumbers.Contains(gate.Definition.Number)))
        {
            gate.IsAttackable = true;
        }
    }

    private IEnumerable<MonsterSpawnArea> GetZoneSpawns(int zone)
    {
        var waveNumber = ImperialGuardianEventDefinition.GetWaveNumber(this.Day, zone);
        return this.Map.Definition.MonsterSpawns
            .Where(spawn => spawn.MonsterDefinition is not null
                            && spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart
                            && spawn.WaveNumber == waveNumber);
    }

    private bool IsGate(MonsterDefinition definition)
    {
        var number = definition.Number;
        return this._definition.BlockingGateNumbers.Contains(number)
               || this._definition.EarlyAttackableGateNumbers.Contains(number)
               || this._definition.LateAttackableGateNumbers.Contains(number);
    }

    private async ValueTask SpawnGatesAsync(int zone)
    {
        foreach (var spawnArea in this.GetZoneSpawns(zone).Where(spawn => this.IsGate(spawn.MonsterDefinition!)))
        {
            for (var i = 0; i < spawnArea.Quantity; i++)
            {
                var isBlocking = this._definition.BlockingGateNumbers.Contains(spawnArea.MonsterDefinition!.Number);
                var gate = new ImperialGuardianGate(spawnArea, spawnArea.MonsterDefinition, this.Map, this, this.DropGenerator, this._gameContext.PlugInManager, isBlocking);
                this.ApplyScaling(gate);
                if (!await this.AddNpcAsync(gate).ConfigureAwait(false))
                {
                    continue;
                }

                this._gates.TryAdd(gate, 0);
                gate.Died += this.OnGateDied;
                if (isBlocking)
                {
                    await this.ChangeTerrainAsync(gate.GetBlockedArea(), true).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask SpawnMonstersAsync(int zone)
    {
        foreach (var spawnArea in this.GetZoneSpawns(zone).Where(spawn => !this.IsGate(spawn.MonsterDefinition!)))
        {
            var definition = spawnArea.MonsterDefinition!;
            for (var i = 0; i < spawnArea.Quantity; i++)
            {
                if (this._definition.TrapNumbers.Contains(definition.Number))
                {
                    await this.AddNpcAsync(new Trap(spawnArea, definition, this.Map, new RandomAttackInRangeTrapIntelligence(this.Map))).ConfigureAwait(false);
                    continue;
                }

                var monster = new Monster(spawnArea, definition, this.Map, this.DropGenerator, new BasicMonsterIntelligence(), this._gameContext.PlugInManager, this._gameContext.PathFinderPool, this);
                this.ApplyScaling(monster);
                if (await this.AddNpcAsync(monster).ConfigureAwait(false))
                {
                    this._zoneMonsters.TryAdd(monster, 0);
                }
            }
        }

        this.Logger.LogDebug("{context}: Zone {zone} started with {count} monsters.", this, zone, this._zoneMonsters.Count);
    }

    private async ValueTask<bool> AddNpcAsync(NonPlayerCharacter npc)
    {
        try
        {
            npc.Initialize();
            await this.Map.AddAsync(npc).ConfigureAwait(false);
            npc.OnSpawn();
            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Couldn't spawn {npc}.", this, npc.Definition);
            npc.Dispose();
            return false;
        }
    }

    private void ApplyScaling(AttackableNpcBase npc)
    {
        if (this._monsterScaling is not { } scaling)
        {
            return;
        }

        // The multipliers have to be applied before the npc is initialized, which sets its health.
        Multiply(npc, Stats.Level, scaling.LevelMultiplier);
        Multiply(npc, Stats.MaximumHealth, scaling.HealthMultiplier);
        Multiply(npc, Stats.MinimumPhysBaseDmg, scaling.DamageMultiplier);
        Multiply(npc, Stats.MaximumPhysBaseDmg, scaling.DamageMultiplier);
        Multiply(npc, Stats.DefenseBase, scaling.DefenseMultiplier);
    }

    private void OnGateDied(object? sender, DeathInformation e)
    {
        if (sender is not ImperialGuardianGate gate)
        {
            return;
        }

        gate.Died -= this.OnGateDied;
        if (gate.IsBlocking)
        {
            _ = this.OpenGateAsync(gate);
        }
    }

    private async Task OpenGateAsync(ImperialGuardianGate gate)
    {
        try
        {
            await this.ChangeTerrainAsync(gate.GetBlockedArea(), false).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Couldn't open the gate {gate}.", this, gate);
        }
    }

    private async ValueTask ChangeTerrainAsync((byte StartX, byte StartY, byte EndX, byte EndY) area, bool setBlocked)
    {
        for (var x = area.StartX; x <= area.EndX; x++)
        {
            for (var y = area.StartY; y <= area.EndY; y++)
            {
                this.Map.Terrain.ApplyTerrainAttribute(x, y, TerrainAttributeType.Blocked, setBlocked);
            }
        }

        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p =>
            p.ChangeAttributesAsync(TerrainAttributeType.Blocked, setBlocked, [area])).AsTask()).ConfigureAwait(false);
    }

    private async Task DropFragmentsAsync(Monster boss, DeathInformation e)
    {
        try
        {
            await this.DropFragmentItemsAsync(boss, e).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Couldn't drop the fragments of {boss}.", this, boss);
        }
    }

    private async ValueTask DropFragmentItemsAsync(Monster boss, DeathInformation e)
    {
        if (this._definition.FragmentDrops.FirstOrDefault(drop => drop.MonsterNumber == boss.Definition.Number) is not { } fragmentDrop
            || this._gameContext.Configuration.Items.FirstOrDefault(item => item is { Group: 14 } && item.Number == fragmentDrop.ItemNumber) is not { } fragment)
        {
            return;
        }

        var owners = this.Map.GetObject(e.KillerId) is Player killer ? killer.GetAsEnumerable() : [];
        var count = this._definition.GetFragmentCount(Rand.NextInt(0, 100));
        for (var i = 0; i < count; i++)
        {
            var item = new TemporaryItem { Definition = fragment, Durability = 1 };
            var position = i == 0 ? boss.Position : this.Map.Terrain.GetRandomCoordinate(boss.Position, 1);
            await this.Map.AddAsync(new DroppedItem(item, position, this.Map, null, owners)).ConfigureAwait(false);
        }

        this.Logger.LogDebug("{context}: {boss} dropped {count} {fragment}.", this, boss, count, fragment.Name);
    }
}
