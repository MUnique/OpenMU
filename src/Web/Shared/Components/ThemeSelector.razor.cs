// <copyright file="ThemeSelector.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// A toggle component which switches between the light and dark UI themes.
/// Persists the selection by calling the <see cref="MUnique.OpenMU.Web.Shared.Services.ThemeController"/>
/// (cookie-based, mirrors the <see cref="CultureSelector"/> approach).
/// </summary>
public partial class ThemeSelector : IAsyncDisposable
{
    private static readonly string JsModulePath =
        $"./_content/{typeof(ThemeSelector).Assembly.GetName().Name}/Components/{nameof(ThemeSelector)}.razor.js";

    private IJSObjectReference? _jsModule;

    private bool _isDarkInternal;

    private bool _hydrated;

    /// <summary>
    /// Gets or sets a value indicating whether the current dark-mode state as seen by the server-side renderer.
    /// </summary>
    /// <remarks>
    /// Only used until the first interactive render hydrates the value from the live DOM
    /// (the cascading HttpContext is null during interactive updates so this parameter
    /// would otherwise flip back to false even when the cookie is "dark").
    /// </remarks>
    [Parameter]
    public bool IsDark { get; set; }

    /// <summary>
    /// Gets or sets the navigation manager used for the post-toggle redirect.
    /// </summary>
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JS runtime used to load the theme reader module.
    /// </summary>
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private bool EffectiveIsDark => this._hydrated ? this._isDarkInternal : this.IsDark;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._jsModule is not null)
        {
            try
            {
                await this._jsModule.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // Ignore: circuit already gone.
            }
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        if (!firstRender)
        {
            return;
        }

        try
        {
            this._jsModule = await this.JS
                .InvokeAsync<IJSObjectReference>("import", JsModulePath)
                .ConfigureAwait(true);
            var theme = await this._jsModule
                .InvokeAsync<string?>("current")
                .ConfigureAwait(true);
            this._isDarkInternal = string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase);
            this._hydrated = true;
            this.StateHasChanged();
        }
        catch
        {
            // Fall back to the SSR parameter if JS is unavailable.
        }
    }

    private void Toggle()
    {
        var next = this.EffectiveIsDark ? "light" : "dark";
        var uri = new Uri(this.NavigationManager.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var themeEscaped = Uri.EscapeDataString(next);
        var uriEscaped = Uri.EscapeDataString(uri);

        var fullUri = $"/Theme/Set?theme={themeEscaped}&redirectUri={uriEscaped}";
        this.NavigationManager.NavigateTo(fullUri, forceLoad: true);
    }
}
