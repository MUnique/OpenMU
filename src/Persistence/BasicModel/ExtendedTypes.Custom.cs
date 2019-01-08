// <copyright file="ExtendedTypes.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using Newtonsoft.Json;

    /// <summary>
    /// A plain implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType"/>.
    /// </summary>
    public partial class ItemSlotType
    {
        /// <inheritdoc />
        public override ICollection<int> ItemSlots => base.ItemSlots ?? (base.ItemSlots = new List<int>());
    }

    /// <summary>
    /// A plain implementation of <see cref="MUnique.OpenMU.AttributeSystem.ConstValueAttribute"/>.
    /// </summary>
    public partial class ConstValueAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstValueAttribute"/> class.
        /// </summary>
        public ConstValueAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <remarks>
        /// Required, because we need a setter for the json deserialization.
        /// Because we just can't add a setter to an existing property, we have to add a new one.
        /// </remarks>
        [JsonProperty(nameof(Value))]
        public float RawValue
        {
            get => base.Value;
            set => base.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <remarks>
        /// Reintroduced to mark it with the <see cref="JsonIgnoreAttribute"/>.
        /// </remarks>
        [JsonIgnore]
        public new float Value => base.Value;
    }

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
            get => this.ConstantValue.Value;
            set => this.ConstantValue.Value = value;
        }

        /// <summary>
        /// Gets or sets the type of the aggregate.
        /// </summary>
        public AggregateType AggregateType
        {
            get => this.ConstantValue.AggregateType;
            set => this.ConstantValue.AggregateType = value;
        }
    }

    /// <summary>
    /// Extended <see cref="Item"/> to implement <see cref="CloneItemOptionLink"/>.
    /// </summary>
    public partial class Item
    {
        /// <summary>
        /// Clones the item option link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>The cloned item option link.</returns>
        protected override DataModel.Entities.ItemOptionLink CloneItemOptionLink(DataModel.Entities.ItemOptionLink link)
        {
            var persistentLink = new ItemOptionLink();
            persistentLink.AssignValues(link);
            return persistentLink;
        }
    }
}
