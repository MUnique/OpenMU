// <copyright file="ZenConsumptionHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.MuHelper;

/// <summary>
/// Handles the periodic Zen consumption during offline leveling.
/// </summary>
internal sealed class ZenConsumptionHandler
{
    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperConfiguration _configuration;
    private DateTime _lastPayTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZenConsumptionHandler"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ZenConsumptionHandler(OfflineLevelingPlayer player)
    {
        this._player = player;
        this._configuration = player.GameContext.FeaturePlugIns.GetPlugIn<MuHelperFeaturePlugIn>()?.Configuration
                              ?? new MuHelperConfiguration();
        this._lastPayTimestamp = player.StartTimestamp;
    }

    /// <summary>
    /// Deducts Zen from the player based on the server configuration and the player's level.
    /// </summary>
    public async Task DeductZenAsync()
    {
        if (DateTime.UtcNow - this._lastPayTimestamp < this._configuration.PayInterval)
        {
            return;
        }

        var amount = MuHelperZenCostCalculator.Calculate(this._player, this._configuration, this._player.StartTimestamp);

        if (amount > 0 && this._player.TryRemoveMoney(amount))
        {
            this._lastPayTimestamp = DateTime.UtcNow;
            await this._player
                .InvokeViewPlugInAsync<IMuHelperStatusUpdatePlugIn>(p => p.ConsumeMoneyAsync((uint)amount))
                .ConfigureAwait(false);
        }
        else if (amount > 0)
        {
            this._player.Logger.LogDebug("Offline leveling stopped for {CharacterName} due to insufficient Zen.", this._player.Name);
            await this._player.StopAsync().ConfigureAwait(false);
        }
        else
        {
            // Price is 0 or less, no action required.
        }
    }
}