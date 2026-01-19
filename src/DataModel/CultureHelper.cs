// <copyright file="CultureHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using System.Globalization;
using Nito.Disposables;

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
}