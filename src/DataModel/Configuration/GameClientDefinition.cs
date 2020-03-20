// <copyright file="GameClientDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using MUnique.OpenMU.DataModel.Composition;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// Defines a game client.
    /// </summary>
    [AggregateRoot]
    public class GameClientDefinition : IGameClientVersion
    {
        /// <summary>
        /// Gets or sets the season.
        /// </summary>
        public byte Season { get; set; }

        /// <summary>
        /// Gets or sets the episode.
        /// </summary>
        public byte Episode { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public ClientLanguage Language { get; set; }

        /// <summary>
        /// Gets or sets the version which is defined in the client binaries.
        /// </summary>
        public byte[] Version { get; set; }

        /// <summary>
        /// Gets or sets the serial which is defined in the client binaries.
        /// </summary>
        public byte[] Serial { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}