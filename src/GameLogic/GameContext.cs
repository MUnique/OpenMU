// <copyright file="GameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The game context which holds all data of the game together.
    /// </summary>
    public class GameContext : IGameContext, IDisposable
    {
        private readonly IDictionary<ushort, GameMap> mapList;

        private readonly Timer recoverTimer;

        private readonly IMapInitializer mapInitializer;

        private readonly Timer tasksTimer;

        private readonly SemaphoreSlim playerListLock = new SemaphoreSlim(1);

        /// <summary>
        /// Keeps the list of all players.
        /// </summary>
        private readonly List<Player> playerList = new List<Player>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameContext" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="mapInitializer">The map initializer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public GameContext(GameConfiguration configuration, IPersistenceContextProvider persistenceContextProvider, IMapInitializer mapInitializer, ILoggerFactory loggerFactory, PlugInManager plugInManager)
        {
            try
            {
                this.Configuration = configuration;
                this.PersistenceContextProvider = persistenceContextProvider;
                this.PlugInManager = plugInManager;
                this.mapInitializer = mapInitializer;
                this.LoggerFactory = loggerFactory;
                this.ItemPowerUpFactory = new ItemPowerUpFactory(loggerFactory.CreateLogger<ItemPowerUpFactory>());
                this.mapList = new Dictionary<ushort, GameMap>();
                this.recoverTimer = new Timer(this.RecoverTimerElapsed, null, this.Configuration.RecoveryInterval, this.Configuration.RecoveryInterval);

                this.tasksTimer = new Timer(this.ExecutePeriodicTasks, null, 1000, 1000);
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
        public IEnumerable<GameMap> Maps => this.mapList.Values;

        /// <inheritdoc/>
        public GameConfiguration Configuration { get; }

        /// <inheritdoc/>
        public PlugInManager PlugInManager { get; }

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
        public int PlayerCount => this.playerList.Count;

        /// <inheritdoc/>
        public GameMap? GetMap(ushort mapId, bool createIfNotExists = true)
        {
            if (this.mapList.TryGetValue(mapId, out var map))
            {
                return map;
            }

            if (!createIfNotExists)
            {
                return null;
            }

            GameMap? createdMap;
            lock (this.mapInitializer)
            {
                if (this.mapList.TryGetValue(mapId, out map))
                {
                    return map;
                }

                createdMap = this.mapInitializer.CreateGameMap(mapId);
                if (createdMap is null)
                {
                    return null;
                }

                this.mapList.Add(mapId, createdMap);
                createdMap.ObjectAdded += (sender, args) => this.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>()?.ObjectAddedToMap(args.Map, args.Object);
                createdMap.ObjectRemoved += (sender, args) => this.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>()?.ObjectRemovedFromMap(args.Map, args.Object);
            }

            // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
            this.mapInitializer.InitializeState(createdMap);
            this.GameMapCreated?.Invoke(this, createdMap);

            return createdMap;
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

            this.playerListLock.Wait();
            try
            {
                this.playerList.Add(player);
            }
            finally
            {
                this.playerListLock.Release();
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

            this.playerListLock.Wait();
            try
            {
                this.playerList.Remove(player);
            }
            finally
            {
                this.playerListLock.Release();
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
            if (this.playerList.Count == 0)
            {
                return;
            }

            this.playerListLock.Wait();
            try
            {
                for (int i = this.playerList.Count - 1; i >= 0; --i)
                {
                    var player = this.playerList[i];
                    action(player);
                }
            }
            finally
            {
                this.playerListLock.Release();
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
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "recoverTimer", Justification = "Null-conditional confuses the code analysis.")]
        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                this.recoverTimer.Dispose();
                this.tasksTimer.Dispose();
            }
        }

        private void ExecutePeriodicTasks(object? state)
        {
            this.PlugInManager.GetPlugInPoint<IPeriodicTaskPlugIn>()?.ExecuteTask(this);
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
}
