// <copyright file="MagicEffectDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Magic Effect Definition. It can be an effect from an consumed item, or by a buff.
    /// </summary>
    public class MagicEffectDefinition
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <remarks>
        /// This number is a reference for the game client.
        /// Negative numbers are for internal usage and their effects are not meant to be exposed to the game client.
        /// </remarks>
        public short Number { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sub type.
        /// Same sub type = cant stack. adding a magic effect with the same sub type will cause the existing magic effect to disappear.
        /// </summary>
        public byte SubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect change is sent to observers.
        /// </summary>
        /// <remarks>
        /// Some effects are not externally visible, but only to the player himself.
        /// </remarks>
        public bool InformObservers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect gets stopped by a death of the player.
        /// </summary>
        public bool StopByDeath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the duration of the effect should be sent.
        /// </summary>
        public bool SendDuration { get; set; }

        /// <summary>
        /// Gets or sets the powerUp definition which is used to create the actual power up element.
        /// </summary>
        [MemberOfAggregate]
        public virtual PowerUpDefinitionWithDuration PowerUpDefinition { get; set; }
    }
}
