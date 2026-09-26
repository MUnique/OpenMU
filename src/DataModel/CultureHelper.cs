// <copyright file="CultureHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using System.Globalization;
using System.IO;
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
    /// Gets a weekday caption, preferring deployed resources over platform culture data.
    /// </summary>
    /// <param name="day">The weekday.</param>
    /// <returns>The localized weekday name.</returns>
    public static string GetDayName(DayOfWeek day)
    {
        var culture = CultureInfo.CurrentUICulture;
        var resources = Properties.Resources.ResourceManager.GetResourceSet(culture, true, false);
        return resources?.GetString($"DayOfWeek_{day}") ?? culture.DateTimeFormat.GetDayName(day);
    }

    /// <summary>
    /// Gets the available cultures for a specific resource.
    /// </summary>
    /// <typeparam name="TResources">The type of the resources.</typeparam>
    /// <returns>The available cultures of the given resource type.</returns>
    public static IEnumerable<CultureInfo> GetAvailableCultures<TResources>()
    {
        var resourceManager = new ResourceManager(typeof(TResources));

        // ICU can enumerate zh-Hans-CN instead of zh-CN. Include the exact culture names
        // of deployed satellite assemblies so regional resources are found on every platform.
        var assembly = typeof(TResources).Assembly;
        var directory = Path.GetDirectoryName(assembly.Location) ?? AppContext.BaseDirectory;
        var satelliteName = $"{assembly.GetName().Name}.resources.dll";
        var deployedCultures = Directory.EnumerateDirectories(directory)
            .Where(path => File.Exists(Path.Combine(path, satelliteName)))
            .Select(path => CultureInfo.GetCultureInfo(Path.GetFileName(path)));
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Concat(deployedCultures)
            .Distinct();
        var result = cultures
            .Except([CultureInfo.InvariantCulture])
            .Where(culture => culture is { IsNeutralCulture: true, TwoLetterISOLanguageName: "en" }
                              || !object.Equals(resourceManager.GetResourceSet(culture, true, false), null))
            .WhereNotNull()
            .ToList();

        return result;
    }
}