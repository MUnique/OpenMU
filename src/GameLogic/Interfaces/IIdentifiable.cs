// <copyright file="IIdentifiable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Interfaces
{
    /// <summary>
    /// Interface for a identifiable object.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        ushort Id { get; }
    }
}
