// <copyright file="Captions.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Page which compares the captions of the configuration with their sources and applies selected changes.
/// It guides through the steps: linking the built-in captions once, then reviewing and applying the changes.
/// </summary>
public partial class Captions : IDisposable
{
    private const string NeutralFilterValue = "-";

    private CaptionComparison? _comparison;

    private List<ChangeViewModel> _changes = [];

    private Exception? _exception;

    private string? _message;

    private bool _isBusy;

    private bool _isLinking;

    private CaptionLinkStep _linkStep;

    private Stopwatch _linkStopwatch = new();

    private Stopwatch _stepStopwatch = new();

    private CancellationTokenSource? _linkProgressRefresh;

    private readonly CancellationTokenSource _disposeCts = new();

    /// <summary>
    /// The version of the linking state; it's incremented when captions are linked,
    /// so that the result of a check which was started before is discarded.
    /// </summary>
    private int _linkVersion;

    /// <summary>
    /// The built-in captions which can be linked to their sources, by the type name of their owner.
    /// It's <see langword="null"/> while it's not checked yet.
    /// </summary>
    private IReadOnlyDictionary<string, int>? _linkableCaptions;

    private bool _isCheckingLinkableCaptions;

    /// <summary>
    /// Gets or sets the setup service.
    /// </summary>
    [Inject]
    public SetupService SetupService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the caption service.
    /// </summary>
    [Inject]
    public ConfigurationCaptionService CaptionService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<Captions> Logger { get; set; } = null!;

    private string LanguageFilter { get; set; } = string.Empty;

    private string KindFilter { get; set; } = string.Empty;

    private IEnumerable<ChangeViewModel> FilteredChanges => this._changes
        .Where(c => this.LanguageFilter == string.Empty || (c.Change.CultureName ?? NeutralFilterValue) == this.LanguageFilter)
        .Where(c => this.KindFilter == string.Empty || c.Change.Kind.ToString() == this.KindFilter);

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts.Cancel();
        this._disposeCts.Dispose();
        this._linkProgressRefresh?.Cancel();
        this._linkProgressRefresh?.Dispose();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (this.SetupService.IsInstalled && !this.SetupService.IsUpdateRequired)
        {
            await this.LoadAsync().ConfigureAwait(true);
            if (this._comparison?.LinkedCaptions > 0)
            {
                // Checking takes a while when the data initialization has to be executed in memory, so the page is shown in the meantime.
                _ = this.CheckLinkableCaptionsAsync(this._disposeCts.Token);
            }
        }
    }

    private static string GetLanguageCaption(string? cultureName)
    {
        if (cultureName is null)
        {
            return Resources.CaptionNeutralText;
        }

        try
        {
            return $"{CultureInfo.GetCultureInfo(cultureName).NativeName} ({cultureName})";
        }
        catch (CultureNotFoundException)
        {
            return cultureName;
        }
    }

    private static string GetKindCaption(CaptionChangeKind kind)
    {
        return Resources.ResourceManager.GetString($"CaptionChangeKind_{kind}", Resources.Culture) ?? kind.ToString();
    }

    private static string GetKindDescription(CaptionChangeKind kind)
    {
        return Resources.ResourceManager.GetString($"CaptionChangeKind_{kind}_Description", Resources.Culture) ?? string.Empty;
    }

    private static string GetKindCssClass(CaptionChangeKind kind)
    {
        return kind switch
        {
            CaptionChangeKind.Missing => "text-bg-success",
            CaptionChangeKind.Updated => "text-bg-info",
            CaptionChangeKind.Removed => "text-bg-secondary",
            _ => "text-bg-warning",
        };
    }

    private RenderFragment RenderLinkButton(string cssClass) => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "class", $"btn {cssClass}");
        builder.AddAttribute(2, "type", "button");
        builder.AddAttribute(3, "disabled", this._isBusy);
        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, this.OnLinkClickAsync));
        builder.AddContent(5, Resources.LinkBuiltInCaptions);
        builder.CloseElement();
    };

    private async Task LoadAsync()
    {
        try
        {
            this._comparison = await this.CaptionService.CompareAsync().ConfigureAwait(true);
            this._changes = this._comparison.Changes.Select(c => new ChangeViewModel(c)).ToList();
            this.LanguageFilter = string.Empty;
            this.KindFilter = string.Empty;
        }
        catch (Exception ex)
        {
            this._exception = ex;
            this._comparison = new CaptionComparison(0, [], []);
            this._changes = [];
        }
    }

    /// <summary>
    /// Checks in the background if built-in captions can be linked, which are not linked yet,
    /// e.g. because an update of OpenMU added their sources.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token, which is canceled when the page is disposed.</param>
    private async Task CheckLinkableCaptionsAsync(CancellationToken cancellationToken)
    {
        this._isCheckingLinkableCaptions = true;
        var linkVersion = this._linkVersion;
        try
        {
            // The continuation runs on the renderer's synchronization context, so the state can be changed safely.
            var linkableCaptions = await this.CaptionService.FindLinkableCaptionsAsync().ConfigureAwait(true);
            if (!cancellationToken.IsCancellationRequested && linkVersion == this._linkVersion)
            {
                this._linkableCaptions = linkableCaptions;
            }
        }
        catch (Exception ex)
        {
            // It's just a hint, linking is still possible manually.
            this.Logger.LogWarning(ex, "Couldn't check for built-in captions which are not linked to their sources yet.");
        }
        finally
        {
            this._isCheckingLinkableCaptions = false;
            if (!cancellationToken.IsCancellationRequested)
            {
                this.StateHasChanged();
            }
        }
    }

    private void SelectRecommended()
    {
        foreach (var change in this.FilteredChanges)
        {
            change.Selected = change.Change.IsRecommended;
        }
    }

    private void SelectNone()
    {
        foreach (var change in this.FilteredChanges)
        {
            change.Selected = false;
        }
    }

    private async Task OnApplyClickAsync()
    {
        this._exception = null;
        this._message = null;
        this._isBusy = true;
        this.StateHasChanged();
        try
        {
            var selectedIds = this._changes.Where(c => c.Selected).Select(c => c.Change.Id).ToList();
            var applied = await this.CaptionService.ApplyChangesAsync(selectedIds).ConfigureAwait(true);
            this._message = string.Format(Resources.AppliedCaptionChanges, applied);
            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this._exception = ex;
        }
        finally
        {
            this._isBusy = false;
            this.StateHasChanged();
        }
    }

    private async Task OnLinkClickAsync()
    {
        this._exception = null;
        this._message = null;
        this._isBusy = true;
        this._isLinking = true;
        this._linkVersion++;
        this._linkableCaptions = null;
        this._linkStep = CaptionLinkStep.LoadingConfiguration;
        this._linkStopwatch.Restart();
        this._stepStopwatch.Restart();
        this._linkProgressRefresh = new CancellationTokenSource();
        _ = this.RefreshLinkProgressAsync(this._linkProgressRefresh.Token);
        this.StateHasChanged();

        // The progress is created on the renderer's synchronization context, so the steps are reported there.
        var progress = new Progress<CaptionLinkStep>(step =>
        {
            this._linkStep = step;
            this._stepStopwatch.Restart();
            this.StateHasChanged();
        });
        try
        {
            var (linked, skipped) = await this.CaptionService.LinkBuiltInCaptionsAsync(progress).ConfigureAwait(true);
            this._comparison = null;
            this.StateHasChanged();
            await this.LoadAsync().ConfigureAwait(true);
            this._message = string.Format(Resources.LinkedBuiltInCaptions, linked, skipped);
            if (this._changes.Count > 0)
            {
                this._message += " " + Resources.CaptionsNextStepReview;
            }
        }
        catch (Exception ex)
        {
            this._exception = ex;
        }
        finally
        {
            if (this._linkProgressRefresh is { } refresh)
            {
                this._linkProgressRefresh = null;
                await refresh.CancelAsync().ConfigureAwait(true);
                refresh.Dispose();
            }

            this._linkStopwatch.Stop();
            this._isBusy = false;
            this._isLinking = false;
            this.StateHasChanged();
        }
    }

    /// <summary>
    /// Refreshes the progress (elapsed time and estimated progress of the current step) while linking.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token which stops the refresh.</param>
    private async Task RefreshLinkProgressAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // expected when linking is finished
        }
    }

    /// <summary>
    /// The view model of a <see cref="CaptionChange"/>.
    /// </summary>
    private sealed class ChangeViewModel
    {
        public ChangeViewModel(CaptionChange change)
        {
            this.Change = change;
            this.Selected = change.IsRecommended;
        }

        public CaptionChange Change { get; }

        public bool Selected { get; set; }
    }
}
