// <copyright file="MiniGameState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames
{
    /// <summary>
    /// The state of a mini game.
    /// </summary>
    public enum MiniGameState
    {
        /// <summary>
        /// The state is not defined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The game is open for entrance.
        /// </summary>
        Open,

        /// <summary>
        /// The game is closed for entrance.
        /// </summary>
        Closed,

        /// <summary>
        /// The game is running and closed for entrance.
        /// </summary>
        Playing,

        /// <summary>
        /// The game has ended and waiting to be disposed.
        /// </summary>
        Ended,

        /// <summary>
        /// The game is disposed.
        /// </summary>
        Disposed,
    }
}