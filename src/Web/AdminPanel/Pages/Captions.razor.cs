// <copyright file="Captions.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Page which compares the captions of the configuration with their sources and applies selected changes.
/// </summary>
public partial class Captions
{
    private const string NeutralFilterValue = "-";

    private List<ChangeViewModel>? _changes;

    private IReadOnlyList<string> _unresolvedSourceKeys = [];

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

    private IEnumerable<ChangeViewModel> FilteredChanges => (this._changes ?? [])
        .Where(c => this.LanguageFilter == string.Empty || (c.Change.CultureName ?? NeutralFilterValue) == this.LanguageFilter)
        .Where(c => this.KindFilter == string.Empty || c.Change.Kind.ToString() == this.KindFilter);

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (this.SetupService.IsInstalled && !this.SetupService.IsUpdateRequired)
        {
            await this.LoadChangesAsync().ConfigureAwait(true);
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

    private async Task LoadChangesAsync()
    {
        try
        {
            var (changes, unresolvedSourceKeys) = await this.CaptionService.DetermineChangesAsync().ConfigureAwait(true);
            this._changes = changes.Select(c => new ChangeViewModel(c)).ToList();
            this._unresolvedSourceKeys = unresolvedSourceKeys;
        }
        catch (Exception ex)
        {
            this._exception = ex;
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
            var selectedIds = this._changes!.Where(c => c.Selected).Select(c => c.Change.Id).ToList();
            var applied = await this.CaptionService.ApplyChangesAsync(selectedIds).ConfigureAwait(true);
            this._message = string.Format(Resources.AppliedCaptionChanges, applied);
            await this.LoadChangesAsync().ConfigureAwait(true);
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
            this._message = string.Format(Resources.LinkedBuiltInCaptions, linked, skipped);
            this._changes = null;
            this.StateHasChanged();
            await this.LoadChangesAsync().ConfigureAwait(true);
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
