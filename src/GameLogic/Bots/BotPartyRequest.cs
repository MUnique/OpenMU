// <copyright file="BotPartyRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// A player's search for party members, read from a <c>/post</c> like "party 300-350 bk dw" or
/// "party bc7": which levels or which event and - optionally - which character classes the player
/// looks for.
/// </summary>
/// <param name="MinimumLevel">The lowest wanted level; 0 when the post names an event only.</param>
/// <param name="MaximumLevel">The highest wanted level; <see cref="int.MaxValue"/> when the post names a minimum or an event only.</param>
/// <param name="ClassNumbers">
/// The numbers of the wanted character classes, as configured in the
/// <see cref="BotChatConfiguration.PartyClassAliases"/>; empty when the post names no class.
/// </param>
/// <param name="MiniGame">
/// The event named in the post, e.g. the Blood Castle 7 for "bc7"; only bots which may enter it answer.
/// <c>null</c> when the post names no event.
/// </param>
internal sealed partial record BotPartyRequest(int MinimumLevel, int MaximumLevel, IReadOnlyCollection<byte> ClassNumbers, MiniGameDefinition? MiniGame = null)
{
    /// <summary>
    /// Reads a party search from a posted message. The message must contain one of the keywords and a
    /// level: "300-350" (a range), "300+", "min 300" or a single "300" (a minimum) - or an event with its
    /// level, like "bc7", "ds 5" or "chaos castle 3".
    /// </summary>
    /// <param name="message">The posted message.</param>
    /// <param name="keywords">The comma-separated words which mark a party search, e.g. <c>party, pt</c>.</param>
    /// <param name="classAliases">The words for the character classes.</param>
    /// <param name="eventKeywords">The comma-separated words for each event type, e.g. <c>bc, blood castle</c>.</param>
    /// <param name="miniGames">The events of the game, from which a named event takes its entry rules.</param>
    /// <returns>The party search, or <c>null</c> when the message is no party search.</returns>
    /// <remarks>
    /// A post without a level or event is no party search: "thx for the party" must not call the bots.
    /// The event's level brackets aren't copied here: the bots are checked against the event's own
    /// entry rules, which differ between the classes and can be changed by the server owner.
    /// An event level which doesn't exist ("bc9") makes no search, and the event's digit is never
    /// taken for a character level - "party bc 7" doesn't call every bot above level 7.
    /// </remarks>
    internal static BotPartyRequest? TryParse(
        string message,
        string keywords,
        IEnumerable<BotClassAlias> classAliases,
        IEnumerable<(MiniGameType Type, string Keywords)> eventKeywords,
        IEnumerable<MiniGameDefinition> miniGames)
    {
        if (!BotChatComposer.ContainsAnyKeyword(message, keywords))
        {
            return null;
        }

        var text = message.ToLowerInvariant();
        MiniGameDefinition? miniGame = null;
        if (FindEvent(text, eventKeywords) is { } namedEvent)
        {
            miniGame = miniGames.FirstOrDefault(definition => definition.Type == namedEvent.Type && definition.GameLevel == namedEvent.Level);
            if (miniGame is null)
            {
                return null;
            }

            text = text.Remove(namedEvent.Index, namedEvent.Length).Insert(namedEvent.Index, " ");
        }

        var levels = ParseLevels(text);
        if (levels is null && miniGame is null)
        {
            return null;
        }

        var classNumbers = classAliases
            .Where(alias => BotChatComposer.ContainsAnyKeyword(text, alias.Keywords))
            .Select(alias => alias.ClassNumber)
            .Distinct()
            .ToList();
        var (minimum, maximum) = levels ?? (0, int.MaxValue);
        return new BotPartyRequest(minimum, maximum, classNumbers, miniGame);
    }

    /// <summary>
    /// Determines whether the level is wanted.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns><c>true</c>, if the level is within the wanted range.</returns>
    internal bool IsWantedLevel(int level) => level >= this.MinimumLevel && level <= this.MaximumLevel;

    /// <summary>
    /// Finds the first event word followed by a level, e.g. "bc7" or "blood castle 7"; the event word
    /// must stand on its own, so "abc7" names no event.
    /// </summary>
    private static (MiniGameType Type, byte Level, int Index, int Length)? FindEvent(string message, IEnumerable<(MiniGameType Type, string Keywords)> eventKeywords)
    {
        (MiniGameType Type, byte Level, int Index, int Length)? first = null;
        foreach (var (type, keywords) in eventKeywords)
        {
            var words = (keywords ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(word => Regex.Replace(Regex.Escape(word.ToLowerInvariant()), @"(\\ )+", @"\s+"))
                .ToList();
            if (words.Count == 0)
            {
                continue;
            }

            var pattern = $@"(?<![\p{{L}}\p{{N}}])(?:{string.Join("|", words.OrderByDescending(word => word.Length))})\s*(\d{{1,2}})(?![\p{{L}}\p{{N}}])";
            var match = Regex.Match(message, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
            if (match.Success && (first is null || match.Index < first.Value.Index))
            {
                first = (type, byte.Parse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture), match.Index, match.Length);
            }
        }

        return first;
    }

    private static (int Minimum, int Maximum)? ParseLevels(string message)
    {
        if (RangeRegex().Match(message) is { Success: true } range)
        {
            var first = ParseNumber(range.Groups[1].Value);
            var second = ParseNumber(range.Groups[2].Value);
            return (Math.Min(first, second), Math.Max(first, second));
        }

        if (MinimumRegex().Match(message) is { Success: true } minimum)
        {
            return (ParseNumber(minimum.Groups[1].Success ? minimum.Groups[1].Value : minimum.Groups[2].Value), int.MaxValue);
        }

        return NumberRegex().Match(message) is { Success: true } number
            ? (ParseNumber(number.Groups[1].Value), int.MaxValue)
            : null;
    }

    private static int ParseNumber(string text) => int.Parse(text, NumberStyles.None, CultureInfo.InvariantCulture);

    /// <summary>"300-350", "300 - 350", "300~350".</summary>
    [GeneratedRegex(@"\b(\d{1,4})\s*[-~]\s*(\d{1,4})\b")]
    private static partial Regex RangeRegex();

    /// <summary>"300+" or "min 300", "min. 300".</summary>
    [GeneratedRegex(@"\b(\d{1,4})\s*\+|\bmin\.?\s*(\d{1,4})\b")]
    private static partial Regex MinimumRegex();

    /// <summary>A single number, e.g. in "party 300" or "lvl 300".</summary>
    [GeneratedRegex(@"\b(\d{1,4})\b")]
    private static partial Regex NumberRegex();
}
