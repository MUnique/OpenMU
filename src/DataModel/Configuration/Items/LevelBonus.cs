// <copyright file="LevelBonus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Defines a constant bonus, depending on item level.
    /// </summary>
    public class LevelBonus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelBonus"/> class.
        /// </summary>
        public LevelBonus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelBonus"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="constantValue">The constant value.</param>
        public LevelBonus(int level, float constantValue)
        {
            this.Level = level;
            this.AdditionalValue = constantValue;
        }

        /// <summary>
        /// Gets or sets the level of the item.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the additional value element to the base value.
        /// </summary>
        [Transient]
        public ConstantElement AdditionalValueElement { get; set; }

        /// <summary>
        /// Gets or sets the additional value to the base value.
        /// </summary>
        public float AdditionalValue
        {
            get => this.AdditionalValueElement?.Value ?? 0;
            set => this.AdditionalValueElement = Equals(value, 0f) ? null : new ConstantElement(value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Level: {this.Level}: {this.AdditionalValue}";
        }
    }
}
