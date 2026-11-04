// <copyright file="LowLevel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for an account with low level characters.
/// </summary>
internal class LowLevel : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowLevel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    public LowLevel(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration, accountName, level)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateDarkLord()
    {
        var character = this.CreateCharacter(this.AccountName + "Dl", CharacterClassNumber.DarkLord, this.Level, 3);
        character.Inventory!.Items.Add(this.CreateSmallAxe(0));
        character.Inventory.Items.Add(this.CreateArmorItem(52, 5, 8)); // Leather Armor
        character.Inventory.Items.Add(this.CreateArmorItem(49, 5, 9)); // Leather Pants
        character.Inventory.Items.Add(this.CreateArmorItem(63, 5, 10, Stats.DamageReflection)); // Leather Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(65, 5, 11, Stats.DamageReflection)); // Leather Boots
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateKnight()
    {
        var character = this.CreateCharacter(this.AccountName + "Dk", CharacterClassNumber.DarkKnight, this.Level, 0);
        character.Inventory!.Items.Add(this.CreateSmallAxe(0));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 5, 8)); // Leather Armor
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 5, 7)); // Leather Helm
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 5, 9)); // Leather Pants
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 5, 10, Stats.DamageReflection)); // Leather Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 5, 11, Stats.DamageReflection)); // Leather Boots
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateElf()
    {
        var character = this.CreateCharacter(this.AccountName + "Elf", CharacterClassNumber.FairyElf, this.Level, 2);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 20;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 30;

        character.Inventory!.Items.Add(this.CreateShortBow(1));
        character.Inventory.Items.Add(this.CreateArrows(0));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 10, 8)); // Vine Armor
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 10, 7)); // Vine Helm
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 10, 9)); // Vine Pants
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 10, 10)); // Vine Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 10, 11)); // Vine Boots

        character.Inventory.Items.Add(this.CreateOrb(67, 8)); // Healing Orb
        character.Inventory.Items.Add(this.CreateOrb(75, 9)); // Defense Orb
        character.Inventory.Items.Add(this.CreateOrb(68, 10)); // Damage Orb
        this.AddElfItems(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateWizard()
    {
        var character = this.CreateCharacter(this.AccountName + "Dw", CharacterClassNumber.DarkWizard, this.Level, 1);
        character.Inventory!.Items.Add(this.CreateSkullStaff(0));
        character.Inventory.Items.Add(this.CreateArmorItem(52, 2, 8)); // Pad Armor
        character.Inventory.Items.Add(this.CreateArmorItem(47, 2, 7)); // Pad Helm
        character.Inventory.Items.Add(this.CreateArmorItem(49, 2, 9)); // Pad Pants
        character.Inventory.Items.Add(this.CreateArmorItem(63, 2, 10)); // Pad Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(65, 2, 11)); // Pad Boots
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    private Item CreateSkullStaff(byte itemSlot)
    {
        var skullStaff = this.Context.CreateNew<Item>();
        skullStaff.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 5 && def.Number == 0); // skull staff
        skullStaff.Durability = skullStaff.Definition?.Durability ?? 0;
        skullStaff.ItemSlot = itemSlot;
        return skullStaff;
    }

    private Item CreateSmallAxe(byte itemSlot)
    {
        var smallAxe = this.Context.CreateNew<Item>();
        smallAxe.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 1 && def.Number == 0); // small axe
        smallAxe.Durability = smallAxe.Definition?.Durability ?? 0;
        smallAxe.ItemSlot = itemSlot;
        return smallAxe;
    }

    private Item CreateShortBow(byte itemSlot)
    {
        var shortBow = this.Context.CreateNew<Item>();
        shortBow.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 4 && def.Number == 0); // short bow
        shortBow.Durability = shortBow.Definition?.Durability ?? 0;
        shortBow.ItemSlot = itemSlot;
        return shortBow;
    }
}