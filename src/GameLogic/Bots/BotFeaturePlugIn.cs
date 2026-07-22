// <copyright file="BotFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature plugin which spawns and maintains server-side bots.
/// Appears in the "Feature Plugins" section of the admin panel next to the MU Helper and reset features.
/// </summary>
[PlugIn]
[Display(Name = "Bots", Description = "Spawns server-side bots which hunt monsters on the maps. Configure the accounts to animate and enable the feature.")]
[Guid("6F3A2B91-7C4E-4D88-9A1F-2E5C0B7A4D63")]
public class BotFeaturePlugIn : IFeaturePlugIn, IPeriodicTaskPlugIn, ISupportCustomConfiguration<BotConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Delay before the first spawn attempt, giving the server time to finish starting up
    /// (maps, configuration and plugins fully initialized).
    /// </summary>
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(15);

    private static readonly TimeSpan MaintenanceInterval = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PartyReformInterval = TimeSpan.FromMinutes(60);

    /// <summary>
    /// The typical activity of a player base by local hour (0..1): quietest in the early morning,
    /// busiest in the evening. Scales between <see cref="BotConfiguration.MinOnlineSharePercent"/>
    /// and 100% of the bot population.
    /// </summary>
    private static readonly double[] ActivityByHour =
    [
        0.30, 0.15, 0.05, 0.00, 0.00, 0.05, 0.10, 0.20, 0.30, 0.35, 0.40, 0.45,
        0.50, 0.50, 0.55, 0.60, 0.70, 0.80, 0.90, 1.00, 1.00, 0.95, 0.80, 0.50,
    ];

    /// <summary>
    /// The state of the feature, per game server: the plugin instance is shared by all game servers of
    /// the process, while <see cref="ExecuteTaskAsync"/> is called by each of them separately. One shared
    /// state would mean the server whose timer fires first animates the whole population (which is how
    /// the bots used to end up on a single server), and the other servers doing nothing at all.
    /// </summary>
    private readonly ConcurrentDictionary<IGameContext, ServerState> _states = new();

    /// <summary>
    /// The phase of a game server's single startup pass. The periodic task timer fires every second
    /// WITHOUT awaiting the previous invocation, so during the minutes-long generation/spawn further
    /// ticks arrive concurrently - they must neither re-enter the startup nor run the maintenance
    /// (e.g. the presence rotation) against a half-spawned population.
    /// </summary>
    private enum StartupPhase
    {
        /// <summary>The startup has not run yet.</summary>
        NotStarted = 0,

        /// <summary>The startup (generation and spawn) is in progress.</summary>
        InProgress = 1,

        /// <summary>The startup is done; the server is in maintenance mode.</summary>
        Done = 2,
    }

    /// <inheritdoc />
    public BotConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the bot configuration of the given game context, so the bot handlers can read their
    /// admin-panel editable settings without being handed the plugin around (same shape as
    /// <see cref="BotResetHandler.GetResetConfiguration"/>).
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration, or <c>null</c> when the bot feature is not configured.</returns>
    public static BotConfiguration? GetConfiguration(IGameContext gameContext)
        => gameContext.FeaturePlugIns.GetPlugIn<BotFeaturePlugIn>()?.Configuration;

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var state = this._states.GetOrAdd(gameContext, _ => new ServerState());
        var configuration = this.Configuration ??= CreateDefaultConfiguration();

        if (configuration.PurgeBots)
        {
            await this.PurgeAsync(gameContext, state, configuration).ConfigureAwait(false);
            return;
        }

        if (!configuration.Enabled)
        {
            // Switching the feature off takes effect right away instead of at the next restart: the
            // bots log out, and nothing is deleted. Re-checked on the following ticks, the feature may
            // get enabled again later - then the population is spawned from scratch.
            await this.StopBotsAsync(gameContext, state, "the bot feature was switched off").ConfigureAwait(false);
            return;
        }

        if (state.StartupState == (int)StartupPhase.Done)
        {
            await this.RunMaintenanceAsync(gameContext, state).ConfigureAwait(false);
            return;
        }

        if (DateTime.UtcNow < state.NextRunUtc
            || Interlocked.CompareExchange(ref state.StartupState, (int)StartupPhase.InProgress, (int)StartupPhase.NotStarted) != (int)StartupPhase.NotStarted)
        {
            return;
        }

        try
        {
            await this.SpawnPopulationAsync(gameContext, state, configuration).ConfigureAwait(false);
        }
        finally
        {
            // Like before: the startup runs once, even when parts of it failed (the errors are
            // logged); the maintenance pass takes over from here.
            Interlocked.Exchange(ref state.StartupState, (int)StartupPhase.Done);
        }
    }

    /// <summary>
    /// Stops every bot this server animates and puts it back to the state before the startup pass, so
    /// the population is spawned from scratch if the feature is switched on again. Nothing is deleted.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="state">The state of this server.</param>
    /// <param name="reason">The reason to log, if there was anything to stop.</param>
    private async ValueTask StopBotsAsync(GameContext gameContext, ServerState state, string reason)
    {
        if (state.Manager.BotCount == 0 && state.StartupState == (int)StartupPhase.NotStarted)
        {
            // The usual case of a server without bots - this runs on every tick, so it stays cheap.
            return;
        }

        // Take over the startup state machine, so the bots are not stopped while a startup pass is
        // still spawning them (it would spawn into the emptied manager afterwards).
        if (Interlocked.CompareExchange(ref state.StartupState, (int)StartupPhase.InProgress, (int)StartupPhase.Done) != (int)StartupPhase.Done
            && Interlocked.CompareExchange(ref state.StartupState, (int)StartupPhase.InProgress, (int)StartupPhase.NotStarted) != (int)StartupPhase.NotStarted)
        {
            // A startup pass is running; retried on one of the next ticks.
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);
        try
        {
            var stopped = state.Manager.BotCount;
            await state.Manager.StopAllAsync().ConfigureAwait(false);
            state.PendingRespawns.Clear();
            if (stopped > 0)
            {
                logger.LogInformation("Stopped {Stopped} bot(s): {Reason}.", stopped, reason);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to stop the bots.");
        }
        finally
        {
            Interlocked.Exchange(ref state.StartupState, (int)StartupPhase.NotStarted);
        }
    }

    /// <summary>
    /// Carries out a requested purge: every bot account is deleted and the feature switches itself off,
    /// so the population is NOT generated again - the difference to <see cref="BotConfiguration.ResetBots"/>.
    /// Works with the feature enabled or disabled, and whatever number of accounts is configured.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="state">The state of this server.</param>
    /// <param name="configuration">The bot configuration.</param>
    private async ValueTask PurgeAsync(GameContext gameContext, ServerState state, BotConfiguration configuration)
    {
        if (DateTime.UtcNow < state.NextRunUtc)
        {
            // The same head start the startup pass gets - and where a failed purge backs off to.
            return;
        }

        // The bots have to be gone before their accounts are: one which is still online would go on
        // saving a character whose row is about to be deleted.
        await this.StopBotsAsync(gameContext, state, "the bot population is being purged").ConfigureAwait(false);
        if (state.Manager.BotCount > 0
            || Interlocked.CompareExchange(ref state.StartupState, (int)StartupPhase.InProgress, (int)StartupPhase.NotStarted) != (int)StartupPhase.NotStarted)
        {
            // A startup pass still holds the state machine; retried on one of the next ticks.
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);
        try
        {
            var partition = await BotServerPartition.CreateAsync(gameContext, configuration, logger).ConfigureAwait(false);
            if (!partition.IsGenerator)
            {
                // Another server deletes the accounts and clears the flags. This one only had to stop
                // its own bots; the switched-off feature keeps it from starting them again.
                return;
            }

            var generator = new BotGenerator(gameContext, logger);
            var deleted = await generator.DeleteAllBotsAsync().ConfigureAwait(false);

            // Switching the feature off is what makes this a purge instead of a reset: the generation
            // below would otherwise create the whole population again, right after it was deleted.
            configuration.Enabled = false;
            configuration.PurgeBots = false;
            configuration.ResetBots = false;
            await this.PersistConfigurationAsync(gameContext, configuration, logger).ConfigureAwait(false);

            logger.LogInformation("Purge requested: deleted {Deleted} bot account(s) and switched the bot feature off.", deleted);
        }
        catch (Exception ex)
        {
            // The flag stays set, so the purge is retried - backed off, to not repeat it every second.
            state.NextRunUtc = DateTime.UtcNow + MaintenanceInterval;
            logger.LogError(ex, "Failed to purge the bot population.");
        }
        finally
        {
            Interlocked.Exchange(ref state.StartupState, (int)StartupPhase.NotStarted);
        }
    }

    private async ValueTask SpawnPopulationAsync(GameContext gameContext, ServerState state, BotConfiguration configuration)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);

        var generator = new BotGenerator(gameContext, logger);
        var partition = state.Partition = await BotServerPartition.CreateAsync(gameContext, configuration, logger).ConfigureAwait(false);

        if (configuration.ResetBots && !partition.IsGenerator)
        {
            // Not silent: the flag is set, but this server is not the one which acts on it.
            logger.LogInformation("Reset requested: another game server of the deployment carries it out.");
        }

        if (configuration.ResetBots && partition.IsGenerator)
        {
            try
            {
                var deleted = await generator.DeleteAllBotsAsync().ConfigureAwait(false);
                logger.LogInformation("Reset requested: deleted {Deleted} bot account(s); {Requested} account(s) are generated again.", deleted, Math.Max(configuration.NumberOfAccounts, 0));

                // Clear the flag (in memory and persisted) so the next restart does not purge again.
                configuration.ResetBots = false;
                await this.PersistConfigurationAsync(gameContext, configuration, logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reset the bot population.");
            }
        }

        if (partition.IsGenerator)
        {
            try
            {
                // Generate the persistent bot population if it is not there yet (idempotent). Only this
                // server does it - see BotServerPartition.IsGenerator; the others find the accounts once
                // they exist and retry the spawns of their own share meanwhile.
                var created = await generator.EnsureBotsAsync(configuration.NumberOfAccounts, configuration.MaxCharactersPerAccount).ConfigureAwait(false);
                if (created > 0)
                {
                    logger.LogInformation("Generated {Created} new bot account(s).", created);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate the bot population.");
            }
        }

        var charactersPerAccount = Math.Clamp(
            Math.Min(configuration.MaxCharactersPerAccount, gameContext.Configuration.MaximumCharactersPerAccount),
            1,
            BotConfiguration.MaxCharactersPerAccountLimit);

        var started = 0;
        var total = 0;
        for (var i = partition.FirstAccount; i < partition.FirstAccount + partition.AccountCount; i++)
        {
            var loginName = BotGenerator.GetLoginName(i);
            for (byte slot = 0; slot < charactersPerAccount; slot++)
            {
                total++;
                try
                {
                    if (await state.Manager.SpawnBotAsync(gameContext, loginName, slot).ConfigureAwait(false))
                    {
                        started++;
                    }
                    else
                    {
                        // The account may just not be generated yet (another server is generating the
                        // population right now) - the maintenance pass retries it.
                        state.PendingRespawns.Enqueue((loginName, slot));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to spawn bot for account '{LoginName}' (slot {Slot}).", loginName, slot);
                }
            }
        }

        // The proof-of-concept accounts remain an optional extra hook to animate existing (non-bot)
        // accounts. They are not part of the partitioned population, so only one server animates them.
        if (partition.IsGenerator)
        {
            foreach (var loginName in configuration.ParseProofOfConceptAccounts())
            {
                try
                {
                    if (await state.Manager.SpawnBotAsync(gameContext, loginName).ConfigureAwait(false))
                    {
                        started++;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to spawn proof-of-concept bot for account '{LoginName}'.", loginName);
                }
            }
        }

        logger.LogInformation("Bot feature started {Started} of {Total} bots.", started, total);

        try
        {
            await state.Manager.FormPartiesAsync(gameContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to form bot parties.");
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        foreach (var state in this._states.Values)
        {
            state.NextRunUtc = DateTime.UtcNow;
        }
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return CreateDefaultConfiguration();
    }

    private static BotConfiguration CreateDefaultConfiguration()
    {
        return new BotConfiguration();
    }

    /// <summary>
    /// Runs the periodic post-spawn maintenance: the presence rotation (one bot in or out per pass, so
    /// the population ebbs and flows smoothly over the day) and an hourly party re-formation which
    /// groups bots that lost or never had a party (e.g. after rotating back in).
    /// </summary>
    private async ValueTask RunMaintenanceAsync(GameContext gameContext, ServerState state)
    {
        if (DateTime.UtcNow < state.NextMaintenanceUtc)
        {
            return;
        }

        // The engine fires the periodic tasks every second WITHOUT awaiting the previous run, so a pass
        // which takes longer than its interval (spawning a bot loads a whole account from the database)
        // would otherwise overlap with itself: two passes restarting the same bot, rotating the presence
        // twice, forming parties in parallel. One pass at a time - like the startup below.
        if (Interlocked.CompareExchange(ref state.MaintenanceRunning, 1, 0) != 0)
        {
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        try
        {
            state.NextMaintenanceUtc = DateTime.UtcNow + MaintenanceInterval;

            var configuration = this.Configuration;
            if (configuration?.Enabled != true)
            {
                return;
            }

            await this.RespawnPendingAsync(gameContext, state).ConfigureAwait(false);
            await this.RestartFaultedBotsAsync(gameContext, state, logger).ConfigureAwait(false);
            await this.EvolveDueMastersAsync(gameContext, state, logger).ConfigureAwait(false);

            if (configuration.PresenceRotation)
            {
                await this.RotatePresenceAsync(gameContext, state, configuration, logger).ConfigureAwait(false);
            }

            if (DateTime.UtcNow >= state.NextPartyReformUtc)
            {
                state.NextPartyReformUtc = DateTime.UtcNow + PartyReformInterval;
                await state.Manager.FormPartiesAsync(gameContext).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot maintenance failed.");
        }
        finally
        {
            Interlocked.Exchange(ref state.MaintenanceRunning, 0);
        }
    }

    /// <summary>
    /// Restarts bots whose AI keeps throwing (see <see cref="BotPlayer.AwaitsFaultRestart"/>). The
    /// engine's attribute system is not thread-safe, and a lost race can corrupt a character's attribute
    /// graph for good: the bot stops playing and every following tick throws the same exception, up to a
    /// flood of them per second. A fresh login rebuilds the graph and heals it - the same thing a player
    /// would do, and the only cure available from outside the engine. Runs from the maintenance pass,
    /// which is the only place allowed to restart a bot.
    /// </summary>
    private async ValueTask RestartFaultedBotsAsync(GameContext gameContext, ServerState state, ILogger logger)
    {
        foreach (var bot in state.Manager.Bots)
        {
            if (!bot.AwaitsFaultRestart)
            {
                continue;
            }

            var loginName = bot.Account?.LoginName;
            var characterSlot = bot.SelectedCharacter?.CharacterSlot;
            bot.AwaitsFaultRestart = false;
            try
            {
                if (!await state.Manager.RestartBotAsync(gameContext, bot).ConfigureAwait(false)
                    && loginName is not null
                    && characterSlot is { } slot)
                {
                    state.PendingRespawns.Enqueue((loginName, slot));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to restart the faulted bot '{Name}'.", bot.Name);
            }
        }
    }

    /// <summary>
    /// Evolves bots which reached the game's maximum level into their master class (see
    /// <see cref="BotMasterHandler"/> for the rules, including the iron rule of reset servers).
    /// Runs from the maintenance pass - outside the bot's own AI tick - because the evolved bot is
    /// restarted right away (see <see cref="BotManager.RestartBotAsync"/>), which must not happen
    /// from within one of its own timer callbacks.
    /// </summary>
    private async ValueTask EvolveDueMastersAsync(GameContext gameContext, ServerState state, ILogger logger)
    {
        foreach (var bot in state.Manager.Bots)
        {
            // Not while it hunts with a human (the restart would desert the group), sits in an NPC
            // dialog or lies dead - like a due reset, the evolution simply happens on a later pass.
            if (BotPartyHandler.HasHumanCompanion(bot)
                || bot.PlayerState.CurrentState != PlayerState.EnteredWorld
                || !bot.IsAlive)
            {
                continue;
            }

            if (bot.AwaitsMasterRestart)
            {
                await this.RestartEvolvedBotAsync(gameContext, state, bot, logger).ConfigureAwait(false);
                continue;
            }

            if (BotMasterHandler.IsMasterEvolutionDue(bot))
            {
                // The class change and its save go through the bot's own tick, like every other
                // bot-initiated mutation (see OfflinePlayer.PendingBotActions): running them from the
                // maintenance pass would write the character through the same persistence context the
                // combat tick is using at that very moment. The restart follows on the next pass -
                // it must NOT happen from within one of the bot's own timer callbacks.
                bot.PendingBotActions.Enqueue(async () =>
                {
                    if (await BotMasterHandler.TryEvolveAsync(bot).ConfigureAwait(false))
                    {
                        bot.AwaitsMasterRestart = true;
                    }
                });
            }
        }
    }

    /// <summary>
    /// Gives the freshly evolved bot the "relog" it needs: the master class's base attributes (master
    /// experience rate, master points per level) and the master level stat are only mounted when a
    /// character enters the world (see <see cref="BotManager.RestartBotAsync"/>).
    /// </summary>
    private async ValueTask RestartEvolvedBotAsync(GameContext gameContext, ServerState state, BotPlayer bot, ILogger logger)
    {
        // Captured before the restart - the disposed bot loses its account and character.
        var loginName = bot.Account?.LoginName;
        var characterSlot = bot.SelectedCharacter?.CharacterSlot;
        bot.AwaitsMasterRestart = false;
        try
        {
            if (!await state.Manager.RestartBotAsync(gameContext, bot).ConfigureAwait(false)
                && loginName is not null
                && characterSlot is { } slot)
            {
                // The evolution is persisted; only the presence is at risk. RespawnPendingAsync
                // drops the entry when the bot is (still or again) online, so a kept-alive old
                // instance or a rotation comeback doesn't get doubled.
                logger.LogWarning("Evolved bot '{Name}' could not be respawned right away; retrying on the next pass.", bot.Name);
                state.PendingRespawns.Enqueue((loginName, slot));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to restart bot '{Name}' after its master evolution.", bot.Name);
        }
    }

    /// <summary>
    /// Retries bringing back bots whose respawn after the master evolution failed.
    /// </summary>
    private async ValueTask RespawnPendingAsync(GameContext gameContext, ServerState state)
    {
        var count = state.PendingRespawns.Count;
        for (var i = 0; i < count && state.PendingRespawns.TryDequeue(out var entry); i++)
        {
            if (state.Manager.IsActive(entry.Login, entry.Slot))
            {
                continue;
            }

            if (!await state.Manager.SpawnBotAsync(gameContext, entry.Login, entry.Slot).ConfigureAwait(false))
            {
                state.PendingRespawns.Enqueue(entry);
            }
        }
    }

    /// <summary>
    /// Rotates the presence of the bots THIS server animates (see <see cref="BotServerPartition"/>):
    /// each server keeps the daily curve within its own share of the population.
    /// </summary>
    private async ValueTask RotatePresenceAsync(GameContext gameContext, ServerState state, BotConfiguration configuration, ILogger logger)
    {
        if (state.Partition is not { AccountCount: > 0 } partition)
        {
            return;
        }

        var charactersPerAccount = configuration.GetEffectiveCharactersPerAccount();
        var totalPopulation = partition.AccountCount * charactersPerAccount;
        if (totalPopulation <= 0)
        {
            return;
        }

        var minShare = Math.Clamp(configuration.MinOnlineSharePercent, 0, 100) / 100.0;

        // Local wall-clock time on purpose (the rest of the class uses UtcNow for durations): this curve
        // models human presence - fewest in the early morning, most in the evening - so it has to follow
        // the players' day, which is the host's local time, not UTC. On a UTC-configured host the two
        // coincide; on a host set to the player base's zone, local time keeps the peak in their evening.
        var activity = ActivityByHour[DateTime.Now.Hour];
        var targetOnline = (int)Math.Round(totalPopulation * (minShare + ((1.0 - minShare) * activity)));
        var online = state.Manager.Bots.Count;

        if (online < targetOnline)
        {
            // Bring one bot online: pick a random character which is currently offline.
            var offline = new List<(string Login, byte Slot)>();
            for (var i = partition.FirstAccount; i < partition.FirstAccount + partition.AccountCount; i++)
            {
                var loginName = BotGenerator.GetLoginName(i);
                for (byte slot = 0; slot < charactersPerAccount; slot++)
                {
                    if (!state.Manager.IsActive(loginName, slot))
                    {
                        offline.Add((loginName, slot));
                    }
                }
            }

            if (offline.SelectRandom() is { Login: not null } candidate
                && await state.Manager.SpawnBotAsync(gameContext, candidate.Login, candidate.Slot).ConfigureAwait(false))
            {
                logger.LogInformation("Bot presence rotation: +1 (online {Online}/{Target} of {Total}).", online + 1, targetOnline, totalPopulation);
            }
        }
        else if (online > targetOnline)
        {
            var stopped = await state.Manager.StopRandomBotAsync().ConfigureAwait(false);
            if (stopped is not null)
            {
                logger.LogInformation("Bot presence rotation: -1 '{Name}' (online {Online}/{Target} of {Total}).", stopped, online - 1, targetOnline, totalPopulation);
            }
        }
    }

    /// <summary>
    /// Persists the current configuration back to its <see cref="PlugInConfiguration"/> row, so a
    /// programmatic change (e.g. clearing the reset flag) survives a restart.
    /// </summary>
    private async ValueTask PersistConfigurationAsync(GameContext gameContext, BotConfiguration configuration, ILogger logger)
    {
        try
        {
            // Load the configuration through a fresh context so the PlugInConfiguration entity is tracked
            // and the change is actually persisted - the in-memory cached config graph is not tracked.
            using var context = gameContext.PersistenceContextProvider.CreateNewContext();
            var typeId = typeof(BotFeaturePlugIn).GUID;
            var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
            var entity = gameConfiguration?.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId);
            if (entity is null)
            {
                logger.LogWarning("Could not find the bot plugin configuration row to persist.");
                return;
            }

            entity.SetConfiguration(configuration, gameContext.PlugInManager.CustomConfigReferenceHandler);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist the bot plugin configuration.");
        }
    }

    /// <summary>
    /// The bot feature's state of ONE game server: its own share of the population, its own bots, and
    /// its own startup and maintenance schedule (see <see cref="_states"/>).
    /// </summary>
    private sealed class ServerState
    {
        /// <summary>
        /// The <see cref="StartupPhase"/> of this server, as a raw <see cref="int"/>: it is transitioned
        /// with <see cref="Interlocked"/>, which has no overload for arbitrary enum types, so the field
        /// stays an <see cref="int"/> and the phase constants are cast at the call sites.
        /// </summary>
        private int _startupState;

        /// <summary>
        /// 1 while a maintenance pass runs, so the next timer tick doesn't start a second one in
        /// parallel. Interlocked-updated, hence a field.
        /// </summary>
        private int _maintenanceRunning;

        /// <summary>
        /// Gets the bots this server animates.
        /// </summary>
        public BotManager Manager { get; } = new();

        /// <summary>
        /// Gets the bots whose spawn failed - the account may not be generated yet (another game server
        /// is generating the population), or a respawn after the master evolution did not go through.
        /// Retried on the following maintenance passes.
        /// </summary>
        public ConcurrentQueue<(string Login, byte Slot)> PendingRespawns { get; } = new();

        /// <summary>
        /// Gets or sets the share of the bot population which this server animates.
        /// </summary>
        public BotServerPartition? Partition { get; set; }

        /// <summary>
        /// Gets or sets the time of this server's (single) startup pass.
        /// </summary>
        public DateTime NextRunUtc { get; set; } = DateTime.UtcNow + StartupDelay;

        /// <summary>
        /// Gets or sets the time of this server's next maintenance pass.
        /// </summary>
        public DateTime NextMaintenanceUtc { get; set; } = DateTime.UtcNow + StartupDelay + StartupDelay;

        /// <summary>
        /// Gets or sets the time of this server's next bot party re-formation.
        /// </summary>
        public DateTime NextPartyReformUtc { get; set; } = DateTime.UtcNow + PartyReformInterval;

        /// <summary>
        /// Gets a reference to the startup state, for the interlocked transitions of the startup pass.
        /// </summary>
        public ref int StartupState => ref this._startupState;

        /// <summary>
        /// Gets a reference to the maintenance flag, for the interlocked guard of the maintenance pass.
        /// </summary>
        public ref int MaintenanceRunning => ref this._maintenanceRunning;
    }
}
