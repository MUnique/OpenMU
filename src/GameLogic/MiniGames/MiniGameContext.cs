// <copyright file="MiniGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// The context of a mini game.
/// </summary>
public class MiniGameContext : Disposable, IEventStateProvider
{
    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly SemaphoreSlim _enterLock = new (1);

    private readonly HashSet<Player> _enteredPlayers = new ();

    private readonly CancellationTokenSource _gameEndedCts = new ();

    private readonly HashSet<byte> _currentSpawnWaves = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public MiniGameContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
    {
        this._gameContext = gameContext;
        this._mapInitializer = mapInitializer;
        this.Key = key;
        this.Definition = definition;
        this.Logger = this._gameContext.LoggerFactory.CreateLogger(this.GetType());

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
    /// Gets the logger for this instance.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> which is cancelled when the game ends.
    /// </summary>
    protected CancellationToken GameEndedToken => this._gameEndedCts.Token;

    /// <summary>
    /// Tries to enter the mini game. It will fail, if it's full, of if it's not in an open state.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <param name="enterResult">The result of entering the game.</param>
    /// <returns>A value indicating whether entering had success.</returns>
    public bool TryEnter(Player player, out EnterResult enterResult)
    {
        this._enterLock.Wait();
        try
        {
            if (this.State != MiniGameState.Open)
            {
                enterResult = EnterResult.NotOpen;
                return false;
            }

            if (this._enteredPlayers.Count >= this.Definition.MaximumPlayerCount)
            {
                enterResult = EnterResult.Full;
                return false;
            }

            this._enteredPlayers.Add(player);
        }
        finally
        {
            this._enterLock.Release();
        }

        player.CurrentMiniGame = this;
        player.PlayerPickedUpItem += this.OnPlayerPickedUpItem;
        enterResult = EnterResult.Success;
        return true;
    }

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber)
    {
        return this._currentSpawnWaves.Contains(waveNumber);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        this._enterLock.Wait();
        try
        {
            this.State = MiniGameState.Disposed;
        }
        finally
        {
            this._enterLock.Release();
        }

        try
        {
            this.MovePlayersToSafezoneAsync().AsTask().WaitAndUnwrapException();

            this.Map.ObjectAdded -= this.OnObjectAddedToMap;
            this.Map.ObjectRemoved -= this.OnObjectRemovedFromMap;

            this._gameContext.RemoveMiniGame(this);
            this._gameEndedCts.Cancel();
            this._gameEndedCts.Dispose();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during dispose: {0}", ex.Message);
        }
    }

    /// <summary>
    /// Executes the action for each player of this game.
    /// </summary>
    /// <param name="playerAction">The action which should be executed for each player of the game.</param>
    protected async ValueTask ForEachPlayerAsync(Action<Player> playerAction)
    {
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this._enteredPlayers.ForEach(playerAction);
        }
        finally
        {
            this._enterLock.Release();
        }
    }

    /// <summary>
    /// Will be called when the game has been started.
    /// </summary>
    /// <param name="players">The player which started with the game.</param>
    protected virtual void OnGameStart(ICollection<Player> players)
    {
        // can be overwritten
    }

    /// <summary>
    /// Will be called when a monster of the game has been killed.
    /// </summary>
    /// <param name="sender">The sender (monster) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnMonsterDied(object? sender, DeathInformation e)
    {
        // can be overwritten
    }

    /// <summary>
    /// Will be called when a destructible of the game has been destroyed.
    /// </summary>
    /// <param name="sender">The sender (destructible) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnDestructibleDied(object? sender, DeathInformation e)
    {
        // can be overwritten
    }

    /// <summary>
    /// Will be called when an item has been picked up by player.
    /// </summary>
    /// <param name="sender">The sender (player) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnPlayerPickedUpItem(object? sender, ILocateable e)
    {
        // can be overwritten
    }

    /// <summary>
    /// Shows the achieved score to the player.
    /// </summary>
    /// <param name="player">The player to which it should be shown.</param>
    protected virtual void ShowScore(Player player)
    {
        // can be overwritten
    }

    /// <summary>
    /// Will be called when the game has been ended.
    /// </summary>
    /// <param name="finishers">The players which finished the game to the end.</param>
    protected virtual void GameEnded(ICollection<Player> finishers)
    {
        foreach (var player in finishers)
        {
            this.ShowScore(player);
        }
    }

    /// <summary>
    /// Will be called when an object has been ended to map.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The event parameters.</param>
    protected virtual void OnObjectAddedToMap(object? sender, (GameMap Map, ILocateable Object) args)
    {
        this._gameContext.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>()?.ObjectAddedToMap(args.Map, args.Object);
        if (args.Object is Monster monster)
        {
            monster.Died += this.OnMonsterDied;
        }

        if (args.Object is Destructible destructible)
        {
            destructible.Died += this.OnDestructibleDied;
        }
    }

    /// <summary>
    /// Will be called when an object has been removed from map.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The event parameters.</param>
    protected virtual void OnObjectRemovedFromMap(object? sender, (GameMap Map, ILocateable Object) args)
    {
        this._gameContext.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>()?.ObjectRemovedFromMap(args.Map, args.Object);
        if (args.Object is not Player player)
        {
            return;
        }

        player.CurrentMiniGame = null;
        bool cantGameProceed;
        this._enterLock.Wait();
        try
        {
            this._enteredPlayers.Remove(player);
            cantGameProceed = this._enteredPlayers.Count == 0 && this.State != MiniGameState.Open;
        }
        finally
        {
            this._enterLock.Release();
        }

        if (cantGameProceed)
        {
            this._gameEndedCts.Cancel();
        }
    }

    /// <summary>
    /// Finishes the event.
    /// </summary>
    protected virtual void FinishEvent()
    {
        this._gameEndedCts.Cancel();
    }

    /// <summary>
    /// Gives the rewards to the player.
    /// </summary>
    /// <param name="player">The player who should receive the rewards.</param>
    /// <param name="rank">The rank of the player in the current game.</param>
    protected void GiveRewards(Player player, int rank)
    {
        var rewards = this.Definition.Rewards.Where(r => r.Rank is null || r.Rank == rank);
        foreach (var reward in rewards)
        {
            this.GiveReward(player, reward);
        }
    }

    /// <summary>
    /// Saves the ranking of this game.
    /// </summary>
    /// <param name="scoreEntries">The entries of the ranking.</param>
    protected void SaveRanking(IEnumerable<(int Rank, Character Character, int Score)> scoreEntries)
    {
        if (!this.Definition.SaveRankingStatistics)
        {
            return;
        }

        try
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext<MiniGameRankingEntry>();
            context.Attach(this.Definition);
            var instanceId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;
            foreach (var score in scoreEntries)
            {
                var entry = context.CreateNew<MiniGameRankingEntry>();
                entry.GameInstanceId = instanceId;
                entry.Rank = score.Rank;
                entry.Score = score.Score;
                entry.Character = score.Character;
                entry.MiniGame = this.Definition;
                entry.Timestamp = timestamp;
            }

            context.SaveChanges();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error while saving mini game ranking");
        }
    }

    private void GiveReward(Player player, MiniGameReward reward)
    {
        switch (reward.RewardType)
        {
            case MiniGameRewardType.Experience:
                player.AddExperience(reward.RewardAmount, null);
                break;
            case MiniGameRewardType.Money:
                if (!player.TryAddMoney(reward.RewardAmount))
                {
                    player.ShowMessage("Couldn't add reward money, inventory is full.");
                }

                break;
            case MiniGameRewardType.Item:
                this.GiveItemReward(player, reward);
                break;
            case MiniGameRewardType.ItemDrop:
                this.GiveItemReward(player, reward, true);
                break;
            case MiniGameRewardType.Undefined:
                this.Logger.LogWarning($"Undefined reward type in {reward.GetId()}");
                break;
            default:
                this.Logger.LogError($"Reward type {reward.RewardType} in {reward.GetId()} is not implemented!");
                throw new NotImplementedException($"Reward type {reward.RewardType} is not implemented");
        }
    }

    private void GiveItemReward(Player player, MiniGameReward reward, bool drop = false)
    {
        if (reward.ItemReward is null)
        {
            this.Logger.LogWarning($"Item reward is not set in {reward.GetId()}");
            return;
        }

        var item = this._gameContext.DropGenerator.GenerateItemDrop(reward.ItemReward);
        if (item is null)
        {
            this.Logger.LogInformation($"No item has been generated by reward {reward.GetId()} for player {player}.");
            return;
        }

        for (int i = 1; i <= reward.RewardAmount; i++)
        {
            var droppedItem = new DroppedItem(item, player.RandomPosition, this.Map, player, player.GetAsEnumerable());
            var shouldDrop = drop || !(player.Inventory?.AddItem(item) ?? false);
            if (!shouldDrop) continue;
            this.Logger.LogInformation($"Reward {item} for {player} has been dropped by players coordinates {player.Position}.");
            this.Map.Add(droppedItem);
        }
    }

    private void RunSpawnWaves()
    {
        foreach (var spawnWave in this.Definition.SpawnWaves)
        {
            this.RunSpawnWaveAsync(spawnWave).ConfigureAwait(false);
        }
    }

    private async ValueTask RunSpawnWaveAsync(MiniGameSpawnWave spawnWave)
    {
        try
        {
            if (spawnWave.StartTime > TimeSpan.Zero)
            {
                await Task.Delay(spawnWave.StartTime, this._gameEndedCts.Token).ConfigureAwait(false);
            }

            this.Logger.LogInformation("Starting next wave: {0}", spawnWave.Description);
            if (spawnWave.Message is { } message)
            {
                await this.ShowMessageAsync(message);
            }

            this._currentSpawnWaves.Add(spawnWave.WaveNumber);
            this._mapInitializer.InitializeNpcsOnWaveStart(this.Map, this, spawnWave.WaveNumber);
            await Task.Delay(spawnWave.EndTime - spawnWave.StartTime, this._gameEndedCts.Token).ConfigureAwait(false);
            this.Logger.LogInformation("Wave ended: {0}", spawnWave.Description);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during spawn wave {0}: {1}", spawnWave.WaveNumber, ex.Message);
        }
        finally
        {
            this._currentSpawnWaves.Remove(spawnWave.WaveNumber);
        }
    }

    private async ValueTask RunGameAsync()
    {
        var countdownMessageDuration = TimeSpan.FromSeconds(30);
        try
        {
            var enterDuration = this.Definition.EnterDuration.Subtract(countdownMessageDuration).AtLeast(countdownMessageDuration);
            var gameDuration = this.Definition.GameDuration.AtLeast(countdownMessageDuration);
            var exitDuration = this.Definition.ExitDuration.Subtract(countdownMessageDuration).AtLeast(countdownMessageDuration);

            await Task.Delay(enterDuration, this._gameEndedCts.Token).ConfigureAwait(false);
            if (this._enteredPlayers.Count == 0)
            {
                return;
            }

            await this.ShowCountdownMessageAsync().ConfigureAwait(false);
            await Task.Delay(countdownMessageDuration).ConfigureAwait(false);

            await this.StartAsync().ConfigureAwait(false);

            try
            {
                await Task.Delay(gameDuration, this._gameEndedCts.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                this.Logger.LogInformation("Finished event earlier");
            }

            await this.StopAsync().ConfigureAwait(false);
            await Task.Delay(exitDuration).ConfigureAwait(false);
            await this.ShowCountdownMessageAsync().ConfigureAwait(false);
            await Task.Delay(countdownMessageDuration).ConfigureAwait(false);

            this.Logger.LogInformation("Shutting down event");
            await this.ShutdownGameAsync().ConfigureAwait(false);
        }
        catch (TaskCanceledException ex)
        {
            this.Logger.LogDebug(ex, "Received TaskCanceledException: {0}", ex.Message);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during mini game event: {0}", ex.Message);
        }
        finally
        {
            this.Dispose();
        }
    }

    private async ValueTask StartAsync()
    {
        ICollection<Player> players;
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this.State = MiniGameState.Playing;
            players = this._enteredPlayers.ToList();
        }
        finally
        {
            this._enterLock.Release();
        }

        this.OnGameStart(players);
        this.RunSpawnWaves();
        this._mapInitializer.InitializeNpcsOnEventStart(this.Map, this);
    }

    private async ValueTask ShowCountdownMessageAsync()
    {
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this._enteredPlayers.ForEach(p => p.ViewPlugIns.GetPlugIn<IUpdateMiniGameStateViewPlugIn>()?.UpdateState(this.Definition.Type, this.State));
        }
        finally
        {
            this._enterLock.Release();
        }
    }

    private ValueTask ShowMessageAsync(string message)
    {
        return this.ForEachPlayerAsync(player => player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.GoldenCenter));
    }

    private async ValueTask StopAsync()
    {
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this.State = MiniGameState.Ended;
            this._gameEndedCts.Cancel();
        }
        finally
        {
            this._enterLock.Release();
        }

        this._currentSpawnWaves.Clear();
        this.Map.ClearEventSpawnedNpcs();

        List<Player> players;
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            players = this._enteredPlayers.ToList();
        }
        finally
        {
            this._enterLock.Release();
        }

        this.GameEnded(players);
    }

    private GameMap CreateMap()
    {
        if (this.Map is not null)
        {
            throw new InvalidOperationException("The map is already created.");
        }

        var mapDefinition = this.Definition.Entrance?.Map ?? throw new InvalidOperationException($"{nameof(this.Definition)} contains no entrance map.");
        var map = this._mapInitializer.CreateGameMap(mapDefinition);

        map.ObjectRemoved += this.OnObjectRemovedFromMap;
        map.ObjectAdded += this.OnObjectAddedToMap;
        return map;
    }

    private async ValueTask ShutdownGameAsync()
    {
        await this.MovePlayersToSafezoneAsync().ConfigureAwait(false);

        this.Dispose();
    }

    private async ValueTask MovePlayersToSafezoneAsync()
    {
        List<Player> players;
        await this._enterLock.WaitAsync().ConfigureAwait(false);
        try
        {
            players = this._enteredPlayers.ToList();
            this._enteredPlayers.Clear();
        }
        finally
        {
            this._enterLock.Release();
        }

        foreach (var player in players)
        {
            player.WarpToSafezone();
        }
    }
}