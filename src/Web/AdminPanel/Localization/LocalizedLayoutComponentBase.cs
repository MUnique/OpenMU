namespace MUnique.OpenMU.Web.AdminPanel.Localization;

using System;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Localization;

/// <summary>
/// Base class for layout components that react to language changes.
/// </summary>
public abstract class LocalizedLayoutComponentBase : LayoutComponentBase, IDisposable
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
