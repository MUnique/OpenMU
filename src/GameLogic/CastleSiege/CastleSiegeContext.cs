// <copyright file="CastleSiegeContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Holds the runtime state of Castle Siege for one game context.
/// </summary>
/// <remarks>
/// Runtime registrations and <see cref="ExecutionLock"/> are scoped to this game context. Deployments which run
/// multiple game-server processes require external coordination when registrations are changed concurrently.
/// </remarks>
public class CastleSiegeContext : IEventStateProvider
{
    private static readonly TimeSpan JoinSideEffectDuration = TimeSpan.FromDays(7);

    private readonly IGameContext _gameContext;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Player, CastleSiegeJoinSide> _notifiedPlayerJoinSides = new();
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Player, byte> _siegeMapPlayers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeContext"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="configuration">The Castle Siege configuration.</param>
    public CastleSiegeContext(IGameContext gameContext, CastleSiegeConfiguration configuration)
    {
        this._gameContext = gameContext;
        this.Configuration = configuration;
        this.Schedule = new CastleSiegeSchedule(configuration.StateSchedule);
        this.NpcController = new CastleSiegeNpcController(this);
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    public CastleSiegeState CurrentState { get; internal set; }

    /// <summary>
    /// Gets the UTC time at which the current state started.
    /// </summary>
    public DateTime StateStartTimeUtc { get; internal set; }

    /// <summary>
    /// Gets the UTC time at which the current state ends.
    /// </summary>
    public DateTime StateEndTimeUtc { get; internal set; }

    /// <summary>
    /// Gets the Castle Siege configuration.
    /// </summary>
    public CastleSiegeConfiguration Configuration { get; }

    /// <summary>
    /// Gets the persistent Castle Siege state.
    /// </summary>
    public CastleSiegeData SiegeData { get; private set; } = null!;

    /// <summary>
    /// Gets the guild registrations keyed by their persistent guild identifier.
    /// </summary>
    public System.Collections.Concurrent.ConcurrentDictionary<Guid, CastleSiegeGuildRegistration> RegisteredGuilds { get; } = new();

    /// <summary>
    /// Gets the selected guilds keyed by their runtime guild identifier.
    /// </summary>
    public System.Collections.Concurrent.ConcurrentDictionary<uint, CastleSiegeGuildParticipant> FinalGuildList { get; } = new();

    /// <summary>
    /// Gets the assigned join sides keyed by persistent character identifier.
    /// </summary>
    public System.Collections.Concurrent.ConcurrentDictionary<Guid, CastleSiegeJoinSide> PlayerJoinSides { get; } = new();

    /// <summary>
    /// Gets the participating characters keyed by their persistent character identifier.
    /// </summary>
    public System.Collections.Concurrent.ConcurrentDictionary<Guid, CastleSiegeParticipant> ParticipantTracking { get; } = new();

    /// <summary>
    /// Gets or sets the runtime identifier of the guild which currently owns the castle during the battle.
    /// </summary>
    public uint? MiddleOwnerGuildId { get; set; }

    /// <summary>
    /// Gets the active Castle Siege NPCs.
    /// </summary>
    public List<CastleSiegeNpcRuntime> ActiveNpcs { get; } = new();

    /// <summary>
    /// Gets the controller for Castle Siege NPC lifecycle and lookup operations.
    /// </summary>
    public CastleSiegeNpcController NpcController { get; }

    /// <summary>
    /// Gets or sets the player which currently operates the Crown.
    /// </summary>
    public Player? CrownUser { get; set; }

    /// <summary>
    /// Gets the players which currently operate the Crown switches.
    /// </summary>
    public Player?[] SwitchUsers { get; } = new Player?[2];

    /// <summary>
    /// Gets or sets the accumulated Crown operation time.
    /// </summary>
    public TimeSpan CrownAccumulatedTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Crown can currently be operated.
    /// </summary>
    public bool IsCrownAvailable { get; set; }

    /// <inheritdoc />
    public bool IsEventRunning => this.CurrentState == CastleSiegeState.Start;

    /// <summary>
    /// Gets the remaining time of the current state.
    /// </summary>
    public TimeSpan RemainingTime => this.GetRemainingTime(DateTime.UtcNow);

    /// <summary>
    /// Gets or sets the player whose active Crown attempt was announced to the client.
    /// </summary>
    internal Player? PreviousCrownUser { get; set; }

    /// <summary>
    /// Gets or sets the UTC time of the previous Crown progress update.
    /// </summary>
    internal DateTime LastCrownUpdateUtc { get; set; }

    /// <summary>
    /// Gets the switch information which was last broadcast to the siege map, keyed by network object identifier.
    /// </summary>
    internal Dictionary<ushort, CastleSiegeSwitchInfo> LastBroadcastSwitchInfos { get; } = [];

    /// <summary>
    /// Gets or sets the Crown availability which was last broadcast to the siege map.
    /// </summary>
    internal bool? LastBroadcastCrownAvailability { get; set; }

    /// <summary>
    /// Gets a value indicating whether the context has been initialized.
    /// </summary>
    internal bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the game context.
    /// </summary>
    internal IGameContext GameContext => this._gameContext;

    /// <summary>
    /// Gets the configured weekly schedule.
    /// </summary>
    internal CastleSiegeSchedule Schedule { get; }

    /// <summary>
    /// Gets the lock which prevents overlapping timer executions for this context.
    /// </summary>
    internal SemaphoreSlim ExecutionLock { get; } = new(1, 1);

    /// <summary>
    /// Gets or sets the next regular notification time.
    /// </summary>
    internal DateTime NextNotificationUtc { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Gets or sets the next NPC persistence time.
    /// </summary>
    internal DateTime NextNpcSaveUtc { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Gets or sets the next economy persistence time.
    /// </summary>
    internal DateTime NextEconomySaveUtc { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Gets or sets a value indicating whether the in-memory economy state needs to be persisted.
    /// </summary>
    internal bool IsEconomyPersistencePending { get; set; }

    /// <summary>
    /// Gets or sets the next participant tracking time.
    /// </summary>
    internal DateTime NextParticipantUpdateUtc { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Gets the Ready-state countdown values which have already been sent.
    /// </summary>
    internal HashSet<int> SentReadyCountdownMinutes { get; } = new();

    /// <summary>
    /// Gets or sets the last force-state request processed by this context.
    /// </summary>
    internal int LastForceRequestVersion { get; set; }

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber) => false;

    /// <summary>
    /// Gets the side of a player in the current Castle Siege.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The assigned side, or <see cref="CastleSiegeJoinSide.None"/>.</returns>
    public CastleSiegeJoinSide GetPlayerJoinSide(Player player)
    {
        if (this.Configuration.CastleSiegeMapDefinition?.Number != player.CurrentMap?.Definition.Number)
        {
            return CastleSiegeJoinSide.None;
        }

        return this.GetTrackedPlayerJoinSide(player);
    }

    /// <summary>
    /// Assigns the current Castle Siege side to all online players on the Castle Siege map.
    /// </summary>
    /// <returns>A task that represents the asynchronous synchronization operation.</returns>
    public async ValueTask SetPlayerJoinSideAsync()
    {
        if (this.Configuration.CastleSiegeMapDefinition is null)
        {
            return;
        }

        var activeCharacterIds = new HashSet<Guid>();
        var activePlayers = new HashSet<Player>();
        foreach (var player in this.GetSiegePlayers())
        {
            if (player.SelectedCharacter is not { } character)
            {
                continue;
            }

            activeCharacterIds.Add(character.Id);
            activePlayers.Add(player);
            await this.SynchronizePlayerJoinSideAsync(player).ConfigureAwait(false);
        }

        foreach (var player in this._notifiedPlayerJoinSides.Keys.Where(player => !activePlayers.Contains(player)))
        {
            await this.ClearPlayerJoinSideAsync(player).ConfigureAwait(false);
        }

        foreach (var characterId in this.PlayerJoinSides.Keys.Where(id => !activeCharacterIds.Contains(id)))
        {
            this.PlayerJoinSides.TryRemove(characterId, out _);
        }
    }

    /// <summary>
    /// Loads the persistent Castle Siege state and registrations.
    /// </summary>
    public async ValueTask LoadAsync()
    {
        this.SiegeData = await this.LoadDataAsync().ConfigureAwait(false)
            ?? throw new InvalidOperationException("The persistent Castle Siege state does not exist.");
        this.NpcController.InitializePersistentStructures();
        await this.LoadRegistrationsAsync().ConfigureAwait(false);
        await this.LoadFinalGuildListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the persistent Castle Siege owner and economy state.
    /// </summary>
    public async ValueTask SaveOwnerAsync()
    {
        if (this.SiegeData is null)
        {
            return;
        }

        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeData), false, this._gameContext.Configuration);
        var persistentData = await context.GetByIdAsync<CastleSiegeData>(this.SiegeData.Id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("The persistent Castle Siege state no longer exists.");

        CopyScalarState(this.SiegeData, persistentData);
        await context.SaveChangesAsync().ConfigureAwait(false);
        this.IsEconomyPersistencePending = false;
    }

    /// <summary>
    /// Replaces the persistent selected-guild list with the current runtime snapshot.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async ValueTask SaveFinalGuildListAsync()
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            this._gameContext.Configuration);
        var persistentData = await context.GetByIdAsync<CastleSiegeData>(this.SiegeData.Id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("The persistent Castle Siege state no longer exists.");

        foreach (var oldGuild in persistentData.Guilds.ToList())
        {
            await context.DeleteAsync(oldGuild).ConfigureAwait(false);
        }

        persistentData.Guilds.Clear();
        foreach (var guild in CastleSiegeGuildSelector.OrderFinalGuilds(this.FinalGuildList.Values))
        {
            var persistentGuild = context.CreateNew<CastleSiegeGuild>();
            persistentGuild.GuildId = guild.PersistentGuildId;
            persistentGuild.GuildName = guild.GuildName;
            persistentGuild.Side = guild.Side;
            persistentGuild.Score = guild.Score;
            persistentGuild.IsAllianceMaster = guild.IsAllianceMaster;
            persistentData.Guilds.Add(persistentGuild);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the persistent NPC states from a complete runtime snapshot.
    /// </summary>
    public async ValueTask SaveNpcStatesAsync()
    {
        if (this.SiegeData is null)
        {
            return;
        }

        this.NpcController.SynchronizeNpcStates();
        var persistedNpcs = this.NpcController.GetRuntimeSnapshot()
            .Where(npc => npc.Definition.IsPersistedToDatabase)
            .ToList();
        if (persistedNpcs.Count == 0 || persistedNpcs.Any(npc => npc.PersistedState is null))
        {
            return;
        }

        var statesToSave = persistedNpcs
            .Select(npc => npc.PersistedState!)
            .ToDictionary(GetNpcKey);

        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeData), false, this._gameContext.Configuration);
        var persistentData = await context.GetByIdAsync<CastleSiegeData>(this.SiegeData.Id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("The persistent Castle Siege state no longer exists.");

        foreach (var target in persistentData.NpcStates.ToList())
        {
            if (statesToSave.Remove(GetNpcKey(target), out var source))
            {
                CopyNpcState(source, target);
            }
            else
            {
                await context.DeleteAsync(target).ConfigureAwait(false);
                persistentData.NpcStates.Remove(target);
            }
        }

        foreach (var source in statesToSave.Values)
        {
            var target = context.CreateNew<CastleSiegeNpcState>();
            CopyNpcState(source, target);
            persistentData.NpcStates.Add(target);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes all guild registrations of the completed cycle.
    /// </summary>
    public async ValueTask ClearRegistrationsAsync()
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeGuildRegistration), false, this._gameContext.Configuration);
        foreach (var registration in (await context.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false)).ToList())
        {
            await context.DeleteAsync(registration).ConfigureAwait(false);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        this.RegisteredGuilds.Clear();
    }

    /// <summary>
    /// Tracks a player after a map entry operation.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="map">The entered map.</param>
    internal void TrackPlayer(Player player, GameMap map)
    {
        if (this.Configuration.CastleSiegeMapDefinition?.Number == map.Definition.Number)
        {
            this._siegeMapPlayers[player] = 0;
        }
        else
        {
            this._siegeMapPlayers.TryRemove(player, out _);
        }
    }

    /// <summary>
    /// Stops tracking a player after a map exit operation.
    /// </summary>
    /// <param name="player">The player.</param>
    internal void UntrackPlayer(Player player)
    {
        this._siegeMapPlayers.TryRemove(player, out _);
    }

    /// <summary>
    /// Initializes the current battle owner from the selected defending alliance.
    /// A later Crown capture replaces this value with the attacking guild identifier.
    /// </summary>
    internal void InitializeBattleOwner()
    {
        this.MiddleOwnerGuildId = this.FinalGuildList.Values
                                      .FirstOrDefault(guild => guild.Side == CastleSiegeJoinSide.Defense
                                                               && guild.PersistentGuildId == this.SiegeData.OwnerGuildId)
                                      ?.GuildId
                                  ?? this.FinalGuildList.Values
                                      .FirstOrDefault(guild => guild.Side == CastleSiegeJoinSide.Defense && guild.IsAllianceMaster)
                                      ?.GuildId;
    }

    /// <summary>
    /// Gets the previously assigned side without requiring the player to still be on the siege map.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The assigned side, or <see cref="CastleSiegeJoinSide.None"/>.</returns>
    internal CastleSiegeJoinSide GetTrackedPlayerJoinSide(Player player)
    {
        if (player.SelectedCharacter is { } character
            && this.PlayerJoinSides.TryGetValue(character.Id, out var assignedSide))
        {
            return assignedSide;
        }

        return player.GuildStatus is { } guildStatus
               && this.FinalGuildList.TryGetValue(guildStatus.GuildId, out var participatingGuild)
            ? participatingGuild.Side
            : CastleSiegeJoinSide.None;
    }

    /// <summary>
    /// Gets a snapshot of players currently tracked on the Castle Siege map.
    /// </summary>
    /// <returns>The tracked players which are still on the Castle Siege map.</returns>
    internal IReadOnlyList<Player> GetSiegePlayers()
    {
        var mapNumber = this.Configuration.CastleSiegeMapDefinition?.Number;
        return this._siegeMapPlayers.Keys
            .Where(player => player.CurrentMap?.Definition.Number == mapNumber)
            .ToList();
    }

    /// <summary>
    /// Executes an action concurrently for players currently on the Castle Siege map.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that represents the asynchronous fan-out operation.</returns>
    internal async ValueTask ForEachSiegePlayerAsync(Func<Player, Task> action)
    {
        var actions = this.GetSiegePlayers().Select(action);
        await Task.WhenAll(actions).ConfigureAwait(false);
    }

    /// <summary>
    /// Assigns and sends the current Castle Siege side to a player entering the battle map.
    /// </summary>
    /// <param name="player">The player to synchronize.</param>
    /// <returns>A task that represents the asynchronous synchronization operation.</returns>
    internal async ValueTask SynchronizePlayerJoinSideAsync(Player player)
    {
        if (player.SelectedCharacter is not { } character
            || this.Configuration.CastleSiegeMapDefinition?.Number != player.CurrentMap?.Definition.Number)
        {
            return;
        }

        var side = await this.ResolvePlayerJoinSideAsync(player).ConfigureAwait(false);
        var notificationRequired = !this._notifiedPlayerJoinSides.TryGetValue(player, out var notifiedSide)
                                   || notifiedSide != side;
        this.PlayerJoinSides[character.Id] = side;
        await SetJoinSideMagicEffectAsync(player, side).ConfigureAwait(false);
        if (!notificationRequired)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<ICastleSiegeJoinSidePlugIn>(
                plugIn => plugIn.ShowJoinSideAsync(side))
            .ConfigureAwait(false);
        this._notifiedPlayerJoinSides[player] = side;
    }

    /// <summary>
    /// Clears a player's Castle Siege side and its client-visible effect.
    /// </summary>
    /// <param name="player">The player to clear.</param>
    /// <returns>A task that represents the asynchronous clear operation.</returns>
    internal async ValueTask ClearPlayerJoinSideAsync(Player player)
    {
        if (player.SelectedCharacter is { } character)
        {
            this.PlayerJoinSides.TryRemove(character.Id, out _);
        }

        this._notifiedPlayerJoinSides.TryRemove(player, out _);
        await SetJoinSideMagicEffectAsync(player, CastleSiegeJoinSide.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears all cached player-side assignments and connection notifications.
    /// </summary>
    internal void ClearPlayerJoinSides()
    {
        this.PlayerJoinSides.Clear();
        this._notifiedPlayerJoinSides.Clear();
    }

    /// <summary>
    /// Creates and persists a guild registration.
    /// </summary>
    /// <param name="guildId">The persistent guild identifier.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The created registration.</returns>
    internal async ValueTask<CastleSiegeGuildRegistration> AddRegistrationAsync(Guid guildId, string guildName)
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            this._gameContext.Configuration);
        var registration = context.CreateNew<CastleSiegeGuildRegistration>();
        registration.GuildId = guildId;
        registration.GuildName = guildName;

        // Registration orders stay monotonic for one cycle and are intentionally not compacted after unregistration.
        registration.RegistrationOrder = this.RegisteredGuilds.IsEmpty
            ? 1
            : this.RegisteredGuilds.Values.Max(entry => entry.RegistrationOrder) + 1;
        await context.SaveChangesAsync().ConfigureAwait(false);
        this.RegisteredGuilds[guildId] = registration;
        return registration;
    }

    /// <summary>
    /// Deletes a persisted guild registration.
    /// </summary>
    /// <param name="registration">The registration.</param>
    internal async ValueTask RemoveRegistrationAsync(CastleSiegeGuildRegistration registration)
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            this._gameContext.Configuration);
        if (await context.GetByIdAsync<CastleSiegeGuildRegistration>(registration.Id).ConfigureAwait(false) is { } persistentRegistration)
        {
            await context.DeleteAsync(persistentRegistration).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        this.RegisteredGuilds.TryRemove(registration.GuildId, out _);
    }

    /// <summary>
    /// Increments and persists the submitted mark count.
    /// </summary>
    /// <param name="registration">The registration.</param>
    /// <returns>The updated mark count, or <see langword="null"/> if the registration no longer exists.</returns>
    internal async ValueTask<int?> IncrementMarksAsync(CastleSiegeGuildRegistration registration)
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            this._gameContext.Configuration);
        if (await context.GetByIdAsync<CastleSiegeGuildRegistration>(registration.Id).ConfigureAwait(false) is not { } persistentRegistration)
        {
            this.RegisteredGuilds.TryRemove(registration.GuildId, out _);
            return null;
        }

        persistentRegistration.Marks++;
        await context.SaveChangesAsync().ConfigureAwait(false);
        registration.Marks = persistentRegistration.Marks;
        return registration.Marks;
    }

    /// <summary>
    /// Initializes the context at the state which contains <paramref name="utcNow"/>.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>The initialized state period.</returns>
    internal async ValueTask<CastleSiegeStatePeriod> InitializeAsync(DateTime utcNow)
    {
        this.SiegeData = await this.LoadDataAsync().ConfigureAwait(false)
            ?? await this.CreateDataAsync().ConfigureAwait(false);
        this.NpcController.InitializePersistentStructures();
        await this.LoadRegistrationsAsync().ConfigureAwait(false);
        await this.LoadFinalGuildListAsync().ConfigureAwait(false);
        var period = this.Schedule.GetCurrentEventPeriod(utcNow);
        this.SetPeriod(period);
        await this.InitializeSiegeMapPlayersAsync().ConfigureAwait(false);
        this.NextNpcSaveUtc = utcNow.AddMinutes(2);
        this.IsInitialized = true;
        return period;
    }

    /// <summary>
    /// Sets the current state period.
    /// </summary>
    /// <param name="period">The state period.</param>
    internal void SetPeriod(CastleSiegeStatePeriod period)
    {
        this.CurrentState = period.State;
        this.StateStartTimeUtc = period.StartUtc;
        this.StateEndTimeUtc = period.EndUtc;
        this.NextNotificationUtc = DateTime.MaxValue;
        this.SentReadyCountdownMinutes.Clear();
    }

    /// <summary>
    /// Gets the remaining time at the specified UTC time.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>The remaining time.</returns>
    internal TimeSpan GetRemainingTime(DateTime utcNow)
    {
        var remaining = this.StateEndTimeUtc - utcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    private static void CopyScalarState(CastleSiegeData source, CastleSiegeData target)
    {
        target.OwnerGuildId = source.OwnerGuildId;
        target.IsOccupied = source.IsOccupied;
        target.TaxChaos = source.TaxChaos;
        target.TaxStore = source.TaxStore;
        target.TaxHunt = source.TaxHunt;
        target.IsHuntZoneEnabled = source.IsHuntZoneEnabled;
        target.TributeMoney = source.TributeMoney;
    }

    private static void CopyNpcState(CastleSiegeNpcState source, CastleSiegeNpcState target)
    {
        target.MonsterNumber = source.MonsterNumber;
        target.InstanceId = source.InstanceId;
        target.DefenseLevel = source.DefenseLevel;
        target.RegenLevel = source.RegenLevel;
        target.LifeLevel = source.LifeLevel;
        target.CurrentHp = source.CurrentHp;
    }

    private static (short MonsterNumber, byte InstanceId) GetNpcKey(CastleSiegeNpcState state)
    {
        return (state.MonsterNumber, state.InstanceId);
    }

    private static CastleSiegeMagicEffectNumber? GetMagicEffectNumber(CastleSiegeJoinSide side)
    {
        return side switch
        {
            CastleSiegeJoinSide.Defense => CastleSiegeMagicEffectNumber.Defense,
            CastleSiegeJoinSide.Attack1 => CastleSiegeMagicEffectNumber.Attack1,
            CastleSiegeJoinSide.Attack2 => CastleSiegeMagicEffectNumber.Attack2,
            CastleSiegeJoinSide.Attack3 => CastleSiegeMagicEffectNumber.Attack3,
            _ => null,
        };
    }

    private static bool IsJoinSideEffect(short effectNumber)
    {
        return Enum.IsDefined<CastleSiegeMagicEffectNumber>((CastleSiegeMagicEffectNumber)effectNumber);
    }

    private static async ValueTask SetJoinSideMagicEffectAsync(Player player, CastleSiegeJoinSide side)
    {
        var expectedEffectNumber = GetMagicEffectNumber(side);
        var activeSideEffects = (await player.MagicEffectList
                .GetActiveEffectsSnapshotAsync()
                .ConfigureAwait(false))
            .Where(effect => IsJoinSideEffect(effect.Id))
            .ToList();
        foreach (var effect in activeSideEffects)
        {
            if (effect.Id != (short?)expectedEffectNumber)
            {
                await effect.DisposeAsync().ConfigureAwait(false);
            }
        }

        if (expectedEffectNumber is not { } expected
            || activeSideEffects.Any(effect => effect.Id == (short)expected))
        {
            return;
        }

        if (player.GameContext.Configuration.MagicEffects.FirstOrDefault(
                effect => effect.Number == (short)expected) is not { } effectDefinition)
        {
            return;
        }

        await player.MagicEffectList
            .AddEffectAsync(new MagicEffect(JoinSideEffectDuration, effectDefinition))
            .ConfigureAwait(false);
    }

    private async ValueTask InitializeSiegeMapPlayersAsync()
    {
        this._siegeMapPlayers.Clear();
        if (this.Configuration.CastleSiegeMapDefinition is not { } siegeMapDefinition)
        {
            return;
        }

        foreach (var player in await this._gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.CurrentMap?.Definition.Number == siegeMapDefinition.Number)
            {
                this._siegeMapPlayers[player] = 0;
            }
        }
    }

    private async ValueTask<CastleSiegeData?> LoadDataAsync()
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeData), false, this._gameContext.Configuration);
        return (await context.GetAsync<CastleSiegeData>().ConfigureAwait(false)).FirstOrDefault();
    }

    private async ValueTask<CastleSiegeData> CreateDataAsync()
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeData), false, this._gameContext.Configuration);
        var data = context.CreateNew<CastleSiegeData>();
        await context.SaveChangesAsync().ConfigureAwait(false);
        return data;
    }

    private async ValueTask LoadRegistrationsAsync()
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CastleSiegeGuildRegistration), false, this._gameContext.Configuration);
        var registrations = await context.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false);
        this.RegisteredGuilds.Clear();
        foreach (var registration in registrations)
        {
            this.RegisteredGuilds[registration.GuildId] = registration;
        }
    }

    private async ValueTask LoadFinalGuildListAsync()
    {
        this.FinalGuildList.Clear();
        if (this._gameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        foreach (var guild in this.SiegeData.Guilds)
        {
            var runtimeGuildId = await gameServerContext.GuildServer
                .GetGuildIdAsync(guild.GuildId)
                .ConfigureAwait(false);
            if (runtimeGuildId == 0)
            {
                continue;
            }

            this.FinalGuildList[runtimeGuildId] = new CastleSiegeGuildParticipant
            {
                GuildId = runtimeGuildId,
                PersistentGuildId = guild.GuildId,
                GuildName = guild.GuildName,
                Side = guild.Side,
                Score = guild.Score,
                IsAllianceMaster = guild.IsAllianceMaster,
            };
        }

        this.InitializeBattleOwner();
    }

    private async ValueTask<CastleSiegeJoinSide> ResolvePlayerJoinSideAsync(Player player)
    {
        if (player.GuildStatus is not { } guildStatus)
        {
            return CastleSiegeJoinSide.None;
        }

        if (this.FinalGuildList.TryGetValue(guildStatus.GuildId, out var runtimeGuild))
        {
            return runtimeGuild.Side;
        }

        if (this._gameContext is not IGameServerContext gameServerContext
            || await gameServerContext.GuildServer
                .GetPersistentGuildIdAsync(guildStatus.GuildId)
                .ConfigureAwait(false) is not { } persistentGuildId)
        {
            return CastleSiegeJoinSide.None;
        }

        var persistedEntry = this.FinalGuildList.FirstOrDefault(
            entry => entry.Value.PersistentGuildId == persistentGuildId);
        if (persistedEntry.Value is null)
        {
            return CastleSiegeJoinSide.None;
        }

        this.FinalGuildList.TryRemove(persistedEntry.Key, out _);
        persistedEntry.Value.GuildId = guildStatus.GuildId;
        this.FinalGuildList[guildStatus.GuildId] = persistedEntry.Value;
        return persistedEntry.Value.Side;
    }
}
