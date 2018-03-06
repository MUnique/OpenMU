// <copyright file="AppearanceData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The appearance data of an character.
    /// </summary>
    public class AppearanceData : IAppearanceData
    {
        /// <summary>
        /// Gets or sets the character class.
        /// </summary>
        public virtual CharacterClass CharacterClass { get; set; }

        /// <summary>
        /// Gets or sets the equipped items.
        /// </summary>
        public virtual ICollection<ItemAppearance> EquippedItems { get; protected set; }

        /// <inheritdoc />
        IEnumerable<ItemAppearance> IAppearanceData.EquippedItems => this.EquippedItems;
    }
}
