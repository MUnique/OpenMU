// <copyright file="IInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    /// <summary>
    /// Interface for an initializer.
    /// </summary>
    public interface IInitializer
    {
        /// <summary>
        /// Runs the initializer which creates the initial data.
        /// </summary>
        void Initialize();
    }
}
