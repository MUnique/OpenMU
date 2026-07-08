// <copyright file="OfflinePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An offline player that continues leveling after the real client disconnects.
/// </summary>
public class OfflinePlayer : Player
{
    /// <summary>
    /// How long an attack by a player stays "hot" as a self-defense target, counted from the LAST hit
    /// (every attack refreshes it). Long enough to hold a grudge: an attacker who breaks off and comes
    /// back within this window is engaged again on sight, instead of being forgiven after seconds.
    /// </summary>
    private static readonly TimeSpan AggressionMemory = TimeSpan.FromMinutes(5);

    private OfflinePlayerMuHelper? _intelligence;
    private Task? _intelligenceDisposeTask;
    private Player? _lastAggressor;
    private DateTime _lastAggressionUtc;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflinePlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public OfflinePlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <summary>
    /// Gets the login name this offline player belongs to.
    /// </summary>
    public string? AccountLoginName => this.Account?.LoginName;

    /// <summary>
    /// Gets the start timestamp of the offline session.
    /// </summary>
    public DateTime StartTimestamp { get; internal set; }

    /// <summary>
    /// Gets or sets the position the intelligence hunts around. For a plain offline player this is
    /// the spawn position and never changes. Bots update it to roam between hunting grounds.
    /// </summary>
    public Point HuntingOrigin { get; set; }

    /// <summary>
    /// Gets a value indicating whether the player should keep playing after dying and respawning.
    /// A normal offline session ends on death; bots override this to keep running forever.
    /// </summary>
    public virtual bool RespawnAndContinue => false;

    /// <summary>
    /// Gets actions queued from outside the AI tick (e.g. skill learning on level-up), which the
    /// <see cref="OfflinePlayerMuHelper"/> drains at the start of each tick. This serializes such
    /// mutations with the combat handler, so e.g. the skill list is never modified while combat is
    /// enumerating it.
    /// </summary>
    internal System.Collections.Concurrent.ConcurrentQueue<Func<ValueTask>> PendingBotActions { get; } = new();

    /// <summary>
    /// Gets the player who most recently attacked this bot (self-defense target), if the aggression
    /// is recent enough and the aggressor is still a viable target.
    /// </summary>
    internal Player? RecentAggressor
    {
        get
        {
            if (this._lastAggressor is { } aggressor
                && DateTime.UtcNow - this._lastAggressionUtc <= AggressionMemory
                && aggressor.IsAlive
                && !aggressor.IsAtSafezone())
            {
                return aggressor;
            }

            return null;
        }
    }

    /// <summary>
    /// Initializes the offline player by loading the account fresh from the database.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    /// <param name="characterName">The character name to continue with.</param>
    /// <returns><c>true</c> if successfully started.</returns>
    public async ValueTask<bool> InitializeAsync(string loginName, string characterName)
    {
        try
        {
            this.StartTimestamp = DateTime.UtcNow;

            var account = await this.PersistenceContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
            if (account is null)
            {
                this.Logger.LogError("Failed to load account {LoginName} for offline session.", loginName);
                return false;
            }

            var character = account.Characters?.FirstOrDefault(c => c.Name == characterName);
            if (character is null)
            {
                this.Logger.LogError("Character {CharacterName} not found in account {LoginName}.", characterName, loginName);
                return false;
            }

            this.Account = account;

            await this.AdvanceToCharacterSelectionStateAsync().ConfigureAwait(false);

            await this.SetupCharacterAsync(character).ConfigureAwait(false);

            await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

            this.HuntingOrigin = this.Position;

            this.StartIntelligence();

            this.Logger.LogDebug(
                "Offline player started for character {CharacterName} on map {Map} at {Position}.",
                character.Name,
                character.CurrentMap?.Name,
                this.Position);

            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to initialize offline player for {LoginName}.", loginName);
            return false;
        }
    }

    /// <summary>
    /// Stops the offline player and removes it from the world.
    /// </summary>
    public virtual async ValueTask StopAsync()
    {
        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Registers a player who attacked this bot, so the combat AI can defend itself.
    /// </summary>
    /// <param name="aggressor">The player who attacked this bot.</param>
    internal void RegisterAggressor(Player aggressor)
    {
        this._lastAggressor = aggressor;
        this._lastAggressionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Executes and removes all queued <see cref="PendingBotActions"/>.
    /// </summary>
    internal async ValueTask DrainPendingBotActionsAsync()
    {
        while (this.PendingBotActions.TryDequeue(out var action))
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Queued bot action failed for {Account}.", this.AccountLoginName);
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask InternalDisconnectAsync()
    {
        if (this._intelligence is { } intelligence)
        {
            this._intelligence = null;
            this._intelligenceDisposeTask = Task.Run(async () =>
            {
                try
                {
                    await intelligence.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error disposing intelligence for offline player {AccountLoginName}.", this.AccountLoginName);
                }
            });
        }

        await base.InternalDisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._intelligenceDisposeTask is { } disposeTask)
        {
            await disposeTask.ConfigureAwait(false);
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        => new OfflineViewPlugInContainer(this);

    /// <summary>
    /// Starts the intelligence which drives this offline player. Overridden by bots to also run navigation.
    /// </summary>
    protected virtual void StartIntelligence()
    {
        this._intelligence = new OfflinePlayerMuHelper(this);
        this._intelligence.Start();
    }

    private async ValueTask AdvanceToCharacterSelectionStateAsync()
    {
        // Advance state to allow the intelligence to perform actions.
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.LoginScreen).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Authenticated).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.CharacterSelection).ConfigureAwait(false);
    }

    private async ValueTask SetupCharacterAsync(Character character)
    {
        await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);
        await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);
    }
}