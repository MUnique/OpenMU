// <copyright file="GameMaster2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializes a game master account with the other character classes.
/// </summary>
internal class GameMaster2 : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMaster2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameMaster2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "testgm2", 400)
    {
        this.AddAllSkills = true;
    }

    /// <inheritdoc />
    protected override Account CreateAccount()
    {
        var account = base.CreateAccount();
        account.State = AccountState.GameMaster;
        account.Characters.Add(this.CreateSummoner());
        account.Characters.Add(this.CreateFistMaster());
        account.Characters.ForEach(c =>
        {
            c.CharacterStatus = CharacterStatus.GameMaster;
            c.LevelUpPoints = 20000;
        });
        return account;
    }

    protected Character CreateSummoner()
    {
        var character = this.CreateCharacter(this.AccountName + "Sum", CharacterClassNumber.DimensionMaster, this.Level, 0);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseVitality).Value = 2000;
        character.LevelUpPoints = 2000;
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 5, 36, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc AA Stick+15+16+L+ExcDmg
        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 5, 23, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Book of Lagle+15+16+L+ExcDmg
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 40, 15, "Semeden")); // Sememden Armor+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 40, 15, "Semeden")); // Sememden Helm+15+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 40, 15, "Chrono")); // Chrono Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 40, 15, "Semeden")); // Sememden Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 40, 15, "Semeden")); // Sememden Boots+13+16+L
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 12)); // Exc Pendant of Lightning
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.Ring1Slot, ItemGroups.Misc1, 24, 0, "Chrono")); // Chrono Ring of Magic
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.Ring2Slot, 9)); // Ring of Poison
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 43, 15)); // Wing of Dimension+15

        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.GoldFenrir));

        this.AddTestJewelsAndPotions(character.Inventory);

        character.Inventory!.Items.Add(this.CreateWeapon(52, 5, 21, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Book of Sahamutt+15+16+L+ExcDmg
        character.Inventory!.Items.Add(this.CreateWeapon(53, 5, 22, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Book of Neil+15+16+L+ExcDmg
        return character;
    }

    private Character CreateFistMaster()
    {
        var character = this.CreateCharacter(this.AccountName + "Rf", CharacterClassNumber.FistMaster, this.Level, 1);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseVitality).Value = 2000;
        character.LevelUpPoints = 2000;
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 35, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Phoenix Soul Star+15+16+L+ExcDmg

        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.ArmorSlot, 73, 8, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.HelmSlot, 73, 7, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.PantsSlot, 73, 9, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateArmorItem(InventoryConstants.BootsSlot, 73, 11, null, 15, 4, true));
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 12)); // Exc Pendant of Lightning
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.Ring1Slot, 8)); // Ring of Ice
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.Ring2Slot, 9)); // Ring of Poison
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 50, 15)); // Cape of Overrule +15

        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.GoldFenrir));

        this.AddTestJewelsAndPotions(character.Inventory);

        character.Inventory!.Items.Add(this.CreateWeapon(52, 0, 34, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Piercing Blade Glove+15+16+L+ExcDmg
        character.Inventory!.Items.Add(this.CreateWeapon(53, 0, 33, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc Storm Hard Glove+15+16+L+ExcDmg

        return character;
    }
}