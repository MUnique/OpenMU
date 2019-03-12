// <copyright file="IWorldObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Interface of an world observer.
    /// </summary>
    public interface IWorldObserver
    {
        /// <summary>
        /// Gets the view plug ins.
        /// </summary>
        /// <value>
        /// The view plug ins.
        /// </value>
        ICustomPlugInContainer<IViewPlugIn> ViewPlugIns { get; }
    }
}
