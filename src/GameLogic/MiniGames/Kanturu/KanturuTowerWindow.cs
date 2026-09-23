// <copyright file="KanturuTowerWindow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Reads and writes the Tower of Refinement open window. The window is stored in the
/// Kanturu start plug-in configuration, so it survives server restarts: the persisted
/// JSON is loaded back at startup. Reads use the live configuration, so they also work
/// where no asynchronous call is possible, such as the scheduler start gate.
/// </summary>
internal static class KanturuTowerWindow
{
    /// <summary>
    /// Gets the UTC time until which the tower is open, if a window is currently stored.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The stored end of the open window, if any.</returns>
    public static DateTime? GetOpenUntilUtc(IGameContext gameContext)
    {
        return GetConfiguration(gameContext)?.TowerOpenUntilUtc;
    }

    /// <summary>
    /// Stores the end of the open window, both live and persisted. A persistence failure
    /// only affects restart survival; the running game is unaffected.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="untilUtc">The UTC time until which the tower is open, or <c>null</c> to clear the window.</param>
    /// <param name="logger">The logger.</param>
    public static async ValueTask SetOpenUntilUtcAsync(IGameContext gameContext, DateTime? untilUtc, ILogger logger)
    {
        try
        {
            var configuration = GetConfiguration(gameContext);
            if (configuration is null)
            {
                logger.LogWarning("The Kanturu start plugin configuration is not available to store the tower window.");
                return;
            }

            configuration.TowerOpenUntilUtc = untilUtc;

            using var context = gameContext.PersistenceContextProvider.CreateNewContext();
            var entity = await FindConfigurationEntityAsync(context, gameContext).ConfigureAwait(false);
            if (entity is null)
            {
                logger.LogWarning("Could not find the Kanturu start plugin configuration row to persist the tower window.");
                return;
            }

            entity.SetConfiguration(configuration, gameContext.PlugInManager.CustomConfigReferenceHandler);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist the Kanturu tower window.");
        }
    }

    private static KanturuStartConfiguration? GetConfiguration(IGameContext gameContext)
    {
        try
        {
            var startPlugIn = gameContext.PlugInManager
                .GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.Kanturu);
            if (startPlugIn is ISupportCustomConfiguration<KanturuStartConfiguration> { Configuration: { } configuration })
            {
                return configuration;
            }
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger(typeof(KanturuTowerWindow)).LogError(ex, "Failed to read the Kanturu tower window.");
        }

        return null;
    }

    private static async ValueTask<PlugInConfiguration?> FindConfigurationEntityAsync(IContext context, IGameContext gameContext)
    {
        var typeId = typeof(KanturuStartPlugIn).GUID;
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
        return gameConfiguration?.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId);
    }
}
