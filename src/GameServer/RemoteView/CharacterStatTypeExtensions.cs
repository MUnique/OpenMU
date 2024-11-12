// <copyright file="CharacterStatTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Network.Packets;

/// <summary>
/// Extensions for <see cref="CharacterStatAttribute"/>.
/// </summary>
public static class CharacterStatTypeExtensions
{
    private static readonly Dictionary<AttributeDefinition, CharacterStatAttribute> AttributesToStatTypes = new()
    {
        { Stats.BaseAgility, CharacterStatAttribute.Agility },
        { Stats.BaseEnergy, CharacterStatAttribute.Energy },
        { Stats.BaseStrength, CharacterStatAttribute.Strength },
        { Stats.BaseVitality, CharacterStatAttribute.Vitality },
        { Stats.BaseLeadership, CharacterStatAttribute.Leadership },
    };

    private static readonly Dictionary<CharacterStatAttribute, AttributeDefinition> StatTypesToAttributes = new()
    {
        { CharacterStatAttribute.Agility, Stats.BaseAgility },
        { CharacterStatAttribute.Energy, Stats.BaseEnergy },
        { CharacterStatAttribute.Strength, Stats.BaseStrength },
        { CharacterStatAttribute.Vitality, Stats.BaseVitality },
        { CharacterStatAttribute.Leadership, Stats.BaseLeadership },
    };

    /// <summary>
    /// Gets the attribute definition of a <see cref="CharacterStatAttribute"/>.
    /// </summary>
    /// <param name="statType">Type of the stat.</param>
    /// <returns>The corresponding <see cref="AttributeDefinition"/>.</returns>
    public static AttributeDefinition GetAttributeDefinition(this CharacterStatAttribute statType)
    {
        return StatTypesToAttributes[statType];
    }

    /// <summary>
    /// Gets the <see cref="CharacterStatAttribute"/> of the specified <see cref="AttributeDefinition"/>.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The corresponding <see cref="CharacterStatAttribute"/>.</returns>
    public static CharacterStatAttribute GetStatType(this AttributeDefinition attributeDefinition)
    {
        return AttributesToStatTypes[attributeDefinition];
    }
}