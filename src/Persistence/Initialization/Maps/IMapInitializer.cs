// <copyright file="IMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initializer for a <see cref="GameMapDefinition"/>.
    /// </summary>
    internal interface IMapInitializer : IInitializer
    {
        /// <summary>
        /// Sets the <see cref="GameMapDefinition.SafezoneMap"/>.
        /// This needs to be done after the first commit, because of possible circular references.
        /// </summary>
        void SetSafezoneMap();
    }
}