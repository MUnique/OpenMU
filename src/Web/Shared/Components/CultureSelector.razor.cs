// <copyright file="CultureSelector.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A selection component for supported UI-Cultures (languages).
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class CultureSelector
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureSelector"/> class.
    /// </summary>
    /// <param name="localizationOptions">The localization options.</param>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="navigationManager">The navigation manager.</param>
    public CultureSelector(IOptions<RequestLocalizationOptions> localizationOptions, IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        this._jsRuntime = jsRuntime;
        this._navigationManager = navigationManager;
        this.SupportedUICultures = localizationOptions.Value.SupportedUICultures?.OrderBy(c => c.NativeName).ToList() ?? [];
    }

    /// <summary>
    /// Gets or sets the selected culture code.
    /// </summary>
    public string SelectedCultureCode
    {
        get => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        set
        {
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == value)
            {
                return;
            }

            CultureInfo.CurrentUICulture = new CultureInfo(value);
            this.SetCultureInCookie();
        }
    }

    /// <summary>
    /// Gets the supported UI cultures.
    /// </summary>
    private IList<CultureInfo> SupportedUICultures { get; }

    /// <summary>
    /// Sets the culture in a cookie by calling the <see cref="CultureController"/>.
    /// </summary>
    private void SetCultureInCookie()
    {
        var uri = new Uri(this._navigationManager.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var cultureEscaped = Uri.EscapeDataString(CultureInfo.CurrentUICulture.Name);
        var uriEscaped = Uri.EscapeDataString(uri);

        var fullUri = $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}";
        this._navigationManager.NavigateTo(fullUri, forceLoad: true);
    }
}
