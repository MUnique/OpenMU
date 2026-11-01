// <copyright file="DiscordChatText.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Text;
using System.Text.RegularExpressions;
using MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// Converts the texts of chat messages between the game and Discord.
/// </summary>
public static partial class DiscordChatText
{
    /// <summary>
    /// The prefix of the names of Discord users in the game. Character names can't contain it,
    /// so Discord users can't pretend to be a character in the game.
    /// </summary>
    public const char DiscordSenderPrefix = BridgedChatSender.Prefix;

    /// <summary>
    /// The maximum length of a sender name in the game.
    /// </summary>
    private const int MaximumSenderLength = 10;

    /// <summary>
    /// Formats a message of the game for Discord.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="message">The message.</param>
    /// <returns>The text for Discord.</returns>
    public static string ToDiscord(string sender, string message)
    {
        return $"**{DiscordMessageFormatter.Escape(sender)}**: {DiscordMessageFormatter.Escape(message)}";
    }

    /// <summary>
    /// Gets the name of a Discord user in the game, which is the name of its character with a prefix.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <returns>The name in the game.</returns>
    public static string ToGameSender(string characterName)
    {
        var sender = DiscordSenderPrefix + characterName;
        return sender.Length > MaximumSenderLength ? sender[..MaximumSenderLength] : sender;
    }

    /// <summary>
    /// Converts a Discord message to a message for the game: Mentions are removed, custom emojis are
    /// replaced by their name, markdown and line breaks are removed, and the length is limited.
    /// </summary>
    /// <param name="text">The text of the Discord message.</param>
    /// <param name="maximumLength">The maximum length.</param>
    /// <returns>The message for the game.</returns>
    public static string ToGame(string text, int maximumLength)
    {
        text = CustomEmojiRegex().Replace(text, ":$1:");
        text = MentionRegex().Replace(text, string.Empty);
        var result = new StringBuilder(text.Length);
        foreach (var character in text)
        {
            if (char.IsControl(character) || char.IsWhiteSpace(character))
            {
                if (result.Length > 0 && result[^1] != ' ')
                {
                    result.Append(' ');
                }
            }
            else if (character is not ('*' or '_' or '~' or '`' or '|' or '\\'))
            {
                result.Append(character);
            }
        }

        var message = result.ToString().Trim();
        return message.Length > maximumLength ? message[..maximumLength].TrimEnd() : message;
    }

    [GeneratedRegex(@"<a?:(\w+):\d+>")]
    private static partial Regex CustomEmojiRegex();

    [GeneratedRegex(@"<(@[!&]?|#)\d+>|@(everyone|here)")]
    private static partial Regex MentionRegex();
}
