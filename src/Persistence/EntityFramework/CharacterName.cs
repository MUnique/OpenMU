// <copyright file="CharacterName.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// ReSharper disable UnusedAutoPropertyAccessor.Local They're used by the entity framework core (reflection).
namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Subset of a <see cref="Character"/>, used to query character names by id and vice-versa.
    /// </summary>
    [Table(nameof(Character), Schema = "data")]
    public class CharacterName
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }
    }
}