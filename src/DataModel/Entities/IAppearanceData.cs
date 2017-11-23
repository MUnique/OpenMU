// <copyright file="IAppearanceData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The interface for the appearance data.
    /// </summary>
    public interface IAppearanceData
    {
        /// <summary>
        /// Gets the character class.
        /// </summary>
        CharacterClass CharacterClass { get; }

        /// <summary>
        /// Gets the equipped items.
        /// </summary>
        IEnumerable<ItemAppearance> EquippedItems { get; }
    }
}
