namespace MUnique.OpenMU.Web.AdminPanel.Localization;

using System;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Localization;

/// <summary>
/// Base class for components which require localized strings. It subscribes to <see cref="LocalizationService.LanguageChanged"/>
/// so derived components automatically refresh when the language changes.
/// </summary>
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    /// <summary>
    /// Gets or sets the localization service.
    /// </summary>
    [Inject]
    protected LocalizationService Localization { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.Localization.LanguageChanged += this.OnLanguageChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Localization.LanguageChanged -= this.OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        _ = this.InvokeAsync(this.StateHasChanged);
    }
}
