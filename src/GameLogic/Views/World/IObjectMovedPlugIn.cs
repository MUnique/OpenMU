// <copyright file="IObjectMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    /// <summary>
    /// Interface of a view whose implementation informs about moved objects.
    /// </summary>
    public interface IObjectMovedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// An object moved on the map.
        /// </summary>
        /// <param name="movedObject">The moved object.</param>
        /// <param name="moveType">Type of the move.</param>
        void ObjectMoved(ILocateable movedObject, MoveType moveType);
    }
}