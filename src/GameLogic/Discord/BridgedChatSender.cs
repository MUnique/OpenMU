// <copyright file="BridgedChatSender.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// The sender of a chat message which was written outside of the game, e.g. in Discord, and bridged into it.
/// Its name is the name of a character with a prefix, which character names can't contain,
/// so it can't be mistaken for a character.
/// </summary>
public static class BridgedChatSender
{
    /// <summary>
    /// The prefix of the names of bridged senders.
    /// </summary>
    public const char Prefix = '@';

    /// <summary>
    /// Determines whether the sender of a chat message is a bridged sender.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <returns><c>true</c>, if the sender is bridged.</returns>
    public static bool IsBridged(string sender) => sender.Length > 1 && sender[0] == Prefix;

    /// <summary>
    /// Gets the name of a bridged sender without its prefix.
    /// </summary>
    /// <param name="sender">The bridged sender.</param>
    /// <returns>The name without its prefix.</returns>
    public static string GetName(string sender) => IsBridged(sender) ? sender[1..] : sender;
}
