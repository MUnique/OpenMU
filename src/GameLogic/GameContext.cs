// <copyright file="GameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The game context which holds all data of the game together.
/// </summary>
public class GameContext : Disposable, IGameContext
{
    private readonly Dictionary<ushort, GameMap> _mapList = new ();

    private readonly Dictionary<MiniGameMapKey, MiniGameContext> _miniGames = new ();

    private readonly Timer _recoverTimer;

    private readonly IMapInitializer _mapInitializer;

    private readonly Timer _tasksTimer;

    private readonly SemaphoreSlim _playerListLock = new (1);

    /// <summary>
    /// Keeps the list of all players.
    /// </summary>
    private readonly List<Player> _playerList = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameContext" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="mapInitializer">The map initializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    public GameContext(GameConfiguration configuration, IPersistenceContextProvider persistenceContextProvider, IMapInitializer mapInitializer, ILoggerFactory loggerFactory, PlugInManager plugInManager, IDropGenerator dropGenerator)
    {
        try
        {
            this.Configuration = configuration;
            this.PersistenceContextProvider = persistenceContextProvider;
            this.PlugInManager = plugInManager;
            this._mapInitializer = mapInitializer;
            this.LoggerFactory = loggerFactory;
            this.DropGenerator = dropGenerator;
            this.ItemPowerUpFactory = new ItemPowerUpFactory(loggerFactory.CreateLogger<ItemPowerUpFactory>());
            this._recoverTimer = new Timer(this.RecoverTimerElapsed, null, this.Configuration.RecoveryInterval, this.Configuration.RecoveryInterval);
            this._tasksTimer = new Timer(this.ExecutePeriodicTasks, null, 1000, 1000);
            this.FeaturePlugIns = new FeaturePlugInContainer(this.PlugInManager);
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

    /// <summary>
    /// Gets the initialized maps which are hosted on this context.
    /// </summary>
    public IEnumerable<GameMap> Maps => this._mapList.Values;

    /// <inheritdoc/>
    public GameConfiguration Configuration { get; }

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
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public int PlayerCount => this._playerList.Count;

    /// <inheritdoc/>
    public GameMap? GetMap(ushort mapId, bool createIfNotExists = true)
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
        lock (this._mapInitializer)
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
            createdMap.ObjectAdded += (sender, args) => this.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>()?.ObjectAddedToMap(args.Map, args.Object);
            createdMap.ObjectRemoved += (sender, args) => this.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>()?.ObjectRemovedFromMap(args.Map, args.Object);
        }

        // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
        this._mapInitializer.InitializeState(createdMap);
        this.GameMapCreated?.Invoke(this, createdMap);

        return createdMap;
    }

    /// <summary>
    /// Gets the mini game map which is meant to be hosted by the game.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player.</param>
    /// <returns>The hosted mini game instance.</returns>
    public MiniGameContext GetMiniGame(MiniGameDefinition miniGameDefinition, Player requester)
    {
        var miniGameKey = MiniGameMapKey.Create(miniGameDefinition, requester);

        if (this._miniGames.TryGetValue(miniGameKey, out var miniGameContext))
        {
            return miniGameContext;
        }

        lock (this._mapInitializer)
        {
            if (this._miniGames.TryGetValue(miniGameKey, out miniGameContext))
            {
                return miniGameContext;
            }

            switch (miniGameDefinition.Type)
            {
                case MiniGameType.DevilSquare:
                    miniGameContext = new DevilSquareContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
                default:
                    miniGameContext = new MiniGameContext(miniGameKey, miniGameDefinition, this, this._mapInitializer);
                    break;
            }

            this._miniGames.Add(miniGameKey, miniGameContext);
        }

        var createdMap = miniGameContext.Map;

        // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
        this._mapInitializer.InitializeState(createdMap);
        this.GameMapCreated?.Invoke(this, createdMap);
        return miniGameContext;
    }

    /// <inheritdoc />
    public void RemoveMiniGame(MiniGameContext miniGameContext)
    {
        miniGameContext.Dispose();
        this._miniGames.Remove(miniGameContext.Key);
    }

    /// <summary>
    /// Adds the player to the game.
    /// </summary>
    /// <param name="player">The player.</param>
    public virtual void AddPlayer(Player player)
    {
        player.PlayerLeftWorld += this.PlayerLeftWorld;
        player.PlayerEnteredWorld += this.PlayerEnteredWorld;
        player.PlayerDisconnected += this.PlayerDisconnected;

        this._playerListLock.Wait();
        try
        {
            this._playerList.Add(player);
        }
        finally
        {
            this._playerListLock.Release();
        }
    }

    /// <summary>
    /// Removes the player from the game.
    /// </summary>
    /// <param name="player">The player.</param>
    public virtual void RemovePlayer(Player player)
    {
        if (player.SelectedCharacter != null)
        {
            this.PlayersByCharacterName.Remove(player.SelectedCharacter.Name);
        }

        player.CurrentMap?.Remove(player);

        this._playerListLock.Wait();
        try
        {
            this._playerList.Remove(player);
        }
        finally
        {
            this._playerListLock.Release();
        }

        player.PlayerDisconnected -= this.PlayerDisconnected;
        player.PlayerEnteredWorld -= this.PlayerEnteredWorld;
        player.PlayerLeftWorld -= this.PlayerLeftWorld;
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
    public void ForEachPlayer(Action<Player> action)
    {
        if (this._playerList.Count == 0)
        {
            return;
        }

        this._playerListLock.Wait();
        try
        {
            for (int i = this._playerList.Count - 1; i >= 0; --i)
            {
                var player = this._playerList[i];
                action(player);
            }
        }
        finally
        {
            this._playerListLock.Release();
        }
    }

    /// <inheritdoc/>
    public void SendGlobalMessage(string message, MessageType messageType)
    {
        this.ForEachPlayer(player => player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, messageType));
    }

    /// <inheritdoc/>
    public void SendGlobalNotification(string message)
    {
        var regex = new Regex(Regex.Escape("!"));
        var sendingMessage = regex.Replace(message, string.Empty, 1);
        this.SendGlobalMessage(sendingMessage, MessageType.GoldenCenter);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this._recoverTimer.Dispose();
        this._tasksTimer.Dispose();
    }

    private void ExecutePeriodicTasks(object? state)
    {
        try
        {
            this.PlugInManager.GetPlugInPoint<IPeriodicTaskPlugIn>()?.ExecuteTask(this);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private void RecoverTimerElapsed(object? state)
    {
        this.ForEachPlayer(player =>
        {
            if (player.SelectedCharacter != null && player.PlayerState.CurrentState == PlayerState.EnteredWorld)
            {
                player.Regenerate();
            }
        });
    }

    private void PlayerDisconnected(object? sender, EventArgs e)
    {
        if (sender is not Player player)
        {
            return;
        }

        this.RemovePlayer(player);
    }

    private void PlayerEnteredWorld(object? sender, EventArgs e)
    {
        if (sender is not Player player)
        {
            return;
        }

        this.PlayersByCharacterName.Add(player.SelectedCharacter!.Name, player);
    }

    private void PlayerLeftWorld(object? sender, EventArgs e)
    {
        if (sender is not Player player)
        {
            return;
        }

        this.PlayersByCharacterName.Remove(player.SelectedCharacter!.Name);
    }
}