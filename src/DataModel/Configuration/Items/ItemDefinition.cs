// <copyright file="ItemDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines an item.
/// </summary>
[Cloneable]
public partial class ItemDefinition
{
    /// <summary>
    /// Gets or sets the (Sub-)Id of this item. Must be unique in an item group.
    /// </summary>
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the item slot where it can get equipped.
    /// </summary>
    public virtual ItemSlotType? ItemSlot { get; set; }

    /// <summary>
    /// Gets or sets the width of the Item.
    /// </summary>
    public byte Width { get; set; }

    /// <summary>
    /// Gets or sets the weight of the Item.
    /// </summary>
    public byte Height { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item can be dropped by monsters.
    /// </summary>
    public bool DropsFromMonsters { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance acts as ammunition for another equipped weapon.
    /// </summary>
    public bool IsAmmunition { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items of this kind are bound to character.
    /// That means, it can't be traded, moved to the vault/personal shop or picked up from the ground by other players.
    /// </summary>
    public bool IsBoundToCharacter { get; set; }

    /// <summary>
    /// Gets or sets the storage limit per character which is checked on pick-up.
    /// A value of 0 means, that there is no limit.
    /// A value 'n' above 0 means, that the inventory of the character can store
    /// at most 'n' items of this kind.
    /// </summary>
    public int StorageLimitPerCharacter { get; set; }

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item drop level, which indicates the minimum monster lvl of which this item can be dropped.
    /// </summary>
    public byte DropLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum item level.
    /// </summary>
    public byte MaximumItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum durability of this item at Level 0.
    /// </summary>
    public byte Durability { get; set; }

    /// <summary>
    /// Gets or sets the item Group (0-15). TODO: Might change item groups to classes, and replace this by it.
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Gets or sets the value which defines the worth of an item in zen currency.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the formula to calculate the required experience for a specific pet level.
    /// Only applies, if this item is actually a trainable pet.
    /// The variable for the pet level is "level".
    /// </summary>
    public string? PetExperienceFormula { get; set; }

    /// <summary>
    /// Gets or sets the effect which is applied when this item is consumed.
    /// Creating a consume handler plugin is not required when this effect definition is set.
    /// </summary>
    public virtual MagicEffectDefinition? ConsumeEffect { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of sockets an instance of this item can have.
    /// </summary>
    public int MaximumSockets { get; set; }

    /// <summary>
    /// Gets or sets the skill which this items adds to the skill list while wearing or which can be learned by consuming this item.
    /// TODO: Split these two usages into different properties?.
    /// </summary>
    public virtual Skill? Skill { get; set; }

    /// <summary>
    /// Gets or sets the character classes which are qualified to wear this Item.
    /// </summary>
    public virtual ICollection<CharacterClass> QualifiedCharacters { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the possible item set groups.
    /// </summary>
    /// <remarks>
    /// With this we can define a lot of things, for example:
    ///   - double wear bonus of single swords
    ///   - set bonus for defense rate
    ///   - set bonus for defense, if level is greater than 9
    ///   - ancient sets.
    /// </remarks>
    public virtual ICollection<ItemSetGroup> PossibleItemSetGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the possible item options.
    /// </summary>
    public virtual ICollection<ItemOptionDefinition> PossibleItemOptions { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the requirements for wearing this item.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeRequirement> Requirements { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the base PowerUps of this item, for example min/max damage for weapons.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemBasePowerUpDefinition> BasePowerUpAttributes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the drop item groups (one per possible item level), which are used to
    /// generate a new item when this item gets dropped by a player.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemDropItemGroup> DropItems { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Name} ({this.Group}, {this.Number}) [{this.Width}x{this.Height}]";
    }

    /// <summary>
    /// Gets the name for level.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <returns>The name of the item of a certain level.</returns>
    public string GetNameForLevel(byte itemLevel)
    {
        var itemName = this.Name;
        if (itemName?.Contains(';') ?? false)
        {
            var tokens = itemName.Split(';');
            if (tokens.Length > itemLevel)
            {
                itemName = tokens[itemLevel];
            }
        }

        return itemName ?? string.Empty;
    }
}