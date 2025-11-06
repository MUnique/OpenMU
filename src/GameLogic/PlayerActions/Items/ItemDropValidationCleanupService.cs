// <copyright file="ItemDropValidationCleanupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background service that periodically cleans up old item drop validation entries.
/// </summary>
public class ItemDropValidationCleanupService : BackgroundService
{
    private const int CleanupIntervalMinutes = 5;

    private readonly ItemDropValidator _validator;
    private readonly ILogger<ItemDropValidationCleanupService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemDropValidationCleanupService"/> class.
    /// </summary>
    /// <param name="validator">The item drop validator to clean up.</param>
    /// <param name="logger">The logger.</param>
    public ItemDropValidationCleanupService(ItemDropValidator validator, ILogger<ItemDropValidationCleanupService> logger)
    {
        this._validator = validator;
        this._logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation("Item drop validation cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(CleanupIntervalMinutes), stoppingToken).ConfigureAwait(false);
                
                this._validator.CleanupOldEntries();
                this._logger.LogDebug("Cleaned up old item drop validation entries");
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during item drop validation cleanup");
            }
        }

        this._logger.LogInformation("Item drop validation cleanup service stopped");
    }
}