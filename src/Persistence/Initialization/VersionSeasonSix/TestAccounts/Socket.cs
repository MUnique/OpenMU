// <copyright file="Socket.cs" company="MUnique">
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
/// Initializer for an account with socket items.
/// </summary>
internal class Socket : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Socket"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Socket(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "socket", 380)
    {
    }

    /// <inheritdoc />
    protected override Character CreateDarkLord()
    {
        var character = this.CreateDarkLord(CharacterClassNumber.DarkLord);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 635;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 200;
        character.LevelUpPoints -= 1135; // for the added strength and agility
        character.Inventory!.Items.Add(this.CreateSocketItem(InventoryConstants.LeftHandSlot, ItemGroups.Scepters, 17, (SocketSubOptionType.Lightning, 0), (SocketSubOptionType.Ice, 0), (SocketSubOptionType.Fire, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.ArmorSlot, ItemGroups.Armor, 51, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.HelmSlot, ItemGroups.Helm, 51, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.PantsSlot, ItemGroups.Pants, 51, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 51, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.BootsSlot, ItemGroups.Boots, 51, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
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

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 1170;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 190;
        character.LevelUpPoints -= 1660; // for the added strength, agility and energy
        character.LevelUpPoints += 160; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateSocketItem(InventoryConstants.LeftHandSlot, ItemGroups.Swords, 26, (SocketSubOptionType.Lightning, 0), (SocketSubOptionType.Ice, 0), (SocketSubOptionType.Fire, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.RightHandSlot, ItemGroups.Swords, 26, (SocketSubOptionType.Lightning, 0), (SocketSubOptionType.Ice, 0), (SocketSubOptionType.Fire, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.ArmorSlot, ItemGroups.Armor, 45, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.HelmSlot, ItemGroups.Helm, 45, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.PantsSlot, ItemGroups.Pants, 45, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 45, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.BootsSlot, ItemGroups.Boots, 45, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 5, 13)); // Dragon Wings +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot));
        this.AddDarkKnightItems(character.Inventory);
        this.AddTestJewelsAndPotions(character.Inventory);
        this.AddPets(character.Inventory);

        return character;
    }

    /// <inheritdoc />
    protected override Character CreateElf()
    {
        var character = this.CreateElf(CharacterClassNumber.MuseElf);
        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 1080;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 200;
        character.LevelUpPoints -= 1580; // for the added strength, agility and energy
        character.LevelUpPoints += 160; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateArrows(InventoryConstants.LeftHandSlot));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.RightHandSlot, ItemGroups.Bows, 23, (SocketSubOptionType.Lightning, 0), (SocketSubOptionType.Ice, 0), (SocketSubOptionType.Fire, 1), (SocketSubOptionType.Lightning, 1), (SocketSubOptionType.Fire, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.ArmorSlot, ItemGroups.Armor, 49, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.HelmSlot, ItemGroups.Helm, 49, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.PantsSlot, ItemGroups.Pants, 49, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 49, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.BootsSlot, ItemGroups.Boots, 49, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 3, 13)); // Wings of Spirits +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlueFenrir));
        character.Inventory.Items.Add(this.CreateOrb(67, 8)); // Healing Orb
        character.Inventory.Items.Add(this.CreateOrb(75, 9)); // Defense Orb
        character.Inventory.Items.Add(this.CreateOrb(68, 10)); // Damage Orb
        this.AddElfItems(character.Inventory);
        return character;
    }

    /// <inheritdoc />
    protected override Character CreateWizard()
    {
        var character = this.CreateWizard(CharacterClassNumber.SoulMaster);

        character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 324;
        character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
        character.Attributes.First(a => a.Definition == Stats.BaseEnergy).Value += 1206;
        character.LevelUpPoints -= 1630; // for the added strength, agility, energy
        character.LevelUpPoints += 160; // after level 220, one point more

        character.Inventory!.Items.Add(this.CreateSocketItem(InventoryConstants.LeftHandSlot, ItemGroups.Staff, 31, (SocketSubOptionType.Lightning, 0), (SocketSubOptionType.Ice, 0), (SocketSubOptionType.Fire, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.RightHandSlot, ItemGroups.Shields, 20, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1)));
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.ArmorSlot, ItemGroups.Armor, 52, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1))); // Enis Legendary Armor+13+16+L
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.HelmSlot, ItemGroups.Helm, 52, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1))); // Enis Legendary Helm+13+16+L
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.PantsSlot, ItemGroups.Pants, 52, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1))); // Enis Legendary Pants+13+16+L
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.GlovesSlot, ItemGroups.Gloves, 52, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1))); // Anubis Legendary Gloves+13+16+L
        character.Inventory.Items.Add(this.CreateSocketItem(InventoryConstants.BootsSlot, ItemGroups.Boots, 52, (SocketSubOptionType.Earth, 0), (SocketSubOptionType.Water, 0), (SocketSubOptionType.Wind, 1))); // Enis Legendary Boots+13+16+L
        character.Inventory.Items.Add(this.CreateWings(InventoryConstants.WingsSlot, 4, 13)); // Wings of Soul +13
        character.Inventory.Items.Add(this.CreateFenrir(InventoryConstants.PetSlot, ItemOptionTypes.BlackFenrir));
        this.AddTestJewelsAndPotions(character.Inventory);
        return character;
    }

    private Item CreateSocketItem(byte itemSlot, ItemGroups group, byte number, params (SocketSubOptionType Element, int Number)[] socketSlots)
    {
        var item = this.Context.CreateNew<Item>();
        item.Definition = this.GameConfiguration.Items.First(def => def.Group == (byte)group && def.Number == number);
        item.Durability = item.Definition.Durability;
        item.ItemSlot = itemSlot;
        item.Level = 13;
        item.HasSkill = item.Definition.Skill != null;
        item.SocketCount = item.Definition.MaximumSockets;
        var slot = 0;
        foreach (var socketOption in socketSlots)
        {
            var socketOptionLink = this.Context.CreateNew<ItemOptionLink>();
            socketOptionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.SocketOption && o.SubOptionType == (int)socketOption.Element)
                .First(o => o.Number == socketOption.Number);
            socketOptionLink.Index = slot++;
            socketOptionLink.Level = 1;
            item.ItemOptions.Add(socketOptionLink);
        }

        var optionLink = this.Context.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
            .First(o => o.OptionType == ItemOptionTypes.Option);
        optionLink.Level = 4;
        item.ItemOptions.Add(optionLink);

        var luckLink = this.Context.CreateNew<ItemOptionLink>();
        luckLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
            .First(o => o.OptionType == ItemOptionTypes.Luck);
        item.ItemOptions.Add(luckLink);

        return item;
    }
}