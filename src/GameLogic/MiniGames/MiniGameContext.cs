// <copyright file="MiniGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx;

/// <summary>
/// The context of a mini game.
/// </summary>
public class MiniGameContext : AsyncDisposable, IEventStateProvider
{
    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly AsyncReaderWriterLock _enterLock = new();

    private readonly HashSet<Player> _enteredPlayers = new();
    private readonly ConcurrentDictionary<MonsterDefinition, bool> _rewardRelatedKills;

    private readonly CancellationTokenSource _gameEndedCts = new();

    private readonly ConcurrentDictionary<byte, MiniGameSpawnWave> _currentSpawnWaves = new();
    private readonly List<ChangeEventContext> _remainingEvents = new();

    private Stopwatch? _elapsedTimeSinceStart;

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
        this.DropGenerator = this._gameContext.DropGenerator;

        this.Map = this.CreateMap();

        this._rewardRelatedKills = new(
            this.Definition.Rewards
                .Where(r => r.RequiredKill is not null)
                .Select(r => new KeyValuePair<MonsterDefinition, bool>(r.RequiredKill!, false))
                .Distinct());

        this.State = MiniGameState.Open;

        _ = Task.Run(() => this.RunGameAsync(this.GameEndedToken), this.GameEndedToken);
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
    /// Gets the player count.
    /// </summary>
    public int PlayerCount
    {
        get
        {
            using var l = this._enterLock.ReaderLock();
            return this._enteredPlayers.Count(p => p.IsAlive);
        }
    }

    /// <summary>
    /// Gets a value indicating whether it's allowed to kill other players without consequences.
    /// </summary>
    public virtual bool AllowPlayerKilling { get; }

    /// <summary>
    /// Gets the remaining time of the event, in case it has been finished by the player earlier than the timeout.
    /// </summary>
    protected virtual TimeSpan RemainingTime => TimeSpan.Zero;

    /// <summary>
    /// Gets a value indicating whether a winner is existing.
    /// </summary>
    protected virtual Player? Winner => null;

    /// <summary>
    /// Gets the logger for this instance.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> which is cancelled when the game ends.
    /// </summary>
    protected CancellationToken GameEndedToken => this._gameEndedCts.Token;

    /// <summary>
    /// Gets the next event which targets should be fulfilled by the players.
    /// </summary>
    protected ChangeEventContext? NextEvent { get; private set; }

    /// <summary>
    /// Gets or sets the drop generator which should be used during the mini game.
    /// </summary>
    protected IDropGenerator DropGenerator { get; set; }

    /// <summary>
    /// Gets the minimum player count to start the game.
    /// </summary>
    protected virtual int MinimumPlayerCount => 1;

    /// <summary>
    /// Tries to enter the mini game. It will fail, if it's full, of if it's not in an open state.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <returns>A value indicating whether entering had success.</returns>
    public async ValueTask<EnterResult> TryEnterAsync(Player player)
    {
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            if (this.State != MiniGameState.Open)
            {
                return EnterResult.NotOpen;
            }

            if (this._enteredPlayers.Count >= this.Definition.MaximumPlayerCount)
            {
                return EnterResult.Full;
            }

            if (!await this.AreEquippedItemsAllowedAsync(player).ConfigureAwait(false))
            {
                return EnterResult.Failed;
            }

            this._enteredPlayers.Add(player);
        }

        player.CurrentMiniGame = this;
        player.PlayerPickedUpItem += this.OnPlayerPickedUpItemAsync;

        return EnterResult.Success;
    }

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber)
    {
        return this._currentSpawnWaves.ContainsKey(waveNumber);
    }

    /// <summary>
    /// Determines whether an item allowed to be equipped during this game.
    /// </summary>
    /// <param name="item">The item.</param>
    public virtual bool IsItemAllowedToEquip(Item item)
    {
        // Additional checks can be implemented in specific mini games.
        return true;
    }

    /// <summary>
    /// Determines whether performing the specified skill is allowed during the mini game.
    /// </summary>
    /// <param name="skill">The skill.</param>
    /// <param name="attacker">The attacker or skill performer.</param>
    /// <param name="target">The target.</param>
    public virtual bool IsSkillAllowed(Skill skill, Player attacker, IAttackable target)
    {
        // Additional checks can be implemented in specific mini games.
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Definition.Name} for {this._gameContext}";
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        try
        {
            this.Logger.LogDebug("{context}: Disposing mini game...", this);
            await base.DisposeAsyncCore().ConfigureAwait(false);

            using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
            {
                this.State = MiniGameState.Disposed;
            }

            await this.MovePlayersToSafezoneAsync().ConfigureAwait(false);

            this.Map.ObjectAdded -= this.OnObjectAddedToMapAsync;
            this.Map.ObjectRemoved -= this.OnObjectRemovedFromMapAsync;

            await this._gameContext.RemoveMiniGameAsync(this).ConfigureAwait(false);
            await this._gameEndedCts.CancelAsync();
            this._gameEndedCts.Dispose();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error during dispose: {ex}", this, ex);
        }
    }

    /// <summary>
    /// Executes the action for each player of this game.
    /// </summary>
    /// <param name="playerAction">The action which should be executed for each player of the game.</param>
    protected async ValueTask ForEachPlayerAsync(Func<Player, Task> playerAction)
    {
        using var @lock = await this._enterLock.ReaderLockAsync().ConfigureAwait(false);
        await this._enteredPlayers.Select(playerAction).WhenAll().ConfigureAwait(false);
    }

    /// <summary>
    /// Will be called when the game has been started.
    /// </summary>
    /// <param name="players">The player which started with the game.</param>
    protected virtual async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        this._elapsedTimeSinceStart = new Stopwatch();
        this._elapsedTimeSinceStart.Start();

        var startEvents = this.Definition.ChangeEvents
            .OrderBy(e => e.Index)
            .TakeWhile(e => e is { Index: <= 0, NumberOfKills: 0 })
            .ToList();

        foreach (var changeEvent in startEvents)
        {
            await this.ApplyChangeEventAsync(changeEvent).ConfigureAwait(false);
        }

        this._remainingEvents.AddRange(
            this.Definition.ChangeEvents
                .Except(startEvents)
                .Select(e => new ChangeEventContext(e, this._enteredPlayers.Count)));
        this.UpdateNextEvent();
    }

    /// <summary>
    /// Will be called when a monster of the game has been killed.
    /// </summary>
    /// <param name="sender">The sender (monster) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnMonsterDied(object? sender, DeathInformation e)
    {
        if (sender is AttackableNpcBase npc)
        {
            this._rewardRelatedKills.TryUpdate(npc.Definition, true, false);
            this.CheckKillForEventChanges(npc);
        }
    }

    /// <summary>
    /// Will be called when a player of the game has been killed.
    /// </summary>
    /// <param name="sender">The sender (player) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnPlayerDied(object? sender, DeathInformation e)
    {
        if (sender is Player player)
        {
            this.CheckKillForEventChanges(player);
        }
    }

    /// <summary>
    /// Will be called when a destructible of the game has been destroyed.
    /// </summary>
    /// <param name="sender">The sender (destructible) of the event.</param>
    /// <param name="e">The event parameters.</param>
    protected virtual void OnDestructibleDied(object? sender, DeathInformation e)
    {
        if (sender is AttackableNpcBase npc)
        {
            this._rewardRelatedKills.TryUpdate(npc.Definition, true, false);
            this.CheckKillForEventChanges(npc);
        }
    }

    /// <summary>
    /// Will be called when an item has been picked up by player.
    /// </summary>
    /// <param name="args">The event parameters.</param>
    protected virtual ValueTask OnPlayerPickedUpItemAsync((Player Picker, ILocateable DroppedItem) args)
    {
        // can be overwritten
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Shows the achieved score to the player.
    /// </summary>
    /// <param name="player">The player to which it should be shown.</param>
    protected virtual ValueTask ShowScoreAsync(Player player)
    {
        // can be overwritten
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Will be called when the game has been ended.
    /// </summary>
    /// <param name="finishers">The players which finished the game to the end.</param>
    protected virtual async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        foreach (var player in finishers)
        {
            await this.ShowScoreAsync(player).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Called when an item was dropped on the map.
    /// </summary>
    /// <param name="item">The item.</param>
    protected virtual void OnItemDroppedOnMap(DroppedItem item)
    {
        // can be overwritten
    }

    /// <summary>
    /// Will be called when an object has been ended to map.
    /// </summary>
    /// <param name="args">The event parameters.</param>
    protected virtual async ValueTask OnObjectAddedToMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (this._gameContext.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>() is { } plugInPoint)
        {
            await plugInPoint.ObjectAddedToMapAsync(args.Map, args.Object).ConfigureAwait(false);
        }

        if (args.Object is Monster monster)
        {
            monster.Died += this.OnMonsterDied;
        }

        if (args.Object is Destructible destructible)
        {
            destructible.Died += this.OnDestructibleDied;
        }

        if (args.Object is Player player)
        {
            player.Died += this.OnPlayerDied;
        }

        if (args.Object is DroppedItem item)
        {
            this.OnItemDroppedOnMap(item);
        }
    }

    /// <summary>
    /// Will be called when an object has been removed from map.
    /// </summary>
    /// <param name="args">The event parameters.</param>
    protected virtual async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        try
        {
            if (this._gameContext.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>() is { } plugInPoint)
            {
                await plugInPoint.ObjectRemovedFromMapAsync(args.Map, args.Object).ConfigureAwait(false);
            }

            if (args.Object is not Player player)
            {
                return;
            }

            player.CurrentMiniGame = null;
            player.PlayerPickedUpItem -= this.OnPlayerPickedUpItemAsync;
            bool cantGameProceed;
            using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
            {
                player.Died -= this.OnPlayerDied;
                this._enteredPlayers.Remove(player);
                cantGameProceed = this._enteredPlayers.Count == 0 && this.State != MiniGameState.Open;
            }

            if (cantGameProceed)
            {
                await this._gameEndedCts.CancelAsync().ConfigureAwait(false);
            }
            else if (player.IsAlive)
            {
                this.CheckKillForEventChanges(player);
            }
            else
            {
                // no action required.
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Error when handling the removed map object {obj}.", this, args.Object);
        }
    }

    /// <summary>
    /// Called when the map terrain changed due to an <see cref="MiniGameChangeEvent"/>.
    /// </summary>
    /// <param name="changeEvent">The change event.</param>
    protected virtual ValueTask OnTerrainChangedAsync(MiniGameChangeEvent changeEvent)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called before the map terrain is changing due to an <see cref="MiniGameChangeEvent"/>.
    /// </summary>
    /// <param name="changeEvent">The change event.</param>
    protected virtual ValueTask OnTerrainChangingAsync(MiniGameChangeEvent changeEvent)
    {
        return ValueTask.CompletedTask;
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
    /// <returns>The bonus score and the given money.</returns>
    protected async Task<(int BonusScore, int GivenMoney)> GiveRewardsAndGetBonusScoreAsync(Player player, int rank)
    {
        int bonusScore = 0;
        int givenMoney = 0;
        var rewards = this.Definition.Rewards.Where(r => this.DoesRewardApply(player, rank, r));
        foreach (var reward in rewards)
        {
            var result = await this.GiveRewardAsync(player, reward).ConfigureAwait(false);
            bonusScore += result.BonusScore;
            givenMoney += result.GivenMoney;
        }

        return (bonusScore, givenMoney);
    }

    /// <summary>
    /// Saves the ranking of this game.
    /// </summary>
    /// <param name="scoreEntries">The entries of the ranking.</param>
    protected async ValueTask SaveRankingAsync(IEnumerable<(int Rank, Character Character, int Score)> scoreEntries)
    {
        if (!this.Definition.SaveRankingStatistics)
        {
            return;
        }

        try
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext<MiniGameRankingEntry>(false);
            var instanceId = GuidV7.NewGuid();
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

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Error while saving mini game ranking: {ex}", this, ex);
        }
    }

    private async ValueTask<(int BonusScore, int GivenMoney)> GiveRewardAsync(Player player, MiniGameReward reward)
    {
        switch (reward.RewardType)
        {
            case MiniGameRewardType.Experience:
                await player.AddExperienceAsync(reward.RewardAmount, null).ConfigureAwait(false);
                break;
            case MiniGameRewardType.ExperiencePerRemainingSeconds:
                var seconds = (int)this.RemainingTime.TotalSeconds;
                if (seconds > 0)
                {
                    await player.AddExperienceAsync(seconds * reward.RewardAmount, null).ConfigureAwait(false);
                }

                break;
            case MiniGameRewardType.Money:
                if (!player.TryAddMoney(reward.RewardAmount))
                {
                    await player.ShowMessageAsync("Couldn't add reward money, inventory is full.").ConfigureAwait(false);
                }

                return (0, reward.RewardAmount);
            case MiniGameRewardType.Item:
                await this.GiveItemRewardAsync(player, reward).ConfigureAwait(false);
                break;
            case MiniGameRewardType.ItemDrop:
                await this.GiveItemRewardAsync(player, reward, true).ConfigureAwait(false);
                break;
            case MiniGameRewardType.Score:
                return (reward.RewardAmount, 0);
            case MiniGameRewardType.Undefined:
                this.Logger.LogWarning($"Undefined reward type in {reward.GetId()}");
                break;
            default:
                this.Logger.LogError($"Reward type {reward.RewardType} in {reward.GetId()} is not implemented!");
                throw new NotImplementedException($"Reward type {reward.RewardType} is not implemented");
        }

        return (0, 0);
    }

    private async ValueTask GiveItemRewardAsync(Player player, MiniGameReward reward, bool drop = false)
    {
        if (reward.ItemReward is null)
        {
            this.Logger.LogWarning("{context}: Item reward is not set in {reward}", this, reward.GetId());
            return;
        }

        for (int i = 0; i < reward.RewardAmount; i++)
        {
            if (reward.ItemReward.Chance < 1
                && reward.ItemReward.Chance != 0 // If we don't add a chance (legacy), we assume that it's 1.
                && !Rand.NextRandomBool(reward.ItemReward.Chance))
            {
                this.Logger.LogDebug("{context}: No item has been generated by reward {reward} for player {player} due to missed chance ({reward.ItemReward.Chance}).", this, reward.GetId(), player, reward.ItemReward.Chance);
                continue;
            }

            var item = this._gameContext.DropGenerator.GenerateItemDrop(reward.ItemReward);
            if (item is null)
            {
                this.Logger.LogDebug("{context}: No item has been generated by reward {reward} for player {player}.", this, reward.GetId(), player);
                return;
            }

            var droppedItem = new DroppedItem(item, player.RandomPosition, this.Map, player, player.GetAsEnumerable());

            var shouldDrop = drop || !(player.Inventory is not null && await player.Inventory.AddItemAsync(item).ConfigureAwait(false));
            if (shouldDrop)
            {
                this.Logger.LogDebug("{context}: Reward {item} for {player} has been dropped by players coordinates {position}.", this, item, player, player.Position);
                await this.Map.AddAsync(droppedItem).ConfigureAwait(false);
            }
        }
    }

    private async Task RunSpawnWavesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // We already need to start all tasks, because they may overlap.
            // So it's not okay to run them one after another.
            var waveTasks = this.Definition.SpawnWaves
                .OrderBy(wave => wave.WaveNumber)
                .Select(wave => this.RunSpawnWaveAsync(wave, cancellationToken))
                .ToList();
            await Task.WhenAll(waveTasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during spawn waves: {0}", ex.Message);
        }
    }

    private async Task RunSpawnWaveAsync(MiniGameSpawnWave spawnWave, CancellationToken cancellationToken)
    {
        try
        {
            while (spawnWave.StartTime > this._elapsedTimeSinceStart?.Elapsed)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Wait at least a second (or half of the remaining time) to the next check
                var timeUntilNextCheck = (spawnWave.StartTime - this._elapsedTimeSinceStart!.Elapsed) / 2;
                var requiredDelay = timeUntilNextCheck > TimeSpan.FromSeconds(1)
                    ? timeUntilNextCheck
                    : TimeSpan.FromSeconds(1);
                await Task.Delay(requiredDelay, cancellationToken).ConfigureAwait(false);
            }

            this.Logger.LogDebug("{context}: Starting next wave: {wave}", this, spawnWave.Description);
            if (spawnWave.Message is { } message)
            {
                await this.ShowMessageAsync(message).ConfigureAwait(false);
            }

            if (!this._currentSpawnWaves.TryAdd(spawnWave.WaveNumber, spawnWave))
            {
                this.Logger.LogWarning("{context}: Duplicate spawn wave number in event: {wave}. Check your configuration, every spawn wave needs a distinct number.", this, spawnWave.Description);
            }

            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, spawnWave.WaveNumber).ConfigureAwait(false);
            await Task.Delay(spawnWave.EndTime - spawnWave.StartTime, cancellationToken).ConfigureAwait(false);
            this.Logger.LogDebug("{context}: Wave ended: {wave}", this, spawnWave.Description);
        }
        catch (OperationCanceledException)
        {
            // do nothing, as it's expected when game ends ...
            throw;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during spawn wave {0}: {1}", spawnWave.WaveNumber, ex.Message);
        }
        finally
        {
            this._currentSpawnWaves.Remove(spawnWave.WaveNumber, out _);
        }
    }

    private async ValueTask RunGameAsync(CancellationToken cancellationToken)
    {
        this.Logger.LogDebug("{context}: Running the game ...", this);
        var countdownMessageDuration = TimeSpan.FromSeconds(30);
        try
        {
            var enterDuration = this.Definition.EnterDuration.AtLeast(countdownMessageDuration);
            var gameDuration = this.Definition.GameDuration.AtLeast(countdownMessageDuration);
            var exitDuration = this.Definition.ExitDuration.Subtract(countdownMessageDuration).AtLeast(countdownMessageDuration);

            this.Logger.LogDebug("{context}: Waiting for entering players for {enterDuration}", this, enterDuration);

            var messagePeriod = TimeSpan.FromMinutes(1);
            for (; enterDuration >= messagePeriod; enterDuration = enterDuration.Subtract(messagePeriod))
            {
                if (this.Definition.MapCreationPolicy != MiniGameMapCreationPolicy.Shared)
                {
                    await this.ShowMessageAsync($"{this.Definition.Name} starts in {(int)enterDuration.TotalMinutes} minutes.").ConfigureAwait(false);
                }

                await Task.Delay(messagePeriod, cancellationToken).ConfigureAwait(false);
            }

            if (enterDuration >= TimeSpan.Zero)
            {
                await Task.Delay(enterDuration, cancellationToken).ConfigureAwait(false);
            }

            await this.CloseEntranceAsync().ConfigureAwait(false);
            if (this.PlayerCount < this.MinimumPlayerCount)
            {
                await this.ShowMessageAsync($"Can't start with less than {this.MinimumPlayerCount} players.").ConfigureAwait(false);
                if (this.Definition.EntranceFee > 0)
                {
                    await this.ForEachPlayerAsync(async player => player.TryAddMoney(this.Definition.EntranceFee)).ConfigureAwait(false);
                }

                return;
            }

            await this.ShowCountdownMessageAsync().ConfigureAwait(false);
            this.Logger.LogDebug("{context}: Waiting for the countdown duration of {countdownMessageDuration}", this, countdownMessageDuration);
            await Task.Delay(countdownMessageDuration, cancellationToken).ConfigureAwait(false);

            this.Logger.LogDebug("{context}: Starting the game...", this);
            await this.StartAsync().ConfigureAwait(false);

            this.Logger.LogDebug("{context}: Waiting for the game duration of {gameDuration}", this, gameDuration);
            try
            {
                await Task.Delay(gameDuration, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                this.Logger.LogDebug("{context}: Finished event earlier", this);
            }

            this.Logger.LogDebug("{context}: Stopping the game...", this);
            await this.StopAsync().ConfigureAwait(false);

            this.Logger.LogDebug("{context}: Waiting for the exit duration of {exitDuration}", this, exitDuration);
            await Task.Delay(exitDuration, default(CancellationToken)).ConfigureAwait(false);
            await this.ShowCountdownMessageAsync().ConfigureAwait(false);

            this.Logger.LogDebug("{context}: Waiting for the exit countdown duration of {countdownMessageDuration}", this, countdownMessageDuration);
            await Task.Delay(countdownMessageDuration, default(CancellationToken)).ConfigureAwait(false);

            this.Logger.LogDebug("{context}: Shutting down event", this);
            await this.ShutdownGameAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            this.Logger.LogDebug(ex, "{context}: Received OperationCanceledException: {0}", this, ex);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error during mini game event: {ex}", this, ex);
        }
        finally
        {
            await this.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask CloseEntranceAsync()
    {
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            this.State = MiniGameState.Closed;
        }
    }

    private async ValueTask StartAsync()
    {
        ICollection<Player> players;
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            this.State = MiniGameState.Playing;
            players = this._enteredPlayers.ToList();
        }

        await this.OnGameStartAsync(players).ConfigureAwait(false);
        await this._mapInitializer.InitializeNpcsOnEventStartAsync(this.Map, this).ConfigureAwait(false);
        _ = Task.Run(() => this.RunSpawnWavesAsync(this.GameEndedToken), this.GameEndedToken);
    }

    private async ValueTask ShowCountdownMessageAsync()
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IUpdateMiniGameStateViewPlugIn>(p => p.UpdateStateAsync(this.Definition.Type, this.State)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask ShowMessageAsync(string message, MessageType messageType = MessageType.GoldenCenter)
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, messageType)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask StopAsync()
    {
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            this.State = MiniGameState.Ended;
            await this._gameEndedCts.CancelAsync().ConfigureAwait(false);
        }

        this._currentSpawnWaves.Clear();
        await this.Map.ClearEventSpawnedNpcsAsync().ConfigureAwait(false);

        List<Player> players;
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            players = this._enteredPlayers.ToList();
        }

        await this.GameEndedAsync(players).ConfigureAwait(false);
    }

    private GameMap CreateMap()
    {
        if (this.Map is not null)
        {
            throw new InvalidOperationException("The map is already created.");
        }

        var mapDefinition = this.Definition.Entrance?.Map ?? throw new InvalidOperationException($"{nameof(this.Definition)} contains no entrance map.");
        var map = this._mapInitializer.CreateGameMap(mapDefinition);

        map.ObjectRemoved += this.OnObjectRemovedFromMapAsync;
        map.ObjectAdded += this.OnObjectAddedToMapAsync;
        return map;
    }

    private async ValueTask ShutdownGameAsync()
    {
        await this.MovePlayersToSafezoneAsync().ConfigureAwait(false);

        await this.DisposeAsync().ConfigureAwait(false);
    }

    private async ValueTask MovePlayersToSafezoneAsync()
    {
        List<Player> players;
        using (await this._enterLock.WriterLockAsync().ConfigureAwait(false))
        {
            players = this._enteredPlayers.ToList();
            this._enteredPlayers.Clear();
        }

        foreach (var player in players)
        {
            await player.WarpToSafezoneAsync().ConfigureAwait(false);
        }
    }

    private bool DoesRewardApply(Player player, int playerRank, MiniGameReward reward)
    {
        if (reward.Rank is not null && reward.Rank != playerRank)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Alive) && (!player.IsAlive || player.CurrentMap != this.Map))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Dead) && (player.IsAlive && player.CurrentMap == this.Map))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerExists) && this.Winner is null)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerNotExists) && this.Winner is not null)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Winner) && this.Winner != player)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Loser)
            && (this.Winner == player || (player.Party == this.Winner?.Party && player.Party is not null)))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinningParty)
            && (this.Winner?.Party is null || this.Winner.Party != player.Party))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerOrInWinningParty)
            && (this.Winner?.Party is null || this.Winner.Party != player.Party)
            && this.Winner != player)
        {
            return false;
        }

        if (reward.RequiredKill is { } requiredKill
            && (!this._rewardRelatedKills.TryGetValue(requiredKill, out var killed)
                || !killed))
        {
            return false;
        }

        return true;
    }

    private async Task ApplyChangeEventAsync(MiniGameChangeEvent changeEvent, string? triggeredBy = null)
    {
        try
        {
            if (changeEvent.TerrainChanges.Any())
            {
                await this.OnTerrainChangingAsync(changeEvent).ConfigureAwait(false);
                await this.UpdateClientTerrainAsync(changeEvent.TerrainChanges).ConfigureAwait(false);
                this.UpdateServerTerrain(changeEvent.TerrainChanges);
                await this.Map.ClearDropsOnInvalidTerrain().ConfigureAwait(false);
                await this.OnTerrainChangedAsync(changeEvent).ConfigureAwait(false);
            }

            if (changeEvent.SpawnArea is { } spawnArea)
            {
                for (int i = 0; i < spawnArea.Quantity; i++)
                {
                    await this._mapInitializer.InitializeSpawnAsync(i, this.Map, spawnArea, this).ConfigureAwait(false);
                }
            }

            if (changeEvent.Message is { } message)
            {
                await this.ShowMessageAsync(string.Format(message, triggeredBy)).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected exception at change event {changeEvent}: {ex}", changeEvent.Description, ex);
        }
    }

    private void UpdateServerTerrain(ICollection<MiniGameTerrainChange> changes)
    {
        foreach (var change in changes)
        {
            var isSafezone = change.TerrainAttribute is TerrainAttributeType.Safezone;
            var map = isSafezone ? this.Map.Terrain.SafezoneMap : this.Map.Terrain.WalkMap;
            var targetValue = isSafezone ? change.SetTerrainAttribute : !change.SetTerrainAttribute;
            for (var x = change.StartX; x <= change.EndX; x++)
            {
                for (var y = change.StartY; y <= change.EndY; y++)
                {
                    map[x, y] = targetValue;
                    this.Map.Terrain.UpdateAiGridValue(x, y);
                }
            }
        }
    }

    private async ValueTask UpdateClientTerrainAsync(ICollection<MiniGameTerrainChange> changes)
    {
        var groupedChanges = changes
            .Where(c => c.IsClientUpdateRequired)
            .GroupBy(
                c => (c.SetTerrainAttribute, c.TerrainAttribute),
                c => (c.StartX, c.StartY, c.EndX, c.EndY))
            .Select(g => (g.Key, Areas: g.ToList()))
            .ToList();

        await this.ForEachPlayerAsync(async player =>
        {
            foreach (var group in groupedChanges)
            {
                await player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p => p.ChangeAttributesAsync(group.Key.TerrainAttribute, group.Key.SetTerrainAttribute, group.Areas)).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    private void CheckKillForEventChanges(IAttackable killedObject)
    {
        if (this.NextEvent is not { } nextEvent)
        {
            return;
        }

        if (this.IsKillValid(killedObject, nextEvent.Definition) && nextEvent.RegisterKill())
        {
            this._remainingEvents.Remove(nextEvent);
            this.UpdateNextEvent();
            _ = Task.Run(() => this.ApplyChangeEventAsync(nextEvent.Definition, killedObject.LastDeath?.KillerName));
        }
    }

    private void UpdateNextEvent()
    {
        this.NextEvent = this._remainingEvents.Count == 0
            ? null
            : this._remainingEvents.MinBy(e => e.Definition.Index);
    }

    private bool IsKillValid(IAttackable killedObject, MiniGameChangeEvent definition)
    {
        if (definition.MinimumTargetLevel.HasValue && killedObject.Attributes[Stats.Level] < definition.MinimumTargetLevel)
        {
            return false;
        }

        if (definition.Target == KillTarget.AnyMonster && killedObject is not Monster)
        {
            return false;
        }

        if (definition.Target == KillTarget.Specific
            && (killedObject is not NonPlayerCharacter npc || npc.Definition.Number != definition.TargetDefinition?.Number))
        {
            return false;
        }

        return true;
    }

    private async ValueTask<bool> AreEquippedItemsAllowedAsync(Player player)
    {
        if (player.Inventory is not { } inventory)
        {
            return false;
        }

        var result = true;
        foreach (var item in inventory.EquippedItems)
        {
            if (!this.IsItemAllowedToEquip(item))
            {
                await player.ShowMessageAsync($"Can't enter event with equipped item '{item.Definition?.Name ?? item.ToString()}'.").ConfigureAwait(false);
                result = false;
            }
        }

        return result;
    }

    /// <summary>
    /// The context of the <see cref="MiniGameChangeEvent"/>.
    /// </summary>
    protected class ChangeEventContext
    {
        private int _actualKills;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeEventContext"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="playerCount">The player count.</param>
        public ChangeEventContext(MiniGameChangeEvent definition, int playerCount)
        {
            this.Definition = definition;
            this.RequiredKills = definition.NumberOfKills;
            if (this.Definition.MultiplyKillsByPlayers)
            {
                this.RequiredKills *= playerCount;
            }
        }

        /// <summary>
        /// Gets the definition of the change event.
        /// </summary>
        public MiniGameChangeEvent Definition { get; }

        /// <summary>
        /// Gets the required kills.
        /// </summary>
        public int RequiredKills { get; }

        /// <summary>
        /// Gets the actual kills.
        /// </summary>
        public int ActualKills => this._actualKills;

        /// <summary>
        /// Registers a kill and returns if the target has been achieved.
        /// </summary>
        /// <returns>True, if the target has been achieved just right now.</returns>
        public bool RegisterKill()
        {
            if (this._actualKills == this.RequiredKills)
            {
                // Already achieved.
                return false;
            }

            return Interlocked.Increment(ref this._actualKills) == this.RequiredKills;
        }
    }
}