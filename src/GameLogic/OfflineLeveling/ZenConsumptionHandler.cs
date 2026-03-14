// <copyright file="ZenConsumptionHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.MuHelper;

/// <summary>
/// Handles the periodic Zen consumption during offline leveling.
/// </summary>
internal sealed class ZenConsumptionHandler
{
    private readonly OfflineLevelingPlayer _player;
    private DateTime _lastPayTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZenConsumptionHandler"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ZenConsumptionHandler(OfflineLevelingPlayer player)
    {
        this._player = player;
        this._lastPayTimestamp = player.StartTimestamp;
    }

    /// <summary>
    /// Deducts Zen from the player based on the server configuration and the player's level.
    /// </summary>
    public async Task DeductZenAsync()
    {
        var helperConfig = this._player.GameContext.FeaturePlugIns.GetPlugIn<MuHelperFeaturePlugIn>()?.Configuration
                           ?? new MuHelperServerConfiguration();

        if (DateTime.UtcNow.Subtract(this._lastPayTimestamp) < helperConfig.PayInterval)
        {
            return;
        }

        var currentStage = (int)(DateTime.UtcNow.Subtract(this._player.StartTimestamp) / helperConfig.StageInterval);
        currentStage = Math.Max(0, currentStage);
        currentStage = Math.Min(helperConfig.CostPerStage.Count - 1, currentStage);

        var costMultiplier = helperConfig.CostPerStage[currentStage];
        var totalLevel = (int)(this._player.Level + (this._player.Attributes?[Stats.MasterLevel] ?? 0));
        var amount = costMultiplier * totalLevel;

        if (amount > 0 && this._player.TryRemoveMoney(amount))
        {
            this._lastPayTimestamp = DateTime.UtcNow;
            await this._player
                .InvokeViewPlugInAsync<IMuHelperStatusUpdatePlugIn>(p => p.ConsumeMoneyAsync((uint)amount))
                .ConfigureAwait(false);
        }
        else if (amount > 0)
        {
            this._player.Logger.LogDebug("Offline leveling stopped for {0} due to insufficient Zen.", this._player.Name);
            await this._player.StopAsync().ConfigureAwait(false);
        }
        else
        {
            // The cost is 0 or less; no action required.
        }
    }
}
