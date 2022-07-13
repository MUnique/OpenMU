// <copyright file="MiniGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
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
using Nito.AsyncEx.Synchronous;

/// <summary>
/// The context of a mini game.
/// </summary>
public class MiniGameContext : Disposable, IEventStateProvider
{
    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly AsyncLock _enterLock = new ();

    private readonly HashSet<Player> _enteredPlayers = new ();
    private readonly ConcurrentDictionary<MonsterDefinition, bool> _rewardRelatedKills;

    private readonly CancellationTokenSource _gameEndedCts = new ();

    private readonly HashSet<byte> _currentSpawnWaves = new ();
    private readonly List<ChangeEventContext> _remainingEvents = new ();

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

        this._rewardRelatedKills = new (
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
    /// Tries to enter the mini game. It will fail, if it's full, of if it's not in an open state.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <returns>A value indicating whether entering had success.</returns>
    public async ValueTask<EnterResult> TryEnterAsync(Player player)
    {
        using (await this._enterLock.LockAsync().ConfigureAwait(false))
        {
            if (this.State != MiniGameState.Open)
            {
                return EnterResult.NotOpen;
            }

            if (this._enteredPlayers.Count >= this.Definition.MaximumPlayerCount)
            {
                return EnterResult.Full;
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
        return this._currentSpawnWaves.Contains(waveNumber);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // TODO: implement AsyncDisposable
        using (this._enterLock.Lock())
        {
            this.State = MiniGameState.Disposed;
        }

        try
        {
            this.MovePlayersToSafezoneAsync().AsTask().WaitAndUnwrapException();

            this.Map.ObjectAdded -= this.OnObjectAddedToMapAsync;
            this.Map.ObjectRemoved -= this.OnObjectRemovedFromMapAsync;

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
    protected async ValueTask ForEachPlayerAsync(Func<Player, Task> playerAction)
    {
        using var @lock = await this._enterLock.LockAsync().ConfigureAwait(false);
        await this._enteredPlayers.Select(playerAction).WhenAll();
    }

    /// <summary>
    /// Will be called when the game has been started.
    /// </summary>
    /// <param name="players">The player which started with the game.</param>
    protected virtual async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        var startEvents = this.Definition.ChangeEvents
            .OrderBy(e => e.Index)
            .TakeWhile(e => e.NumberOfKills == 0)
            .ToList();

        foreach (var changeEvent in startEvents)
        {
            await this.ApplyChangeEventAsync(changeEvent);
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
            await this.ShowScoreAsync(player);
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
        this._gameContext.PlugInManager.GetPlugInPoint<IObjectAddedToMapPlugIn>()?.ObjectAddedToMap(args.Map, args.Object);
        if (args.Object is Monster monster)
        {
            monster.Died += this.OnMonsterDied;
        }

        if (args.Object is Destructible destructible)
        {
            destructible.Died += this.OnDestructibleDied;
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
            this._gameContext.PlugInManager.GetPlugInPoint<IObjectRemovedFromMapPlugIn>()?.ObjectRemovedFromMap(args.Map, args.Object);
            if (args.Object is not Player player)
            {
                return;
            }

            player.CurrentMiniGame = null;
            bool cantGameProceed;
            using (await this._enterLock.LockAsync().ConfigureAwait(false))
            {
                this._enteredPlayers.Remove(player);
                cantGameProceed = this._enteredPlayers.Count == 0 && this.State != MiniGameState.Open;
            }

            if (cantGameProceed)
            {
                this._gameEndedCts.Cancel();
            }
            else
            {
                this.CheckKillForEventChanges(player);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error when handling the removed map object {args.Object}.");
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
    /// <returns>The bonus score and the given money.</returns>
    protected async Task<(int BonusScore, int GivenMoney)> GiveRewardsAndGetBonusScoreAsync(Player player, int rank)
    {
        int bonusScore = 0;
        int givenMoney = 0;
        var rewards = this.Definition.Rewards.Where(r => this.DoesRewardApply(player, rank, r));
        foreach (var reward in rewards)
        {
            var result = await this.GiveRewardAsync(player, reward);
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

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error while saving mini game ranking");
        }
    }

    private async ValueTask<(int BonusScore, int GivenMoney)> GiveRewardAsync(Player player, MiniGameReward reward)
    {
        switch (reward.RewardType)
        {
            case MiniGameRewardType.Experience:
                await player.AddExperienceAsync(reward.RewardAmount, null);
                break;
            case MiniGameRewardType.ExperiencePerRemainingSeconds:
                var seconds = (int)this.RemainingTime.TotalSeconds;
                if (seconds > 0)
                {
                    await player.AddExperienceAsync(seconds * reward.RewardAmount, null);
                }

                break;
            case MiniGameRewardType.Money:
                if (!player.TryAddMoney(reward.RewardAmount))
                {
                    await player.ShowMessageAsync("Couldn't add reward money, inventory is full.");
                }

                return (0, reward.RewardAmount);
            case MiniGameRewardType.Item:
                await this.GiveItemRewardAsync(player, reward);
                break;
            case MiniGameRewardType.ItemDrop:
                await this.GiveItemRewardAsync(player, reward, true);
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
            this.Logger.LogWarning($"Item reward is not set in {reward.GetId()}");
            return;
        }

        for (int i = 0; i < reward.RewardAmount; i++)
        {
            var item = this._gameContext.DropGenerator.GenerateItemDrop(reward.ItemReward);
            if (item is null)
            {
                this.Logger.LogInformation($"No item has been generated by reward {reward.GetId()} for player {player}.");
                return;
            }

            var droppedItem = new DroppedItem(item, player.RandomPosition, this.Map, player, player.GetAsEnumerable());

            var shouldDrop = drop || !(player.Inventory is not null && await player.Inventory.AddItemAsync(item));
            if (shouldDrop)
            {
                this.Logger.LogInformation($"Reward {item} for {player} has been dropped by players coordinates {player.Position}.");
                await this.Map.AddAsync(droppedItem);
            }
        }
    }

    private async Task RunSpawnWavesAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var spawnWave in this.Definition.SpawnWaves)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await this.RunSpawnWaveAsync(spawnWave, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // game ended.
        }
    }

    private async ValueTask RunSpawnWaveAsync(MiniGameSpawnWave spawnWave, CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (spawnWave.StartTime > TimeSpan.Zero)
            {
                await Task.Delay(spawnWave.StartTime, cancellationToken).ConfigureAwait(false);
            }

            this.Logger.LogInformation("Starting next wave: {0}", spawnWave.Description);
            if (spawnWave.Message is { } message)
            {
                await this.ShowMessageAsync(message);
            }

            this._currentSpawnWaves.Add(spawnWave.WaveNumber);
            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, spawnWave.WaveNumber);
            await Task.Delay(spawnWave.EndTime - spawnWave.StartTime, cancellationToken).ConfigureAwait(false);
            this.Logger.LogInformation("Wave ended: {0}", spawnWave.Description);
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

    private async ValueTask RunGameAsync(CancellationToken cancellationToken)
    {
        var countdownMessageDuration = TimeSpan.FromSeconds(30);
        try
        {
            var enterDuration = this.Definition.EnterDuration.Subtract(countdownMessageDuration).AtLeast(countdownMessageDuration);
            var gameDuration = this.Definition.GameDuration.AtLeast(countdownMessageDuration);
            var exitDuration = this.Definition.ExitDuration.Subtract(countdownMessageDuration).AtLeast(countdownMessageDuration);

            await Task.Delay(enterDuration, cancellationToken).ConfigureAwait(false);
            if (this._enteredPlayers.Count == 0)
            {
                return;
            }

            await this.ShowCountdownMessageAsync().ConfigureAwait(false);
            await Task.Delay(countdownMessageDuration).ConfigureAwait(false);

            await this.StartAsync().ConfigureAwait(false);

            try
            {
                await Task.Delay(gameDuration, cancellationToken).ConfigureAwait(false);
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
        using (await this._enterLock.LockAsync().ConfigureAwait(false))
        {
            this.State = MiniGameState.Playing;
            players = this._enteredPlayers.ToList();
        }

        await this.OnGameStartAsync(players);
        await this._mapInitializer.InitializeNpcsOnEventStartAsync(this.Map, this);
        _ = Task.Run(() => this.RunSpawnWavesAsync(this.GameEndedToken), this.GameEndedToken);
    }

    private async ValueTask ShowCountdownMessageAsync()
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IUpdateMiniGameStateViewPlugIn>(p => p.UpdateStateAsync(this.Definition.Type, this.State)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask ShowMessageAsync(string message)
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask StopAsync()
    {
        using (await this._enterLock.LockAsync().ConfigureAwait(false))
        {
            this.State = MiniGameState.Ended;
            this._gameEndedCts.Cancel();
        }

        this._currentSpawnWaves.Clear();
        await this.Map.ClearEventSpawnedNpcsAsync();

        List<Player> players;
        using (await this._enterLock.LockAsync().ConfigureAwait(false))
        {
            players = this._enteredPlayers.ToList();
        }

        await this.GameEndedAsync(players);
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

        this.Dispose();
    }

    private async ValueTask MovePlayersToSafezoneAsync()
    {
        List<Player> players;
        using (await this._enterLock.LockAsync().ConfigureAwait(false))
        {
            players = this._enteredPlayers.ToList();
            this._enteredPlayers.Clear();
        }

        foreach (var player in players)
        {
            await player.WarpToSafezoneAsync();
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
        if (changeEvent.TerrainChanges.Any())
        {
            await this.UpdateClientTerrain(changeEvent.TerrainChanges);
            this.UpdateServerTerrain(changeEvent.TerrainChanges);
        }

        if (changeEvent.SpawnArea is { } spawnArea)
        {
            for (int i = 0; i < spawnArea.Quantity; i++)
            {
                await this._mapInitializer.InitializeSpawnAsync(this.Map, spawnArea, this);
            }
        }

        if (changeEvent.Message is { } message)
        {
            await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(string.Format(message, triggeredBy), Interfaces.MessageType.GoldenCenter)).AsTask());
        }
    }

    private void UpdateServerTerrain(ICollection<MiniGameTerrainChange> changes)
    {
        foreach (var change in changes)
        {
            var isSafezone = change.TerrainAttribute is TerrainAttributeType.Safezone;
            var map = isSafezone ? this.Map.Terrain.SafezoneMap : this.Map.Terrain.WalkMap;

            for (var x = change.StartX; x <= change.EndX; x++)
            {
                for (var y = change.StartY; y <= change.EndY; y++)
                {
                    map[x, y] = isSafezone ? change.SetTerrainAttribute : !change.SetTerrainAttribute;
                    this.Map.Terrain.UpdateAiGridValue(x, y);
                }
            }
        }
    }

    private async ValueTask UpdateClientTerrain(ICollection<MiniGameTerrainChange> changes)
    {
        var groupedChanges = changes
            .GroupBy(
                c => (c.SetTerrainAttribute, c.TerrainAttribute),
                c => (c.StartX, c.StartY, c.EndX, c.EndY))
            .Select(g => (g.Key, Areas: g.ToList()))
            .ToList();

        await this.ForEachPlayerAsync(async player =>
        {
            foreach (var group in groupedChanges)
            {
                await player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p => p.ChangeAttributesAsync(group.Key.TerrainAttribute, group.Key.SetTerrainAttribute, group.Areas));
            }
        });
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
        this.NextEvent = this._remainingEvents.MinBy(e => e.Definition.Index);
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