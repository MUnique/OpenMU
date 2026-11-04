// <copyright file="GoblinPointsForPlayTimePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Gives players Goblin Points for their play time.
/// </summary>
/// <remarks>
/// A player gets the points each time it played for the configured interval since it entered
/// the game or got the points the last time. The game server owns the balance, so it's changed
/// directly instead of by a coin grant.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GoblinPointsForPlayTimePlugIn_Name), Description = nameof(PlugInResources.GoblinPointsForPlayTimePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("29A6D015-A94B-4F96-A638-6134DDC9ED0C")]
public class GoblinPointsForPlayTimePlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<GoblinPointsForPlayTimeConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <summary>
    /// The time between the checks of the play time of the players.
    /// </summary>
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    private readonly ConditionalWeakTable<Player, StrongBox<DateTime>> _countingSince = new();

    private DateTime _nextRunUtc = DateTime.UtcNow;

    /// <inheritdoc />
    public GoblinPointsForPlayTimeConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        if (now < this._nextRunUtc)
        {
            return;
        }

        this._nextRunUtc = now + CheckInterval;
        if (CashShopFeaturePlugIn.GetSettings(gameContext) is null)
        {
            return;
        }

        var configuration = this.Configuration ??= new GoblinPointsForPlayTimeConfiguration();
        foreach (var player in await gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (!this.IsEligible(player, configuration))
            {
                this._countingSince.Remove(player);
                continue;
            }

            var countingSince = this._countingSince.GetValue(player, _ => new StrongBox<DateTime>(now));
            if (now - countingSince.Value < configuration.Interval)
            {
                continue;
            }

            countingSince.Value = now;
            await this.GivePointsAsync(player, configuration.Points).ConfigureAwait(false);
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
        return new GoblinPointsForPlayTimeConfiguration();
    }

    private bool IsEligible(Player player, GoblinPointsForPlayTimeConfiguration configuration)
    {
        return player.PlayerState.CurrentState == PlayerState.EnteredWorld
               && player.Account is not null
               && (configuration.IncludeOfflineLeveling || player is not OfflinePlayer)
               && (player.Attributes?[Stats.Level] ?? 0) >= configuration.MinimumLevel;
    }

    private async ValueTask GivePointsAsync(Player player, int points)
    {
        if (points <= 0 || player.Account is not { } account)
        {
            return;
        }

        try
        {
            await player.RunPersistenceExclusiveAsync(() =>
            {
                account.GoblinPoints = (int)Math.Min((long)account.GoblinPoints + points, int.MaxValue);
                return ValueTask.CompletedTask;
            }).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GoblinPointsForPlayTime), points).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error when giving Goblin Points for the play time of player {player}.", player);
        }
    }
}
