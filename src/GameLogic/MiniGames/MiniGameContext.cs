// <copyright file="MiniGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The context of a mini game.
/// </summary>
public class MiniGameContext : AsyncDisposable, IEventStateProvider
{
    /// <summary>
    /// The duration of the countdown message phase, which also acts as the minimum
    /// duration of the entering, game, and exit phases.
    /// </summary>
    private static readonly TimeSpan CountdownMessageDuration = TimeSpan.FromSeconds(30);

    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly MiniGamePlayerRegistry _players;
    private readonly MiniGameRewardService _rewards;
    private readonly MiniGameChangeEventProcessor _changeEvents;
    private readonly MiniGameSpawnWaveRunner _spawnWaves;

    private readonly CancellationTokenSource _gameEndedCts = new();

    private readonly SkippableDelay _skipDelay;

    private Stopwatch? _elapsedTimeSinceStart;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public MiniGameContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
    {
        this._gameContext = gameContext;
        this._mapInitializer = mapInitializer;
        this.Key = key;
        this.Definition = definition;
        this.EnterEndsAtUtc = DateTime.UtcNow.Add(definition.EnterDuration.AtLeast(CountdownMessageDuration));
        this.Logger = this._gameContext.LoggerFactory.CreateLogger(this.GetType());
        this.DropGenerator = this._gameContext.DropGenerator;
        this._skipDelay = new SkippableDelay(this.Logger, this);

        this.Map = this.CreateMap();

        this._players = new MiniGamePlayerRegistry(this.Definition);

        // Rewards intentionally follow the game's (possibly overridden) drop generator
        // instead of the game context one: ChaosCastleDropGenerator only overrides monster
        // kill drops and delegates reward generation back to the context generator,
        // so this is behavior-preserving while staying correct for custom generators.
        this._rewards = new MiniGameRewardService(this.Definition, this._gameContext, this.Map, () => this.DropGenerator, () => this.RemainingTime, () => this.Winner, this.Logger, this);

        this._changeEvents = new MiniGameChangeEventProcessor(
            this.Definition,
            this.Map,
            this._mapInitializer,
            this,
            this.Logger,
            (message, args) => this.ShowGoldenMessageAsync(message, args),
            this.OnTerrainChangingAsync,
            this.OnTerrainChangedAsync,
            this.ForEachPlayerAsync);
        this._spawnWaves = new MiniGameSpawnWaveRunner(
            this.Definition,
            this.Map,
            this._mapInitializer,
            this,
            this.Logger,
            this,
            message => this.ShowGoldenMessageAsync(message),
            () => this._elapsedTimeSinceStart?.Elapsed);

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
    /// Gets the UTC time when the entering phase ends. It's derived from the creation
    /// time and the configured <see cref="MiniGameDefinition.EnterDuration"/> and is used
    /// for the entrance announcements.
    /// </summary>
    public DateTime EnterEndsAtUtc { get; }

    /// <summary>
    /// Gets the map on which the game takes place.
    /// </summary>
    public GameMap Map { get; }

    /// <summary>
    /// Gets the current state of the game. The state is owned by the player
    /// registry, so reading it here and entering through
    /// <see cref="TryEnterAsync"/> can never disagree about it.
    /// </summary>
    public MiniGameState State => this._players.State;

    /// <inheritdoc />
    public bool IsEventRunning => this.State == MiniGameState.Playing;

    /// <summary>
    /// Gets the player count.
    /// </summary>
    public int PlayerCount => this._players.CountAlive();

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
    /// Gets the kills required by the next event which targets should be fulfilled
    /// by the players, or 0 when there is no next event.
    /// </summary>
    protected int NextEventRequiredKills => this._changeEvents.Current?.RequiredKills ?? 0;

    /// <summary>
    /// Gets the kills already registered toward the next event, or 0 when there is
    /// no next event.
    /// </summary>
    protected int NextEventActualKills => this._changeEvents.Current?.ActualKills ?? 0;

    /// <summary>
    /// Gets or sets the drop generator which should be used during the mini game.
    /// </summary>
    protected IDropGenerator DropGenerator { get; set; }

    /// <summary>
    /// Gets the minimum player count to start the game.
    /// </summary>
    protected virtual int MinimumPlayerCount => 1;

    /// <summary>
    /// Tries to enter the mini game. It fails if it's full or if it's not in an open state.
    /// </summary>
    /// <param name="player">The player that tries to enter.</param>
    /// <returns>A value indicating whether entering had success.</returns>
    public async ValueTask<EnterResult> TryEnterAsync(Player player)
    {
        var result = await this._players.TryEnterAsync(player, this.AreEquippedItemsAllowedAsync).ConfigureAwait(false);
        if (result != EnterResult.Success)
        {
            return result;
        }

        player.CurrentMiniGame = this;
        player.PlayerPickedUpItem += this.OnPlayerPickedUpItemAsync;

        return EnterResult.Success;
    }

    /// <summary>
    /// Skips the currently running timed wait of this game, e.g. the entering phase, the
    /// countdown, or an event-specific standby time. It's intended for game masters and
    /// automated tests, so that long waiting times can be bypassed.
    /// Only the wait which is running right now is affected; if no wait is currently
    /// running, nothing happens. In particular, a skip never arms a future wait, so it
    /// can't leak into a later phase and brick the event.
    /// Kill-based waits (waiting until enough monsters are killed) can't be skipped this
    /// way; use <c>/killall</c> for those instead.
    /// </summary>
    /// <returns><c>true</c> if a running wait has been skipped; otherwise, <c>false</c>.</returns>
    public bool SkipCurrentWait()
    {
        return this._skipDelay.TrySkip();
    }

    /// <summary>
    /// Announces to all players of this game that a game master skipped the current wait,
    /// so that the sudden progress of the event doesn't confuse anyone.
    /// </summary>
    /// <param name="gameMasterName">The name of the game master which skipped the wait.</param>
    public ValueTask AnnounceSkipAsync(string gameMasterName)
    {
        return this.ShowGoldenMessageAsync(nameof(PlayerMessage.SkipWaitAnnouncedFormat), gameMasterName);
    }

    /// <inheritdoc />
    public bool IsSpawnWaveActive(byte waveNumber)
    {
        return this._spawnWaves.IsActive(waveNumber);
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

    /// <summary>
    /// Waits for the specified duration, unless the wait is skipped through
    /// <see cref="SkipCurrentWait"/> or the <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="duration">The duration to wait.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> when the wait was skipped; otherwise, <c>false</c>.</returns>
    protected Task<bool> DelayWithSkipAsync(TimeSpan duration, CancellationToken cancellationToken)
    {
        return this._skipDelay.WaitAsync(duration, cancellationToken);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        try
        {
            this.Logger.LogDebug("{context}: Disposing mini game...", this);
            await base.DisposeAsyncCore().ConfigureAwait(false);

            await this._players.SetStateAsync(MiniGameState.Disposed).ConfigureAwait(false);

            await this.MovePlayersToSafezoneAsync().ConfigureAwait(false);

            this.Map.ObjectAdded -= this.OnObjectAddedToMapAsync;
            this.Map.ObjectRemoved -= this.OnObjectRemovedFromMapAsync;

            await this._gameContext.MiniGames.RemoveAsync(this).ConfigureAwait(false);
            await this._gameEndedCts.CancelAsync().ConfigureAwait(false);
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
        await this._players.ForEachAsync(playerAction).ConfigureAwait(false);
    }

    /// <summary>
    /// Will be called when the game has been started.
    /// </summary>
    /// <param name="players">The player which started with the game.</param>
    protected virtual async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        this._elapsedTimeSinceStart = new Stopwatch();
        this._elapsedTimeSinceStart.Start();

        await this._changeEvents.InitializeAsync(players.Count).ConfigureAwait(false);
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
            this._rewards.RegisterKill(npc.Definition);
            this._changeEvents.NotifyKill(npc);
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
            this._changeEvents.NotifyKill(player);
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
            this._rewards.RegisterKill(npc.Definition);
            this._changeEvents.NotifyKill(npc);
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
            player.Died -= this.OnPlayerDied;
            var remainingPlayerCount = await this._players.RemoveAsync(player).ConfigureAwait(false);
            var cantGameProceed = remainingPlayerCount == 0 && this.State != MiniGameState.Open;

            if (cantGameProceed)
            {
                await this._gameEndedCts.CancelAsync().ConfigureAwait(false);
            }
            else if (player.IsAlive)
            {
                this._changeEvents.NotifyKill(player);
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
    protected Task<(int BonusScore, int GivenMoney)> GiveRewardsAndGetBonusScoreAsync(Player player, int rank)
    {
        return this._rewards.GiveRewardsAndGetBonusScoreAsync(player, rank);
    }

    /// <summary>
    /// Saves the ranking of this game.
    /// </summary>
    /// <param name="scoreEntries">The entries of the ranking.</param>
    protected ValueTask SaveRankingAsync(IEnumerable<(int Rank, Character Character, int Score)> scoreEntries)
    {
        return this._rewards.SaveRankingAsync(scoreEntries);
    }

    /// <summary>
    /// Shows a golden center-screen message to all players in the mini game.
    /// </summary>
    /// <param name="message">
    /// The localized message template to be shown. The template will be translated
    /// using each player's culture before formatting.
    /// </param>
    /// <param name="args">
    /// Optional format arguments which will be applied to the translated message
    /// using <see cref="string.Format(string,object[])"/>.
    /// </param>
    protected async ValueTask ShowGoldenMessageAsync(LocalizedString message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message.Value))
        {
            return;
        }

        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                message.GetTranslation(player.Culture) is { Length: > 0 } translation
                    ? p.ShowMessageAsync(string.Format(translation, args), MessageType.GoldenCenter)
                    : ValueTask.CompletedTask)
            .AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Shows a golden center-screen message to all players in the mini game.
    /// </summary>
    /// <param name="messageKey">
    /// The key to a localized message template to be shown.
    /// The template will be translated using each player's culture before formatting.
    /// </param>
    /// <param name="args">
    /// Optional format arguments which will be applied to the translated message
    /// using <see cref="string.Format(string,object[])"/>.
    /// </param>
    protected async ValueTask ShowGoldenMessageAsync(string messageKey, params object?[] args)
    {
        await this.ForEachPlayerAsync(player => player.ShowLocalizedGoldenMessageAsync(messageKey, args).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask RunGameAsync(CancellationToken cancellationToken)
    {
        this.Logger.LogDebug("{context}: Running the game ...", this);
        try
        {
            var enterDuration = this.Definition.EnterDuration.AtLeast(CountdownMessageDuration);
            var gameDuration = this.Definition.GameDuration.AtLeast(CountdownMessageDuration);
            var exitDuration = this.Definition.ExitDuration.Subtract(CountdownMessageDuration).AtLeast(CountdownMessageDuration);

            this.Logger.LogDebug("{context}: Waiting for entering players for {enterDuration}", this, enterDuration);

            var messagePeriod = TimeSpan.FromMinutes(1);
            for (; enterDuration >= messagePeriod; enterDuration = enterDuration.Subtract(messagePeriod))
            {
                if (this.Definition.MapCreationPolicy != MiniGameMapCreationPolicy.Shared)
                {
                    await this.ShowGoldenMessageAsync(nameof(PlayerMessage.MiniGameStartsInMinutesFormat), this.Definition.Name, (int)enterDuration.TotalMinutes).ConfigureAwait(false);
                }

                if (await this.DelayWithSkipAsync(messagePeriod, cancellationToken).ConfigureAwait(false))
                {
                    // One skip ends the whole entering phase, not just one minute of it.
                    enterDuration = TimeSpan.Zero;
                    break;
                }
            }

            if (enterDuration > TimeSpan.Zero)
            {
                await this.DelayWithSkipAsync(enterDuration, cancellationToken).ConfigureAwait(false);
            }

            await this.CloseEntranceAsync().ConfigureAwait(false);
            if (this.PlayerCount < this.MinimumPlayerCount)
            {
                await this.ShowGoldenMessageAsync(nameof(PlayerMessage.MiniGameCantStartWithLessThanPlayers), this.MinimumPlayerCount).ConfigureAwait(false);
                if (this.Definition.EntranceFee > 0)
                {
                    await this.ForEachPlayerAsync(async player => player.TryAddMoney(this.Definition.EntranceFee)).ConfigureAwait(false);
                }

                return;
            }

            await this.ShowCountdownMessageAsync().ConfigureAwait(false);
            this.Logger.LogDebug("{context}: Waiting for the countdown duration of {countdownDuration}", this, CountdownMessageDuration);
            await this.DelayWithSkipAsync(CountdownMessageDuration, cancellationToken).ConfigureAwait(false);

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

            this.Logger.LogDebug("{context}: Waiting for the exit countdown duration of {countdownDuration}", this, CountdownMessageDuration);
            await Task.Delay(CountdownMessageDuration, default(CancellationToken)).ConfigureAwait(false);

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
        await this._players.SetStateAsync(MiniGameState.Closed).ConfigureAwait(false);
    }

    private async ValueTask StartAsync()
    {
        await this._players.SetStateAsync(MiniGameState.Playing).ConfigureAwait(false);
        var players = await this._players.GetSnapshotAsync().ConfigureAwait(false);

        await this.OnGameStartAsync(players).ConfigureAwait(false);
        await this._mapInitializer.InitializeNpcsOnEventStartAsync(this.Map, this).ConfigureAwait(false);
        _ = Task.Run(() => this._spawnWaves.RunAsync(this.GameEndedToken), this.GameEndedToken);
    }

    private async ValueTask ShowCountdownMessageAsync()
    {
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IUpdateMiniGameStateViewPlugIn>(p => p.UpdateStateAsync(this.Definition.Type, this.State)).AsTask()).ConfigureAwait(false);
    }

    private async ValueTask StopAsync()
    {
        await this._players.SetStateAsync(MiniGameState.Ended).ConfigureAwait(false);
        await this._gameEndedCts.CancelAsync().ConfigureAwait(false);

        this._spawnWaves.Clear();
        await this.Map.ClearEventSpawnedNpcsAsync().ConfigureAwait(false);

        var players = await this._players.GetSnapshotAsync().ConfigureAwait(false);

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
        var players = await this._players.ClearAsync().ConfigureAwait(false);

        foreach (var player in players)
        {
            await player.WarpToSafezoneAsync().ConfigureAwait(false);
        }
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
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CantEnterEventWithItem), item.Definition?.Name.GetTranslation(player.Culture) ?? item.ToString()).ConfigureAwait(false);
                result = false;
            }
        }

        return result;
    }
}