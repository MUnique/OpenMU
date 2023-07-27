// <copyright file="QuestBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Base initializer for an account to test quests.
/// </summary>
internal class QuestBase : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    protected QuestBase(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration, accountName, level)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateCharacter(string name, CharacterClassNumber characterClass, int level, byte slot)
    {
        var character = base.CreateCharacter(name, characterClass, level, slot);
        character.CurrentMap = this.GameConfiguration.Maps.First(map => map.Number == Devias.Number);
        character.PositionX = 184;
        character.PositionY = 31;
        character.Inventory!.Money = 100000000;
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.DarkLord);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 200;
        character.Attributes.First(a => a.Definition == Stats.BaseVitality).Value += 100;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 100;
        character.LevelUpPoints -= 700;
        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 0, 13, 4, true, false, Stats.ExcellentDamageChance)); // Exc Kris+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 5, 8, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Armor
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 5, 7, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Helm
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 5, 9, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Pants
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 5, 10, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 5, 11, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Boots

        this.AddDarkLordItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);

        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateKnight()
    {
        var character = this.CreateKnight(CharacterClassNumber.DarkKnight);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 200;
        character.Attributes.First(a => a.Definition == Stats.BaseVitality).Value += 100;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 100;
        character.LevelUpPoints -= 700;
        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 0, 13, 4, true, false, Stats.ExcellentDamageChance)); // Exc Kris+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 1, 3, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Tomahawk+13+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 5, 8, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Armor
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 5, 7, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Helm
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 5, 9, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Pants
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 5, 10, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Gloves
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 5, 11, Stats.DamageReceiveDecrement, 13, 4, true)); // Leather Boots
        character.Inventory.Items.Add(this.CreateJewel(47, Items.Quest.ScrollOfEmperorNumber));
        character.Inventory.Items.Add(this.CreateJewel(48, Items.Quest.BrokenSwordNumber));
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateWizard()
    {
        var character = this.CreateWizard(CharacterClassNumber.DarkWizard);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 200;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 700;
        character.LevelUpPoints -= 1200; // for the added strength, agility, energy

        character.Inventory!.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 7, 8, null, 13, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 7, 7, null, 13, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 7, 9, null, 13, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 7, 10, null, 13, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 7, 11, null, 13, 4, true));

        character.Inventory.Items.Add(this.CreateJewel(47, Items.Quest.ScrollOfEmperorNumber));
        character.Inventory.Items.Add(this.CreateJewel(48, Items.Quest.SoulShardOfWizardNumber));

        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateElf()
    {
        var character = this.CreateElf(CharacterClassNumber.FairyElf);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 200;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 600;
        character.LevelUpPoints -= 800; // for the added strength and agility

        character.Inventory!.Items.Add(this.CreateWeapon(1, (int)ItemGroups.Bows, 6, 13, 4, true, true));
        character.Inventory.Items.Add(this.CreateArrows(0));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 12, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 12, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 12, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.GlovesSlot, 12, 10, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 12, 11, null, 15, 4, true));

        character.Inventory.Items.Add(this.CreateJewel(47, Items.Quest.ScrollOfEmperorNumber));
        character.Inventory.Items.Add(this.CreateJewel(48, Items.Quest.TearOfElfNumber));
        character.Inventory.Items.Add(this.CreateOrb(68, 8)); // Healing Orb
        character.Inventory.Items.Add(this.CreateOrb(69, 9)); // Defense Orb
        character.Inventory.Items.Add(this.CreateOrb(70, 10)); // Damage Orb
        this.AddElfItems(character.Inventory);
        return character;
    }
}