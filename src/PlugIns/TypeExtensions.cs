// <copyright file="TypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

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

    /// <summary>
    /// Determines whether this type is a <see cref="Nullable{T}"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if the specified type is a nullable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Gets the generic type argument of the nullable.
    /// </summary>
    /// <param name="type">The nullable type.</param>
    /// <returns>The generic type argument of the nullable.</returns>
    public static Type GetTypeOfNullable(this Type type) => type.IsNullable() ? type.GetGenericArguments().First() : type;
}