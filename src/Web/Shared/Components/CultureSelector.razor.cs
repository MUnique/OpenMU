// <copyright file="CultureSelector.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A selection component for supported UI-Cultures (languages).
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class CultureSelector
{
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureSelector"/> class.
    /// </summary>
    /// <param name="localizationOptions">The localization options.</param>
    /// <param name="navigationManager">The navigation manager.</param>
    public CultureSelector(IOptions<RequestLocalizationOptions> localizationOptions, NavigationManager navigationManager)
    {
        this._navigationManager = navigationManager;
        this.SupportedUICultures = localizationOptions.Value.SupportedUICultures?.OrderBy(c => c.NativeName).ToList() ?? [];
    }

    /// <summary>
    /// Gets or sets the selected culture code.
    /// </summary>
    /// <remarks>
    /// The current UI culture isn't necessarily one of the supported cultures, e.g. when the
    /// browser requests an unsupported language and the server falls back to its own culture.
    /// In this case, the supported culture is selected which provides the displayed texts.
    /// </remarks>
    public string SelectedCultureCode
    {
        get
        {
            return this.GetDisplayedCultureCode();
        }
        set
        {
            if (this.GetDisplayedCultureCode() == value)
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
    /// Gets the code of the supported culture whose texts are displayed for the current UI culture.
    /// </summary>
    /// <returns>
    /// The current UI culture or its nearest supported parent culture. If there is none, the texts come
    /// from the neutral resources, so the neutral language is returned.
    /// </returns>
    private string GetDisplayedCultureCode()
    {
        for (var culture = CultureInfo.CurrentUICulture; !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
        {
            if (this.SupportedUICultures.Any(supported => supported.Name == culture.Name))
            {
                return culture.Name;
            }
        }

        return LocalizedString.NeutralLanguageCode;
    }

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
