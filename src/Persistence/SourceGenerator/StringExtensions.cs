// <copyright file="StringExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts the name to camel case.
    /// </summary>
    /// <param name="name">The name which should be converted.</param>
    /// <returns>The converted name in camel case.</returns>
    internal static string ToCamelCase(this string name)
    {
        return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
    }
}