// <copyright file="GameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Threading;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// The game context which holds all data of the game together.
/// </summary>
public class GameContext : AsyncDisposable, IGameContext
{
    private const string DefaultExperienceFormula = "if(level == 0, 0, if(level < 256, 10 * (level + 8) * (level - 1) * (level - 1), (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256))))";
    private const string DefaultMasterExperienceFormula = "(505 * level * level * level) + (35278500 * level) + (228045 * level * level)";

    private static readonly Meter Meter = new(MeterName);

    private static readonly Counter<int> PlayerCounter = Meter.CreateCounter<int>("PlayerCount");

    private static readonly Counter<int> MapCounter = Meter.CreateCounter<int>("MapCount");

    private static readonly IObjectPool<PathFinder> PathFinderPoolInstance = new LimitedObjectPool<PathFinder>(new PathFinderPoolingPolicy());

    private readonly Dictionary<ushort, GameMap> _mapList = new();

    private readonly Timer _recoverTimer;

    private readonly IMapInitializer _mapInitializer;

    private readonly AsyncLock _mapInitializerLock = new();

    private readonly Timer _tasksTimer;

    private readonly AsyncReaderWriterLock _playerListLock = new();

    /// <summary>
    /// Keeps the list of all players.
    /// </summary>
    private readonly List<Player> _playerList = new();

    /// <summary>
    /// Keeps the players of <see cref="_playerList"/> which are logged in, see <see cref="LoggedInPlayerCount"/>.
    /// Guarded by <see cref="_loginStateLock"/>.
    /// </summary>
    private readonly HashSet<Player> _loggedInPlayers = new();

    /// <summary>
    /// Keeps the players of <see cref="_playerList"/> which wait for their login, see <see cref="WaitingForLoginCount"/>.
    /// Guarded by <see cref="_loginStateLock"/>.
    /// </summary>
    private readonly HashSet<Player> _waitingPlayers = new();

    /// <summary>
    /// Keeps the players whose states this context counts: the ones of <see cref="_playerList"/>, but
    /// added and removed together with <see cref="_loggedInPlayers"/>. Guarded by <see cref="_loginStateLock"/>.
    /// </summary>
    private readonly HashSet<Player> _countedPlayers = new();

    /// <summary>
    /// The reservations of logins which take over the place of a logged in player, by that player,
    /// see <see cref="TryReserveLoginPlace"/>. Until that player isn't logged in anymore, they don't
    /// take a place of their own. Guarded by <see cref="_loginStateLock"/>.
    /// </summary>
    private readonly Dictionary<Player, LoginPlaceReservation> _takeovers = new();

    /// <summary>
    /// The lock for the counted players, the logged in and waiting players and the reserved places of logins.
    /// It's only held for short, synchronous work, and no other lock is taken while it's held.
    /// </summary>
    private readonly Lock _loginStateLock = new();

    private readonly IDisposable _configChangeHandlerRegistration;

    private readonly ILogger<GameContext> _logger;

    private int _periodicTasksStopped;

    /// <summary>
    /// The number of places which are reserved by logins in progress, see <see cref="TryReserveLoginPlace"/>.
    /// Guarded by <see cref="_loginStateLock"/>.
    /// </summary>
    private int _reservedLoginPlaces;

    /// <summary>
    /// The number of entries of <see cref="_loggedInPlayers"/>, see <see cref="LoggedInPlayerCount"/>.
    /// Changed under <see cref="_loginStateLock"/>, read without it.
    /// </summary>
    private int _loggedInPlayerCount;

    /// <summary>
    /// The number of entries of <see cref="_waitingPlayers"/>, see <see cref="WaitingForLoginCount"/>.
    /// Changed under <see cref="_loginStateLock"/>, read without it.
    /// </summary>
    private int _waitingForLoginCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameContext" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="mapInitializer">The map initializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="changeMediator">The cange mediator.</param>
    public GameContext(GameConfiguration configuration, IPersistenceContextProvider persistenceContextProvider, IMapInitializer mapInitializer, ILoggerFactory loggerFactory, PlugInManager plugInManager, IDropGenerator dropGenerator, IConfigurationChangeMediator changeMediator)
    {
        try
        {
            this.Configuration = configuration;
            this.PersistenceContextProvider = persistenceContextProvider;
            this.PlugInManager = plugInManager;
            this._mapInitializer = mapInitializer;
            this.LoggerFactory = loggerFactory;
            this._logger = loggerFactory.CreateLogger<GameContext>();
            this.DropGenerator = dropGenerator;
            this.ConfigurationChangeMediator = changeMediator;
            this.ItemPowerUpFactory = new ItemPowerUpFactory(loggerFactory.CreateLogger<ItemPowerUpFactory>(), configuration);
            this.PartyManager = new PartyManager(configuration.MaximumPartySize, loggerFactory.CreateLogger<Party>());
            this._recoverTimer = new Timer(this.RecoverTimerElapsed, null, this.Configuration.RecoveryInterval, this.Configuration.RecoveryInterval);
            this._tasksTimer = new Timer(this.ExecutePeriodicTasks, null, 1000, 1000);
            this.FeaturePlugIns = new FeaturePlugInContainer(this.PlugInManager);
            this.MiniGames = new MiniGameManager(this, mapInitializer);
            this.MiniGames.GameMapCreated += (_, map) => this.GameMapCreated?.Invoke(this, map);
            this.MiniGames.GameMapRemoved += (_, map) => this.GameMapRemoved?.Invoke(this, map);
            this._configChangeHandlerRegistration = this.ConfigurationChangeMediator.RegisterObject(this.Configuration, this, this.OnGameConfigurationChangeAsync);
            this.DuelRoomManager = new DuelRoomManager(this.Configuration.DuelConfiguration!);
            this.ExperienceTable = CreateExpTable(this.Configuration.ExperienceFormula ?? DefaultExperienceFormula, this.Configuration.MaximumLevel);
            this.MasterExperienceTable = CreateExpTable(this.Configuration.MasterExperienceFormula ?? DefaultMasterExperienceFormula, this.Configuration.MaximumMasterLevel);
        }
        catch (Exception ex)
        {
            loggerFactory.CreateLogger<GameContext>().LogError(ex, "Unexpected error in constructor of GameContext.");
            throw;
        }
    }

    /// <summary>
    /// Occurs when a game map got created.
    /// </summary>
    public event EventHandler<GameMap>? GameMapCreated;

    /// <summary>
    /// Occurs when a game map got removed.
    /// </summary>
    /// <remarks>
    /// Currently, maps are never removed.
    /// It may make sense to remove unused maps after a certain period.
    /// </remarks>
    public event EventHandler<GameMap>? GameMapRemoved;

    /// <inheritdoc />
    public event EventHandler? LoggedInPlayerCountChanged;

    /// <inheritdoc />
    public virtual float ExperienceRate => this.Configuration.ExperienceRate;

    /// <inheritdoc />
    public virtual float MasterExperienceRate => this.Configuration.MasterExperienceRate;

    /// <inheritdoc />
    public virtual bool PvpEnabled { get; }

    /// <inheritdoc/>
    public GameConfiguration Configuration { get; }

    /// <inheritdoc/>
    public TimeZoneInfo ServerTimeZone { get; set; } = TimeZoneInfo.Utc;

    /// <inheritdoc/>
    public long[] ExperienceTable { get; private set; }

    /// <inheritdoc/>
    public long[] MasterExperienceTable { get; private set; }

    /// <inheritdoc/>
    public IConfigurationChangeMediator ConfigurationChangeMediator { get; }

    /// <inheritdoc/>
    public PlugInManager PlugInManager { get; }

    /// <inheritdoc/>
    public IDropGenerator DropGenerator { get; }

    /// <inheritdoc />
    public IMiniGameManager MiniGames { get; }

    /// <inheritdoc />
    public FeaturePlugInContainer FeaturePlugIns { get; }

    /// <inheritdoc />
    public Offline.OfflinePlayerManager OfflinePlayerManager { get; } = new();

    /// <inheritdoc/>
    public IItemPowerUpFactory ItemPowerUpFactory { get; }

    /// <inheritdoc/>
    public IPersistenceContextProvider PersistenceContextProvider { get; }

    /// <summary>
    /// Gets the players by character name dictionary.
    /// </summary>
    public ConcurrentDictionary<string, Player> PlayersByCharacterName { get; } = new ConcurrentDictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public DuelRoomManager DuelRoomManager { get; set; }

    /// <inheritdoc />
    public ConcurrentDictionary<(Player Attacker, Player Defender), DateTime> SelfDefenseState { get; } = new();

    /// <inheritdoc />
    public IPartyManager PartyManager { get; }

    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the path finder pool.
    /// </summary>
    public IObjectPool<PathFinder> PathFinderPool => PathFinderPoolInstance;

    /// <inheritdoc />
    public int PlayerCount => this._playerList.Count;

    /// <inheritdoc />
    public int LoggedInPlayerCount => Volatile.Read(ref this._loggedInPlayerCount);

    /// <inheritdoc />
    public int WaitingForLoginCount => Volatile.Read(ref this._waitingForLoginCount);

    /// <summary>
    /// Gets a value indicating whether the periodic tasks of this context have been stopped,
    /// e.g. because the hosting server is shutting down. Once stopped, <see cref="ExecutePeriodicTasks"/>
    /// no longer runs any periodic plug-ins.
    /// </summary>
    public bool ArePeriodicTasksStopped => Volatile.Read(ref this._periodicTasksStopped) != 0;

    /// <summary>
    /// Gets the name of the meter of this class.
    /// </summary>
    internal static string MeterName => typeof(GameContext).FullName ?? nameof(GameContext);

    /// <summary>
    /// Gets a value indicating whether a periodic task pass may run right now. It may not once the
    /// context is being (or has been) disposed or its periodic tasks have been stopped, e.g. because
    /// the hosting server is shutting down and a pass would race the shutdown's disconnect loop.
    /// </summary>
    internal bool ShouldExecutePeriodicTasks => !this.IsDisposed && !this.IsDisposing && !this.ArePeriodicTasksStopped;

    /// <summary>
    /// Stops the periodic tasks of this context (the per-second plug-in tasks and the recovery
    /// timer), so that no periodic plug-in runs concurrently with a subsequent teardown, such as
    /// the player disconnect loop of a shutting-down game server. The timers are also stopped on
    /// disposal; calling this earlier only closes the race window between the shutdown start and
    /// the disposal.
    /// </summary>
    public void StopPeriodicTasks()
    {
        if (Interlocked.Exchange(ref this._periodicTasksStopped, 1) == 0)
        {
            this._tasksTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this._recoverTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Gets the initialized maps which are hosted on this context.
    /// </summary>
    public async ValueTask<IEnumerable<GameMap>> GetMapsAsync()
    {
        using var l = await this._mapInitializerLock.LockAsync().ConfigureAwait(false);
        return this._mapList.Values.Concat(this.MiniGames.Maps).ToList();
    }

    /// <inheritdoc/>
    public async ValueTask<GameMap?> GetMapAsync(ushort mapId, bool createIfNotExists = true)
    {
        if (this._mapList.TryGetValue(mapId, out var map))
        {
            return map;
        }

        if (!createIfNotExists)
        {
            return null;
        }

        GameMap? createdMap;
        using (await this._mapInitializerLock.LockAsync().ConfigureAwait(false))
        {
            if (this._mapList.TryGetValue(mapId, out map))
            {
                return map;
            }

            createdMap = this._mapInitializer.CreateGameMap(mapId);
            if (createdMap is null)
            {
                return null;
            }

            this._mapList.Add(mapId, createdMap);
            createdMap.ObjectAdded += async args =>
            {
                if (this.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>() is { } plugInPoint)
                {
                    await plugInPoint.ObjectAddedToMapAsync(args.Map, args.Object).ConfigureAwait(false);
                }
            };
            createdMap.ObjectRemoved += async args =>
            {
                if (this.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>() is { } plugInPoint)
                {
                    await plugInPoint.ObjectRemovedFromMapAsync(args.Map, args.Object).ConfigureAwait(false);
                }
            };
        }

        // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
        await this._mapInitializer.InitializeStateAsync(createdMap).ConfigureAwait(false);
        this.GameMapCreated?.Invoke(this, createdMap);
        MapCounter.Add(1);

        return createdMap;
    }

    /// <summary>
    /// Adds the player to the game.
    /// </summary>
    /// <param name="player">The player.</param>
    public virtual async ValueTask AddPlayerAsync(Player player)
    {
        player.PlayerLeftWorld += this.PlayerLeftWorldAsync;
        player.PlayerEnteredWorld += this.PlayerEnteredWorldAsync;
        player.PlayerDisconnected += this.RemovePlayerAsync;
        lock (this._loginStateLock)
        {
            this._countedPlayers.Add(player);
        }

        using (await this._playerListLock.WriterLockAsync().ConfigureAwait(false))
        {
            this._playerList.Add(player);
        }

        PlayerCounter.Add(1);

        // Offline players, for example, are already logged in when they get added.
        this.UpdateLoggedIn(player);
    }

    /// <inheritdoc />
    public void OnPlayerStateChanged(Player player, State previousState, State currentState)
    {
        // Most state changes happen within the game, e.g. when the player opens an NPC dialog or
        // changes the map. They don't change any count, so they don't need the lock.
        if (previousState.IsLoggedIn() == currentState.IsLoggedIn()
            && previousState.IsWaitingForLogin() == currentState.IsWaitingForLogin())
        {
            return;
        }

        this.UpdateLoggedIn(player);
    }

    /// <inheritdoc />
    public (IDisposable? Reservation, int OccupiedPlaces) TryReserveLoginPlace(int maximumPlayers, Player? replacedPlayer)
    {
        lock (this._loginStateLock)
        {
            // The place of the replaced player is taken over while it's logged in, unless another login
            // takes it over already.
            var takesOver = replacedPlayer is not null
                && this._loggedInPlayers.Contains(replacedPlayer)
                && !this._takeovers.ContainsKey(replacedPlayer);

            // A login adds itself to the logged in players before it releases its reservation, and the
            // logged in players only get fewer otherwise. So, this sum never misses a login which passed.
            var occupiedPlaces = this._loggedInPlayerCount + this._reservedLoginPlaces - (takesOver ? 1 : 0);
            if (occupiedPlaces >= maximumPlayers)
            {
                return (null, occupiedPlaces);
            }

            if (takesOver)
            {
                // It takes a place of its own when the replaced player isn't logged in anymore, see RemoveLoggedInPlayer.
                var takeover = new LoginPlaceReservation(this, replacedPlayer);
                this._takeovers.Add(replacedPlayer!, takeover);
                return (takeover, occupiedPlaces);
            }

            this._reservedLoginPlaces++;
            return (new LoginPlaceReservation(this, null), occupiedPlaces);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IList<Player>> GetPlayersAsync()
    {
        using var l = await this._playerListLock.ReaderLockAsync().ConfigureAwait(false);
        if (this._playerList.Count == 0)
        {
            return [];
        }

        return this._playerList.ToList();
    }

    /// <summary>
    /// Removes the player from the game.
    /// </summary>
    /// <param name="player">The player.</param>
    public virtual async ValueTask RemovePlayerAsync(Player player)
    {
        bool removed;
        using (await this._playerListLock.WriterLockAsync().ConfigureAwait(false))
        {
            removed = this._playerList.Remove(player);
        }

        bool loggedOut;
        lock (this._loginStateLock)
        {
            // Together, so that a state change of the player which runs at the same time can't
            // count it as logged in again after it was removed.
            this._countedPlayers.Remove(player);
            loggedOut = this.RemoveLoggedInPlayer(player);
            this.SetWaitingForLogin(player, false);
        }

        if (loggedOut)
        {
            this.OnLoggedInPlayerCountChanged();
        }

        if (!removed)
        {
            return;
        }

        PlayerCounter.Add(-1);
        if (player.SelectedCharacter != null)
        {
            this.PlayersByCharacterName.TryRemove(player.SelectedCharacter.Name, out _);
        }

        if (player.CurrentMap is { } currentMap)
        {
            // Awaited and guarded: discarding the ValueTask left the removal running in the
            // background - racing the player's own disposal on the dispose-without-disconnect
            // paths - with any exception lost, while this method has to stay exception-free for
            // the teardown paths which call it right before the dispose.
            try
            {
                await currentMap.RemoveAsync(player).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Error while removing player {Player} from its map.", player);
            }
        }

        player.PlayerDisconnected -= this.RemovePlayerAsync;
        player.PlayerEnteredWorld -= this.PlayerEnteredWorldAsync;
        player.PlayerLeftWorld -= this.PlayerLeftWorldAsync;
    }

    /// <summary>
    /// Gets the player by the character name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The player by character name.</returns>
    public Player? GetPlayerByCharacterName(string name)
    {
        this.PlayersByCharacterName.TryGetValue(name, out var player);
        return player;
    }

    /// <inheritdoc />
    public async ValueTask ForEachPlayerAsync(Func<Player, Task> action)
    {
        if (this._playerList.Count == 0)
        {
            return;
        }

        var playerList = await this.GetPlayersAsync().ConfigureAwait(false);
        await playerList.Select(action).WhenAll().ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the specified action for each player, grouped by their culture.
    /// </summary>
    /// <param name="stateFactory">The state factory which creates a state for each culture group.</param>
    /// <param name="action">The action to execute for each player and culture state.</param>
    /// <typeparam name="TCultureState">The type of the culture state.</typeparam>
    public async ValueTask ForEachPlayerGroupedByCultureAsync<TCultureState>(Func<CultureInfo, TCultureState> stateFactory, Func<Player, TCultureState, Task> action)
    {
        if (this._playerList.Count == 0)
        {
            return;
        }

        var playerList = await this.GetPlayersAsync().ConfigureAwait(false);
        await playerList
            .GroupBy(p => p.Culture)
            .SelectMany(g =>
            {
                var state = stateFactory(g.Key);
                return g.Select(player => action(player, state));
            })
            .WhenAll().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShowGlobalLocalizedMessageAsync(MessageType messageType, string messageKey, params object?[] formatArguments)
    {
        await this.ForEachPlayerGroupedByCultureAsync<string>(
            cultureInfo =>
            {
                if (formatArguments.Length > 0)
                {
                    return string.Format(PlayerMessage.ResourceManager.GetString(messageKey, cultureInfo) ?? string.Empty, formatArguments);
                }

                return PlayerMessage.ResourceManager.GetString(messageKey, cultureInfo) ?? string.Empty;
            },
            (player, message) => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, messageType)).AsTask())
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask SendGlobalMessageAsync(string message, MessageType messageType)
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, messageType)).AsTask()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask SendGlobalChatMessageAsync(string sender, string message, ChatMessageType messageType)
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(message, sender, messageType)).AsTask()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask SendGlobalNotificationAsync(string message)
    {
        var sendingMessage = message.TrimStart('!');
        await this.SendGlobalMessageAsync(sendingMessage, MessageType.GoldenCenter).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask SendGlobalNotificationAsync(LocalizedString message, params object?[] formatArguments)
    {
        await this.ForEachPlayerGroupedByCultureAsync(
            cultureInfo =>
            {
                var text = message.GetTranslation(cultureInfo) ?? string.Empty;
                return formatArguments.Length > 0 ? string.Format(cultureInfo, text, formatArguments) : text;
            },
            (player, text) => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(text, MessageType.GoldenCenter)).AsTask())
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        this.StopPeriodicTasks();
        this._configChangeHandlerRegistration.Dispose();
        await this._recoverTimer.DisposeAsync().ConfigureAwait(false);
        await this._tasksTimer.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private static long[] CreateExpTable(string experienceFormula, short maximumLevel)
    {
        var argument = new Argument("level");
        var expression = new Expression(experienceFormula);
        expression.addArguments(argument);

        return Enumerable.Range(0, maximumLevel + 2)
            .Select(level =>
            {
                argument.setArgumentValue(level);
                return (long)expression.calculate();
            })
            .ToArray();
    }

    /// <summary>
    /// Counts the player as logged in, waiting for its login or neither, depending on its current state.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <remarks>
    /// The current state is read under the lock, instead of being passed by the caller: when two updates
    /// of the same player run at the same time, the later one sees the latest state, so they can't undo
    /// each other. A player which isn't (or no longer) in this context isn't counted.
    /// </remarks>
    private void UpdateLoggedIn(Player player)
    {
        bool changed;
        lock (this._loginStateLock)
        {
            var state = this._countedPlayers.Contains(player) ? player.PlayerState.CurrentState : PlayerState.Finished;
            if (state.IsLoggedIn())
            {
                changed = this._loggedInPlayers.Add(player);
                if (changed)
                {
                    this._loggedInPlayerCount++;
                }
            }
            else
            {
                changed = this.RemoveLoggedInPlayer(player);
            }

            this.SetWaitingForLogin(player, state.IsWaitingForLogin());
        }

        if (changed)
        {
            this.OnLoggedInPlayerCountChanged();
        }
    }

    /// <summary>
    /// Adds the player to or removes it from the players which wait for their login. It has to be called under <see cref="_loginStateLock"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="waiting">If set to <c>true</c>, the player waits for its login.</param>
    private void SetWaitingForLogin(Player player, bool waiting)
    {
        if (waiting && this._waitingPlayers.Add(player))
        {
            this._waitingForLoginCount++;
        }
        else if (!waiting && this._waitingPlayers.Remove(player))
        {
            this._waitingForLoginCount--;
        }
    }

    /// <summary>
    /// Removes the player from the logged in players. It has to be called under <see cref="_loginStateLock"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c>, if it was logged in.</returns>
    private bool RemoveLoggedInPlayer(Player player)
    {
        if (!this._loggedInPlayers.Remove(player))
        {
            return false;
        }

        this._loggedInPlayerCount--;
        if (this._takeovers.Remove(player, out var takeover))
        {
            // The login which takes over its place needs a place of its own now - in the same step, so
            // that no other login can take it in between.
            takeover.TakesOver = false;
            this._reservedLoginPlaces++;
        }

        return true;
    }

    private void OnLoggedInPlayerCountChanged()
    {
        if (this.LoggedInPlayerCountChanged is not { } handler)
        {
            return;
        }

        // It's raised within the state change of a player, which must not fail because of a
        // subscriber: a failing disconnect, for example, would leave the player in the game.
        foreach (var subscriber in handler.GetInvocationList())
        {
            try
            {
                ((EventHandler)subscriber).Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "A subscriber of the changed number of logged in players failed.");
            }
        }
    }

    private ValueTask OnGameConfigurationChangeAsync(Action unregisterAction, GameConfiguration gameConfiguration, GameContext context)
    {
        this._recoverTimer.Change(gameConfiguration.RecoveryInterval, gameConfiguration.RecoveryInterval);
        this.ExperienceTable = CreateExpTable(gameConfiguration.ExperienceFormula ?? DefaultExperienceFormula, gameConfiguration.MaximumLevel);
        this.MasterExperienceTable = CreateExpTable(gameConfiguration.MasterExperienceFormula ?? DefaultMasterExperienceFormula, gameConfiguration.MaximumMasterLevel);
        return ValueTask.CompletedTask;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void ExecutePeriodicTasks(object? state)
    {
        if (!this.ShouldExecutePeriodicTasks)
        {
            return;
        }

        try
        {
            if (this.PlugInManager.GetPlugInPoint<IPeriodicTaskPlugIn>() is { } plugInPoint)
            {
                await plugInPoint.ExecuteTaskAsync(this).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void RecoverTimerElapsed(object? state)
    {
        try
        {
            await this.ForEachPlayerAsync(player =>
            {
                if (player.SelectedCharacter != null && !player.PlayerState.CurrentState.IsDisconnectedOrFinished())
                {
                    return player.RegenerateAsync();
                }

                return Task.CompletedTask;
            }).ConfigureAwait(false);
        }
        catch
        {
            // This should never happen as we already handle Exceptions in player.RegenerateAsync.
            // However, if the player disconnects in the meantime, it could happen :-).
        }
    }

    private ValueTask PlayerEnteredWorldAsync(Player player)
    {
        this.PlayersByCharacterName.TryAdd(player.SelectedCharacter!.Name, player);
        return ValueTask.CompletedTask;
    }

    private ValueTask PlayerLeftWorldAsync(Player player)
    {
        this.PlayersByCharacterName.TryRemove(player.SelectedCharacter!.Name, out _);
        return ValueTask.CompletedTask;
    }

    private void ReleaseLoginPlace(LoginPlaceReservation reservation)
    {
        lock (this._loginStateLock)
        {
            if (reservation is { TakesOver: true, ReplacedPlayer: { } replacedPlayer })
            {
                this._takeovers.Remove(replacedPlayer);
            }
            else
            {
                this._reservedLoginPlaces--;
            }
        }
    }

    /// <summary>
    /// The place which a login reserved, see <see cref="TryReserveLoginPlace"/>.
    /// </summary>
    private sealed class LoginPlaceReservation : IDisposable
    {
        private GameContext? _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPlaceReservation"/> class.
        /// </summary>
        /// <param name="context">The context in which the place is reserved.</param>
        /// <param name="replacedPlayer">The logged in player whose place is taken over; <c>null</c> for a place of its own.</param>
        public LoginPlaceReservation(GameContext context, Player? replacedPlayer)
        {
            this._context = context;
            this.ReplacedPlayer = replacedPlayer;
            this.TakesOver = replacedPlayer is not null;
        }

        /// <summary>
        /// Gets the logged in player whose place is taken over; <c>null</c> for a reservation of a place of its own.
        /// </summary>
        public Player? ReplacedPlayer { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this reservation takes over the place of <see cref="ReplacedPlayer"/>,
        /// instead of taking a place of its own. It's set to <c>false</c> when that player isn't logged in anymore.
        /// Changed and read under the lock of the context.
        /// </summary>
        public bool TakesOver { get; set; }

        /// <summary>
        /// Releases the place; again calls do nothing.
        /// </summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref this._context, null)?.ReleaseLoginPlace(this);
        }
    }
}