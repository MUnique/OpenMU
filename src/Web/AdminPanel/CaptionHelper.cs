// <copyright file="CaptionHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.RegularExpressions;

namespace MUnique.OpenMU.Web.AdminPanel;

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
        return WordSeparatorRegex.Replace(input, "$1 $2");
    }
}
