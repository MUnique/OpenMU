// <copyright file="Ancient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Creates an account equipped with ancient sets.
/// </summary>
internal class Ancient : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ancient"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Ancient(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "ancient", 330)
    {
    }

    /// <inheritdoc />
    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.DarkLord);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 560;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 200;
        character.LevelUpPoints -= 1060; // for the added strength and agility
        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 2, 12, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Great Lord Scepter+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 26, 13, "Agnis")); // Agnis Armor+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 26, 13, "Agnis")); // Agnis Helm+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 26, 13, "Broy")); // Broy Pants+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 26, 13, "Broy")); // Broy Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 26, 13, "Broy")); // Broy Boots+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PendantSlot, ItemGroups.Misc1, 25, 0, "Broy")); // Broy Pendant of Ice
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.Ring1Slot, ItemGroups.Misc1, 9, 0, "Agnis")); // Agnis Ring of Poison
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 30, 13, 13)); // Cape +13
        character.Inventory.Items.Add(this.CreatePet(InventoryConstants.PetSlot, 4)); // Horse

        this.AddDarkLordItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    /// <inheritdoc />
    protected override Character CreateKnight()
    {
        var character = this.CreateKnight(CharacterClassNumber.BladeKnight);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 500;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 190;
        character.LevelUpPoints -= 990; // for the added strength, agility and energy
        character.LevelUpPoints += 110; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateFullAncient(InventoryConstants.LeftHandSlot, ItemGroups.Swords, 14, 13, "Hyon")); // Hyon LS+13+16+S+L
        character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 0, 5, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Blade+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 1, 13, "Vicious")); // Vicious Dragon Armor+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 1, 13, "Vicious")); // Vicious Dragon Helm+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 1, 13, "Vicious")); // Vicious Dragon Pants+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 1, 13, "Hyon")); // Hyon Dragon Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 1, 13, "Hyon")); // Hyon Dragon Boots+13+16+L
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 5, 13)); // Dragon Wings +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot));
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.Ring1Slot, ItemGroups.Misc1, 22, 0, "Vicious")); // Vicious Ring of Earth

        this.AddDarkKnightItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc />
    protected override Character CreateElf()
    {
        var character = this.CreateElf(CharacterClassNumber.MuseElf);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 200;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 1000;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 200;
        character.LevelUpPoints -= 1400; // for the added strength, agility and energy
        character.LevelUpPoints += 110; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateArrows(0));
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.RightHandSlot, ItemGroups.Bows, 5, 13, "Gywen")); // Gywen Silver Bow+13+16+S+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 14, 13, "Aruan")); // Gywen Armor+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 14, 13, "Aruan")); // Aruan Helm+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 14, 13, "Aruan")); // Aruan Pants+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 14, 13, "Gywen")); // Gywen Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 14, 13, "Aruan")); // Aruan Boots+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PendantSlot, ItemGroups.Misc1, 28, 0, "Gywen")); // Gywen Pendant of Ability
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 3, 13)); // Wings of Spirits +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlueFenrir));

        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    /// <inheritdoc />
    protected override Character CreateWizard()
    {
        var character = this.CreateWizard(CharacterClassNumber.SoulMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 1200;
        character.LevelUpPoints -= 1600; // for the added strength, agility, energy
        character.LevelUpPoints += 110; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 5, 11, 13, 4, true, false, Stats.ExcellentDamageChance)); // Exc Staff of Kundun+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.RightHandSlot, ItemGroups.Shields, 6, 13, "Heras")); // Heras Skull Shield+13+16+S+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 3, 13, "Enis")); // Enis Legendary Armor+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 3, 13, "Enis")); // Enis Legendary Helm+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 3, 13, "Enis")); // Enis Legendary Pants+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 3, 13, "Anubis")); // Anubis Legendary Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 3, 13, "Enis")); // Enis Legendary Boots+13+16+L
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 4, 13)); // Wings of Soul +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlackFenrir));
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.Ring1Slot, ItemGroups.Misc1, 21, 0, "Anubis")); // Anubis Ring of Fire

        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }
}