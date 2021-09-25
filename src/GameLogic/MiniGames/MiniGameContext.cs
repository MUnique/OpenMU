// <copyright file="MiniGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
    using MUnique.OpenMU.GameLogic.PlugIns;

    /// <summary>
    /// The context of a mini game.
    /// </summary>
    public sealed class MiniGameContext : Disposable, IEventStateProvider
    {
        private readonly IGameContext gameContext;
        private readonly IMapInitializer mapInitializer;
        private readonly object enterLock = new ();

        private readonly HashSet<Player> enteredPlayers = new ();

        private readonly CancellationTokenSource gameEndedCts = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniGameContext"/> class.
        /// </summary>
        /// <param name="key">The key of this context.</param>
        /// <param name="definition">The definition of the mini game.</param>
        /// <param name="gameContext">The game context, to which this game belongs.</param>
        /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
        public MiniGameContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
        {
            this.gameContext = gameContext;
            this.mapInitializer = mapInitializer;
            this.Key = key;
            this.Definition = definition;

            this.Map = this.CreateMap();

            this.State = MiniGameState.Open;

            _ = Task.Run(this.RunGameAsync);
        }

        /// <summary>
        /// Gets the key of this instance, which should be unique within a <see cref="IGameContext"/>.
        /// </summary>
        public MiniGameMapKey Key { get; }

        /// <summary>
        /// Gets the definition of the game.
        /// </summary>
        public MiniGameDefinition Definition { get; }

        /// <summary>
        /// Gets the map on which the game takes place.
        /// </summary>
        public GameMap Map { get; }

        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        public MiniGameState State { get; private set; }

        /// <inheritdoc />
        public bool IsEventRunning => this.State == MiniGameState.Playing;

        /// <summary>
        /// Tries to enter the mini game. It will fail, if it's full, of if it's not in an open state.
        /// </summary>
        /// <param name="player">The player which tries to enter.</param>
        /// <param name="enterResult">The result of entering the game.</param>
        /// <returns>A value indicating whether entering had success.</returns>
        public bool TryEnter(Player player, out EnterResult enterResult)
        {
            lock (this.enterLock)
            {
                if (this.State != MiniGameState.Open)
                {
                    enterResult = EnterResult.NotOpen;
                    return false;
                }

                if (this.enteredPlayers.Count >= this.Definition.MaximumPlayerCount)
                {
                    enterResult = EnterResult.Full;
                    return false;
                }

                this.enteredPlayers.Add(player);
            }

            player.CurrentMiniGame = this;
            enterResult = EnterResult.Success;
            return true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            lock (this.enterLock)
            {
                this.State = MiniGameState.Disposed;
                if (this.enteredPlayers.Any())
                {
                    this.MovePlayersToSafezone();
                }
            }

            this.Map.ObjectAdded -= this.OnObjectAddedToMap;
            this.Map.ObjectRemoved -= this.OnObjectRemovedFromMap;

            this.gameContext.RemoveMiniGame(this);
            this.gameEndedCts.Dispose();
        }

        private async Task RunGameAsync()
        {
            try
            {
                await Task.Delay(this.Definition.EnterDuration, this.gameEndedCts.Token).ConfigureAwait(false);
                if (this.enteredPlayers.Any())
                {
                    this.Start();
                    await Task.Delay(this.Definition.GameDuration, this.gameEndedCts.Token).ConfigureAwait(false);
                    this.Stop();
                    await Task.Delay(this.Definition.ExitDuration, this.gameEndedCts.Token).ConfigureAwait(false);
                    this.FinishGame();
                }
                else
                {
                    this.Dispose();
                }
            }
            catch (TaskCanceledException)
            {
                this.Dispose();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, ex.StackTrace);
            }
        }

        private void Start()
        {
            lock (this.enterLock)
            {
                this.State = MiniGameState.Playing;
                this.UpdateMiniGameState();
            }

            this.mapInitializer.InitializeNpcsOnEventStart(this.Map, this);
            // todo send timer?
        }

        private void UpdateMiniGameState()
        {
            this.enteredPlayers.ForEach(p => p.ViewPlugIns.GetPlugIn<IUpdateMiniGameStatePlugIn>()?.UpdateState(this.Definition.Type, this.State));
        }

        private void Stop()
        {
            lock (this.enterLock)
            {
                this.State = MiniGameState.Ended;
                this.UpdateMiniGameState();
            }

            this.Map.ClearEventSpawnedNpcs();

            // todo: show result, give rewards
        }

        private GameMap CreateMap()
        {
            if (this.Map is not null)
            {
                throw new InvalidOperationException("The map is already created.");
            }

            var mapDefinition = this.Definition.Entrance?.Map ?? throw new InvalidOperationException($"{nameof(this.Definition)} contains no entrance map.");
            var map = this.mapInitializer.CreateGameMap(mapDefinition);

            map.ObjectRemoved += this.OnObjectRemovedFromMap;
            map.ObjectAdded += this.OnObjectAddedToMap;
            return map;
        }

        private void OnObjectAddedToMap(object? sender, (GameMap Map, ILocateable Object) args)
        {
            this.gameContext.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>()?.ObjectAddedToMap(args.Map, args.Object);
        }

        private void OnObjectRemovedFromMap(object? sender, (GameMap Map, ILocateable Object) args)
        {
            this.gameContext.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>()?.ObjectRemovedFromMap(args.Map, args.Object);
            if (args.Object is not Player player)
            {
                return;
            }

            player.CurrentMiniGame = null;
            var canGameProceed = false;
            lock (this.enterLock)
            {
                this.enteredPlayers.Remove(player);
                canGameProceed = this.enteredPlayers.Count == 0 && this.State != MiniGameState.Open;
            }

            if (canGameProceed)
            {
                this.gameEndedCts.Cancel();
            }

            // todo: send results?
        }

        private void FinishGame()
        {
            this.MovePlayersToSafezone();

            this.Dispose();
        }

        private void MovePlayersToSafezone()
        {
            List<Player> players;
            lock (this.enterLock)
            {
                players = this.enteredPlayers.ToList();
                this.enteredPlayers.Clear();
            }

            foreach (var player in players)
            {
                player.WarpToSafezone();
            }
        }
    }
}