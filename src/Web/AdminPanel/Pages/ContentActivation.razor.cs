// <copyright file="ContentActivation.razor.cs" company="MUnique">
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
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which allows to activate and deactivate the content of the game configuration in batches,
/// e.g. to restrict the game to the content of a certain season.
/// </summary>
/// <remarks>
/// The page only keeps the filters and the pending selection. Changes are previewed, confirmed,
/// audited and saved by the shared <see cref="ContentActivationOperations"/>, which the agents use, too.
/// </remarks>
public partial class ContentActivation : ComponentBase, IAsyncDisposable
{
    private readonly Dictionary<Guid, bool> _pendingStates = new();

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private IDisposable? _navigationLockDisposable;
    private int _loadGeneration;

    private Dictionary<ContentActivationType, IReadOnlyList<ContentActivationEntry>>? _entries;
    private ContentActivationType _selectedContentType = ContentActivationType.Maps;
    private StateFilter _stateFilter = StateFilter.All;
    private GameVersion _targetVersion = GameVersion.Season6Episode3;
    private string _filter = string.Empty;

    /// <summary>
    /// The filter for the active state of the shown entries.
    /// </summary>
    public enum StateFilter
    {
        /// <summary>
        /// All entries are shown.
        /// </summary>
        All,

        /// <summary>
        /// Only active entries are shown.
        /// </summary>
        OnlyActive,

        /// <summary>
        /// Only inactive entries are shown.
        /// </summary>
        OnlyInactive,
    }

    /// <summary>
    /// Gets or sets the shared operations which change the active state of the content.
    /// </summary>
    [Inject]
    public ContentActivationOperations Operations { get; set; } = null!;

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
    public ILogger<ContentActivation> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    private ContentActivationType SelectedContentType
    {
        get => this._selectedContentType;
        set
        {
            this._selectedContentType = value;
            this._filter = string.Empty;
        }
    }

    private bool HasPendingChanges => this._pendingStates.Count > 0;

    private IReadOnlyList<ContentActivationEntry> SelectedEntries =>
        this._entries?.GetValueOrDefault(this._selectedContentType) ?? [];

    private IEnumerable<ContentActivationEntry> ShownEntries =>
        this.SelectedEntries
            .Where(entry => this._stateFilter switch
            {
                StateFilter.OnlyActive => this.IsActive(entry),
                StateFilter.OnlyInactive => !this.IsActive(entry),
                _ => true,
            })
            .Where(entry => string.IsNullOrWhiteSpace(this._filter)
                            || entry.Name.Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase)
                            || entry.Key.Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase)
                            || GetCaption(entry.IntroducedIn).Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase));

    private int ActiveCount => this.SelectedEntries.Count(this.IsActive);

    private int TotalCount => this.SelectedEntries.Count;

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
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token));
        return base.OnInitializedAsync();
    }

    private static string GetCaption(ContentActivationType contentType)
    {
        return contentType switch
        {
            ContentActivationType.Maps => Resources.GameMaps,
            ContentActivationType.CharacterClasses => Resources.CharacterClasses,
            ContentActivationType.Monsters => Resources.Monsters,
            ContentActivationType.Items => Resources.Items,
            ContentActivationType.MiniGames => Resources.MiniGames,
            _ => contentType.ToString(),
        };
    }

    /// <summary>
    /// Gets the caption of a game version, e.g. "Version 0.97d", "Version 0.99G+" or "Season 3 Episode 1".
    /// </summary>
    /// <remarks>
    /// The caption is calculated from the value of the version, see the remarks of <see cref="GameVersion"/>.
    /// </remarks>
    /// <param name="version">The game version.</param>
    /// <returns>The caption of the game version.</returns>
    private static string GetCaption(GameVersion version)
    {
        const int lettersInAlphabet = 26;
        var value = (int)version;
        if (version == GameVersion.Unknown)
        {
            return Resources.GameVersionUnknown;
        }

        if (value >= 100000)
        {
            var season = value / 100000;
            var episode = (value % 100000) / 1000;
            return episode == 0
                ? string.Format(CultureInfo.CurrentCulture, Resources.SeasonFormat, season)
                : string.Format(CultureInfo.CurrentCulture, Resources.SeasonEpisodeFormat, season, episode);
        }

        var patch = value % 100;
        var suffix = patch switch
        {
            0 => string.Empty,
            <= lettersInAlphabet => ((char)('a' + patch - 1)).ToString(CultureInfo.InvariantCulture),
            _ => $"{(char)('A' + patch - lettersInAlphabet - 1)}+",
        };
        return string.Format(CultureInfo.CurrentCulture, Resources.GameVersionFormat, string.Create(CultureInfo.InvariantCulture, $"{value / 10000}.{(value / 100) % 100:00}{suffix}"));
    }

    private bool IsActive(ContentActivationEntry entry)
    {
        return this._pendingStates.TryGetValue(entry.Id, out var isActive) ? isActive : entry.IsActive;
    }

    private void SetActive(ContentActivationEntry entry, bool isActive)
    {
        if (isActive == entry.IsActive)
        {
            this._pendingStates.Remove(entry.Id);
        }
        else
        {
            this._pendingStates[entry.Id] = isActive;
        }
    }

    private void SetActiveOfShownEntries(bool isActive)
    {
        foreach (var entry in this.ShownEntries.ToList())
        {
            this.SetActive(entry, isActive);
        }
    }

    /// <summary>
    /// Activates the content of all types which was introduced up to the target version, and
    /// deactivates the content which was introduced later. Content of an unknown version is not changed.
    /// </summary>
    private async Task OnRestrictToTargetVersionClickAsync()
    {
        if (this.HasPendingChanges)
        {
            this.ToastService.ShowError(Resources.SaveOrDiscardChangesFirst);
            return;
        }

        var target = this._targetVersion;
        await this.ChangeAsync((apply, token) => this.Operations.RestrictToGameVersionAsync(target, apply, token, default)).ConfigureAwait(true);
    }

    private async Task OnSaveButtonClickAsync()
    {
        if (!this.HasPendingChanges)
        {
            this.ToastService.ShowSuccess(Resources.NoChangesToSave);
            return;
        }

        var activate = this._pendingStates.Where(p => p.Value).Select(p => p.Key).ToList();
        var deactivate = this._pendingStates.Where(p => !p.Value).Select(p => p.Key).ToList();
        await this.ChangeAsync((apply, token) => this.Operations.SetActiveAsync(activate, deactivate, apply, token, default)).ConfigureAwait(true);
    }

    /// <summary>
    /// Previews the change, lets the user confirm it and applies exactly the previewed change.
    /// </summary>
    /// <param name="change">The change, which is previewed when its first argument is <c>false</c>,
    /// and applied with the confirmation token of the preview when it's <c>true</c>.</param>
    private async Task ChangeAsync(Func<bool, string?, ValueTask<JsonObject>> change)
    {
        try
        {
            var preview = await change(false, null).ConfigureAwait(true);
            var activated = preview["change"]?["activated"]?.GetValue<int>() ?? 0;
            var deactivated = preview["change"]?["deactivated"]?.GetValue<int>() ?? 0;
            if (activated + deactivated == 0)
            {
                this._pendingStates.Clear();
                this.ToastService.ShowSuccess(Resources.NoChangesToSave);
                return;
            }

            var isConfirmed = await this.ModalService.ShowQuestionAsync(
                    Resources.ContentActivation,
                    string.Format(CultureInfo.CurrentCulture, Resources.ContentActivationConfirmFormat, activated, deactivated))
                .ConfigureAwait(true);
            if (!isConfirmed)
            {
                return;
            }

            await change(true, preview["confirmationToken"]!.GetValue<string>()).ConfigureAwait(true);
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

            this._entries = entries
                .GroupBy(entry => entry.Type)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<ContentActivationEntry>)group.ToList());
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
            this.Logger.LogError(ex, "Error loading the content activation data");
        }
    }
}
