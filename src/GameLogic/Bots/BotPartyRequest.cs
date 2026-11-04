// <copyright file="BotPartyRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// A player's search for party members, read from a <c>/post</c> like "party 300-350 bk dw":
/// which levels and - optionally - which character classes the player looks for.
/// </summary>
/// <param name="MinimumLevel">The lowest wanted level.</param>
/// <param name="MaximumLevel">The highest wanted level; <see cref="int.MaxValue"/> when the post names a minimum only.</param>
/// <param name="ClassNumbers">
/// The numbers of the wanted character classes, as configured in the
/// <see cref="BotChatConfiguration.PartyClassAliases"/>; empty when the post names no class.
/// </param>
internal sealed partial record BotPartyRequest(int MinimumLevel, int MaximumLevel, IReadOnlyCollection<byte> ClassNumbers)
{
    /// <summary>
    /// Reads a party search from a posted message. The message must contain one of the keywords and a
    /// level: "300-350" (a range), "300+", "min 300" or a single "300" (a minimum).
    /// </summary>
    /// <param name="message">The posted message.</param>
    /// <param name="keywords">The comma-separated words which mark a party search, e.g. <c>party, pt</c>.</param>
    /// <param name="classAliases">The words for the character classes.</param>
    /// <returns>The party search, or <c>null</c> when the message is no party search.</returns>
    /// <remarks>
    /// A post without a level is no party search: "thx for the party" must not call the bots.
    /// </remarks>
    internal static BotPartyRequest? TryParse(string message, string keywords, IEnumerable<BotClassAlias> classAliases)
    {
        if (!BotChatComposer.ContainsAnyKeyword(message, keywords)
            || ParseLevels(message.ToLowerInvariant()) is not { } levels)
        {
            return null;
        }

        var classNumbers = classAliases
            .Where(alias => BotChatComposer.ContainsAnyKeyword(message, alias.Keywords))
            .Select(alias => alias.ClassNumber)
            .Distinct()
            .ToList();
        return new BotPartyRequest(levels.Minimum, levels.Maximum, classNumbers);
    }

    /// <summary>
    /// Determines whether the level is wanted.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns><c>true</c>, if the level is within the wanted range.</returns>
    internal bool IsWantedLevel(int level) => level >= this.MinimumLevel && level <= this.MaximumLevel;

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
