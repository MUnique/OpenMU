// <copyright file="JsonConverterRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides a registry for globally used <see cref="JsonConverter" /> instances
/// which can be added to <see cref="JsonSerializerOptions" /> where required.
/// </summary>
public static class JsonConverterRegistry
{
    /// <summary>
    /// Backing field which stores all registered <see cref="JsonConverter" /> instances.
    /// </summary>
    private static readonly List<JsonConverter> _converters = new List<JsonConverter>();

    /// <summary>
    /// Gets an enumerable collection of all registered <see cref="JsonConverter" /> instances.
    /// </summary>
    public static IEnumerable<JsonConverter> Converters => _converters;

    /// <summary>
    /// Registers the specified <paramref name="converter" /> so that it can be
    /// reused wherever custom <see cref="JsonSerializerOptions" /> are created.
    /// </summary>
    /// <param name="converter">The JSON converter to register.</param>
    public static void RegisterConverter(JsonConverter converter)
    {
        _converters.Add(converter);
    }

    /// <summary>
    /// Clears all previously registered <see cref="JsonConverter" /> instances.
    /// </summary>
    public static void ClearConverters()
    {
        _converters.Clear();
    }
}