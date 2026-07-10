// <copyright file="BotFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

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

    private readonly BotManager _botManager = new();

    /// <summary>
    /// Bots whose respawn after the master evolution failed (see <see cref="EvolveDueMastersAsync"/>),
    /// retried on the following maintenance passes - without this, a transient spawn failure would
    /// leave the character offline until a server restart when the presence rotation is disabled.
    /// </summary>
    private readonly System.Collections.Concurrent.ConcurrentQueue<(string Login, byte Slot)> _pendingRespawns = new();

    private DateTime _nextRunUtc = DateTime.UtcNow + StartupDelay;
    private DateTime _nextMaintenanceUtc = DateTime.UtcNow + StartupDelay + StartupDelay;
    private DateTime _nextPartyReformUtc = DateTime.UtcNow + PartyReformInterval;

    /// <summary>
    /// 0 = startup not run yet, 1 = startup in progress, 2 = startup done (maintenance mode).
    /// The periodic task timer fires every second WITHOUT awaiting the previous invocation, so
    /// during the minutes-long generation/spawn further ticks arrive concurrently - they must
    /// neither re-enter the startup nor run the maintenance (e.g. the presence rotation) against
    /// a half-spawned population.
    /// </summary>
    private int _startupState;

    /// <inheritdoc />
    public BotConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the bot manager.
    /// </summary>
    public BotManager BotManager => this._botManager;

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this._startupState == 2)
        {
            await this.RunMaintenanceAsync(gameContext).ConfigureAwait(false);
            return;
        }

        if (DateTime.UtcNow < this._nextRunUtc
            || Interlocked.CompareExchange(ref this._startupState, 1, 0) != 0)
        {
            return;
        }

        var configuration = this.Configuration ??= CreateDefaultConfiguration();
        if (!configuration.Enabled)
        {
            // Not spawned - re-check on the following ticks, the feature may get enabled later.
            Interlocked.Exchange(ref this._startupState, 0);
            return;
        }

        try
        {
            await this.SpawnPopulationAsync(gameContext, configuration).ConfigureAwait(false);
        }
        finally
        {
            // Like before: the startup runs once, even when parts of it failed (the errors are
            // logged); the maintenance pass takes over from here.
            Interlocked.Exchange(ref this._startupState, 2);
        }
    }

    private async ValueTask SpawnPopulationAsync(GameContext gameContext, BotConfiguration configuration)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);

        var generator = new BotGenerator(gameContext, logger);

        if (configuration.ResetBots)
        {
            try
            {
                var deleted = await generator.DeleteAllBotsAsync().ConfigureAwait(false);
                logger.LogInformation("Reset requested: purged {Deleted} bot account(s).", deleted);

                // Clear the flag (in memory and persisted) so the next restart does not purge again.
                configuration.ResetBots = false;
                await this.PersistConfigurationAsync(gameContext, configuration, logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reset the bot population.");
            }
        }

        try
        {
            // Generate the persistent bot population if it is not there yet (idempotent).
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

        var charactersPerAccount = Math.Clamp(
            Math.Min(configuration.MaxCharactersPerAccount, gameContext.Configuration.MaximumCharactersPerAccount),
            1,
            BotConfiguration.MaxCharactersPerAccountLimit);

        var started = 0;
        var total = 0;
        for (var i = 1; i <= configuration.NumberOfAccounts; i++)
        {
            var loginName = BotGenerator.GetLoginName(i);
            for (byte slot = 0; slot < charactersPerAccount; slot++)
            {
                total++;
                try
                {
                    if (await this._botManager.SpawnBotAsync(gameContext, loginName, slot).ConfigureAwait(false))
                    {
                        started++;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to spawn bot for account '{LoginName}' (slot {Slot}).", loginName, slot);
                }
            }
        }

        // The proof-of-concept accounts remain an optional extra hook to animate existing (non-bot) accounts.
        foreach (var loginName in configuration.ParseProofOfConceptAccounts())
        {
            try
            {
                if (await this._botManager.SpawnBotAsync(gameContext, loginName).ConfigureAwait(false))
                {
                    started++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to spawn proof-of-concept bot for account '{LoginName}'.", loginName);
            }
        }

        logger.LogInformation("Bot feature started {Started} of {Total} bots.", started, total);

        try
        {
            await this._botManager.FormPartiesAsync(gameContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to form bot parties.");
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRunUtc = DateTime.UtcNow;
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
    private async ValueTask RunMaintenanceAsync(GameContext gameContext)
    {
        if (DateTime.UtcNow < this._nextMaintenanceUtc)
        {
            return;
        }

        this._nextMaintenanceUtc = DateTime.UtcNow + MaintenanceInterval;

        var configuration = this.Configuration;
        if (configuration?.Enabled != true)
        {
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        try
        {
            await this.RespawnPendingAsync(gameContext).ConfigureAwait(false);
            await this.EvolveDueMastersAsync(gameContext, logger).ConfigureAwait(false);

            if (configuration.PresenceRotation)
            {
                await this.RotatePresenceAsync(gameContext, configuration, logger).ConfigureAwait(false);
            }

            if (DateTime.UtcNow >= this._nextPartyReformUtc)
            {
                this._nextPartyReformUtc = DateTime.UtcNow + PartyReformInterval;
                await this._botManager.FormPartiesAsync(gameContext).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot maintenance failed.");
        }
    }

    /// <summary>
    /// Evolves bots which reached the game's maximum level into their master class (see
    /// <see cref="BotMasterHandler"/> for the rules, including the iron rule of reset servers).
    /// Runs from the maintenance pass - outside the bot's own AI tick - because the evolved bot is
    /// restarted right away (see <see cref="BotManager.RestartBotAsync"/>), which must not happen
    /// from within one of its own timer callbacks.
    /// </summary>
    private async ValueTask EvolveDueMastersAsync(GameContext gameContext, ILogger logger)
    {
        foreach (var bot in this._botManager.Bots)
        {
            if (!BotMasterHandler.IsMasterEvolutionDue(bot))
            {
                continue;
            }

            // Not while it hunts with a human (the restart would desert the group), sits in an NPC
            // dialog or lies dead - like a due reset, the evolution simply happens on a later pass.
            if (BotPartyHandler.HasHumanCompanion(bot)
                || bot.PlayerState.CurrentState != PlayerState.EnteredWorld
                || !bot.IsAlive)
            {
                continue;
            }

            // Captured before the restart - the disposed bot loses its account and character.
            var loginName = bot.Account?.LoginName;
            var characterSlot = bot.SelectedCharacter?.CharacterSlot;
            try
            {
                if (await BotMasterHandler.TryEvolveAsync(bot).ConfigureAwait(false)
                    && !await this._botManager.RestartBotAsync(gameContext, bot).ConfigureAwait(false)
                    && loginName is not null
                    && characterSlot is { } slot)
                {
                    // The evolution is persisted; only the presence is at risk. RespawnPendingAsync
                    // drops the entry when the bot is (still or again) online, so a kept-alive old
                    // instance or a rotation comeback doesn't get doubled.
                    logger.LogWarning("Evolved bot '{Name}' could not be respawned right away; retrying on the next pass.", bot.Name);
                    this._pendingRespawns.Enqueue((loginName, slot));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to evolve bot '{Name}' into its master class.", bot.Name);
            }
        }
    }

    /// <summary>
    /// Retries bringing back bots whose respawn after the master evolution failed.
    /// </summary>
    private async ValueTask RespawnPendingAsync(GameContext gameContext)
    {
        var count = this._pendingRespawns.Count;
        for (var i = 0; i < count && this._pendingRespawns.TryDequeue(out var entry); i++)
        {
            if (this._botManager.IsActive(entry.Login, entry.Slot))
            {
                continue;
            }

            if (!await this._botManager.SpawnBotAsync(gameContext, entry.Login, entry.Slot).ConfigureAwait(false))
            {
                this._pendingRespawns.Enqueue(entry);
            }
        }
    }

    private async ValueTask RotatePresenceAsync(GameContext gameContext, BotConfiguration configuration, ILogger logger)
    {
        var charactersPerAccount = configuration.GetEffectiveCharactersPerAccount();
        var totalPopulation = configuration.NumberOfAccounts * charactersPerAccount;
        if (totalPopulation <= 0)
        {
            return;
        }

        var minShare = Math.Clamp(configuration.MinOnlineSharePercent, 0, 100) / 100.0;
        var activity = ActivityByHour[DateTime.Now.Hour];
        var targetOnline = (int)Math.Round(totalPopulation * (minShare + ((1.0 - minShare) * activity)));
        var online = this._botManager.Bots.Count;

        if (online < targetOnline)
        {
            // Bring one bot online: pick a random character which is currently offline.
            var offline = new List<(string Login, byte Slot)>();
            for (var i = 1; i <= configuration.NumberOfAccounts; i++)
            {
                var loginName = BotGenerator.GetLoginName(i);
                for (byte slot = 0; slot < charactersPerAccount; slot++)
                {
                    if (!this._botManager.IsActive(loginName, slot))
                    {
                        offline.Add((loginName, slot));
                    }
                }
            }

            if (offline.SelectRandom() is { Login: not null } candidate
                && await this._botManager.SpawnBotAsync(gameContext, candidate.Login, candidate.Slot).ConfigureAwait(false))
            {
                logger.LogInformation("Bot presence rotation: +1 (online {Online}/{Target} of {Total}).", online + 1, targetOnline, totalPopulation);
            }
        }
        else if (online > targetOnline)
        {
            var stopped = await this._botManager.StopRandomBotAsync().ConfigureAwait(false);
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
}
