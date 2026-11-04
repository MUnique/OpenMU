// <copyright file="CaptionHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Helper class to generate captions for types.
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
            .Replace(" Definitions", "s")
            .Replace(" Definition", "s");
    }

    /// <summary>
    /// Gets a nice caption for types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetTypeCaption(ClassDeclarationSyntax type)
    {
        return SeparateWords(type.Identifier.Text);
    }

    /// <summary>
    /// Gets a pluralized caption for a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetPluralizedTypeCaption(ClassDeclarationSyntax type)
    {
        var result = GetTypeCaption(type);
        result = result
            .Replace(" Definitions", "s")
            .Replace(" Definition", "s");
        if (!result.EndsWith("s"))
        {
            result += "s";
        }

        return result;
    }
}