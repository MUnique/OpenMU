// <copyright file="BotChatComposer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Text;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

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
    /// <param name="timeOfDay">The current time of day; lines of another time of day are left out.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <param name="previous">The line said last time to the same player, which is left out when there are others.</param>
    /// <returns>The picked line, or <c>null</c> when the list has no usable line.</returns>
    internal static BotChatLine? PickLine(IEnumerable<BotChatLine> lines, BotChatTimeOfDay timeOfDay, Func<int, int> pickIndex, object? previous = null)
    {
        var candidates = WithoutPrevious(
            lines.Where(line => !string.IsNullOrWhiteSpace(line.Text.ValueInNeutralLanguage) && FitsTimeOfDay(line.When, timeOfDay)).ToList(),
            previous);
        return candidates.Count == 0 ? null : candidates[pickIndex(candidates.Count)];
    }

    /// <summary>
    /// Determines whether a line or conversation of the given time of day may be said now.
    /// </summary>
    /// <param name="when">The time of day of the line.</param>
    /// <param name="now">The current time of day.</param>
    /// <returns><c>true</c>, if it may be said now.</returns>
    internal static bool FitsTimeOfDay(BotChatTimeOfDay when, BotChatTimeOfDay now)
        => when == BotChatTimeOfDay.Anytime || when == now;

    /// <summary>
    /// Picks the reply to a player's message: a random one of the replies whose keywords match the
    /// message best, or - when no keyword matches - one of the replies without keywords.
    /// </summary>
    /// <param name="replies">The configured replies.</param>
    /// <param name="message">The player's message.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <param name="previous">The reply given last time to the same player, which is left out when there are others.</param>
    /// <param name="requireKeyword">If set, a message without a matching keyword gets no reply, not even a fallback.</param>
    /// <returns>The picked reply, or <c>null</c> when there is no usable reply.</returns>
    /// <remarks>
    /// The best match is the longest matching keyword, so "hi, how are you?" is answered by the
    /// "how are you" replies rather than by the greetings matching "hi".
    /// </remarks>
    internal static BotChatReply? PickWhisperReply(IEnumerable<BotChatReply> replies, string message, Func<int, int> pickIndex, object? previous = null, bool requireKeyword = false)
    {
        var usable = replies
            .Where(reply => !string.IsNullOrWhiteSpace(reply.Text.ValueInNeutralLanguage))
            .ToList();
        var normalizedMessage = $" {Normalize(message)} ";
        var scored = usable
            .Select(reply => (Reply: reply, Score: GetKeywords(reply.Keywords).Where(k => normalizedMessage.Contains($" {k} ", StringComparison.Ordinal)).Select(k => k.Length).DefaultIfEmpty(0).Max()))
            .ToList();
        var bestScore = scored.Count == 0 ? 0 : scored.Max(s => s.Score);
        if (bestScore == 0 && requireKeyword)
        {
            return null;
        }

        var candidates = WithoutPrevious(
            bestScore > 0
                ? scored.Where(s => s.Score == bestScore).Select(s => s.Reply).ToList()
                : usable.Where(reply => !GetKeywords(reply.Keywords).Any()).ToList(),
            previous);
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
    /// Formats a place the way players write it in the chat, e.g. "Devias 215, 40", so a player can
    /// find it with the coordinates of the mini map.
    /// </summary>
    /// <param name="mapName">The (translated) name of the map.</param>
    /// <param name="position">The coordinates on the map; <c>null</c> for the map alone.</param>
    /// <returns>The formatted place.</returns>
    internal static string FormatLocation(string? mapName, Point? position)
        => position is { } point
            ? string.Create(CultureInfo.InvariantCulture, $"{mapName} {point.X}, {point.Y}")
            : mapName ?? string.Empty;

    /// <summary>
    /// Formats an item the way players name it in the chat, e.g. "Wings of Soul +9".
    /// </summary>
    /// <param name="itemName">The (translated) name of the item.</param>
    /// <param name="level">The item level; +0 isn't written.</param>
    /// <returns>The formatted item.</returns>
    internal static string FormatItem(string? itemName, int level)
        => level > 0
            ? string.Create(CultureInfo.InvariantCulture, $"{itemName} +{level}")
            : itemName ?? string.Empty;

    /// <summary>
    /// Recognizes the language of a player's message by its typical words and letters: the language
    /// with the most hits wins, a tie or no hit at all leaves it open.
    /// </summary>
    /// <param name="message">The player's message.</param>
    /// <param name="hints">The words and letters of the languages.</param>
    /// <returns>The language code, or <c>null</c> when the message doesn't tell.</returns>
    internal static string? DetectLanguage(string message, IEnumerable<BotChatLanguageHint> hints)
    {
        var normalizedMessage = $" {Normalize(message)} ";
        var lowerMessage = message.ToLowerInvariant();
        var scores = hints
            .Where(hint => !string.IsNullOrWhiteSpace(hint.Language))
            .Select(hint => (hint.Language, Score:
                GetKeywords(hint.Words).Count(word => normalizedMessage.Contains($" {word} ", StringComparison.Ordinal))
                + (lowerMessage.Any(c => (hint.Letters ?? string.Empty).Contains(c, StringComparison.Ordinal)) ? 1 : 0)))
            .Where(s => s.Score > 0)
            .OrderByDescending(s => s.Score)
            .ToList();
        return scores.Count == 0 || (scores.Count > 1 && scores[0].Score == scores[1].Score)
            ? null
            : scores[0].Language.Trim();
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

    /// <summary>
    /// Determines whether the message contains one of the comma-separated keywords as whole words,
    /// ignoring case and punctuation.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="keywords">The comma-separated keywords, e.g. <c>party, pt</c>.</param>
    /// <returns><c>true</c>, if a keyword is in the message.</returns>
    internal static bool ContainsAnyKeyword(string message, string? keywords)
    {
        var normalizedMessage = $" {Normalize(message)} ";
        return GetKeywords(keywords).Any(keyword => normalizedMessage.Contains($" {keyword} ", StringComparison.Ordinal));
    }

    /// <summary>
    /// Reads a meeting place from a player's message like "devias 215 40" or "devias3 227 227": the map
    /// or the entry of the warp list whose name (in any of its translations) is in the message, and the
    /// first two numbers after removing the name as the coordinates.
    /// </summary>
    /// <param name="message">The player's message.</param>
    /// <param name="maps">The maps of the game.</param>
    /// <param name="warps">The warp list of the game - "devias3" or "losttower7" name a warp, not a map.</param>
    /// <returns>
    /// The map, the warp if the message names one, and the coordinates if the message names them;
    /// <c>null</c> when no map is named.
    /// </returns>
    /// <remarks>
    /// The longest matching name wins, so "lost tower" is not taken for a map called "tower"; between a
    /// map and a warp of the same name ("Devias"), the map. The name is removed before the numbers are
    /// read, so a map like "Kalima 1" keeps its digit.
    /// </remarks>
    internal static (GameMapDefinition Map, WarpInfo? Warp, Point? Position)? FindPlace(string message, IEnumerable<GameMapDefinition> maps, IEnumerable<WarpInfo> warps)
    {
        var normalizedMessage = $" {Normalize(message)} ";
        var mapNames = maps.SelectMany(map => GetNames(map.Name).Select(name => (Map: map, Warp: (WarpInfo?)null, Name: name)));
        var warpNames = warps
            .Where(warp => warp.Gate?.Map is not null)
            .SelectMany(warp => GetNames(warp.Name).Select(name => (Map: warp.Gate!.Map!, Warp: (WarpInfo?)warp, Name: name)));
        var best = mapNames.Concat(warpNames)
            .Where(candidate => normalizedMessage.Contains($" {candidate.Name} ", StringComparison.Ordinal))
            .OrderByDescending(candidate => candidate.Name.Length)
            .ThenBy(candidate => candidate.Warp is null ? 0 : 1)
            .FirstOrDefault();
        if (best.Map is null)
        {
            return null;
        }

        var rest = normalizedMessage.Replace($" {best.Name} ", " ", StringComparison.Ordinal);
        var numbers = rest
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => byte.TryParse(word, NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? (byte?)number : null)
            .Where(number => number is not null)
            .Take(2)
            .ToList();
        Point? position = numbers.Count == 2 ? new Point(numbers[0]!.Value, numbers[1]!.Value) : null;
        return (best.Map, best.Warp, position);

        static IEnumerable<string> GetNames(LocalizedString name)
            => name.GetTranslations()
                .Select(translation => translation.Value)
                .Append(name.ValueInNeutralLanguage)
                .Select(Normalize)
                .Where(normalized => normalized.Length > 0)
                .Distinct();
    }

    /// <summary>
    /// Leaves out the previous pick, unless it's the only candidate - a player who asks twice must
    /// not get the very same answer twice, but an answer is better than none.
    /// </summary>
    private static List<T> WithoutPrevious<T>(List<T> candidates, object? previous)
        where T : class
    {
        if (previous is null || candidates.Count < 2)
        {
            return candidates;
        }

        candidates.RemoveAll(candidate => ReferenceEquals(candidate, previous));
        return candidates;
    }

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
