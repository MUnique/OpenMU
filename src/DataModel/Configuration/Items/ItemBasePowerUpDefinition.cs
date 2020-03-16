// <copyright file="ItemBasePowerUpDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Defines an item base power up definition.
    /// </summary>
    public class ItemBasePowerUpDefinition
    {
        /// <summary>
        /// Gets or sets the target attribute.
        /// </summary>
        public virtual AttributeDefinition TargetAttribute { get; set; }

        /// <summary>
        /// Gets or sets the base value.
        /// </summary>
        [Transient]
        public ConstantElement BaseValueElement { get; set; }

        /// <summary>
        /// Gets or sets the bonus per level.
        /// </summary>
        [MemberOfAggregate]
        public virtual ICollection<LevelBonus> BonusPerLevel { get; protected set; }

        /// <summary>
        /// Gets or sets the additional value to the base value.
        /// </summary>
        public float BaseValue
        {
            get
            {
                return this.BaseValueElement?.Value ?? 0;
            }

            set
            {
                this.BaseValueElement = new ConstantElement(value);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.BaseValue} {this.TargetAttribute}";
        }
    }
}
