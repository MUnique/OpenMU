// <copyright file="PowerUpDefinitionValue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Attributes
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// The power up definition value which can consist of a constant value and several related values which are all added together to get the result.
    /// </summary>
    public class PowerUpDefinitionValue
    {
        /// <summary>
        /// Gets or sets the constant value part of the value.
        /// </summary>
        [MemberOfAggregate]
        [Browsable(false)]
        public SimpleElement ConstantValue { get; protected set; }

        /// <summary>
        /// Gets or sets the related values.
        /// </summary>
        [MemberOfAggregate]
        public virtual ICollection<AttributeRelationship> RelatedValues { get; protected set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.ConstantValue?.Value ?? 0} + {string.Join(" + ", this.RelatedValues.Select(v => $"({v})"))}";
        }
    }
}
