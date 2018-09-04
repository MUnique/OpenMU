// <copyright file="IDictionaryEntity.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;

    /// <summary>
    /// Interface for a dictionary entry of the dictionary adapter.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IDictionaryEntity<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        TKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value identifier.
        /// </summary>
        Guid ValueId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        TValue Value { get; set; }
    }
}