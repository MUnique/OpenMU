// <copyright file="GameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using MUnique.OpenMU.GameLogic.MiniGames;
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

    private static readonly Counter<int> MiniGameCounter = Meter.CreateCounter<int>("MiniGameCount");

    private static readonly IObjectPool<PathFinder> PathFinderPoolInstance = new LimitedObjectPool<PathFinder>(new PathFinderPoolingPolicy());

    private readonly Dictionary<ushort, GameMap> _mapList = new();

    private readonly Dictionary<MiniGameMapKey, MiniGameContext> _miniGames = new();

    private readonly Timer _recoverTimer;

    private readonly IMapInitializer _mapInitializer;

    private readonly AsyncLock _mapInitializerLock = new();

    private readonly Timer _tasksTimer;

    private readonly AsyncReaderWriterLock _playerListLock = new();

    /// <summary>
    /// Keeps the list of all players.
    /// </summary>
    private readonly List<Player> _playerList = new();

    private readonly IDisposable _configChangeHandlerRegistration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameContext" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="mapInitializer">The map initializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    public GameContext(GameConfiguration configuration, IPersistenceContextProvider persistenceContextProvider, IMapInitializer mapInitializer, ILoggerFactory loggerFactory, PlugInManager plugInManager, IDropGenerator dropGenerator, IConfigurationChangeMediator changeMediator)
    {
        try
        {
            this.Configuration = configuration;
            this.PersistenceContextProvider = persistenceContextProvider;
            this.PlugInManager = plugInManager;
            this._mapInitializer = mapInitializer;
            this.LoggerFactory = loggerFactory;
            this.DropGenerator = dropGenerator;
            this.ConfigurationChangeMediator = changeMediator;
            this.ItemPowerUpFactory = new ItemPowerUpFactory(loggerFactory.CreateLogger<ItemPowerUpFactory>());
            this._recoverTimer = new Timer(this.RecoverTimerElapsed, null, this.Configuration.RecoveryInterval, this.Configuration.RecoveryInterval);
            this._tasksTimer = new Timer(this.ExecutePeriodicTasks, null, 1000, 1000);
            this.FeaturePlugIns = new FeaturePlugInContainer(this.PlugInManager);
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
    public virtual float ExperienceRate => this.Configuration.ExperienceRate;

    /// <inheritdoc />
    public virtual bool PvpEnabled { get; }

    /// <inheritdoc/>
    public GameConfiguration Configuration { get; }

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
    public FeaturePlugInContainer FeaturePlugIns { get; }

    /// <inheritdoc/>
    public IItemPowerUpFactory ItemPowerUpFactory { get; }

    /// <inheritdoc/>
    public IPersistenceContextProvider PersistenceContextProvider { get; }

    /// <summary>
    /// Gets the players by character name dictionary.
    /// </summary>
    public IDictionary<string, Player> PlayersByCharacterName { get; } = new ConcurrentDictionary<string, Player>();

    /// <inheritdoc />
    public DuelRoomManager DuelRoomManager { get; set; }

    /// <summary>
    /// Gets the state of the active self defenses.
    /// </summary>
    public ConcurrentDictionary<(Player Attacker, Player Defender), DateTime> SelfDefenseState { get; } = new();

    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the path finder pool.
    /// </summary>
    public IObjectPool<PathFinder> PathFinderPool => PathFinderPoolInstance;

    /// <inheritdoc />
    public int PlayerCount => this._playerList.Count;

    /// <summary>
    /// Gets the name of the meter of this class.
    /// </summary>
    internal static string MeterName => typeof(GameContext).FullName ?? nameof(GameContext);

    /// <summary>
    /// Gets the initialized maps which are hosted on this context.
    /// </summary>
    public async ValueTask<IEnumerable<GameMap>> GetMapsAsync()
    {
        using var l = await this._mapInitializerLock.LockAsync();
        return this._mapList.Values.Concat(this._miniGames.Values.Select(g => g.Map)).ToList();
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
        using (await this._mapInitializerLock.LockAsync())
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
    /// Gets the mini game map which is meant to be hosted by the game.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player.</param>
    /// <returns>The hosted mini game instance.</returns>
    public async ValueTask<MiniGameContext> GetMiniGameAsync(MiniGameDefinition miniGameDefinition, Player requester)
    {
        var miniGameKey = MiniGameMapKey.Create(miniGameDefinition, requester);

        if (this._miniGames.TryGetValue(miniGameKey, out var miniGameContext))
        {
            return miniGameContext;
        }

        using (await this._mapInitializerLock.LockAsync().ConfigureAwait(false))
        {
            if (this._miniGames.TryGetValue(miniGameKey, out miniGameContext))
            {
                return miniGameContext;
            }

            switch (miniGameDefinition.Type)
            {
                case MiniGameType.ChaosCastle:
                    miniGameContext = new ChaosCastleContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
                case MiniGameType.DevilSquare:
                    miniGameContext = new DevilSquareContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
                case MiniGameType.BloodCastle:
                    miniGameContext = new BloodCastleContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
                default:
                    miniGameContext = new MiniGameContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
            }

            this._miniGames.Add(miniGameKey, miniGameContext);
        }

        var createdMap = miniGameContext.Map;

        // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
        await this._mapInitializer.InitializeStateAsync(createdMap).ConfigureAwait(false);
        this.GameMapCreated?.Invoke(this, createdMap);
        MiniGameCounter.Add(1);
        return miniGameContext;
    }

    /// <inheritdoc />
    public async ValueTask RemoveMiniGameAsync(MiniGameContext miniGameContext)
    {
        using var l = await this._mapInitializerLock.LockAsync().ConfigureAwait(false);
        MiniGameCounter.Add(-1);
        miniGameContext.Dispose();
        this._miniGames.Remove(miniGameContext.Key);
        this.GameMapRemoved?.Invoke(this, miniGameContext.Map);
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

        using (await this._playerListLock.WriterLockAsync())
        {
            this._playerList.Add(player);
        }

        PlayerCounter.Add(1);
    }

    /// <inheritdoc />
    public async ValueTask<IList<Player>> GetPlayersAsync()
    {
        using var l = await this._playerListLock.ReaderLockAsync();
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
        PlayerCounter.Add(-1);
        if (player.SelectedCharacter != null)
        {
            this.PlayersByCharacterName.Remove(player.SelectedCharacter.Name);
        }

        player.CurrentMap?.RemoveAsync(player);

        using (await this._playerListLock.WriterLockAsync())
        {
            this._playerList.Remove(player);
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
    protected override async ValueTask DisposeAsyncCore()
    {
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

#pragma warning disable CS1998
    private async ValueTask OnGameConfigurationChangeAsync(Action unregisterAction, GameConfiguration gameConfiguration, GameContext context)
#pragma warning restore CS1998
    {
        this._recoverTimer.Change(gameConfiguration.RecoveryInterval, gameConfiguration.RecoveryInterval);
        this.ExperienceTable = CreateExpTable(gameConfiguration.ExperienceFormula ?? DefaultExperienceFormula, gameConfiguration.MaximumLevel);
        this.MasterExperienceTable = CreateExpTable(gameConfiguration.MasterExperienceFormula ?? DefaultMasterExperienceFormula, gameConfiguration.MaximumMasterLevel);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void ExecutePeriodicTasks(object? state)
    {
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
        this.PlayersByCharacterName.Add(player.SelectedCharacter!.Name, player);
        return ValueTask.CompletedTask;
    }

    private ValueTask PlayerLeftWorldAsync(Player player)
    {
        this.PlayersByCharacterName.Remove(player.SelectedCharacter!.Name);
        return ValueTask.CompletedTask;
    }
}