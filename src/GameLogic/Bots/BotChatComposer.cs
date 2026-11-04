// <copyright file="BotChatComposer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Text;

/// <summary>
/// Chooses the line a bot says and renders it for one receiver: the translation of the receiver's
/// language, with the placeholders filled in. Pure logic - exposed for unit tests.
/// </summary>
internal static class BotChatComposer
{
    /// <summary>
    /// Picks a random line of the situation.
    /// </summary>
    /// <param name="lines">All configured lines.</param>
    /// <param name="situation">The situation.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <returns>The picked line, or <c>null</c> when the situation has no usable line.</returns>
    internal static BotChatLine? PickLine(IEnumerable<BotChatLine> lines, BotChatSituation situation, Func<int, int> pickIndex)
    {
        var candidates = lines
            .Where(line => line.Situation == situation && !string.IsNullOrWhiteSpace(line.Text.ValueInNeutralLanguage))
            .ToList();
        return candidates.Count == 0 ? null : candidates[pickIndex(candidates.Count)];
    }

    /// <summary>
    /// Renders the line for a receiver of the given culture.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="culture">The culture of the receiving player.</param>
    /// <param name="values">The placeholder values by name (without braces), each resolved for the culture.</param>
    /// <returns>The text to send.</returns>
    /// <remarks>
    /// Unknown placeholders are left as they are, so a typo in a configured line is visible in the
    /// game instead of silently swallowing text.
    /// </remarks>
    internal static string Render(BotChatLine line, CultureInfo culture, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        var template = line.Text.GetTranslation(culture) ?? string.Empty;
        if (!template.Contains('{', StringComparison.Ordinal))
        {
            return template;
        }

        var result = new StringBuilder(template);
        foreach (var (name, value) in values)
        {
            var placeholder = "{" + name + "}";
            if (template.Contains(placeholder, StringComparison.Ordinal))
            {
                result.Replace(placeholder, value(culture));
            }
        }

        return result.ToString();
    }
}
