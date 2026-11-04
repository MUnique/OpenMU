// <copyright file="Leaderboards.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which shows the leaderboards of the mini games.
/// </summary>
public sealed partial class Leaderboards : ComponentBase, IDisposable
{
    private MiniGameType? _selectedType;
    private LeaderboardPeriod _period = LeaderboardPeriod.AllTime;
    private CancellationTokenSource? _loadCancellation;

    /// <summary>
    /// The period of the finished games which are counted.
    /// </summary>
    internal enum LeaderboardPeriod
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

    /// <summary>
    /// Gets the mini game types which have a leaderboard.
    /// </summary>
    internal IReadOnlyList<MiniGameType> MiniGameTypes { get; private set; } = [];

    /// <summary>
    /// Gets the entries of the shown leaderboard.
    /// </summary>
    internal IReadOnlyList<MiniGameLeaderboardEntry> Entries { get; private set; } = [];

    /// <summary>
    /// Gets a value indicating whether a leaderboard is loading.
    /// </summary>
    internal bool IsLoading { get; private set; } = true;

    [Inject]
    private MiniGameReadOperations ReadOperations { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<Leaderboards> Logger { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this._loadCancellation?.Cancel();
        this._loadCancellation?.Dispose();
        this._loadCancellation = null;
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        await this.LoadAsync().ConfigureAwait(true);
    }

    private static int? GetDays(LeaderboardPeriod period)
    {
        return period switch
        {
            LeaderboardPeriod.Last7Days => 7,
            LeaderboardPeriod.Last30Days => 30,
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
        // A newer selection cancels the load of the previous one, so that a slow
        // previous load can't overwrite the leaderboard of the current selection.
        if (this._loadCancellation is { } previousCancellation)
        {
            await previousCancellation.CancelAsync().ConfigureAwait(true);
            previousCancellation.Dispose();
        }

        var cancellation = new CancellationTokenSource();
        this._loadCancellation = cancellation;
        var miniGameType = this._selectedType;
        var period = this._period;

        this.IsLoading = true;
        this.Entries = [];
        try
        {
            var leaderboard = await this.ReadOperations.LeaderboardAsync(miniGameType, GetDays(period), cancellation.Token).ConfigureAwait(true);
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            this.MiniGameTypes = leaderboard.AvailableTypes;
            this._selectedType = leaderboard.MiniGameType;
            this.Entries = leaderboard.Entries;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // A newer selection is loading.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the leaderboard of {MiniGameType}.", miniGameType);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            if (!cancellation.IsCancellationRequested)
            {
                this.IsLoading = false;
            }
        }
    }
}
