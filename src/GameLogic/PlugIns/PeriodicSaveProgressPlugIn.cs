// <copyright file="PeriodicSaveProgressPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Saves the progress of players periodically when their status is 'EnteredWorld'.
/// </summary>
[PlugIn(nameof(PeriodicSaveProgressPlugIn), "Saves the progress of players periodically when their status is 'EnteredWorld'.")]
[Guid("CEBBD5BD-B0DF-4768-816D-AF8DF78888B2")]
public class PeriodicSaveProgressPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<PeriodicSaveProgressPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    private DateTime _nextRunUtc = DateTime.UtcNow;

    /// <inheritdoc />
    public PeriodicSaveProgressPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (DateTime.UtcNow < this._nextRunUtc)
        {
            return;
        }

        var configuration = this.Configuration ??= CreateDefaultConfiguration();
        this._nextRunUtc = DateTime.UtcNow + configuration.Interval;

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);
        logger.LogDebug("Starting periodical save...");
        try
        {
            var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
            foreach (var player in players)
            {
                using var loggerScope = player.Logger.BeginScope(
                    ("Account", player.Account?.LoginName ?? string.Empty),
                    ("Character", player.SelectedCharacter?.Name ?? string.Empty));
                try
                {
                    if (player.PlayerState.CurrentState == PlayerState.EnteredWorld)
                    {
                        await player.SaveProgressAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        player.Logger.LogDebug("Skipping saving the progress periodically for player '{player}' because the state is {state}", player, player.PlayerState.CurrentState);
                    }
                }
                catch (Exception ex)
                {
                    player.Logger.LogError(ex, "Error when saving the progress periodically for player '{player}': {ex}", player, ex);
                }
            }

            logger.LogDebug("Finished periodical save without errors.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error when saving player data periodically: {ex}", ex);
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

    private static PeriodicSaveProgressPlugInConfiguration CreateDefaultConfiguration()
    {
        return new PeriodicSaveProgressPlugInConfiguration();
    }
}