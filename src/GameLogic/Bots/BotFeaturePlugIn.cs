// <copyright file="BotFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Linq;
using System.Runtime.InteropServices;
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

    private readonly BotManager _botManager = new();

    private DateTime _nextRunUtc = DateTime.UtcNow + StartupDelay;
    private bool _spawned;

    /// <inheritdoc />
    public BotConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the bot manager.
    /// </summary>
    public BotManager BotManager => this._botManager;

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this._spawned || DateTime.UtcNow < this._nextRunUtc)
        {
            return;
        }

        var configuration = this.Configuration ??= CreateDefaultConfiguration();
        if (!configuration.Enabled)
        {
            return;
        }

        this._spawned = true;

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
        foreach (var loginName in configuration.GetProofOfConceptAccounts())
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
