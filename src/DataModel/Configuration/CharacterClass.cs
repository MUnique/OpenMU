// <copyright file="CharacterClass.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Defines a character class.
/// </summary>
[Cloneable]
public partial class CharacterClass
{
    /// <summary>
    /// Gets or sets the id of a character class.
    /// This will be used to identify the class when getting created,
    /// and as identifier to be sent to client.
    /// </summary>
    public byte Number { get; set; }

    /// <summary>
    /// Gets or sets the name of the character class.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this character class can get created by the user.
    /// </summary>
    public bool CanGetCreated { get; set; }

    /// <summary>
    /// Gets or sets the level requirement when getting created.
    /// The level requirement must be fulfilled by another character of the same account.
    /// </summary>
    public short LevelRequirementByCreation { get; set; }

    /// <summary>
    /// Gets or sets the creation allowed flag which is sent to the client in the character list message if this character class is in its <see cref="Account.UnlockedCharacterClasses"/>.
    /// </summary>
    /// <remarks>
    /// Flag about which characters can be created with this account:
    /// 1 = Summoner
    /// 2 = Dark Lord
    /// 4 = Magic Gladiator
    /// 8 = Rage Fighter.
    /// </remarks>
    public byte CreationAllowedFlag { get; set; }

    /// <summary>
    /// Gets or sets the next generation class, to which a character can upgrade after
    /// fulfilling certain requirements, like quests.
    /// </summary>
    public virtual CharacterClass? NextGenerationClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this class is a master class and therefore can receive master experience for the master tree.
    /// </summary>
    public bool IsMasterClass { get; set; }

    /// <summary>
    /// Gets or sets the percent by which the moving level requirement for warping to other maps is reduced.
    /// </summary>
    public int LevelWarpRequirementReductionPercent { get; set; }

    /// <summary>
    /// Gets or sets the fruit calculation strategy.
    /// </summary>
    public FruitCalculationStrategy FruitCalculation { get; set; }

    /// <summary>
    /// Gets or sets the stat attributes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<StatAttributeDefinition> StatAttributes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the attribute combinations.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeRelationship> AttributeCombinations { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the base attribute values.
    /// For example the amount of health a character got without any added stat point.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ConstValueAttribute> BaseAttributeValues { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the home map.
    /// </summary>
    public virtual GameMapDefinition? HomeMap { get; set; }

    /// <summary>
    /// Gets or sets the combo definition for this class and all
    /// following <see cref="NextGenerationClass"/>es without an explicit combo definition.
    /// </summary>
    [MemberOfAggregate]
    public virtual SkillComboDefinition? ComboDefinition { get; set; }

    /// <summary>
    /// Gets StatAttributeDefinition corresponding to AttributeDefinition.
    /// </summary>
    /// <param name="attributeDefinition">The attribute.</param>
    /// <returns>The corresponding StatAttributeDefinition.</returns>
    public StatAttributeDefinition? GetStatAttribute(AttributeDefinition attributeDefinition)
    {
        return this.StatAttributes.FirstOrDefault(a => a.Attribute == attributeDefinition);
    }
}