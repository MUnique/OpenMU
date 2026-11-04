// <copyright file="BotChatComposer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Text;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Chooses the line a bot says and renders it for one receiver: the translation of the receiver's
/// language, with the placeholders filled in. Pure logic - exposed for unit tests.
/// </summary>
internal static class BotChatComposer
{
    /// <summary>
    /// Picks a random usable line of the list.
    /// </summary>
    /// <param name="lines">The lines of one list, e.g. the greetings.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <returns>The picked line, or <c>null</c> when the list has no usable line.</returns>
    internal static BotChatLine? PickLine(IEnumerable<BotChatLine> lines, Func<int, int> pickIndex)
    {
        var candidates = lines
            .Where(line => !string.IsNullOrWhiteSpace(line.Text.ValueInNeutralLanguage))
            .ToList();
        return candidates.Count == 0 ? null : candidates[pickIndex(candidates.Count)];
    }

    /// <summary>
    /// Picks the reply to a player's message: a random one of the replies whose keywords match the
    /// message best, or - when no keyword matches - one of the replies without keywords.
    /// </summary>
    /// <param name="replies">The configured replies.</param>
    /// <param name="message">The player's message.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <returns>The picked reply, or <c>null</c> when there is no usable reply.</returns>
    /// <remarks>
    /// The best match is the longest matching keyword, so "hi, how are you?" is answered by the
    /// "how are you" replies rather than by the greetings matching "hi".
    /// </remarks>
    internal static BotChatReply? PickWhisperReply(IEnumerable<BotChatReply> replies, string message, Func<int, int> pickIndex)
    {
        var usable = replies
            .Where(reply => !string.IsNullOrWhiteSpace(reply.Text.ValueInNeutralLanguage))
            .ToList();
        var normalizedMessage = $" {Normalize(message)} ";
        var scored = usable
            .Select(reply => (Reply: reply, Score: GetKeywords(reply.Keywords).Where(k => normalizedMessage.Contains($" {k} ", StringComparison.Ordinal)).Select(k => k.Length).DefaultIfEmpty(0).Max()))
            .ToList();
        var bestScore = scored.Count == 0 ? 0 : scored.Max(s => s.Score);
        var candidates = bestScore > 0
            ? scored.Where(s => s.Score == bestScore).Select(s => s.Reply).ToList()
            : usable.Where(reply => !GetKeywords(reply.Keywords).Any()).ToList();
        return candidates.Count == 0 ? null : candidates[pickIndex(candidates.Count)];
    }

    /// <summary>
    /// Renders a text for a receiver of the given culture.
    /// </summary>
    /// <param name="text">The text, e.g. of a <see cref="BotChatLine"/>.</param>
    /// <param name="culture">The culture of the receiving player.</param>
    /// <param name="values">The placeholder values by name (without braces), each resolved for the culture.</param>
    /// <returns>The text to send.</returns>
    /// <remarks>
    /// Unknown placeholders are left as they are, so a typo in a configured line is visible in the
    /// game instead of silently swallowing text.
    /// </remarks>
    internal static string Render(LocalizedString text, CultureInfo culture, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        var template = text.GetTranslation(culture) ?? string.Empty;
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

    /// <summary>
    /// Determines whether the message mentions the name as a whole word, ignoring case and
    /// punctuation - "hi Serael!" mentions Serael, "Seraelion" does not.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="name">The character name.</param>
    /// <returns><c>true</c>, if the message mentions the name.</returns>
    internal static bool MentionsName(string message, string name)
        => Normalize(name) is { Length: > 0 } normalizedName
           && $" {Normalize(message)} ".Contains($" {normalizedName} ", StringComparison.Ordinal);

    private static IEnumerable<string> GetKeywords(string? keywords)
        => (keywords ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Normalize)
            .Where(keyword => keyword.Length > 0);

    /// <summary>
    /// Lower-cases the text and turns everything except letters and digits into single spaces, so
    /// "How are you?!" and "how are you" compare equal and keywords match as whole words.
    /// </summary>
    private static string Normalize(string text)
    {
        var builder = new StringBuilder(text.Length);
        var pendingSpace = false;
        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character))
            {
                if (pendingSpace && builder.Length > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(char.ToLowerInvariant(character));
                pendingSpace = false;
            }
            else
            {
                pendingSpace = true;
            }
        }

        return builder.ToString();
    }
}
