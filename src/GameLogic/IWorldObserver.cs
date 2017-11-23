// <copyright file="IWorldObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Interface of an world observer.
    /// </summary>
    public interface IWorldObserver
    {
        /// <summary>
        /// Gets the world view of the observer.
        /// </summary>
        IWorldView WorldView { get; }
    }
}
