// <copyright file="IRotatable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Interface for a rotatable class. An instance implementing this interface can be rotated on its game map.
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        Direction Rotation { get; set; }
    }
}
