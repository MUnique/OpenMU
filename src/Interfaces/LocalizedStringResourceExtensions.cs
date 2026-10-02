// <copyright file="LocalizedStringResourceExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

/// <summary>
/// Extensions to build <see cref="LocalizedString"/>s from resources, including all available translations.
/// </summary>
public static class LocalizedStringResourceExtensions
{
    // StyleCop doesn't recognize the C# 14 extension receiver as a parameter yet, so it can't be prefixed with 'this'.
#pragma warning disable SA1101

    /// <summary>
    /// Extensions for <see cref="ResourceManager"/>.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    extension(ResourceManager resourceManager)
    {
        /// <summary>
        /// Gets the cultures which have their own resource set, excluding the neutral language.
        /// </summary>
        public IReadOnlyList<CultureInfo> AvailableCultures => CulturesCache.GetValue(resourceManager, FindAvailableCultures);

        /// <summary>
        /// Builds a <see cref="LocalizedString"/> with the neutral text and all translations which are explicitly defined for the key.
        /// If the resource manager is registered at <see cref="LocalizedStringResources"/>, the result also gets
        /// a <see cref="LocalizedString.SourceKey"/> and a <see cref="LocalizedString.SourceStamp"/>.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <returns>The localized string.</returns>
        /// <exception cref="ArgumentException">The key doesn't exist in the neutral resources.</exception>
        public LocalizedString GetLocalizedString(string key)
        {
            var neutral = resourceManager.GetString(key, CultureInfo.InvariantCulture)
                          ?? throw new ArgumentException($"The resource key '{key}' does not exist.", nameof(key));
            var result = new LocalizedString(neutral);
            foreach (var culture in resourceManager.AvailableCultures)
            {
                if (resourceManager.GetResourceSet(culture, true, tryParents: false)?.GetString(key) is { Length: > 0 } text)
                {
                    result = result.WithTranslation(culture, text);
                }
            }

            if (LocalizedStringResources.TryGetName(resourceManager, out var sourceName))
            {
                result = result.WithSourceKey(LocalizedStringResources.CreateSourceKey(sourceName, key)).WithSourceStamp();
            }

            return result;
        }
    }

    /// <summary>
    /// Extensions for <see cref="LocalizedString"/>.
    /// </summary>
    /// <param name="localizedString">The localized string.</param>
    extension(LocalizedString localizedString)
    {
        /// <summary>
        /// Builds a <see cref="LocalizedString"/> from a property of a generated resource class, for example
        /// <c>LocalizedString.FromResource(() => MapNames.Lorencia)</c>.
        /// If the resource class isn't registered yet, it's registered at <see cref="LocalizedStringResources"/>
        /// with the name of the resource class.
        /// </summary>
        /// <param name="resourceProperty">The expression which accesses the static string property of a resource class.</param>
        /// <returns>The localized string, including all available translations, the source key and the source stamp.</returns>
        public static LocalizedString FromResource(Expression<Func<string>> resourceProperty)
        {
            ArgumentNullException.ThrowIfNull(resourceProperty);
            if (resourceProperty.Body is not MemberExpression { Member: PropertyInfo { PropertyType: var propertyType, GetMethod.IsStatic: true, DeclaringType: { } resourceType } property }
                || propertyType != typeof(string)
                || resourceType.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null) is not ResourceManager resourceManager)
            {
                throw new ArgumentException("Expected a static string property of a resource class, e.g. () => MapNames.Lorencia.", nameof(resourceProperty));
            }

            if (!LocalizedStringResources.TryGetName(resourceManager, out _))
            {
                LocalizedStringResources.Register(resourceType.Name, resourceManager);
            }

            return resourceManager.GetLocalizedString(property.Name);
        }

        /// <summary>
        /// Builds the current value of the source of this localized string, see <see cref="LocalizedString.SourceKey"/>.
        /// </summary>
        /// <returns>
        /// The localized string as it's currently defined by its source,
        /// or <see langword="null"/> if it has no source key or the source isn't registered or doesn't contain the key anymore.
        /// </returns>
        public LocalizedString? GetFromSource()
        {
            if (!LocalizedStringResources.TryResolve(localizedString.SourceKey, out var resourceManager, out var resourceKey)
                || resourceManager.GetString(resourceKey, CultureInfo.InvariantCulture) is null)
            {
                return null;
            }

            return resourceManager.GetLocalizedString(resourceKey);
        }
    }

#pragma warning restore SA1101

    private static readonly ConditionalWeakTable<ResourceManager, IReadOnlyList<CultureInfo>> CulturesCache = new();

    private static readonly Lazy<IReadOnlyList<CultureInfo>> CultureCandidates = new(FindCultureCandidates);

    private static IReadOnlyList<CultureInfo> FindAvailableCultures(ResourceManager resourceManager)
    {
        var cultures = new List<CultureInfo>();
        foreach (var culture in CultureCandidates.Value)
        {
            if (resourceManager.GetResourceSet(culture, true, tryParents: false) is not { } resources)
            {
                continue;
            }

            // After a lookup with fallback, the ResourceManager may have cached a parent's set under this culture.
            var isFallback = false;
            for (var parent = culture.Parent; ; parent = parent.Parent)
            {
                if (ReferenceEquals(resources, resourceManager.GetResourceSet(parent, true, tryParents: false)))
                {
                    isFallback = true;
                    break;
                }

                if (string.IsNullOrEmpty(parent.Name))
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

    private static IReadOnlyList<CultureInfo> FindCultureCandidates()
    {
        // ICU may enumerate zh-Hans-CN, but not the zh-CN alias which is used by a satellite assembly.
        // So we add the language-region aliases of script-qualified cultures and the names of the deployed satellite directories.
        var enumerated = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var aliases = enumerated
            .Select(culture => culture.Name.Split('-'))
            .Where(parts => parts is [_, { Length: 4 }, _])
            .Select(parts => TryGetCulture($"{parts[0]}-{parts[2]}"));
        var deployed = EnumerateSatelliteDirectoryNames().Select(TryGetCulture);

        return enumerated
            .Concat(aliases)
            .Concat(deployed)
            .OfType<CultureInfo>()
            .Where(culture => !string.IsNullOrEmpty(culture.Name)
                              && culture.TwoLetterISOLanguageName != LocalizedString.NeutralLanguageCode)
            .DistinctBy(culture => culture.Name, StringComparer.OrdinalIgnoreCase)
            .OrderBy(culture => culture.Name, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();
    }

    private static IEnumerable<string> EnumerateSatelliteDirectoryNames()
    {
        try
        {
            return Directory.EnumerateDirectories(AppContext.BaseDirectory).Select(Path.GetFileName).OfType<string>().ToList();
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static CultureInfo? TryGetCulture(string name)
    {
        try
        {
            return CultureInfo.GetCultureInfo(name, predefinedOnly: true);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }
}
