// <copyright file="TypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the implemented interface of <see cref="ISupportCustomConfiguration{T}"/>.
        /// </summary>
        /// <param name="plugInType">Type of the plug in.</param>
        /// <returns>The implemented interface of <see cref="ISupportCustomConfiguration{T}"/>.</returns>
        public static Type? GetCustomConfigurationSupportInterfaceType(this Type plugInType)
        {
            return plugInType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ISupportCustomConfiguration<>));
        }

        /// <summary>
        /// Gets the generic type parameter of <see cref="ISupportCustomConfiguration{T}"/>.
        /// </summary>
        /// <param name="plugInType">Type of the plug in.</param>
        /// <returns>The generic type parameter of <see cref="ISupportCustomConfiguration{T}"/>.</returns>
        public static Type? GetCustomConfigurationType(this Type plugInType)
        {
            if (plugInType.GetCustomConfigurationSupportInterfaceType() is { } configSupportInterface)
            {
                return configSupportInterface.GenericTypeArguments[0];
            }

            return null;
        }
    }
}