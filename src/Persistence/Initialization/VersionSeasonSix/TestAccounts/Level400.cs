// <copyright file="Level400.cs" company="MUnique">
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
/// Initializer for an account with level 400 characters.
/// </summary>
internal class Level400 : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Level400"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    public Level400(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration, accountName, level)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Level400"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Level400(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "test400", 400)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.LordEmperor);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 1200;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 400;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 400;
        character.LevelUpPoints -= 2000; // for the added strength and agility
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 2, 13, 15, 4, true, true)); // AA Scepter+15+16+L

        // Sunlight Set+15+16+L:
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 33, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 33, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 33, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 33, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 33, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 13)); // Exc Pendant of Fire
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 40, 15)); // Cape of Emperor +15
        character.Inventory.Items.Add(this.CreateHorse(InventoryConstants.PetSlot));

        this.AddDarkLordItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateKnight()
    {
        var character = this.CreateKnight(CharacterClassNumber.BladeMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 1200;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 400;
        character.LevelUpPoints -= 1600; // for the added strength and agility
        character.LevelUpPoints += 180; // after level 220, one point more
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 19, 15, 4, true, true)); // AA Sword+15+16+L
        character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 0, 22, 15, 4, true, true)); // Bone Blade+15+16+L

        // Dragon Knight Set+15+16+L:
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 29, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 29, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 29, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 29, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 29, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 13)); // Exc Pendant of Fire
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 36, 15)); // Wing of Storm +15
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.GoldFenrir));

        this.AddDarkKnightItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateMagicGladiator()
    {
        var character = this.CreateMagicGladiator(CharacterClassNumber.DuelMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 1200;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 400;
        character.LevelUpPoints -= 1600; // for the added strength and agility
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 23, 15, 4, true, true)); // Explosion Blade+15+16+S+L

        // Volcano Set+15+16+L:
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 32, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 32, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 32, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 32, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 13)); // Exc Pendant of Fire
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 39, 15)); // Wing of Ruin +15
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.GoldFenrir));

        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateElf()
    {
        var character = this.CreateElf(CharacterClassNumber.HighElf);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 338;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 1457;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 105;
        character.LevelUpPoints -= 1900; // for the added strength and agility
        character.LevelUpPoints += 180; // after level 220, one point more
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, (int)ItemGroups.Bows, 20, 13, 4, true, true, Stats.ExcellentDamageChance));
        character.Inventory.Items.Add(this.CreateArrows(0));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 31, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 31, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 31, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 31, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 31, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 38, 15)); // Wing of Illusion +15
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
        var character = this.CreateWizard(CharacterClassNumber.GrandMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 1200;
        character.LevelUpPoints -= 1800; // for the added strength and agility
        character.LevelUpPoints += 180; // after level 220, one point more
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 5, 9, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc +15+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.RightHandSlot, 15, 6, null, 15, 4, true)); // +15+16+L

        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 30, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 30, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 30, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 30, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 30, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 37, 15)); // Wing of Eternal +15
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlackFenrir));
        this.AddTestJewelsAndPotions(character.Inventory);

        return character;
    }
}