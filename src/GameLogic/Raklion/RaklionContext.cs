// <copyright file="RaklionContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The context of the raklion event of a game server.
/// </summary>
/// <remarks>
/// Unlike the mini games, the event takes place on the (only) instance of the hatchery map, which is
/// entered through a gate of the raklion map. Its run is:
/// <list type="number">
///   <item>The spider eggs appear in the hatchery (<see cref="RaklionState.Idle"/>).</item>
///   <item>When all eggs are destroyed, Selupan appears (<see cref="RaklionState.Standby"/> to <see cref="RaklionState.StartBattle"/>).</item>
///   <item>After some time, the hatchery gets closed. Only the players inside can fight against Selupan (<see cref="RaklionState.CloseDoor"/>).</item>
///   <item>When Selupan is killed or all players died or left, the battle ends (<see cref="RaklionState.Notify4"/>).</item>
///   <item>After some time, the hatchery opens again and the spider eggs appear (<see cref="RaklionState.End"/>).</item>
/// </list>
/// </remarks>
public sealed class RaklionContext : IEventStateProvider, IDisposable
{
    private readonly GameContext _gameContext;
    private readonly ILogger<RaklionContext> _logger;
    private readonly ConcurrentDictionary<Monster, byte> _spiderEggs = new();
    private readonly ConcurrentDictionary<Monster, byte> _summonedMonsters = new();

    /// <summary>
    /// The players which entered the hatchery while it was open. Only they're allowed to stay in the hatchery.
    /// </summary>
    private readonly ConcurrentDictionary<Player, byte> _battlePlayers = new();

    private RaklionEventDefinition _definition;
    private GameMap? _raklionMap;
    private GameMap? _hatcheryMap;
    private Monster? _selupan;
    private DateTime _stateStart = DateTime.UtcNow;
    private bool _areFewEggsNotified;
    private bool _arePlayersRemoved;
    private int _isSelupanDead;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaklionContext"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="definition">The definition of the event.</param>
    public RaklionContext(GameContext gameContext, RaklionEventDefinition definition)
    {
        this._gameContext = gameContext;
        this._definition = definition;
        this._logger = gameContext.LoggerFactory.CreateLogger<RaklionContext>();
    }

    /// <summary>
    /// Gets the current state of the event.
    /// </summary>
    public RaklionState State { get; private set; } = RaklionState.End;

    /// <summary>
    /// Gets the current state of Selupan.
    /// </summary>
    public SelupanState SelupanState { get; private set; }

    /// <summary>
    /// Gets a value indicating whether players can enter the hatchery.
    /// </summary>
    public bool CanEnterHatchery => this.State is < RaklionState.CloseDoor;

    /// <summary>
    /// Gets the number of the remaining spider eggs.
    /// </summary>
    public int RemainingSpiderEggs => this._spiderEggs.Keys.Count(egg => egg.IsAlive);

    /// <inheritdoc />
    public bool IsEventRunning => this.State is >= RaklionState.Standby and < RaklionState.End;

    /// <summary>
    /// Gets a value indicating whether Selupan can summon monsters, because some of them are missing.
    /// </summary>
    internal bool CanSummon => this.GetMissingSummons().Any();

    private bool IsSelupanDead => Volatile.Read(ref this._isSelupanDead) != 0;

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber) => false;

    /// <summary>
    /// Initializes the context by registering at the maps of the event.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        this._raklionMap = await this._gameContext.GetMapAsync((ushort)this._definition.RaklionMapNumber).ConfigureAwait(false);
        this._hatcheryMap = await this._gameContext.GetMapAsync((ushort)this._definition.HatcheryMapNumber).ConfigureAwait(false);
        if (this._raklionMap is { } raklionMap)
        {
            raklionMap.ObjectAdded += this.OnObjectAddedToRaklionAsync;
        }

        if (this._hatcheryMap is { } hatcheryMap)
        {
            hatcheryMap.ObjectAdded += this.OnObjectAddedToHatcheryAsync;
            hatcheryMap.ObjectRemoved += this.OnObjectRemovedFromHatcheryAsync;
        }
        else
        {
            this._logger.LogWarning("The hatchery map {map} of the raklion event wasn't found.", this._definition.HatcheryMapNumber);
        }
    }

    /// <summary>
    /// Gets the remaining time of the current state, if the state has a duration.
    /// </summary>
    /// <returns>The remaining time.</returns>
    public TimeSpan GetRemainingTime()
    {
        var duration = this.State switch
        {
            RaklionState.Standby => this._definition.SelupanAppearanceDelay,
            RaklionState.StartBattle => this._definition.HatcheryCloseDelay,
            RaklionState.Notify4 => this._definition.HatcheryOpenDelay,
            _ => TimeSpan.Zero,
        };
        var remaining = duration - (DateTime.UtcNow - this._stateStart);
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Executes one step of the event. It's called every second.
    /// </summary>
    public async ValueTask TickAsync()
    {
        if (this._hatcheryMap is null)
        {
            return;
        }

        await this.RemoveUnauthorizedPlayersAsync().ConfigureAwait(false);

        var elapsed = DateTime.UtcNow - this._stateStart;
        switch (this.State)
        {
            case RaklionState.End:
                await this.StartAsync().ConfigureAwait(false);
                break;
            case RaklionState.Idle:
                await this.CheckSpiderEggsAsync().ConfigureAwait(false);
                break;
            case RaklionState.Standby when elapsed >= this._definition.SelupanAppearanceDelay:
                await this.SpawnSelupanAsync().ConfigureAwait(false);
                break;
            case RaklionState.StartBattle when this.IsSelupanDead:
                await this.EndBattleAsync(true).ConfigureAwait(false);
                break;
            case RaklionState.StartBattle when elapsed >= this._definition.HatcheryCloseDelay:
                await this.CloseHatcheryAsync().ConfigureAwait(false);
                break;
            case RaklionState.CloseDoor when this.IsSelupanDead:
                await this.EndBattleAsync(true).ConfigureAwait(false);
                break;
            case RaklionState.CloseDoor when this._battlePlayers.IsEmpty:
                await this.EndBattleAsync(false).ConfigureAwait(false);
                break;
            case RaklionState.Notify4 when !this._arePlayersRemoved && elapsed >= this._definition.PlayerRemovalDelay:
                // The remaining players are moved to the entrance of raklion by the next tick.
                this._arePlayersRemoved = true;
                this._battlePlayers.Clear();
                break;
            case RaklionState.Notify4 when elapsed >= this._definition.HatcheryOpenDelay:
                await this.ChangeStateAsync(RaklionState.End).ConfigureAwait(false);
                break;
            default:
                // nothing to do
                break;
        }
    }

    /// <summary>
    /// Updates the definition of the event, e.g. after it has been changed in the admin panel.
    /// It takes effect for the following states and the next appearance of Selupan.
    /// </summary>
    /// <param name="definition">The definition.</param>
    public void UpdateDefinition(RaklionEventDefinition definition)
    {
        this._definition = definition;
    }

    /// <summary>
    /// Forces the event to proceed to the next state, e.g. by a game master.
    /// </summary>
    public void SkipWaitingTime()
    {
        this._stateStart = DateTime.MinValue;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._raklionMap is { } raklionMap)
        {
            raklionMap.ObjectAdded -= this.OnObjectAddedToRaklionAsync;
        }

        if (this._hatcheryMap is { } hatcheryMap)
        {
            hatcheryMap.ObjectAdded -= this.OnObjectAddedToHatcheryAsync;
            hatcheryMap.ObjectRemoved -= this.OnObjectRemovedFromHatcheryAsync;
        }
    }

    /// <summary>
    /// Changes the state of Selupan and shows it to the players.
    /// </summary>
    /// <param name="selupanState">The state of Selupan.</param>
    internal async ValueTask ChangeSelupanStateAsync(SelupanState selupanState)
    {
        this.SelupanState = selupanState;
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowSelupanStateAsync(selupanState)).AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Summons the missing monsters of the summon wave.
    /// </summary>
    internal async ValueTask SummonMonstersAsync()
    {
        foreach (var spawnArea in this.GetMissingSummons().ToList())
        {
            if (await this.SpawnMonsterAsync(spawnArea, new BasicMonsterIntelligence()).ConfigureAwait(false) is { } monster)
            {
                this._summonedMonsters.TryAdd(monster, 0);
            }
        }
    }

    private static async ValueTask RemoveMonsterAsync(Monster monster)
    {
        if (monster.IsAlive)
        {
            await monster.CurrentMap.RemoveAsync(monster).ConfigureAwait(false);
            monster.Dispose();
        }
    }

    private IEnumerable<MonsterSpawnArea> GetWaveSpawns(byte waveNumber)
    {
        return this._hatcheryMap?.Definition.MonsterSpawns
                   .Where(spawn => spawn.MonsterDefinition is not null
                                   && spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart
                                   && spawn.WaveNumber == waveNumber)
               ?? [];
    }

    private IEnumerable<MonsterSpawnArea> GetMissingSummons()
    {
        foreach (var spawnArea in this.GetWaveSpawns(this._definition.SummonWaveNumber))
        {
            var aliveCount = this._summonedMonsters.Keys.Count(monster => monster.IsAlive && monster.SpawnArea == spawnArea);
            for (var i = aliveCount; i < spawnArea.Quantity; i++)
            {
                yield return spawnArea;
            }
        }
    }

    private async ValueTask StartAsync()
    {
        await this.RemoveEventMonstersAsync().ConfigureAwait(false);
        Volatile.Write(ref this._isSelupanDead, 0);
        this._areFewEggsNotified = false;
        this._arePlayersRemoved = false;
        this._battlePlayers.Clear();
        this.SelupanState = SelupanState.None;

        foreach (var spawnArea in this.GetWaveSpawns(this._definition.SpiderEggWaveNumber))
        {
            for (var i = 0; i < spawnArea.Quantity; i++)
            {
                if (await this.SpawnMonsterAsync(spawnArea, new BasicMonsterIntelligence()).ConfigureAwait(false) is { } egg)
                {
                    this._spiderEggs.TryAdd(egg, 0);
                }
            }
        }

        this._logger.LogInformation("Raklion event started with {count} spider eggs.", this._spiderEggs.Count);
        await this.ChangeStateAsync(RaklionState.Idle).ConfigureAwait(false);
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.RaklionHatcheryOpened)).ConfigureAwait(false);
    }

    private async ValueTask CheckSpiderEggsAsync()
    {
        var remainingEggs = this.RemainingSpiderEggs;
        if (remainingEggs == 0)
        {
            this._spiderEggs.Clear();
            await this.ChangeStateAsync(RaklionState.Standby).ConfigureAwait(false);
            return;
        }

        if (remainingEggs <= this._definition.SpiderEggNotifyCount && !this._areFewEggsNotified)
        {
            this._areFewEggsNotified = true;
            await this.ChangeStateAsync(RaklionState.Notify1).ConfigureAwait(false);
            await this.ChangeStateAsync(RaklionState.Idle).ConfigureAwait(false);
        }
    }

    private async ValueTask SpawnSelupanAsync()
    {
        await this.ChangeStateAsync(RaklionState.Notify2).ConfigureAwait(false);
        var minutes = (int)Math.Ceiling(this._definition.HatcheryCloseDelay.TotalMinutes);
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.RaklionSelupanAppeared), minutes).ConfigureAwait(false);

        await this.ChangeStateAsync(RaklionState.Ready).ConfigureAwait(false);
        var spawnArea = this.GetWaveSpawns(this._definition.SelupanWaveNumber).FirstOrDefault();
        if (spawnArea is null)
        {
            this._logger.LogWarning("The spawn of Selupan isn't configured at the wave {wave} of the hatchery map.", this._definition.SelupanWaveNumber);
        }
        else
        {
            var intelligence = new SelupanIntelligence(this, this._definition, this._logger);
            if (await this.SpawnMonsterAsync(spawnArea, intelligence).ConfigureAwait(false) is { } selupan)
            {
                this._selupan = selupan;
                selupan.Died += this.OnSelupanDied;
            }
        }

        await this.ChangeSelupanStateAsync(SelupanState.Standby).ConfigureAwait(false);
        await this.ChangeStateAsync(RaklionState.StartBattle).ConfigureAwait(false);
        if (this._selupan is null)
        {
            // The event can't be continued without Selupan.
            Volatile.Write(ref this._isSelupanDead, 1);
        }
    }

    private async ValueTask CloseHatcheryAsync()
    {
        await this.ChangeStateAsync(RaklionState.Notify3).ConfigureAwait(false);
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.RaklionHatcheryClosed)).ConfigureAwait(false);
        await this.ChangeStateAsync(RaklionState.CloseDoor).ConfigureAwait(false);
    }

    private async ValueTask EndBattleAsync(bool success)
    {
        if (!success)
        {
            await this.ChangeStateAsync(RaklionState.AllUserDie).ConfigureAwait(false);
            await this.RemoveBossMonstersAsync().ConfigureAwait(false);
        }
        else
        {
            // Selupan is dead, the summoned monsters disappear.
            await this.RemoveBossMonstersAsync().ConfigureAwait(false);
        }

        this._logger.LogInformation("Raklion battle ended, success: {success}.", success);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowBattleResultAsync(success)).AsTask()).ConfigureAwait(false);
        await this.ChangeStateAsync(RaklionState.Notify4).ConfigureAwait(false);
        var minutes = (int)Math.Ceiling(this._definition.HatcheryOpenDelay.TotalMinutes);
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.RaklionHatcheryOpensIn), minutes).ConfigureAwait(false);
    }

    private async ValueTask ChangeStateAsync(RaklionState state)
    {
        this.State = state;
        this._stateStart = DateTime.UtcNow;
        this._logger.LogDebug("Raklion state changed to {state}.", state);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowStateChangeAsync(state)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask<Monster?> SpawnMonsterAsync(MonsterSpawnArea spawnArea, INpcIntelligence intelligence)
    {
        var map = this._hatcheryMap!;
        var monster = new Monster(spawnArea, spawnArea.MonsterDefinition!, map, this._gameContext.DropGenerator, intelligence, this._gameContext.PlugInManager, this._gameContext.PathFinderPool, this);
        try
        {
            monster.Initialize();
            await map.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();
            return monster;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't spawn {monster} of the raklion event.", spawnArea.MonsterDefinition);
            monster.Dispose();
            return null;
        }
    }

    private async ValueTask RemoveBossMonstersAsync()
    {
        if (this._selupan is { } selupan)
        {
            this._selupan = null;
            selupan.Died -= this.OnSelupanDied;

            await RemoveMonsterAsync(selupan).ConfigureAwait(false);
        }

        foreach (var monster in this._summonedMonsters.Keys)
        {
            await RemoveMonsterAsync(monster).ConfigureAwait(false);
        }

        this._summonedMonsters.Clear();
    }

    private async ValueTask RemoveEventMonstersAsync()
    {
        await this.RemoveBossMonstersAsync().ConfigureAwait(false);
        foreach (var egg in this._spiderEggs.Keys)
        {
            await RemoveMonsterAsync(egg).ConfigureAwait(false);
        }

        this._spiderEggs.Clear();
    }

    private void OnSelupanDied(object? sender, DeathInformation e)
    {
        Volatile.Write(ref this._isSelupanDead, 1);
        _ = this.OnSelupanDiedAsync(e.KillerName);
    }

    private async Task OnSelupanDiedAsync(string killerName)
    {
        try
        {
            await this.ChangeSelupanStateAsync(SelupanState.Dead).ConfigureAwait(false);
            await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.RaklionSelupanKilled), killerName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when Selupan died.");
        }
    }

    private async ValueTask OnObjectAddedToRaklionAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            await this.ShowCurrentStateAsync(player).ConfigureAwait(false);
        }
    }

    private async ValueTask OnObjectAddedToHatcheryAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is not Player player)
        {
            return;
        }

        if (this.CanEnterHatchery)
        {
            this._battlePlayers.TryAdd(player, 0);
        }
        else if (!this._battlePlayers.ContainsKey(player))
        {
            // The player is moved out of the hatchery by the next tick.
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.RaklionHatcheryIsClosed)).ConfigureAwait(false);
            return;
        }

        await this.ShowCurrentStateAsync(player).ConfigureAwait(false);
    }

    private ValueTask OnObjectRemovedFromHatcheryAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            this._battlePlayers.TryRemove(player, out _);
        }

        return ValueTask.CompletedTask;
    }

    private async ValueTask ShowCurrentStateAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowCurrentStateAsync(this.State)).ConfigureAwait(false);
        if (this.SelupanState != SelupanState.None)
        {
            await player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowSelupanStateAsync(this.SelupanState)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Moves the players out of the hatchery which aren't allowed to stay, like the original game does.
    /// These are players which entered while it was closed, or which stayed after the battle.
    /// </summary>
    private async ValueTask RemoveUnauthorizedPlayersAsync()
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players)
        {
            if (player.CurrentMap != this._hatcheryMap
                || !player.IsAlive
                || player.PlayerState.CurrentState != PlayerState.EnteredWorld
                || this._battlePlayers.ContainsKey(player))
            {
                continue;
            }

            try
            {
                await player.WarpToSafezoneAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't move {player} out of the hatchery.", player);
            }
        }
    }

    /// <summary>
    /// Shows a golden message to all players of the game server, like the original game does.
    /// </summary>
    private async ValueTask ShowMessageToAllPlayersAsync(string messageKey, params object?[] arguments)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players)
        {
            try
            {
                await player.ShowLocalizedGoldenMessageAsync(messageKey, arguments).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't notify {player} about the raklion event.", player);
            }
        }
    }

    private async ValueTask ForEachPlayerAsync(Func<Player, Task> action)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players.Where(player => player.CurrentMap is { } map && (map == this._raklionMap || map == this._hatcheryMap)))
        {
            try
            {
                await action(player).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't notify {player} about the raklion event.", player);
            }
        }
    }
}
