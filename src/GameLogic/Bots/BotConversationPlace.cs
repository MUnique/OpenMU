// <copyright file="BotConversationPlace.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Where two bots may have a <see cref="BotConversation"/>.
/// </summary>
/// <remarks>New values are appended: the configuration stores the place as a number.</remarks>
public enum BotConversationPlace
{
    /// <summary>
    /// In a town as well as out hunting.
    /// </summary>
    Anywhere,

    /// <summary>
    /// Only when both bots stand in a safezone - "the shop here sells potions cheap".
    /// </summary>
    Town,

    /// <summary>
    /// Only when both bots are out of the safezone, usually fighting - "watch out, behind you!".
    /// </summary>
    Hunting,
}
