// <copyright file="PlugInConfigurationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Extension methods for the plugin configuration.
/// </summary>
public static class PlugInConfigurationExtensions
{
    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <typeparam name="T">The custom configuration type.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="referenceHandler">The reference handler.</param>
    /// <returns>
    /// The custom configuration as <typeparamref name="T" />.
    /// </returns>
    public static T? GetConfiguration<T>(this PlugInConfiguration configuration, ReferenceHandler? referenceHandler)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(configuration.CustomConfiguration))
        {
            return default;
        }

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = referenceHandler,
        };

        return JsonSerializer.Deserialize<T>(configuration.CustomConfiguration, options);
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configurationType">Type of the configuration.</param>
    /// <param name="referenceHandler">The reference handler.</param>
    /// <returns>
    /// The custom configuration as the given specified type.
    /// </returns>
    public static object? GetConfiguration(this PlugInConfiguration configuration, Type configurationType, ReferenceHandler? referenceHandler)
    {
        if (string.IsNullOrWhiteSpace(configuration.CustomConfiguration))
        {
            return default;
        }

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = referenceHandler,
        };

        return JsonSerializer.Deserialize(configuration.CustomConfiguration, configurationType, options);
    }

    /// <summary>
    /// Sets the configuration.
    /// </summary>
    /// <typeparam name="T">The custom configuration type.</typeparam>
    /// <param name="plugInConfiguration">The plug in configuration.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="referenceHandler">The reference handler.</param>
    public static void SetConfiguration<T>(this PlugInConfiguration plugInConfiguration, T configuration, ReferenceHandler? referenceHandler)
    {
        plugInConfiguration.CustomConfiguration = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = referenceHandler });
    }

    /// <summary>
    /// Sets the configuration.
    /// </summary>
    /// <param name="plugInConfiguration">The plug in configuration.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="referenceHandler">The reference handler.</param>
    public static void SetConfiguration(this PlugInConfiguration plugInConfiguration, object configuration, ReferenceHandler? referenceHandler)
    {
        plugInConfiguration.CustomConfiguration = JsonSerializer.Serialize(
            configuration,
            configuration.GetType(),
            new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = referenceHandler });
    }
}