// <copyright file="ISupportIdUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// Interface for an object which supports to set the <see cref="IIdentifiable.Id"/>.
    /// </summary>
    public interface ISupportIdUpdate : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        new ushort Id { get; set; }
    }
}