// <copyright file="ConstValueAttribute.custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using Newtonsoft.Json;

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
}