namespace MUnique.OpenMU.DataModel;

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

/// <summary>
/// Provides access to the model resources like type and property captions and descriptions.
/// </summary>
public static class ModelResourceProvider
{

    private static readonly Regex WordSeparatorRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    private static readonly ConcurrentDictionary<Assembly, ResourceManager?> ResourceManagersByAssembly = new();

    public static string GetTypeCaption<TModel>(CultureInfo? cultureInfo = null)
    {
        return typeof(TModel).GetTypeCaption(cultureInfo);
    }

    /// <summary>
    /// Gets the localized caption for the specified model <paramref name="modelType"/>.
    /// Falls back to a generated spaced caption based on the type name when no resource entry is found.
    /// </summary>
    /// <param name="modelType">The model type whose caption should be resolved.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized caption or a spaced fallback type name.</returns>
    public static string GetTypeCaption(this Type modelType, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = modelType.Name + "_TypeCaption";
        return GetResourceManager(modelType)?.GetString(resourceKey, cultureInfo) ?? SeparateWords(modelType.Name);
    }

    /// <summary>
    /// Gets the localized pluralized caption for the specified model type <typeparamref name="TModel"/>.
    /// Falls back to a generated spaced caption based on the type name when no resource entry is found.
    /// </summary>
    /// <typeparam name="TModel">The model type for which to get the pluralized caption.</typeparam>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized pluralized caption or a spaced fallback representation of the type name.</returns>
    public static string GetPluralizedTypeCaption<TModel>(CultureInfo? cultureInfo = null)
    {
        return typeof(TModel).GetPluralizedTypeCaption(cultureInfo);
    }

    /// <summary>
    /// Gets the localized pluralized caption for the specified model <paramref name="modelType"/>.
    /// Falls back to a generated spaced caption based on the type name when no resource entry is found.
    /// </summary>
    /// <param name="modelType">The model type whose pluralized caption should be resolved.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized pluralized caption or a spaced fallback type name.</returns>
    public static string GetPluralizedTypeCaption(this Type modelType, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = modelType.Name + "_TypeCaptionPlural";
        return GetResourceManager(modelType)?.GetString(resourceKey, cultureInfo) ?? SeparateWords(modelType.Name);
    }

    /// <summary>
    /// Gets the localized description for the specified model type <typeparamref name="TModel"/>.
    /// Returns an empty string when no resource entry is found.
    /// </summary>
    /// <typeparam name="TModel">The model type for which to get the description.</typeparam>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized description or an empty string.</returns>
    public static string GetTypeDescription<TModel>(CultureInfo? cultureInfo = null)
    {
        return typeof(TModel).GetTypeDescription(cultureInfo);
    }

    /// <summary>
    /// Gets the localized description for the specified model <paramref name="modelType"/>.
    /// Returns an empty string when no resource entry is found.
    /// </summary>
    /// <param name="modelType">The model type whose description should be resolved.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized description or an empty string.</returns>
    public static string GetTypeDescription(this Type modelType, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = modelType.Name + "_TypeDescription";
        return GetResourceManager(modelType)?.GetString(resourceKey, cultureInfo) ?? string.Empty;
    }

    /// <summary>
    /// Gets the localized caption for a property of the specified model type <typeparamref name="TModel"/>.
    /// Falls back to a spaced version of the property name when no resource entry is found.
    /// </summary>
    /// <typeparam name="TModel">The model type containing the property.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized property caption or a spaced fallback property name.</returns>
    public static string GetPropertyCaption<TModel>(string propertyName, CultureInfo? cultureInfo = null)
    {
        return typeof(TModel).GetPropertyCaption(propertyName, cultureInfo);
    }

    /// <summary>
    /// Gets the localized caption for a property of the specified model <paramref name="modelType"/>.
    /// Falls back to a spaced version of the property name when no resource entry is found.
    /// </summary>
    /// <param name="modelType">The model type containing the property.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized property caption or a spaced fallback property name.</returns>
    public static string GetPropertyCaption(this Type modelType, string propertyName, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = $"{modelType.Name}_{propertyName}_Caption";
        return GetResourceManager(modelType)?.GetString(resourceKey, cultureInfo) ?? SeparateWords(propertyName);
    }

    /// <summary>
    /// Gets the localized description for a property of the specified model type <typeparamref name="TModel"/>.
    /// Returns an empty string when no resource entry is found.
    /// </summary>
    /// <typeparam name="TModel">The model type containing the property.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized property description or an empty string.</returns>
    public static string GetPropertyDescription<TModel>(string propertyName, CultureInfo? cultureInfo = null)
    {
        return typeof(TModel).GetPropertyDescription(propertyName, cultureInfo);
    }

    /// <summary>
    /// Gets the localized description for a property of the specified model <paramref name="modelType"/>.
    /// Returns an empty string when no resource entry is found.
    /// </summary>
    /// <param name="modelType">The model type containing the property.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized property description or an empty string.</returns>
    public static string GetPropertyDescription(this Type modelType, string propertyName, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = $"{modelType.Name}_{propertyName}_Description";
        return GetResourceManager(modelType)?.GetString(resourceKey, cultureInfo) ?? string.Empty;
    }

    /// <summary>
    /// Gets the localized caption for an enum value of the generic enum type <typeparamref name="TEnum"/>.
    /// Falls back to a spaced version of the enum value name when no resource entry is found.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="enumValue">The enum value instance.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized enum value caption or a spaced fallback value name.</returns>
    public static string GetEnumCaption<TEnum>(this TEnum enumValue, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = $"{typeof(TEnum).Name}_{enumValue}_Caption";
        return GetResourceManager(typeof(TEnum))?.GetString(resourceKey, cultureInfo) ?? SeparateWords(enumValue?.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Gets the localized caption for a specific enum value given its <paramref name="enumType"/> and <paramref name="enumValue"/>.
    /// Falls back to a spaced version of the enum value name when no resource entry is found.
    /// </summary>
    /// <param name="enumType">The enum type.</param>
    /// <param name="enumValue">The enum value.</param>
    /// <param name="cultureInfo">Optional culture to use. If null, <see cref="CultureInfo.CurrentUICulture"/> is used.</param>
    /// <returns>The localized enum value caption or a spaced fallback value name.</returns>
    public static string GetEnumCaption(Type enumType, Enum enumValue, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.CurrentUICulture;
        var resourceKey = $"{enumType.Name}_{enumValue}_Caption";
        return GetResourceManager(enumType)?.GetString(resourceKey, cultureInfo) ?? SeparateWords(enumValue.ToString());
    }

    /// <summary>
    /// Gets the <see cref="ResourceManager"/> associated with the assembly of the specified <paramref name="type"/>.
    /// The manager is cached per assembly. If no matching resource base name is found, returns null.
    /// </summary>
    /// <param name="type">A type contained in the target assembly.</param>
    /// <returns>The <see cref="ResourceManager"/> or null if none exists.</returns>
    private static ResourceManager? GetResourceManager(Type type)
    {
        var assembly = type.Assembly;
        return ResourceManagersByAssembly.GetOrAdd(
            assembly,
            a =>
            {
                var baseName = type.Assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith("Properties.ModelResources.resources"))?[..^".resources".Length];
                if (baseName is null)
                {
                    return null;
                }

                return new(new(baseName), a);
            });
    }

    /// <summary>
    /// Separates the words by a space. Words are detected by transitions from lower-case to upper-case letters.
    /// Also performs simple replacements to singularize or adjust certain suffixes:
    /// Replaces " Definitions" with "s" and " Definition" with an empty string.
    /// Intended as a fallback when no resource text is found.
    /// </summary>
    /// <param name="input">The input identifier to transform.</param>
    /// <returns>The spaced and adjusted representation.</returns>
    private static string SeparateWords(string input)
    {
        return WordSeparatorRegex.Replace(input, "$1 $2")
            .Replace(" Definitions", "s", StringComparison.InvariantCulture)
            .Replace(" Definition", string.Empty, StringComparison.InvariantCulture);
    }
}