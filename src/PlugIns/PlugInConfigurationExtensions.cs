// <copyright file="PlugInConfigurationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using Newtonsoft.Json;

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
        /// <returns>
        /// The custom configuration as <typeparamref name="T" />.
        /// </returns>
        public static T GetConfiguration<T>(this PlugInConfiguration configuration)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(configuration.CustomConfiguration))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(configuration.CustomConfiguration);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configurationType">Type of the configuration.</param>
        /// <returns>
        /// The custom configuration as the given specified type.
        /// </returns>
        public static object? GetConfiguration(this PlugInConfiguration configuration, Type configurationType)
        {
            if (string.IsNullOrWhiteSpace(configuration.CustomConfiguration))
            {
                return default;
            }

            return JsonConvert.DeserializeObject(configuration.CustomConfiguration, configurationType);
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <typeparam name="T">The custom configuration type.</typeparam>
        /// <param name="plugInConfiguration">The plug in configuration.</param>
        /// <param name="configuration">The configuration.</param>
        public static void SetConfiguration<T>(this PlugInConfiguration plugInConfiguration, T configuration)
        {
            plugInConfiguration.CustomConfiguration = JsonConvert.SerializeObject(configuration, Formatting.Indented);
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="plugInConfiguration">The plug in configuration.</param>
        /// <param name="configuration">The configuration.</param>
        public static void SetConfiguration(this PlugInConfiguration plugInConfiguration, object configuration)
        {
            plugInConfiguration.CustomConfiguration = JsonConvert.SerializeObject(configuration, Formatting.Indented);
        }
    }
}