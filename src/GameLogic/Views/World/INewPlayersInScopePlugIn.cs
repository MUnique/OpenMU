// <copyright file="INewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface of a view whose implementation informs about players which went into the observing scope.
    /// </summary>
    public interface INewPlayersInScopePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the new players in scope.
        /// </summary>
        /// <param name="newObjects">The new objects.</param>
        void NewPlayersInScope(IEnumerable<Player> newObjects);
    }
}