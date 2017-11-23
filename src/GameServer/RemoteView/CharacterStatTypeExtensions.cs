// <copyright file="CharacterStatTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The type of a character stat attribute.
    /// </summary>
    public enum CharacterStatType : byte
    {
        /// <summary>
        /// The strength attribute.
        /// </summary>
        Strength = 0,

        /// <summary>
        /// The agility attribute.
        /// </summary>
        Agility = 1,

        /// <summary>
        /// The vitality attribute.
        /// </summary>
        Vitality = 2,

        /// <summary>
        /// The energy attribute.
        /// </summary>
        Energy = 3,

        /// <summary>
        /// The leadership attribute.
        /// </summary>
        Leadership = 4,
    }

    /// <summary>
    /// Extensions for character stat attributes.
    /// </summary>
    public static class CharacterStatTypeExtensions
    {
        private static readonly Dictionary<AttributeDefinition, CharacterStatType> AttributesToStatTypes = new Dictionary<AttributeDefinition, CharacterStatType>
        {
            { Stats.BaseAgility, CharacterStatType.Agility },
            { Stats.BaseEnergy, CharacterStatType.Energy },
            { Stats.BaseStrength, CharacterStatType.Strength },
            { Stats.BaseVitality, CharacterStatType.Vitality },
            { Stats.BaseLeadership, CharacterStatType.Leadership }
        };

        private static readonly Dictionary<CharacterStatType, AttributeDefinition> StatTypesToAttributes = new Dictionary<CharacterStatType, AttributeDefinition>
        {
            { CharacterStatType.Agility, Stats.BaseAgility },
            { CharacterStatType.Energy, Stats.BaseEnergy },
            { CharacterStatType.Strength, Stats.BaseStrength },
            { CharacterStatType.Vitality, Stats.BaseVitality },
            { CharacterStatType.Leadership, Stats.BaseLeadership }
        };

        /// <summary>
        /// Gets the attribute definition of a <see cref="CharacterStatType"/>.
        /// </summary>
        /// <param name="statType">Type of the stat.</param>
        /// <returns>The corrsponding <see cref="AttributeDefinition"/>.</returns>
        public static AttributeDefinition GetAttributeDefinition(this CharacterStatType statType)
        {
            return StatTypesToAttributes[statType];
        }

        /// <summary>
        /// Gets the <see cref="CharacterStatType"/> of the specified <see cref="AttributeDefinition"/>.
        /// </summary>
        /// <param name="attributeDefinition">The attribute definition.</param>
        /// <returns>The corresponding <see cref="CharacterStatType"/>.</returns>
        public static CharacterStatType GetStatType(this AttributeDefinition attributeDefinition)
        {
            return AttributesToStatTypes[attributeDefinition];
        }
    }
}