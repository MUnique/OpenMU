// <copyright file="IMapController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Controller which contains the logic for the shown game map.
    /// Extends <see cref="IAsyncDisposable"/> which needs to be used when the user interface removes the container element.
    /// </summary>
    public interface IMapController : IAsyncDisposable
    {
        /// <summary>
        /// Occurs when <see cref="Objects"/> changed.
        /// </summary>
        event EventHandler ObjectsChanged;

        /// <summary>
        /// Gets the objects which are on the map.
        /// </summary>
        IDictionary<int, ILocateable> Objects { get; }
    }
}