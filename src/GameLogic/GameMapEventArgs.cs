// <copyright file="GameMapEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// Event args for all kind of events in a map concerning <see cref="ILocateable"/> objects.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class GameMapEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapEventArgs"/> class.
        /// </summary>
        /// <param name="gameMap">The game map.</param>
        /// <param name="locatable">The locatable object.</param>
        public GameMapEventArgs(GameMap gameMap, ILocateable locatable)
        {
            this.Map = gameMap;
            this.Object = locatable;
        }

        /// <summary>
        /// Gets the map (sender) of this event.
        /// </summary>
        public GameMap Map { get; }

        /// <summary>
        /// Gets the object which is concerned by the event.
        /// </summary>
        public ILocateable Object { get; }
    }
}