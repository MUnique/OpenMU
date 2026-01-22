// <copyright file="CultureHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using System.Globalization;
using System.Resources;
using Nito.Disposables;
using Nito.Disposables.Internals;

/// <summary>
/// Helper class for culture related operations.
/// </summary>
public static class CultureHelper
{
    /// <summary>
    /// Sets the temporary culture for the current thread.
    /// Should be disposed after usage to revert to the previous culture.
    /// Should not be used between async calls.
    /// </summary>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>A disposable to revert to the previous culture.</returns>
    public static IDisposable SetTemporaryCulture(CultureInfo cultureInfo)
    {
        var oldUiCulture = CultureInfo.CurrentUICulture;
        var oldCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;

        return Disposable.Create(() =>
        {
            CultureInfo.CurrentUICulture = oldUiCulture;
            CultureInfo.CurrentCulture = oldCulture;
        });
    }

    /// <summary>
    /// Gets the available cultures for a specific resource.
    /// </summary>
    /// <typeparam name="TResources">The type of the resources.</typeparam>
    /// <returns>An enumeration of <see cref="CultureInfo.TwoLetterISOLanguageName"/> of available cultures of the given resource type.</returns>
    public static IEnumerable<CultureInfo> GetAvailableCultures<TResources>()
    {
        var resourceManager = new ResourceManager(typeof(TResources));

        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var result = cultures
            .Except([CultureInfo.InvariantCulture])
            .Where(culture => culture is { IsNeutralCulture: true, TwoLetterISOLanguageName: "en" }
                              || !object.Equals(resourceManager.GetResourceSet(culture, true, false), null))
            .WhereNotNull()
            .ToList();

        return result;
    }
}