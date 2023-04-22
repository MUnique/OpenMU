// <copyright file="Level300.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for an account with level 300 characters.
/// </summary>
internal class Level300 : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Level300"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Level300(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "test300", 300)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.DarkLord);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 600;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 200;
        character.LevelUpPoints -= 1100; // for the added strength and agility

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 2, 12, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Great Lord Scepter+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 26, 7, Stats.MaximumHealth, 13, 4, true)); // Exc Ada Helm+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 26, 8, Stats.DamageReceiveDecrement, 13, 4, true)); // Exc Ada Armor+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 26, 9, Stats.MoneyAmountRate, 13, 4, true)); // Exc Ada Pants+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 26, 10, Stats.MaximumMana, 13, 4, true)); // Exc Ada Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 26, 11, Stats.DamageReflection, 13, 4, true)); // Exc Ada Boots+13+16+L
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 30, 13, 13)); // Cape +13
        character.Inventory.Items.Add(this.CreatePet(InventoryConstants.PetSlot, 4)); // Horse

        this.AddDarkLordItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);

        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateKnight()
    {
        var character = this.CreateKnight(CharacterClassNumber.BladeKnight);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 400;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.LevelUpPoints -= 700; // for the added strength and agility
        character.LevelUpPoints += 80; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 0, 13, 4, true, false, Stats.ExcellentDamageChance)); // Exc Kris+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 0, 5, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Blade+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 6, 8, Stats.DamageReceiveDecrement, 13, 4, true)); // Exc Scale Armor+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 6, 7, Stats.MaximumHealth, 13, 4, true)); // Exc Scale Helm+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 6, 9, Stats.MoneyAmountRate, 13, 4, true)); // Exc Scale Pants+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 6, 10, Stats.MaximumMana, 13, 4, true)); // Exc Scale Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 6, 11, Stats.DamageReflection, 13, 4, true)); // Exc Scale Boots+13+16+L
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 5, 13)); // Dragon Wings +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot));

        this.AddDarkKnightItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateElf()
    {
        var character = this.CreateElf(CharacterClassNumber.MuseElf);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 350;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 350;
        character.LevelUpPoints -= 700; // for the added strength and agility
        character.LevelUpPoints += 80; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 12, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 12, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 12, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 12, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 12, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 3, 13)); // Wings of Spirits +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlueFenrir));

        character.Inventory.Items.Add(this.CreateOrb(67, 8)); // Healing Orb
        character.Inventory.Items.Add(this.CreateOrb(75, 9)); // Defense Orb
        character.Inventory.Items.Add(this.CreateOrb(68, 10)); // Damage Orb
        this.AddElfItems(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateWizard()
    {
        var character = this.CreateWizard(CharacterClassNumber.SoulMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 800;
        character.LevelUpPoints -= 1200; // for the added strength, agility, energy
        character.LevelUpPoints += 80; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 7, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 7, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 7, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 7, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 7, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 4, 13)); // Wings of Soul +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlackFenrir));
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }
}