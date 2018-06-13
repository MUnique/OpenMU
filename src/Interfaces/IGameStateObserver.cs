// <copyright file="IGameStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// Interface for an observer which observes the state of a game.
    /// </summary>
    public interface IGameStateObserver
    {
        /// <summary>
        /// Notifies about a changed player count.
        /// </summary>
        /// <param name="playerCount">The player count.</param>
        void PlayerCountChanged(int playerCount);
    }
}
