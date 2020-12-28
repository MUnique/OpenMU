// <copyright file="PowerUpDefinitionValue.custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using MUnique.OpenMU.AttributeSystem;

    /// <summary>
    /// Extended <see cref="PowerUpDefinitionValue"/> by the properties of <see cref="SimpleElement"/>, because they are in a 1:1 relationship.
    /// </summary>
    public partial class PowerUpDefinitionValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerUpDefinitionValue"/> class.
        /// </summary>
        public PowerUpDefinitionValue()
        {
            this.ConstantValue = new MUnique.OpenMU.AttributeSystem.SimpleElement();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float Value
        {
            get => this.ConstantValue!.Value;
            set => this.ConstantValue!.Value = value;
        }

        /// <summary>
        /// Gets or sets the type of the aggregate.
        /// </summary>
        public AggregateType AggregateType
        {
            get => this.ConstantValue!.AggregateType;
            set => this.ConstantValue!.AggregateType = value;
        }
    }
}