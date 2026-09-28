// <copyright file="CrywolfContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The context of the crywolf event of a game server.
/// </summary>
/// <remarks>
/// Unlike the mini games, the event takes place on the (only) instance of the crywolf map. Its run is:
/// <list type="number">
///   <item>The players are notified about the upcoming attack of the army of Balgass (<see cref="CrywolfState.Notify1"/>).</item>
///   <item>The fortress prepares for the war: the common monsters and NPCs of the map disappear (<see cref="CrywolfState.Notify2"/>).</item>
///   <item>The army appears and the elves contract the altars, which protect the statue (<see cref="CrywolfState.Ready"/>).</item>
///   <item>The army attacks. Later, Balgass appears. When he's killed, the fortress has been defended (<see cref="CrywolfState.Start"/>).</item>
///   <item>The result is shown, and the players get experience depending on their score (<see cref="CrywolfState.End"/>).</item>
///   <item>The army disappears and the common monsters return. The heroes are shown (<see cref="CrywolfState.EndCycle"/>).</item>
/// </list>
/// The result of the event (<see cref="Occupation"/>) is kept until the next event. Like in the original game,
/// it's saved in the database (<see cref="CrywolfData"/>), so that it survives a restart of the server.
/// </remarks>
public sealed class CrywolfContext : IEventStateProvider, IDisposable
{
    private static readonly TimeSpan AltarInfoInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan BossMonsterInfoInterval = TimeSpan.FromSeconds(5);

    private readonly GameContext _gameContext;
    private readonly ILogger<CrywolfContext> _logger;
    private readonly ConcurrentDictionary<NonPlayerCharacter, CrywolfEffectDisplay> _hiddenNpcs = new();
    private readonly ConcurrentDictionary<byte, CrywolfMonsterGroupState> _groups = new();
    private readonly ConcurrentDictionary<Monster, byte> _eventMonsters = new();
    private readonly ConcurrentDictionary<Monster, byte> _removedMonsters = new();
    private readonly ConcurrentDictionary<Player, int> _scores = new();
    private readonly List<Player> _heroes = new();

    private CrywolfEventDefinition _definition;
    private GameMap? _map;
    private NonPlayerCharacter? _statue;
    private Monster? _balgass;
    private string? _balgassKillerName;
    private DateTime _stateStart = DateTime.UtcNow;
    private DateTime _lastNotification = DateTime.MinValue;
    private DateTime _lastAltarInfo = DateTime.MinValue;
    private DateTime _lastBossMonsterInfo = DateTime.MinValue;
    private int _lastRemainingTimeStep = -1;
    private int _contractedAltarCount;
    private bool _isBalgassAppearanceDone;
    private bool _isStatueAttackStarted;
    private bool _isStartForced;
    private int _isSkipRequested;
    private IReadOnlyList<CrywolfAltar> _altars = [];
    private bool _isMissingConfigurationLogged;
    private int _isBalgassDead;
    private Guid? _dataId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfContext"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="definition">The definition of the event.</param>
    public CrywolfContext(GameContext gameContext, CrywolfEventDefinition definition)
    {
        this._gameContext = gameContext;
        this._definition = definition;
        this._logger = gameContext.LoggerFactory.CreateLogger<CrywolfContext>();
    }

    /// <summary>
    /// Gets the current state of the event.
    /// </summary>
    public CrywolfState State { get; private set; } = CrywolfState.None;

    /// <summary>
    /// Gets the occupation state of the fortress.
    /// </summary>
    public CrywolfOccupationState Occupation { get; private set; } = CrywolfOccupationState.Peace;

    /// <summary>
    /// Gets the stage of the army: 0, when it doesn't move, 1 when it advances, and 2 when it attacks the statue.
    /// </summary>
    public CrywolfArmyStage ArmyStage => this.State != CrywolfState.Start
        ? CrywolfArmyStage.Waiting
        : this._isStatueAttackStarted ? CrywolfArmyStage.AttackingStatue : CrywolfArmyStage.Advancing;

    /// <summary>
    /// Gets the altars.
    /// </summary>
    public IReadOnlyList<CrywolfAltar> Altars => this._altars;

    /// <summary>
    /// Gets the definition of the event.
    /// </summary>
    public CrywolfEventDefinition Definition => this._definition;

    /// <inheritdoc />
    public bool IsEventRunning => this.State is >= CrywolfState.Notify2 and <= CrywolfState.End;

    /// <summary>
    /// Gets a value indicating whether the battle is running.
    /// </summary>
    public bool IsBattleRunning => this.State == CrywolfState.Start;

    /// <summary>
    /// Gets the remaining time of the current state.
    /// </summary>
    public TimeSpan RemainingTime
    {
        get
        {
            var remaining = this._definition.GetDuration(this.State) - (DateTime.UtcNow - this._stateStart);
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Gets the crywolf map.
    /// </summary>
    internal GameMap? Map => this._map;

    private bool IsBalgassDead => Volatile.Read(ref this._isBalgassDead) != 0;

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber) => false;

    /// <summary>
    /// Initializes the context by registering at the map of the event.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        this._map = await this._gameContext.GetMapAsync((ushort)this._definition.MapNumber).ConfigureAwait(false);
        if (this._map is null)
        {
            this._logger.LogWarning("The map {map} of the crywolf event wasn't found.", this._definition.MapNumber);
            return;
        }

        await this.LoadOccupationAsync().ConfigureAwait(false);
        this._map.ObjectAdded += this.OnObjectAddedToMapAsync;
        this.InitializeNpcs();
        await this.ApplyOccupationAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Executes one step of the event. It's called every second.
    /// </summary>
    public async ValueTask TickAsync()
    {
        if (this._map is null)
        {
            return;
        }

        if (this._altars.Count == 0)
        {
            this.InitializeNpcs();
        }

        if (this._altars.Count != this._definition.AltarNumbers.Count || this._statue is null)
        {
            // Without the altars and the statue (e.g. when the data update wasn't applied yet), the event can't run.
            if (!this._isMissingConfigurationLogged)
            {
                this._isMissingConfigurationLogged = true;
                this._logger.LogWarning("The crywolf event doesn't run, because the statue and the altars aren't spawned on the map {map}.", this._definition.MapNumber);
            }

            return;
        }

        if (Interlocked.Exchange(ref this._isSkipRequested, 0) != 0)
        {
            this.Skip();
        }

        var elapsed = DateTime.UtcNow - this._stateStart;
        switch (this.State)
        {
            case CrywolfState.None:
                await this.CheckStartAsync().ConfigureAwait(false);
                break;
            case CrywolfState.Notify1:
                await this.NotifyAsync(player => player.GetLocalizedMessage(nameof(PlayerMessage.CrywolfNotify1)), true).ConfigureAwait(false);
                await this.ProceedAfterDurationAsync(elapsed, CrywolfState.Notify2).ConfigureAwait(false);
                break;
            case CrywolfState.Notify2:
                await this.RemoveCommonMonstersAsync().ConfigureAwait(false);
                await this.NotifyRemainingTimeAsync(nameof(PlayerMessage.CrywolfContractsStartInMinutes), nameof(PlayerMessage.CrywolfContractsStartInSeconds)).ConfigureAwait(false);
                await this.ProceedAfterDurationAsync(elapsed, CrywolfState.Ready).ConfigureAwait(false);
                break;
            case CrywolfState.Ready:
                await this.RemoveCommonMonstersAsync().ConfigureAwait(false);
                await this.UpdateAltarsAsync().ConfigureAwait(false);
                await this.NotifyRemainingTimeAsync(nameof(PlayerMessage.CrywolfAttackStartsInMinutes), nameof(PlayerMessage.CrywolfAttackStartsInSeconds)).ConfigureAwait(false);
                await this.ProceedAfterDurationAsync(elapsed, CrywolfState.Start).ConfigureAwait(false);
                break;
            case CrywolfState.Start:
                await this.RemoveCommonMonstersAsync().ConfigureAwait(false);
                await this.UpdateBattleAsync(elapsed).ConfigureAwait(false);
                break;
            case CrywolfState.End:
                await this.ProceedAfterDurationAsync(elapsed, CrywolfState.EndCycle).ConfigureAwait(false);
                break;
            case CrywolfState.EndCycle:
                await this.ProceedAfterDurationAsync(elapsed, CrywolfState.None).ConfigureAwait(false);
                break;
            default:
                // nothing to do
                break;
        }

        await this.UpdateEffectsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the definition of the event, e.g. after it has been changed in the admin panel.
    /// </summary>
    /// <param name="definition">The definition.</param>
    public void UpdateDefinition(CrywolfEventDefinition definition)
    {
        this._definition = definition;
    }

    /// <summary>
    /// Forces the event to proceed with the next step: it starts when it's not running, and during the battle
    /// Balgass appears if he didn't yet. Otherwise, the current state ends.
    /// </summary>
    public void SkipWaitingTime()
    {
        // It's applied by the next step, so that it doesn't interfere with a running step.
        Interlocked.Exchange(ref this._isSkipRequested, 1);
    }

    /// <summary>
    /// Determines whether the NPC is hidden, because the fortress isn't in peace.
    /// Players can't talk to hidden NPCs.
    /// </summary>
    /// <param name="npc">The NPC.</param>
    /// <returns><c>true</c>, if the NPC is hidden; otherwise, <c>false</c>.</returns>
    public bool IsNpcHidden(NonPlayerCharacter npc)
    {
        return this._hiddenNpcs.TryGetValue(npc, out var display) && display.Effect == CrywolfEffect.NpcHidden;
    }

    /// <summary>
    /// Determines whether the player loses experience when it's killed on the specified map.
    /// During the battle, it doesn't lose experience on the crywolf map.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns><c>true</c>, if the player keeps its experience; otherwise, <c>false</c>.</returns>
    public bool KeepsExperienceOnDeath(GameMap? map)
    {
        return this.IsBattleRunning && map is not null && map == this._map;
    }

    /// <summary>
    /// Gets the additional success rate of the chaos machine crafting, which is shown to the player.
    /// </summary>
    /// <returns>The additional success rate in percent.</returns>
    public byte GetChaosRateBenefit()
    {
        // The benefits are part of a separate change.
        return 0;
    }

    /// <summary>
    /// Shows the current state of the event to a player, which entered the crywolf map.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowCurrentStateAsync(Player player)
    {
        if (player.CurrentMap != this._map)
        {
            // The client loads the terrain of the occupation state for its current map.
            return;
        }

        await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowStateAsync(this.Occupation, this.State)).ConfigureAwait(false);
        if (this.State is CrywolfState.Ready or CrywolfState.Start)
        {
            var (shield, altarStates) = this.GetStatueAndAltarInfo();
            await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowStatueAndAltarsAsync(shield, altarStates)).ConfigureAwait(false);
        }

        if (this.State == CrywolfState.Start)
        {
            // The remaining time isn't sent here, because the client expects it every 20 seconds, and would count the seconds wrong.
            var (balgassHealth, darkElfCount) = this.GetBossMonsterInfo();
            await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowBossMonsterInfoAsync(balgassHealth, darkElfCount)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tries to contract an altar with the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="altarId">The id of the altar, as the client knows it.</param>
    public async ValueTask ContractAltarAsync(Player player, ushort altarId)
    {
        var altar = this._altars.FirstOrDefault(a => a.Npc.Id == altarId);
        if (altar is null)
        {
            return;
        }

        CrywolfContractResult result;
        if (this.State is not (CrywolfState.Ready or CrywolfState.Start))
        {
            // Unlike the original game, the altars can only be contracted while the event is running.
            result = CrywolfContractResult.NotAvailable;
        }
        else
        {
            lock (altar)
            {
                result = altar.TryStartContract(player, this._definition, DateTime.UtcNow);
            }
        }

        var altarNumber = altar.Index + 1;
        switch (result)
        {
            case CrywolfContractResult.AlreadyContracting:
                // The client sent the request again, e.g. by a double click. Nothing changed.
                break;
            case CrywolfContractResult.Success:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractAttemptInfo), (int)this._definition.ContractDelay.TotalSeconds).ConfigureAwait(false);
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfContractAttempt), player.Name, altarNumber).ConfigureAwait(false);
                break;
            case CrywolfContractResult.Cooldown:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfAltarCooldown), altarNumber).ConfigureAwait(false);
                break;
            case CrywolfContractResult.WrongPosition:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractWrongPosition), altarNumber).ConfigureAwait(false);
                break;
            case CrywolfContractResult.Mounted:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractMounted)).ConfigureAwait(false);
                break;
            case CrywolfContractResult.NotAvailable when this.State is not (CrywolfState.Ready or CrywolfState.Start):
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfNotActive)).ConfigureAwait(false);
                break;
            case CrywolfContractResult.NotAvailable:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfAltarNotAvailable), altarNumber).ConfigureAwait(false);
                break;
            default:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractRequirement), this._definition.MinimumContractLevel).ConfigureAwait(false);
                break;
        }

        byte altarState;
        lock (altar)
        {
            altarState = altar.GetClientState(this._definition.ContractsPerAltar);
        }

        var isSuccess = result is CrywolfContractResult.Success or CrywolfContractResult.AlreadyContracting;
        await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowContractResultAsync(isSuccess, altar.Index, altarState)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._map is { } map)
        {
            map.ObjectAdded -= this.OnObjectAddedToMapAsync;
        }
    }

    /// <summary>
    /// Gets the leader of a group, if it's alive.
    /// </summary>
    /// <param name="waveNumber">The wave number of the group.</param>
    /// <returns>The leader of the group.</returns>
    internal Monster? GetLeader(byte waveNumber)
    {
        return this._groups.TryGetValue(waveNumber, out var group) && group.Leader is { IsAlive: true } leader ? leader : null;
    }

    /// <summary>
    /// Revives the dead members of a group, which is done by the leader of the group.
    /// </summary>
    /// <param name="waveNumber">The wave number of the group.</param>
    internal async ValueTask ReviveGroupMembersAsync(byte waveNumber)
    {
        if (!this._groups.TryGetValue(waveNumber, out var group) || this.State != CrywolfState.Start)
        {
            return;
        }

        // The leader revives one of the dead members at a time.
        var now = DateTime.UtcNow;
        if (now - group.LastRevive < this._definition.MemberReviveInterval
            || group.GetDeadMembers().FirstOrDefault() is not { } spawnArea)
        {
            return;
        }

        group.LastRevive = now;
        await this.SpawnMemberAsync(group, spawnArea).ConfigureAwait(false);
    }

    /// <summary>
    /// Lets a ballista bombard the area around its target point.
    /// </summary>
    /// <param name="ballista">The monster of the ballista.</param>
    /// <param name="definition">The definition of the ballista.</param>
    internal async ValueTask BombardAsync(Monster ballista, CrywolfBallista definition)
    {
        if (this._map is not { } map)
        {
            return;
        }

        var spread = this._definition.BallistaTargetSpread;
        var target = new Point(
            (byte)Math.Clamp(definition.TargetX + Rand.NextInt(-spread, spread + 1), 0, byte.MaxValue),
            (byte)Math.Clamp(definition.TargetY + Rand.NextInt(-spread, spread + 1), 0, byte.MaxValue));
        var players = map.GetAttackablesInRange(target, this._definition.BallistaEffectRadius).OfType<Player>().ToList();
        foreach (var player in players)
        {
            await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowBallistaAttackAsync(ballista, target)).ConfigureAwait(false);
        }

        foreach (var player in players.Where(p => p.IsActive() && !p.IsAtSafezone() && p.IsInRange(target, this._definition.BallistaHitRadius)))
        {
            await player.AttackByAsync(ballista, null, false).ConfigureAwait(false);
        }
    }

    private static string GetRankName(int rank)
    {
        return rank switch
        {
            4 => "S",
            3 => "A",
            2 => "B",
            1 => "C",
            _ => "D",
        };
    }

    /// <summary>
    /// Lets the event proceed, like requested by <see cref="SkipWaitingTime"/>.
    /// </summary>
    private void Skip()
    {
        if (this.State == CrywolfState.None)
        {
            this._isStartForced = true;
        }
        else if (this.State == CrywolfState.Start && !this._isBalgassAppearanceDone)
        {
            // The battle continues at the appearance of Balgass, so that the battle against him can be tested.
            this._stateStart = DateTime.UtcNow - this._definition.BalgassAppearanceDelay;
        }
        else
        {
            this._stateStart = DateTime.MinValue;
        }
    }

    private void InitializeNpcs()
    {
        if (this._map is not { } map)
        {
            return;
        }

        var npcs = map.GetNpcsInRange(new Point(128, 128), byte.MaxValue);
        this._statue = npcs.FirstOrDefault(npc => npc.Definition.Number == this._definition.StatueNumber);
        var altars = new List<CrywolfAltar>();
        for (var i = 0; i < this._definition.AltarNumbers.Count; i++)
        {
            var number = this._definition.AltarNumbers[i];
            if (npcs.FirstOrDefault(npc => npc.Definition.Number == number) is { } npc)
            {
                altars.Add(new CrywolfAltar(i, npc));
            }
        }

        // The list is replaced as a whole, because it's read by the requests of the players.
        this._altars = altars;

        foreach (var npc in npcs.Where(this.IsCommonNpc))
        {
            this._hiddenNpcs.TryAdd(npc, new CrywolfEffectDisplay(npc));
        }
    }

    private bool IsCommonNpc(NonPlayerCharacter npc)
    {
        return npc.Definition.ObjectKind != NpcObjectKind.Monster
               && npc.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic
               && npc != this._statue
               && !this._definition.AltarNumbers.Contains(npc.Definition.Number)
               && !this._definition.AlwaysVisibleNpcNumbers.Contains(npc.Definition.Number);
    }

    private async ValueTask CheckStartAsync()
    {
        var isStartTime = this._definition.IsStartTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this._gameContext.ServerTimeZone));
        if (!isStartTime && !this._isStartForced)
        {
            return;
        }

        this._isStartForced = false;
        await this.ChangeStateAsync(CrywolfState.Notify1).ConfigureAwait(false);
    }

    private async ValueTask ProceedAfterDurationAsync(TimeSpan elapsed, CrywolfState nextState)
    {
        if (elapsed >= this._definition.GetDuration(this.State))
        {
            await this.ChangeStateAsync(nextState).ConfigureAwait(false);
        }
    }

    private async ValueTask ChangeStateAsync(CrywolfState state)
    {
        this.State = state;
        this._stateStart = DateTime.UtcNow;
        this._lastNotification = DateTime.MinValue;
        this._logger.LogInformation("Crywolf state changed to {state}.", state);
        switch (state)
        {
            case CrywolfState.Notify2:
                await this.StartWarAsync().ConfigureAwait(false);
                break;
            case CrywolfState.Ready:
                await this.PrepareBattleAsync().ConfigureAwait(false);
                break;
            case CrywolfState.Start:
                await this.StartBattleAsync().ConfigureAwait(false);
                break;
            case CrywolfState.End:
                await this.EndBattleAsync().ConfigureAwait(false);
                break;
            case CrywolfState.EndCycle:
                await this.EndCycleAsync().ConfigureAwait(false);
                break;
            default:
                await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);
                break;
        }
    }

    private async ValueTask StartWarAsync()
    {
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.CrywolfNotify2)).ConfigureAwait(false);
        this.Occupation = CrywolfOccupationState.War;
        await this.ApplyOccupationAsync().ConfigureAwait(false);
        await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);
        await this.RemoveCommonMonstersAsync().ConfigureAwait(false);
    }

    private async ValueTask PrepareBattleAsync()
    {
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.CrywolfReady)).ConfigureAwait(false);
        await this.RemoveEventMonstersAsync().ConfigureAwait(false);
        Volatile.Write(ref this._isBalgassDead, 0);
        this._balgassKillerName = null;
        this._isBalgassAppearanceDone = false;
        this._isStatueAttackStarted = false;
        this._contractedAltarCount = 0;
        this._scores.Clear();
        foreach (var altar in this._altars)
        {
            lock (altar)
            {
                altar.Reset();
            }
        }

        foreach (var group in this._definition.MonsterGroups)
        {
            await this.SpawnGroupAsync(group).ConfigureAwait(false);
        }

        await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);
    }

    private async ValueTask StartBattleAsync()
    {
        await this.ShowMessageToAllPlayersAsync(nameof(PlayerMessage.CrywolfStart)).ConfigureAwait(false);
        await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);
        this._lastAltarInfo = DateTime.MinValue;
        this._lastBossMonsterInfo = DateTime.MinValue;
        this._lastRemainingTimeStep = -1;
        if (this.CountContractedAltars() == 0)
        {
            await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfNoAltarContracted)).ConfigureAwait(false);
            this.Occupation = CrywolfOccupationState.Occupied;
            await this.ChangeStateAsync(CrywolfState.End).ConfigureAwait(false);
        }
    }

    private async ValueTask UpdateBattleAsync(TimeSpan elapsed)
    {
        await this.UpdateAltarsAsync().ConfigureAwait(false);
        if (this._balgass is not null && this.IsBalgassDead)
        {
            // Balgass is checked first, so that the battle is won when the last contract ended at the same time.
            await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBalgassDefeated)).ConfigureAwait(false);
            this.Occupation = CrywolfOccupationState.Peace;
            await this.ChangeStateAsync(CrywolfState.End).ConfigureAwait(false);
            return;
        }

        if (this.CountContractedAltars() == 0)
        {
            await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfNoAltarContracted)).ConfigureAwait(false);
            this.Occupation = CrywolfOccupationState.Occupied;
            await this.ChangeStateAsync(CrywolfState.End).ConfigureAwait(false);
            return;
        }

        var now = DateTime.UtcNow;
        if (now - this._lastBossMonsterInfo >= BossMonsterInfoInterval)
        {
            this._lastBossMonsterInfo = now;
            var (balgassHealth, darkElfCount) = this.GetBossMonsterInfo();
            await this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowBossMonsterInfoAsync(balgassHealth, darkElfCount)).AsTask()).ConfigureAwait(false);
        }

        // The client counts down the seconds of a minute by itself, and expects the remaining time at each 20 seconds of it.
        var remaining = this.RemainingTime;
        var remainingTimeStep = (int)Math.Ceiling(remaining.TotalSeconds / 20);
        if (remainingTimeStep != this._lastRemainingTimeStep)
        {
            this._lastRemainingTimeStep = remainingTimeStep;
            await this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowRemainingTimeAsync(remaining)).AsTask()).ConfigureAwait(false);
        }

        if (!this._isStatueAttackStarted && elapsed >= this._definition.StatueAttackDelay)
        {
            this._isStatueAttackStarted = true;
            await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfStatueAttack)).ConfigureAwait(false);
        }

        if (!this._isBalgassAppearanceDone && elapsed >= this._definition.BalgassAppearanceDelay)
        {
            this._isBalgassAppearanceDone = true;
            await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBalgassAppeared)).ConfigureAwait(false);
            await this.SpawnBalgassAsync().ConfigureAwait(false);
        }

        if (elapsed >= this._definition.BattleDuration)
        {
            if (this._balgass is not null)
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBalgassAlive)).ConfigureAwait(false);
                this.Occupation = CrywolfOccupationState.Occupied;
            }
            else
            {
                // Balgass didn't appear (e.g. because of the configuration), and the altars were protected.
                this.Occupation = CrywolfOccupationState.Peace;
            }

            await this.ChangeStateAsync(CrywolfState.End).ConfigureAwait(false);
        }
    }

    private async ValueTask EndBattleAsync()
    {
        var isDefended = this.Occupation == CrywolfOccupationState.Peace;
        this._logger.LogInformation("Crywolf battle ended, defended: {defended}.", isDefended);
        await this.ShowMessageToAllPlayersAsync(isDefended ? nameof(PlayerMessage.CrywolfDefenseSucceeded) : nameof(PlayerMessage.CrywolfDefenseFailed)).ConfigureAwait(false);

        // The client only shows the result, when the occupation is final.
        await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);

        foreach (var altar in this._altars)
        {
            Player? contractor;
            lock (altar)
            {
                contractor = altar.Contractor;
                altar.Reset();
                altar.Hide();
            }

            if (contractor is null)
            {
                continue;
            }

            this._scores.AddOrUpdate(contractor, this._definition.ContractorScore, (_, score) => score + this._definition.ContractorScore);
            if (isDefended)
            {
                await this.DropRewardAsync(contractor).ConfigureAwait(false);
            }
        }

        await this.ForEachMapPlayerAsync(player => this.RewardExperienceAsync(player, isDefended).AsTask()).ConfigureAwait(false);

        // Unlike the original game, the heroes are shown already at the end of the battle. The client shows
        // them in the result dialog, which it opens after an animation of some frames, so with a higher frame
        // rate than the original client, the heroes wouldn't arrive in time at the end of the event.
        var heroes = this._scores
            .Where(entry => entry.Key.CurrentMap == this._map)
            .OrderByDescending(entry => entry.Value)
            .Take(Math.Min(this._definition.HeroCount, 5))
            .ToList();
        this._heroes.Clear();
        this._heroes.AddRange(heroes.Select(entry => entry.Key));
        var heroList = heroes
            .Select(entry => new CrywolfHero(entry.Key.Name, entry.Value, entry.Key.SelectedCharacter?.CharacterClass?.Number ?? 0))
            .ToList();
        await this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowHeroListAsync(heroList)).AsTask()).ConfigureAwait(false);
        await this.SaveOccupationAsync().ConfigureAwait(false);
    }

    private async ValueTask EndCycleAsync()
    {
        await this.RemoveEventMonstersAsync().ConfigureAwait(false);
        await this.ApplyOccupationAsync().ConfigureAwait(false);
        await this.RestoreCommonMonstersAsync().ConfigureAwait(false);
        await this.ShowStateToMapPlayersAsync().ConfigureAwait(false);

        if (this.Occupation == CrywolfOccupationState.Peace)
        {
            // Like in the original game, the heroes get their reward at the end of the event.
            foreach (var hero in this._heroes)
            {
                await this.DropRewardAsync(hero).ConfigureAwait(false);
            }
        }

        this._heroes.Clear();
        this._scores.Clear();
    }

    private async ValueTask RewardExperienceAsync(Player player, bool isDefended)
    {
        var score = this._scores.GetValueOrDefault(player);
        var rank = this._definition.GetRank(score);
        var experience = this._definition.GetExperience(rank, isDefended);
        if (experience > 0)
        {
            await player.AddExperienceAsync(experience, null).ConfigureAwait(false);
        }

        await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowPersonalRankAsync(rank, experience)).ConfigureAwait(false);
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfRankResult), GetRankName(rank), experience).ConfigureAwait(false);
    }

    private async ValueTask DropRewardAsync(Player player)
    {
        if (player.CurrentMap is not { } map
            || this._gameContext.Configuration.Items.FirstOrDefault(item => item.Group == this._definition.RewardItemGroup && item.Number == this._definition.RewardItemNumber) is not { } definition)
        {
            return;
        }

        var item = new TemporaryItem { Definition = definition, Durability = 1 };
        await map.AddAsync(new DroppedItem(item, player.Position, map, null, player.GetAsEnumerable())).ConfigureAwait(false);
    }

    private async ValueTask UpdateAltarsAsync()
    {
        var now = DateTime.UtcNow;
        foreach (var altar in this._altars)
        {
            Player? contractor;
            CrywolfContractChange change;
            lock (altar)
            {
                contractor = altar.Contractor;
                change = altar.Update(this._definition, now);
            }

            if (contractor is null)
            {
                continue;
            }

            var altarNumber = altar.Index + 1;
            if (change == CrywolfContractChange.Validated)
            {
                await contractor.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractValid), altarNumber).ConfigureAwait(false);
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfContractValidToOthers), contractor.Name, altarNumber).ConfigureAwait(false);
            }
            else if (change == CrywolfContractChange.Cancelled)
            {
                await contractor.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfContractCancelled), altarNumber).ConfigureAwait(false);
            }
            else
            {
                // nothing changed
            }
        }

        var contractedCount = this.CountContractedAltars();
        var previousCount = Interlocked.Exchange(ref this._contractedAltarCount, contractedCount);
        if (contractedCount != previousCount)
        {
            var (shieldPercentage, _) = this.GetStatueAndAltarInfo();
            if (contractedCount == 0)
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBarrierDisappeared)).ConfigureAwait(false);
            }
            else if (previousCount == 0)
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBarrierCreated)).ConfigureAwait(false);
            }
            else
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBarrierStatus), contractedCount, shieldPercentage).ConfigureAwait(false);
            }
        }

        if (now - this._lastAltarInfo >= AltarInfoInterval)
        {
            this._lastAltarInfo = now;
            var (shield, altarStates) = this.GetStatueAndAltarInfo();
            await this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowStatueAndAltarsAsync(shield, altarStates)).AsTask()).ConfigureAwait(false);
        }
    }

    private int CountContractedAltars()
    {
        return this._altars.Count(altar => altar.State == CrywolfAltarState.Contracted);
    }

    /// <summary>
    /// Gets the shield of the statue in percent, which is the health of the elves of the contracted altars, and the states of the altars.
    /// </summary>
    private (int ShieldPercentage, IReadOnlyList<byte> AltarStates) GetStatueAndAltarInfo()
    {
        double health = 0;
        double maximumHealth = 0;
        var altarStates = new List<byte>();
        foreach (var altar in this._altars)
        {
            Player? contractor;
            lock (altar)
            {
                contractor = altar.State == CrywolfAltarState.Contracted ? altar.Contractor : null;
                altarStates.Add(altar.GetClientState(this._definition.ContractsPerAltar));
            }

            if (contractor?.Attributes is { } attributes)
            {
                health += attributes[Stats.CurrentHealth];
                maximumHealth += attributes[Stats.MaximumHealth];
            }
        }

        var percentage = maximumHealth > 0 ? (int)Math.Clamp(health * 100 / maximumHealth, 0, 100) : 0;
        return (percentage, altarStates);
    }

    private (int BalgassHealthPercentage, int DarkElfCount) GetBossMonsterInfo()
    {
        var balgassHealth = -1;
        if (this._balgass is { IsAlive: true } balgass)
        {
            var maximumHealth = balgass.Attributes[Stats.MaximumHealth];
            balgassHealth = maximumHealth > 0 ? (int)Math.Clamp(balgass.Health * 100 / maximumHealth, 1, 100) : 1;
        }

        var darkElfCount = this._groups.Values.Count(group => group.Leader is { IsAlive: true });
        return (balgassHealth, darkElfCount);
    }

    private async ValueTask NotifyAsync(Func<Player, string> getMessage, bool global)
    {
        var now = DateTime.UtcNow;
        if (now - this._lastNotification < this._definition.NotificationInterval)
        {
            return;
        }

        this._lastNotification = now;
        if (global)
        {
            await this.ShowMessageToAllPlayersAsync(getMessage).ConfigureAwait(false);
        }
        else
        {
            await this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(getMessage(player), MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);
        }
    }

    private ValueTask NotifyRemainingTimeAsync(string minutesMessageKey, string secondsMessageKey)
    {
        var remaining = this.RemainingTime;
        return this.NotifyAsync(
            player => remaining >= TimeSpan.FromMinutes(1)
                ? player.GetLocalizedMessage(minutesMessageKey, (int)Math.Ceiling(remaining.TotalMinutes))
                : player.GetLocalizedMessage(secondsMessageKey, (int)Math.Ceiling(remaining.TotalSeconds)),
            true);
    }

    private async ValueTask ApplyOccupationAsync()
    {
        if (this._map is not { } map)
        {
            return;
        }

        // The client loads the terrain of the occupation by itself. The one of the war and the occupation
        // don't differ for the server.
        CrywolfTerrain.Apply(map.Terrain, this.Occupation == CrywolfOccupationState.Peace);
        foreach (var display in this._hiddenNpcs.Values)
        {
            display.Effect = this.Occupation == CrywolfOccupationState.Peace ? null : CrywolfEffect.NpcHidden;
        }

        await this.UpdateEffectsAsync().ConfigureAwait(false);
    }

    private async ValueTask LoadOccupationAsync()
    {
        try
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CrywolfData), false, this._gameContext.Configuration);
            if ((await context.GetAsync<CrywolfData>().ConfigureAwait(false)).FirstOrDefault() is { } data)
            {
                this._dataId = data.Id;
                this.Occupation = data.IsOccupied ? CrywolfOccupationState.Occupied : CrywolfOccupationState.Peace;
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't load the occupation state of the crywolf fortress.");
        }
    }

    private async ValueTask SaveOccupationAsync()
    {
        try
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CrywolfData), false, this._gameContext.Configuration);
            var data = this._dataId is { } id ? await context.GetByIdAsync<CrywolfData>(id).ConfigureAwait(false) : null;
            data ??= context.CreateNew<CrywolfData>();
            data.IsOccupied = this.Occupation != CrywolfOccupationState.Peace;
            data.LastBattleEnd = DateTime.UtcNow;
            await context.SaveChangesAsync().ConfigureAwait(false);
            this._dataId = data.Id;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't save the occupation state of the crywolf fortress.");
        }
    }

    private async ValueTask UpdateEffectsAsync()
    {
        foreach (var altar in this._altars)
        {
            await altar.Display.UpdateAsync().ConfigureAwait(false);
        }

        foreach (var display in this._hiddenNpcs.Values)
        {
            await display.UpdateAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask SpawnGroupAsync(CrywolfMonsterGroup definition)
    {
        var group = new CrywolfMonsterGroupState(definition, this.GetWaveSpawns(definition.WaveNumber).ToList());
        this._groups[definition.WaveNumber] = group;
        foreach (var spawnArea in group.SpawnAreas)
        {
            await this.SpawnMemberAsync(group, spawnArea).ConfigureAwait(false);
        }
    }

    private async ValueTask SpawnMemberAsync(CrywolfMonsterGroupState group, MonsterSpawnArea spawnArea)
    {
        var monsterDefinition = spawnArea.MonsterDefinition!;
        var ballista = this._definition.Ballistas.FirstOrDefault(b => b.X == spawnArea.X1 && b.Y == spawnArea.Y1);
        var role = monsterDefinition.Number == this._definition.DarkElfNumber
            ? CrywolfMonsterRole.Leader
            : ballista is not null ? CrywolfMonsterRole.Ballista : CrywolfMonsterRole.Soldier;
        var intelligence = new CrywolfMonsterIntelligence(this, role, group.Definition, ballista, this._logger);
        if (await this.SpawnMonsterAsync(spawnArea, intelligence).ConfigureAwait(false) is { } monster)
        {
            group.SetMonster(spawnArea, monster, role == CrywolfMonsterRole.Leader);
        }
    }

    private async ValueTask SpawnBalgassAsync()
    {
        var spawnArea = this.GetWaveSpawns(this._definition.BalgassWaveNumber).FirstOrDefault();
        if (spawnArea is null)
        {
            this._logger.LogWarning("The spawn of Balgass isn't configured at the wave {wave} of the crywolf map.", this._definition.BalgassWaveNumber);
            return;
        }

        var intelligence = new CrywolfMonsterIntelligence(this, CrywolfMonsterRole.Balgass, null, null, this._logger);
        if (await this.SpawnMonsterAsync(spawnArea, intelligence).ConfigureAwait(false) is { } balgass)
        {
            this._balgass = balgass;
        }
    }

    private IEnumerable<MonsterSpawnArea> GetWaveSpawns(byte waveNumber)
    {
        return this._map?.Definition.MonsterSpawns
                   .Where(spawn => spawn.MonsterDefinition is not null
                                   && spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart
                                   && spawn.WaveNumber == waveNumber)
               ?? [];
    }

    private async ValueTask<Monster?> SpawnMonsterAsync(MonsterSpawnArea spawnArea, INpcIntelligence intelligence)
    {
        var map = this._map!;
        var monster = new Monster(spawnArea, spawnArea.MonsterDefinition!, map, this._gameContext.DropGenerator, intelligence, this._gameContext.PlugInManager, this._gameContext.PathFinderPool, this);
        try
        {
            monster.Initialize();
            await map.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();
            this._eventMonsters.TryAdd(monster, 0);
            monster.Died += this.OnEventMonsterDied;
            return monster;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't spawn {monster} of the crywolf event.", spawnArea.MonsterDefinition);
            monster.Dispose();
            return null;
        }
    }

    private async ValueTask RemoveEventMonstersAsync()
    {
        this._balgass = null;
        this._groups.Clear();
        foreach (var monster in this._eventMonsters.Keys)
        {
            monster.Died -= this.OnEventMonsterDied;
            if (monster.IsAlive)
            {
                await monster.CurrentMap.RemoveAsync(monster).ConfigureAwait(false);
                monster.Dispose();
            }
        }

        this._eventMonsters.Clear();
    }

    /// <summary>
    /// Removes the common monsters of the map during the war, like the original game does.
    /// It's done in each step, because dead monsters respawn after some time.
    /// </summary>
    private async ValueTask RemoveCommonMonstersAsync()
    {
        if (this._map is not { } map)
        {
            return;
        }

        var commonMonsters = map.GetNpcsInRange(new Point(128, 128), byte.MaxValue)
            .OfType<Monster>()
            .Where(monster => monster.Definition.ObjectKind == NpcObjectKind.Monster
                              && monster.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic
                              && monster.IsAlive)
            .ToList();
        foreach (var monster in commonMonsters)
        {
            await map.RemoveAsync(monster).ConfigureAwait(false);
            this._removedMonsters.TryAdd(monster, 0);
        }
    }

    private async ValueTask RestoreCommonMonstersAsync()
    {
        if (this._map is not { } map)
        {
            return;
        }

        foreach (var monster in this._removedMonsters.Keys)
        {
            try
            {
                monster.Initialize();
                await map.AddAsync(monster).ConfigureAwait(false);
                monster.OnSpawn();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't restore {monster} after the crywolf event.", monster);
            }
        }

        this._removedMonsters.Clear();
    }

    private void OnEventMonsterDied(object? sender, DeathInformation e)
    {
        if (sender is not Monster monster)
        {
            return;
        }

        monster.Died -= this.OnEventMonsterDied;
        this._eventMonsters.TryRemove(monster, out _);
        if (monster == this._balgass)
        {
            this._balgassKillerName = e.KillerName;
            Volatile.Write(ref this._isBalgassDead, 1);
        }

        _ = this.OnEventMonsterDiedAsync(monster, e);
    }

    private async Task OnEventMonsterDiedAsync(Monster monster, DeathInformation e)
    {
        try
        {
            if (this.State != CrywolfState.Start)
            {
                return;
            }

            var killer = this._map?.GetObject(e.KillerId) as Player;
            if (monster == this._balgass)
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfBalgassKilled), this._balgassKillerName ?? string.Empty).ConfigureAwait(false);
            }
            else if (monster.Definition.Number == this._definition.DarkElfNumber)
            {
                await this.ShowMessageToMapPlayersAsync(nameof(PlayerMessage.CrywolfDarkElfKilled), monster.SpawnArea.WaveNumber, e.KillerName).ConfigureAwait(false);
            }
            else
            {
                // no message for the other monsters
            }

            var score = this._definition.GetScore(monster.Definition.Number);
            if (killer is not null && score > 0)
            {
                var newScore = this._scores.AddOrUpdate(killer, score, (_, current) => current + score);
                await killer.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfCurrentScore), newScore).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when {monster} of the crywolf event died.", monster);
        }
    }

    private async ValueTask OnObjectAddedToMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            await this.ShowCurrentStateAsync(player).ConfigureAwait(false);
        }
    }

    private ValueTask ShowStateToMapPlayersAsync()
    {
        var occupation = this.Occupation;
        var state = this.State;
        return this.ForEachMapPlayerAsync(player => player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowStateAsync(occupation, state)).AsTask());
    }

    /// <summary>
    /// Shows a golden message to all players of the game server, like the original game does.
    /// </summary>
    private ValueTask ShowMessageToAllPlayersAsync(string messageKey)
    {
        return this.ShowMessageToAllPlayersAsync(player => player.GetLocalizedMessage(messageKey));
    }

    private async ValueTask ShowMessageToAllPlayersAsync(Func<Player, string> getMessage)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players)
        {
            try
            {
                var message = getMessage(player);
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't notify {player} about the crywolf event.", player);
            }
        }
    }

    /// <summary>
    /// Shows a golden message to the players of the crywolf map.
    /// </summary>
    private ValueTask ShowMessageToMapPlayersAsync(string messageKey, params object?[] arguments)
    {
        return this.ForEachMapPlayerAsync(player =>
        {
            var message = player.GetLocalizedMessage(messageKey, arguments);
            return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter)).AsTask();
        });
    }

    private async ValueTask ForEachMapPlayerAsync(Func<Player, Task> action)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players.Where(player => player.CurrentMap is { } map && map == this._map))
        {
            try
            {
                await action(player).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Couldn't notify {player} about the crywolf event.", player);
            }
        }
    }
}
