// <copyright file="GameMaster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializes a game master account.
/// </summary>
internal class GameMaster : Level400
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMaster"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameMaster(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "testgm", 400)
    {
        this.AddAllSkills = true;
    }

    /// <inheritdoc />
    protected override Account CreateAccount()
    {
        var account = base.CreateAccount();
        account.State = AccountState.GameMaster;
        account.Characters.ForEach(c =>
        {
            c.CharacterStatus = CharacterStatus.GameMaster;
            c.LevelUpPoints = 20000;
        });
        return account;
    }

    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.LordEmperor);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value = 10000;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseVitality).Value = 2000;
        character.Attributes.First(a => a.Definition == Stats.BaseLeadership).Value = 2000;
        character.LevelUpPoints = 2000;
        character.MasterLevelUpPoints = 100; // To test master skill tree

        character.Inventory!.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 2, 13, 15, 4, true, true)); // AA Scepter+15+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.ArmorSlot, ItemGroups.Armor, 26, 15, "Agnis")); // Agnis Armor+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.HelmSlot, ItemGroups.Helm, 26, 15, "Agnis")); // Agnis Helm+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PantsSlot, ItemGroups.Pants, 26, 15, "Broy")); // Broy Pants+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 26, 15, "Broy")); // Broy Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.BootsSlot, ItemGroups.Boots, 26, 15, "Broy")); // Broy Boots+13+16+L
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.PendantSlot, ItemGroups.Misc1, 25, 0, "Broy")); // Broy Pendant of Ice
        character.Inventory.Items.Add(this.CreateFullAncient(InventoryConstants.Ring1Slot, ItemGroups.Misc1, 9, 0, "Agnis")); // Agnis Ring of Poison
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.PendantSlot, 13)); // Exc Pendant of Fire
        character.Inventory.Items.Add(this.CreateFullOptionJewellery(InventoryConstants.Ring2Slot, 9)); // Exc Ring of Poison
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 40, 15)); // Cape of Emperor +15
        character.Inventory.Items.Add(this.CreateHorse(InventoryConstants.PetSlot));

        this.AddDarkLordItems(character.Inventory);

        return character;
    }
}