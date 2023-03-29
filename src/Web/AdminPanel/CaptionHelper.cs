// <copyright file="CaptionHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// Helper class for captions.
/// </summary>
public static class CaptionHelper
{
    private static readonly Regex WordSeparatorRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    /// <summary>
    /// Separates the words by a space. Words are detected by upper case letters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The separated words.</returns>
    public static string SeparateWords(string input)
    {
        return WordSeparatorRegex.Replace(input, "$1 $2")
            .Replace(" Definition", "s");
    }

    /// <summary>
    /// Gets a nice caption for types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetTypeCaption(Type type)
    {
        if (type.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute
            && displayAttribute.GetName() is { } displayName)
        {
            return displayName;
        }

        return SeparateWords(type.Name);
    }

    /// <summary>
    /// Gets a pluralized caption for a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetPluralizedTypeCaption(Type type)
    {
        var result = GetTypeCaption(type);
        return result.Replace(" Definition", "s", StringComparison.InvariantCulture);
    }

    /// <summary>
    /// Gets a description for types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A description of the type.</returns>
    public static string GetTypeDescription(Type type)
    {
        if (type.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute
            && displayAttribute.GetDescription() is { } description)
        {
            return description;
        }

        return string.Empty;
    }
}
