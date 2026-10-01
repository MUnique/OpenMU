// <copyright file="LocalizedStringResourceExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

/// <summary>
/// Builds multilingual configuration values from neutral and satellite resources.
/// </summary>
public static class LocalizedStringResourceExtensions
{
    // StyleCop does not recognize the C# 14 extension receiver as a parameter; it cannot be prefixed with this.
#pragma warning disable SA1101
    /// <summary>
    /// Resource operations which use only explicitly defined translations.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    extension(ResourceManager resourceManager)
    {
        /// <summary>
        /// Gets the cultures with their own resource set, excluding fallback aliases.
        /// </summary>
        public IReadOnlyList<CultureInfo> AvailableCultures => CulturesCache.GetOrAdd(resourceManager, FindAvailableCultures);

        /// <summary>
        /// Builds a value containing the neutral name and all explicit translations.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <returns>The multilingual value.</returns>
        public LocalizedString GetLocalizedString(string key)
        {
            var neutral = resourceManager.GetString(key, CultureInfo.InvariantCulture)
                ?? throw new ArgumentException("The neutral resource key does not exist.", nameof(key));
            var result = new LocalizedString(neutral);
            foreach (var culture in resourceManager.AvailableCultures)
            {
                if (resourceManager.GetResourceSet(culture, true, tryParents: false)?.GetString(key) is { Length: > 0 } text)
                {
                    result = result.WithTranslation(culture, text);
                }
            }

            return result;
        }
    }

#pragma warning restore SA1101

    /// <summary>
    /// Strongly typed construction of multilingual values.
    /// </summary>
    extension(LocalizedString)
    {
        /// <summary>
        /// Builds a value from a resource property, for example <c>() => MapNames.Lorencia</c>.
        /// </summary>
        /// <param name="resourceProperty">A static string property of a generated resource class.</param>
        /// <returns>The multilingual value.</returns>
        public static LocalizedString FromResource(Expression<Func<string>> resourceProperty)
        {
            ArgumentNullException.ThrowIfNull(resourceProperty);
            if (resourceProperty.Body is not MemberExpression { Member: PropertyInfo property }
                || property.PropertyType != typeof(string)
                || property.GetMethod?.IsStatic != true
                || property.DeclaringType?.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null) is not ResourceManager resourceManager)
            {
                throw new ArgumentException("Expected a static string resource property.", nameof(resourceProperty));
            }

            return resourceManager.GetLocalizedString(property.Name);
        }
    }

    private static readonly ConcurrentDictionary<ResourceManager, IReadOnlyList<CultureInfo>> CulturesCache = new();

    private static IEnumerable<CultureInfo> GetCultureCandidates()
    {
        // ICU may list zh-Hans-CN but omit the valid zh-CN alias used by a satellite.
        // Include language-region aliases for script-qualified cultures without hard-coding languages.
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
            .SelectMany(culture =>
            {
                var parts = culture.Name.Split('-');
                return parts.Length == 3 && parts[1].Length == 4
                    ? new[] { culture, CultureInfo.GetCultureInfo($"{parts[0]}-{parts[2]}") }
                    : new[] { culture };
            })
            .DistinctBy(culture => culture.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<CultureInfo> FindAvailableCultures(ResourceManager resourceManager)
    {
        var cultures = new List<CultureInfo>();
        foreach (var culture in GetCultureCandidates().OrderBy(c => c.Name, StringComparer.Ordinal))
        {
            if (culture.Equals(CultureInfo.InvariantCulture)
                || culture.TwoLetterISOLanguageName == LocalizedString.NeutralLanguageCode
                || resourceManager.GetResourceSet(culture, true, tryParents: false) is not { } resources)
            {
                continue;
            }

            // ResourceManager may already have cached a parent's set under this culture after a fallback lookup.
            var isFallback = false;
            for (var parent = culture.Parent; ; parent = parent.Parent)
            {
                if (ReferenceEquals(resources, resourceManager.GetResourceSet(parent, true, tryParents: false)))
                {
                    isFallback = true;
                    break;
                }

                if (parent.Equals(CultureInfo.InvariantCulture))
                {
                    break;
                }
            }

            if (!isFallback)
            {
                cultures.Add(culture);
            }
        }

        return cultures.AsReadOnly();
    }
}
