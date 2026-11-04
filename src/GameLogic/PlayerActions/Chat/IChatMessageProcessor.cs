// <copyright file="IChatMessageProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

/// <summary>
/// Interface for a chat message processor.
/// </summary>
public interface IChatMessageProcessor
{
    /// <summary>
    /// Sends a chat message from the player to other players.
    /// </summary>
    /// <param name="sender" cref="Player">The sending Player.</param>
    /// <param name="content">The chat message's content.</param>
    /// <returns>The value task with the result.</returns>
    ValueTask ProcessMessageAsync(Player sender, (string Message, string PlayerName) content);
}