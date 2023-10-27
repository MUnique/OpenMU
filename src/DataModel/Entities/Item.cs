// <copyright file="Item.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The item.
/// </summary>
[Cloneable]
public partial class Item
{
    /// <summary>
    /// Gets or sets the item slot in the <see cref="ItemStorage"/>.
    /// </summary>
    public byte ItemSlot { get; set; }

    /// <summary>
    /// Gets or sets the item definition.
    /// </summary>
    [Required]
    public virtual ItemDefinition? Definition { get; set; }

    /// <summary>
    /// Gets or sets the currently remaining durability.
    /// </summary>
    public double Durability { get; set; }

    /// <summary>
    /// Gets or sets the level of the item.
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item instance provides the weapon skill while being equipped.
    /// </summary>
    public bool HasSkill { get; set; }

    /// <summary>
    /// Gets or sets the item options.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemOptionLink> ItemOptions { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the applied item set groups (Ancient Set).
    /// </summary>
    public virtual ICollection<ItemOfItemSet> ItemSetGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the socket count. This limits the amount of socket options in the <see cref="ItemOptions"/>.
    /// </summary>
    public int SocketCount { get; set; }

    /// <summary>
    /// Gets or sets the price which was set by the player for his personal store.
    /// </summary>
    public int? StorePrice { get; set; }

    /// <summary>
    /// Gets or sets the pet experience.
    /// Only applies, if this item is actually a trainable pet.
    /// </summary>
    public int PetExperience { get; set; }

    /// <summary>
    /// Assigns the values of another item to this item.
    /// </summary>
    /// <param name="otherItem">The other item.</param>
    public void AssignValues(Item otherItem)
    {
        this.Definition = otherItem.Definition;
        this.Durability = otherItem.Durability;
        this.Level = otherItem.Level;
        this.HasSkill = otherItem.HasSkill;
        this.SocketCount = otherItem.SocketCount;
        this.PetExperience = otherItem.PetExperience;
        if (otherItem.ItemOptions != null && otherItem.ItemOptions.Any())
        {
            this.ItemOptions.Clear();
            foreach (var option in otherItem.ItemOptions)
            {
                this.ItemOptions.Add(this.CloneItemOptionLink(option));
            }
        }

        if (otherItem.ItemSetGroups != null && otherItem.ItemSetGroups.Any())
        {
            this.ItemSetGroups.Clear();
            foreach (var setGroup in otherItem.ItemSetGroups)
            {
                this.ItemSetGroups.Add(setGroup);
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Slot ").Append(this.ItemSlot).Append(": ");

        if (this.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            stringBuilder.Append("Excellent ");
        }

        var ancientSet = this.ItemSetGroups.FirstOrDefault(s => s.AncientSetDiscriminator != 0)?.ItemSetGroup;
        if (ancientSet != null)
        {
            stringBuilder.Append(ancientSet.Name).Append(" ");
        }

        var itemName = this.Definition?.GetNameForLevel(this.Level);

        stringBuilder.Append(itemName);
        if (this.Level > 0)
        {
            stringBuilder.Append("+").Append(this.Level);
        }

        foreach (var option in this.ItemOptions
                     .Where(o => o.ItemOption?.OptionType != ItemOptionTypes.Luck)
                     .OrderBy(o => o.ItemOption?.OptionType == ItemOptionTypes.Option))
        {
            var levelOption = option.ItemOption?.LevelDependentOptions.FirstOrDefault(o => o.Level == (option.ItemOption.LevelType == LevelType.ItemLevel ? this.Level : option.Level));
            var powerUpDefinition = levelOption?.PowerUpDefinition ?? option.ItemOption?.PowerUpDefinition;
            if (powerUpDefinition is not null)
            {
                stringBuilder.Append("+").Append(powerUpDefinition);
            }
        }

        if (this.HasSkill)
        {
            stringBuilder.Append("+Skill");
        }

        if (this.ItemOptions.Any(opt => opt.ItemOption?.OptionType == ItemOptionTypes.Luck))
        {
            stringBuilder.Append("+Luck");
        }

        if (this.SocketCount > 0)
        {
            stringBuilder.Append("+").Append(this.SocketCount).Append("S");
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Clones the item option link.
    /// </summary>
    /// <param name="link">The link.</param>
    /// <returns>The cloned item option link.</returns>
    protected virtual ItemOptionLink CloneItemOptionLink(ItemOptionLink link)
    {
        return link.Clone();
    }
}