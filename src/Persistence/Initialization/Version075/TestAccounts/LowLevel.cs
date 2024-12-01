// <copyright file="LowLevel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
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
    protected override Character CreateKnight()
    {
        // +9
        var character = this.CreateCharacter(this.AccountName + "Dk", CharacterClassNumber.DarkKnight, this.Level, 0);
        var a = this.CreateSmallAxe(0);
        a.Level = 9;

        var z = this.CreateSmallAxe(1);
        z.Level = 9;
        var optionz = this.Context.CreateNew<ItemOptionLink>();
        optionz.ItemOption = z.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)).PossibleOptions.ElementAt(3);
        z.ItemOptions.Add(optionz);

        var b = this.CreateSmallAxe(52);
        b.Level = 9;
        var optionb = this.Context.CreateNew<ItemOptionLink>();
        optionb.ItemOption = b.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Luck)).PossibleOptions.First();
        b.ItemOptions.Add(optionb);

        var y = this.CreateSmallAxe(47);
        y.Level = 9;
        var optiony1 = this.Context.CreateNew<ItemOptionLink>();
        optiony1.ItemOption = y.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)).PossibleOptions.ElementAt(3);
        y.ItemOptions.Add(optiony1);
        var optiony2 = this.Context.CreateNew<ItemOptionLink>();
        optiony2.ItemOption = y.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Luck)).PossibleOptions.First();
        y.ItemOptions.Add(optiony2);

        // +10
        var c = this.CreateSmallAxe(49);
        c.Level = 10;

        var w = this.CreateSmallAxe(63);
        w.Level = 10;
        var optionw = this.Context.CreateNew<ItemOptionLink>();
        optionw.ItemOption = w.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)).PossibleOptions.ElementAt(3);
        w.ItemOptions.Add(optionw);

        var d = this.CreateSmallAxe(65);
        d.Level = 10;
        var optiond = this.Context.CreateNew<ItemOptionLink>();
        optiond.ItemOption = d.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Luck)).PossibleOptions.First();
        d.ItemOptions.Add(optiond);

        var e = this.CreateSmallAxe(67);
        e.Level = 10;
        var optione1 = this.Context.CreateNew<ItemOptionLink>();
        optione1.ItemOption = e.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)).PossibleOptions.ElementAt(3);
        e.ItemOptions.Add(optione1);
        var optione2 = this.Context.CreateNew<ItemOptionLink>();
        optione2.ItemOption = e.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Luck)).PossibleOptions.First();
        e.ItemOptions.Add(optione2);

        character.Inventory!.Items.Add(a);
        character.Inventory.Items.Add(b);
        character.Inventory.Items.Add(c);
        character.Inventory.Items.Add(d);
        character.Inventory.Items.Add(e);
        character.Inventory.Items.Add(z);
        character.Inventory.Items.Add(y);
        character.Inventory.Items.Add(w);
        // character.Inventory.Items.Add(this.CreateArmorItem(52, 5, 8)); // Leather Armor
        // character.Inventory.Items.Add(this.CreateArmorItem(47, 5, 7)); // Leather Helm
        // character.Inventory.Items.Add(this.CreateArmorItem(49, 5, 9)); // Leather Pants
        // character.Inventory.Items.Add(this.CreateArmorItem(63, 5, 10)); // Leather Gloves
        // character.Inventory.Items.Add(this.CreateArmorItem(65, 5, 11)); // Leather Boots
        this.AddTestJewelsAndPotions(character.Inventory);
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
        this.AddScrolls(character.Inventory);
        return character;
    }

    private Item CreateSkullStaff(byte itemSlot)
    {
        var skullStaff = this.Context.CreateNew<Item>();
        skullStaff.Definition = this.GameConfiguration.Items.First(def => def.Group == 5 && def.Number == 0); // skull staff
        skullStaff.Durability = skullStaff.Definition?.Durability ?? 0;
        skullStaff.ItemSlot = itemSlot;
        return skullStaff;
    }

    private Item CreateSmallAxe(byte itemSlot)
    {
        var smallAxe = this.Context.CreateNew<Item>();
        smallAxe.Definition = this.GameConfiguration.Items.First(def => def.Group == 1 && def.Number == 0); // small axe
        smallAxe.Durability = smallAxe.Definition?.Durability ?? 0;
        smallAxe.ItemSlot = itemSlot;
        return smallAxe;
    }

    private Item CreateShortBow(byte itemSlot)
    {
        var shortBow = this.Context.CreateNew<Item>();
        shortBow.Definition = this.GameConfiguration.Items.First(def => def.Group == 4 && def.Number == 0); // short bow
        shortBow.Durability = shortBow.Definition?.Durability ?? 0;
        shortBow.ItemSlot = itemSlot;
        return shortBow;
    }
}