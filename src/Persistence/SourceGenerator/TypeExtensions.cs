// <copyright file="TypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator;

/// <summary>
/// Extension methods for <see cref="Type"/>s.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the printable C# full name of the type.
    /// </summary>
    /// <param name="type">The type whose name is requested.</param>
    /// <returns>The printable C# full name of the type.</returns>
    internal static string GetCSharpFullName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.FullName ?? type.Name;
        }

        return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => x.FullName).ToArray()) + ">";
    }

    /// <summary>
    /// Gets the printable C#-name of the type.
    /// </summary>
    /// <param name="type">The type whose name is requested.</param>
    /// <returns>The printable C#-name of the type.</returns>
    internal static string GetCSharpName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => x.Name).ToArray()) + ">";
    }
}