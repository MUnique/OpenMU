// <copyright file="MapObject.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The map object which is used for the interop with the javascript map.
    /// </summary>
    public class MapObject
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the map identifier.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public byte X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public byte Y { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        public Direction Direction { get; set; }
    }
}