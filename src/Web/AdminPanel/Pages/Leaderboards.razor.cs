// <copyright file="Leaderboards.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which shows the leaderboards of the mini games.
/// </summary>
public partial class Leaderboards : ComponentBase
{
    private IReadOnlyList<MiniGameType> _miniGameTypes = [];
    private IReadOnlyList<MiniGameLeaderboardEntry> _entries = [];
    private MiniGameType _selectedType;
    private LeaderboardPeriod _period = LeaderboardPeriod.AllTime;
    private bool _isLoading = true;

    /// <summary>
    /// The period of the finished games which are counted.
    /// </summary>
    private enum LeaderboardPeriod
    {
        /// <summary>
        /// All games are counted.
        /// </summary>
        AllTime,

        /// <summary>
        /// The games of the last 30 days are counted.
        /// </summary>
        Last30Days,

        /// <summary>
        /// The games of the last 7 days are counted.
        /// </summary>
        Last7Days,
    }

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<Leaderboards> Logger { get; set; } = null!;

    private MiniGameLeaderboardService LeaderboardService => new(this.PersistenceContextProvider, this.GameConfigurationSource);

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        try
        {
            this._miniGameTypes = await this.LeaderboardService.GetMiniGameTypesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the mini game types of the leaderboards.");
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }

        if (this._miniGameTypes.Count > 0)
        {
            this._selectedType = this._miniGameTypes[0];
            await this.LoadAsync().ConfigureAwait(true);
        }
        else
        {
            this._isLoading = false;
        }
    }

    private static DateTime? GetSince(LeaderboardPeriod period)
    {
        return period switch
        {
            LeaderboardPeriod.Last7Days => DateTime.UtcNow.AddDays(-7),
            LeaderboardPeriod.Last30Days => DateTime.UtcNow.AddDays(-30),
            _ => null,
        };
    }

    private async Task SelectTypeAsync(MiniGameType miniGameType)
    {
        if (this._selectedType != miniGameType)
        {
            this._selectedType = miniGameType;
            await this.LoadAsync().ConfigureAwait(true);
        }
    }

    private async Task OnPeriodChangedAsync(ChangeEventArgs args)
    {
        if (Enum.TryParse<LeaderboardPeriod>(args.Value?.ToString(), out var period) && period != this._period)
        {
            this._period = period;
            await this.LoadAsync().ConfigureAwait(true);
        }
    }

    private async Task LoadAsync()
    {
        this._isLoading = true;
        this._entries = [];
        try
        {
            this._entries = await this.LeaderboardService.GetLeaderboardAsync(this._selectedType, GetSince(this._period)).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the leaderboard of {MiniGameType}.", this._selectedType);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isLoading = false;
        }
    }
}
