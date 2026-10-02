// <copyright file="Captions.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Page which compares the captions of the configuration with their sources and applies selected changes.
/// It guides through the steps: linking the built-in captions once, then reviewing and applying the changes.
/// </summary>
public partial class Captions
{
    private const string NeutralFilterValue = "-";

    private CaptionComparison? _comparison;

    private List<ChangeViewModel> _changes = [];

    private Exception? _exception;

    private string? _message;

    private bool _isBusy;

    private bool _isLinking;

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

    private string LanguageFilter { get; set; } = string.Empty;

    private string KindFilter { get; set; } = string.Empty;

    private IEnumerable<ChangeViewModel> FilteredChanges => this._changes
        .Where(c => this.LanguageFilter == string.Empty || (c.Change.CultureName ?? NeutralFilterValue) == this.LanguageFilter)
        .Where(c => this.KindFilter == string.Empty || c.Change.Kind.ToString() == this.KindFilter);

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (this.SetupService.IsInstalled && !this.SetupService.IsUpdateRequired)
        {
            await this.LoadAsync().ConfigureAwait(true);
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
        if (this._isLinking)
        {
            builder.OpenElement(5, "span");
            builder.AddAttribute(6, "class", "spinner-border spinner-border-sm me-2");
            builder.AddAttribute(7, "role", "status");
            builder.AddAttribute(8, "aria-hidden", "true");
            builder.CloseElement();
            builder.AddContent(9, Resources.LinkingBuiltInCaptions);
        }
        else
        {
            builder.AddContent(10, Resources.LinkBuiltInCaptions);
        }

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
        this.StateHasChanged();
        try
        {
            var (linked, skipped) = await this.CaptionService.LinkBuiltInCaptionsAsync().ConfigureAwait(true);
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
            this._isBusy = false;
            this._isLinking = false;
            this.StateHasChanged();
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
