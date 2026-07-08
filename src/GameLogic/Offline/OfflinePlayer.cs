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
    /// A player who killed this bot this many times gets no (further) revenge: walking back a third
    /// time into the same lost fight would just be a death loop feeding the killer free kills.
    /// </summary>
    private const int RepeatedKillThreshold = 2;

    /// <summary>
    /// How long an attack by a player stays "hot" as a self-defense target, counted from the LAST hit
    /// (every attack refreshes it). Long enough to hold a grudge: an attacker who breaks off and comes
    /// back within this window is engaged again on sight, instead of being forgiven after seconds.
    /// </summary>
    private static readonly TimeSpan AggressionMemory = TimeSpan.FromMinutes(5);

    /// <summary>
    /// How long a revenge stays armed after the respawn. One attempt only: if the bot has not reached
    /// its death site within this time (long routes, fights on the way), it gives up and hunts normally.
    /// </summary>
    private static readonly TimeSpan RevengeDuration = TimeSpan.FromMinutes(3);

    /// <summary>
    /// How long the bot keeps away from hunting grounds near its death site after the same player
    /// killed it repeatedly (see <see cref="RepeatedKillThreshold"/>).
    /// </summary>
    private static readonly TimeSpan DeathSiteAvoidanceDuration = TimeSpan.FromMinutes(10);

    /// <summary>
    /// How long a death counts toward <see cref="RepeatedKillThreshold"/>. A kill by a player the bot
    /// has not seen for this long counts as a fresh grudge again, not as a repeated one.
    /// </summary>
    private static readonly TimeSpan DeathCountMemory = TimeSpan.FromMinutes(30);

    /// <summary>
    /// How often each (human) player killed this bot recently, keyed by character name. Written by the
    /// death plugin and read by the AI ticks, hence concurrent.
    /// </summary>
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, DeathRecord> _deathsByKiller = new();

    private OfflinePlayerMuHelper? _intelligence;
    private Task? _intelligenceDisposeTask;
    private Player? _lastAggressor;
    private DateTime _lastAggressionUtc;

    /// <summary>
    /// The pending (not yet armed, <see cref="RevengeState.ExpiresAtUtc"/> is null) or armed revenge.
    /// The state object is immutable and the field is written atomically, so the death plugin and the
    /// AI ticks can access it without a lock.
    /// </summary>
    private volatile RevengeState? _revenge;

    /// <summary>See <see cref="TryGetDeathSiteToAvoid"/>; immutable and written atomically, like <see cref="_revenge"/>.</summary>
    private volatile DeathSite? _deathSiteToAvoid;

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
    /// Registers that a (human) player killed this bot. The first kill makes a revenge pending: after
    /// respawning on the same map, the bot marches back to the place of its death (driven by the
    /// <see cref="Bots.BotNavigator"/>) with re-armed aggressor memory, so it attacks the killer on
    /// sight. A repeated kill by the same player (see <see cref="RepeatedKillThreshold"/>) cancels
    /// revenge instead and makes the bot avoid hunting grounds near the death site for a while.
    /// </summary>
    /// <param name="killer">The player who killed this bot.</param>
    internal void RegisterDeathByPlayer(Player killer)
    {
        if (this.CurrentMap?.Definition is not { } deathMap)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var deathPosition = this.Position;
        var record = this._deathsByKiller.AddOrUpdate(
            killer.Name,
            _ => new DeathRecord(1, now),
            (_, existing) => now - existing.LastDeathUtc > DeathCountMemory
                ? new DeathRecord(1, now)
                : new DeathRecord(existing.Count + 1, now));

        if (record.Count >= RepeatedKillThreshold)
        {
            this._revenge = null;
            this._deathSiteToAvoid = new DeathSite(deathPosition, deathMap, now + DeathSiteAvoidanceDuration);
            this.Logger.LogInformation(
                "Bot '{Name}' was killed by '{Killer}' again; giving up on revenge and avoiding the area around {Position} for a while.",
                this.Name,
                killer.Name,
                deathPosition);
            return;
        }

        this._revenge = new RevengeState(killer, deathPosition, deathMap, null);
    }

    /// <summary>
    /// Arms a pending revenge once the bot respawned, called by the <see cref="OfflinePlayerMuHelper"/>
    /// when a bot resumes after death. Only a respawn on the map the bot died on qualifies (from any
    /// other map the march back would be meaningless); the aggressor memory is re-armed, so the combat
    /// AI attacks the killer on sight, and the revenge gets its time-to-live.
    /// </summary>
    internal void ArmRevengeAfterRespawn()
    {
        if (this._revenge is not { ExpiresAtUtc: null } revenge)
        {
            return;
        }

        if (!object.Equals(this.CurrentMap?.Definition, revenge.DeathMap))
        {
            this._revenge = null;
            return;
        }

        this._revenge = revenge with { ExpiresAtUtc = DateTime.UtcNow + RevengeDuration };
        this.RegisterAggressor(revenge.Killer);
        this.Logger.LogInformation("Bot '{Name}' returns to avenge its death against '{Killer}'.", this.Name, revenge.Killer.Name);
    }

    /// <summary>
    /// Gets the destination of an armed, still running revenge. Expires the revenge when its
    /// time-to-live ran out or the bot is no longer on the map it died on (e.g. it warped away).
    /// </summary>
    /// <param name="currentMap">The map the bot is currently on.</param>
    /// <param name="deathSite">The place of the bot's death to march back to.</param>
    /// <returns><c>true</c> if a revenge is active and <paramref name="deathSite"/> was set.</returns>
    internal bool TryGetRevengeDestination(GameMapDefinition currentMap, out Point deathSite)
    {
        deathSite = default;
        if (this._revenge is not { ExpiresAtUtc: { } expiresAt } revenge)
        {
            return false;
        }

        if (DateTime.UtcNow > expiresAt)
        {
            this.ExpireRevenge("it timed out before the bot reached the death site");
            return false;
        }

        if (!object.Equals(currentMap, revenge.DeathMap))
        {
            this.ExpireRevenge("the bot left the map it died on");
            return false;
        }

        deathSite = revenge.DeathPosition;
        return true;
    }

    /// <summary>
    /// Ends an active revenge - the single attempt is spent, the bot returns to its normal routine.
    /// The aggressor memory is deliberately left armed: if the killer is still around, the combat AI
    /// engages it, and if it strikes again, self-defense re-arms the memory anyway.
    /// </summary>
    /// <param name="reason">Why the revenge ended, for the log.</param>
    internal void ExpireRevenge(string reason)
    {
        if (this._revenge is { } revenge)
        {
            this._revenge = null;
            this.Logger.LogInformation("Bot '{Name}' revenge against '{Killer}' ended: {Reason}.", this.Name, revenge.Killer.Name, reason);
        }
    }

    /// <summary>
    /// Gets the death site the bot should keep away from when picking a hunting ground - set after the
    /// same player killed it repeatedly, so it stops walking back into the same lost fight.
    /// </summary>
    /// <param name="currentMap">The map the bot is currently on.</param>
    /// <param name="deathSite">The place of the repeated deaths.</param>
    /// <returns><c>true</c> if an avoidance is active on the given map and <paramref name="deathSite"/> was set.</returns>
    internal bool TryGetDeathSiteToAvoid(GameMapDefinition currentMap, out Point deathSite)
    {
        deathSite = default;
        if (this._deathSiteToAvoid is not { } site
            || DateTime.UtcNow > site.AvoidUntilUtc
            || !object.Equals(currentMap, site.Map))
        {
            return false;
        }

        deathSite = site.Position;
        return true;
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

    /// <summary>
    /// How often (and how recently) a specific player killed this bot.
    /// </summary>
    private sealed record DeathRecord(int Count, DateTime LastDeathUtc);

    /// <summary>
    /// A revenge for a death by a player's hand: pending while <see cref="ExpiresAtUtc"/> is null
    /// (the bot has not respawned yet), armed and running once it is set.
    /// </summary>
    private sealed record RevengeState(Player Killer, Point DeathPosition, GameMapDefinition DeathMap, DateTime? ExpiresAtUtc);

    /// <summary>
    /// A death site the bot avoids when picking hunting grounds, after repeated deaths there.
    /// </summary>
    private sealed record DeathSite(Point Position, GameMapDefinition Map, DateTime AvoidUntilUtc);
}
