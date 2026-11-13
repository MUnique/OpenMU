// <copyright file="EffectTimes.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which allows to choose which magic effects send their remaining time to the client,
/// one by one, all at once, or as in the Season 6 data.
/// </summary>
/// <remarks>
/// The page only keeps the filter and the pending selection. Changes are previewed, confirmed,
/// audited and saved by the shared <see cref="EffectTimeOperations"/>, which the agents use, too.
/// </remarks>
public partial class EffectTimes : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// The most names of changed effects which the confirmation lists per state; more are summarized.
    /// </summary>
    private const int MaximumListedNames = 15;

    private readonly Dictionary<Guid, bool> _pendingStates = new();

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private IDisposable? _navigationLockDisposable;
    private int _loadGeneration;

    private IReadOnlyList<EffectTimeEntry>? _entries;
    private IReadOnlyDictionary<short, bool>? _seasonSixDefaults;

    /// <summary>
    /// Gets or sets the shared operations which change whether effects send their time.
    /// </summary>
    [Inject]
    public EffectTimeOperations Operations { get; set; } = null!;

    /// <summary>
    /// Gets or sets the defaults of the effect times, e.g. the ones of Season 6.
    /// </summary>
    [Inject]
    public EffectTimeDefaults Defaults { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the java script runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JavaScript { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<EffectTimes> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the search text which the shown effects contain; the page binds to it.
    /// </summary>
    private string Filter { get; set; } = string.Empty;

    private bool HasPendingChanges => this._pendingStates.Count > 0;

    private IEnumerable<EffectTimeEntry> ShownEntries =>
        (this._entries ?? [])
            .Where(entry => string.IsNullOrWhiteSpace(this.Filter)
                            || entry.Name.Contains(this.Filter.Trim(), StringComparison.OrdinalIgnoreCase)
                            || entry.Number.ToString(CultureInfo.InvariantCulture).Contains(this.Filter.Trim(), StringComparison.Ordinal));

    private int SendingCount => (this._entries ?? []).Count(this.SendsTime);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._navigationLockDisposable?.Dispose();
        this._navigationLockDisposable = null;

        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        try
        {
            await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // we can ignore that ...
        }
        catch
        {
            // and we should not throw exceptions in the dispose method ...
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The page has no parameters, so it loads its data only once, and not again when its parameters are set again.
    /// </remarks>
    protected override Task OnInitializedAsync()
    {
        this._navigationLockDisposable = this.NavigationManager.RegisterLocationChangingHandler(this.OnBeforeInternalNavigationAsync);
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(async () =>
        {
            await this.LoadDataAsync(cts.Token).ConfigureAwait(false);
            await this.LoadSeasonSixDefaultsAsync(cts.Token).ConfigureAwait(false);
        });
        return base.OnInitializedAsync();
    }

    private static string ListNames(JsonNode? names)
    {
        var all = names?.AsArray().Select(n => n!.GetValue<string>()).ToList() ?? [];
        var listed = string.Join(", ", all.Take(MaximumListedNames));
        return all.Count > MaximumListedNames ? $"{listed}, …" : listed;
    }

    private bool SendsTime(EffectTimeEntry entry)
    {
        return this._pendingStates.TryGetValue(entry.Id, out var sendsTime) ? sendsTime : entry.SendsTime;
    }

    private void SetSendsTime(EffectTimeEntry entry, bool sendsTime)
    {
        if (sendsTime == entry.SendsTime)
        {
            this._pendingStates.Remove(entry.Id);
        }
        else
        {
            this._pendingStates[entry.Id] = sendsTime;
        }
    }

    private void SetSendsTimeOfShownEntries(bool sendsTime)
    {
        foreach (var entry in this.ShownEntries.ToList())
        {
            this.SetSendsTime(entry, sendsTime);
        }
    }

    private void SetSeasonSixDefaultsOfShownEntries()
    {
        if (this._seasonSixDefaults is not { } defaults)
        {
            this.ToastService.ShowError(Resources.SeasonSixDefaultsNotLoaded);
            return;
        }

        foreach (var entry in this.ShownEntries.ToList())
        {
            if (defaults.TryGetValue(entry.Number, out var sendsTime))
            {
                this.SetSendsTime(entry, sendsTime);
            }
        }
    }

    private async Task OnSaveButtonClickAsync()
    {
        if (!this.HasPendingChanges)
        {
            this.ToastService.ShowSuccess(Resources.NoChangesToSave);
            return;
        }

        var send = this._pendingStates.Where(p => p.Value).Select(p => p.Key).ToList();
        var hide = this._pendingStates.Where(p => !p.Value).Select(p => p.Key).ToList();
        try
        {
            var preview = await this.Operations.SetSendsTimeAsync(send, hide, false, null, default).ConfigureAwait(true);
            var sendNames = preview["change"]?["send"];
            var hideNames = preview["change"]?["hide"];
            var sendCount = sendNames?.AsArray().Count ?? 0;
            var hideCount = hideNames?.AsArray().Count ?? 0;
            if (sendCount + hideCount == 0)
            {
                this._pendingStates.Clear();
                this.ToastService.ShowSuccess(Resources.NoChangesToSave);
                return;
            }

            var isConfirmed = await this.ModalService.ShowQuestionAsync(
                    Resources.EffectTimes,
                    string.Format(CultureInfo.CurrentCulture, Resources.EffectTimesConfirmFormat, sendCount, ListNames(sendNames), hideCount, ListNames(hideNames)))
                .ConfigureAwait(true);
            if (!isConfirmed)
            {
                return;
            }

            await this.Operations.SetSendsTimeAsync(send, hide, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(true);
            this._pendingStates.Clear();
            this.ToastService.ShowSuccess(Resources.SavedChanges);
            await this.LoadDataAsync(this._disposeCts?.Token ?? default).ConfigureAwait(true);
        }
        catch (AdminOperationException ex)
        {
            this.ToastService.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred on save: {Message}", ex.Message);
            this.ToastService.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.UnexpectedErrorOccurred, ex.Message));
        }
    }

    private void OnCancelButtonClick()
    {
        this._pendingStates.Clear();
    }

    private async ValueTask OnBeforeInternalNavigationAsync(LocationChangingContext context)
    {
        if (!this.HasPendingChanges)
        {
            return;
        }

        var isConfirmed = await this.JavaScript.InvokeAsync<bool>(
                "window.confirm",
                Resources.UnsavedChangesQuestion)
            .ConfigureAwait(true);

        if (!isConfirmed)
        {
            context.PreventNavigation();
        }
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        // A load which completes after a newer one started must not overwrite the newer data.
        var generation = Interlocked.Increment(ref this._loadGeneration);
        using var loading = this.LoadingService.ShowLoadingIndicator();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entries = await this.Operations.GetEntriesAsync(cancellationToken).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();
            if (generation != Volatile.Read(ref this._loadGeneration))
            {
                return;
            }

            this._entries = entries;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Expected when navigating away - ignore
        }
        catch (AdminOperationException ex)
        {
            this.ToastService.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the effect times");
        }
    }

    /// <summary>
    /// Loads the Season 6 defaults after the effects are shown, because the first load executes the
    /// Season 6 data initialization in memory, which takes a while.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task LoadSeasonSixDefaultsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var defaults = await this.Defaults.GetAsync(Persistence.Initialization.VersionSeasonSix.DataInitialization.Id).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();
            this._seasonSixDefaults = defaults;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Expected when navigating away - ignore
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the Season 6 defaults of the effect times");
        }
    }
}
