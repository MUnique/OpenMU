// <copyright file="ThemeSelector.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MUnique.OpenMU.Web.Shared.Properties;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A dropdown component which switches between the light, the dark and the automatic UI theme.
/// The automatic theme follows the color scheme of the operating system or browser.
/// The selection is applied immediately and persisted in the cookie of the <see cref="ThemeController"/>.
/// </summary>
public partial class ThemeSelector : IAsyncDisposable
{
    private static readonly string JsModulePath =
        $"./_content/{typeof(ThemeSelector).Assembly.GetName().Name}/Components/{nameof(ThemeSelector)}.razor.js";

    private static readonly string[] Themes = [ThemeController.LightTheme, ThemeController.DarkTheme, ThemeController.AutoTheme];

    private IJSObjectReference? _jsModule;

    private string? _hydratedTheme;

    private bool _isOpen;

    /// <summary>
    /// Gets or sets the normalized theme preference (light, dark or auto) as seen by the server-side renderer.
    /// </summary>
    /// <remarks>
    /// Only used until the first interactive render hydrates the value from the live DOM
    /// (the cascading HttpContext is null during interactive updates so this parameter
    /// would otherwise flip back to the default even when the cookie has another value).
    /// </remarks>
    [Parameter]
    public string Theme { get; set; } = ThemeController.DefaultTheme;

    /// <summary>
    /// Gets or sets the navigation manager used for the fallback when JavaScript is not available.
    /// </summary>
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JS runtime used to load the theme module.
    /// </summary>
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private string EffectiveTheme => this._hydratedTheme ?? ThemeController.NormalizeTheme(this.Theme);

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
            if (!string.IsNullOrEmpty(theme))
            {
                this._hydratedTheme = ThemeController.NormalizeTheme(theme);
                this.StateHasChanged();
            }
        }
        catch (JSException)
        {
            // Fall back to the SSR parameter if the module can't be loaded.
        }
        catch (JSDisconnectedException)
        {
            // Ignore: circuit already gone.
        }
    }

    private static string GetIcon(string theme)
    {
        return theme switch
        {
            "dark" => "oi-moon",
            "light" => "oi-sun",
            _ => "oi-contrast",
        };
    }

    private static string GetCaption(string theme)
    {
        return theme switch
        {
            "dark" => Resources.ThemeDark,
            "light" => Resources.ThemeLight,
            _ => Resources.ThemeAuto,
        };
    }

    private void ToggleMenu()
    {
        this._isOpen = !this._isOpen;
    }

    private void CloseMenu()
    {
        this._isOpen = false;
    }

    private void OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape")
        {
            this.CloseMenu();
        }
    }

    private async Task SelectAsync(string theme)
    {
        this._isOpen = false;
        if (this._jsModule is null)
        {
            this.NavigateToThemeController(theme);
            return;
        }

        try
        {
            await this._jsModule.InvokeVoidAsync("set", ThemeController.CookieName, theme).ConfigureAwait(true);
            this._hydratedTheme = theme;
        }
        catch (JSException)
        {
            this.NavigateToThemeController(theme);
        }
    }

    private void NavigateToThemeController(string theme)
    {
        var uri = new Uri(this.NavigationManager.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var themeEscaped = Uri.EscapeDataString(theme);
        var uriEscaped = Uri.EscapeDataString(uri);

        var fullUri = $"Theme/Set?theme={themeEscaped}&redirectUri={uriEscaped}";
        this.NavigationManager.NavigateTo(fullUri, forceLoad: true);
    }
}
