// <copyright file="IIdentifiable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;

    /// <summary>
    /// The interface for all identifiable objects of the data model.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier of this instance.
        /// </summary>
        Guid Id { get; set; }
    }
}
