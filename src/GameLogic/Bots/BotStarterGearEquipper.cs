// <copyright file="BotStarterGearEquipper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Linq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Equips a freshly generated bot character with its starter gear: a class- and build-appropriate
/// weapon (with ammunition for bows), a basic armor set, and starting potion stacks - everything at
/// the profile's starter item level. One instance outfits one character: the shared context (persistence
/// context, inventory, class, item level) travels in fields, so each step only takes what actually
/// varies per call instead of threading the same parameters through every helper.
/// </summary>
internal sealed class BotStarterGearEquipper
{
    /// <summary>Item group of bows (need ammunition).</summary>
    private const byte BowGroup = 4;

    /// <summary>Item group of staves/sticks (casters).</summary>
    private const byte StaffGroup = 5;

    /// <summary>Item group of body armor; its item number identifies the armor set.</summary>
    private const byte ArmorGroup = 8;

    /// <summary>
    /// Armor set numbers tried in thematic order; the first the class is qualified for (by its chest piece)
    /// is used: 5 Leather (warriors), 2 Pad (wizards), 10 Vine (elves), 39 Mistery (summoners), then fallbacks.
    /// </summary>
    private static readonly byte[] ArmorSetCandidates = { 5, 2, 10, 39, 6, 0, 4, 8 };

    private readonly IPlayerContext _context;
    private readonly GameConfiguration _configuration;
    private readonly ItemStorage _inventory;
    private readonly Character _character;
    private readonly CharacterClass _characterClass;
    private readonly byte _starterItemLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotStarterGearEquipper"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="configuration">The game configuration which defines the items.</param>
    /// <param name="character">The character to equip. Its inventory and class must be set.</param>
    /// <param name="starterItemLevel">The upgrade level of the starter items (level 0 for fresh characters).</param>
    public BotStarterGearEquipper(IPlayerContext context, GameConfiguration configuration, Character character, byte starterItemLevel)
    {
        this._context = context;
        this._configuration = configuration;
        this._inventory = character.Inventory!;
        this._character = character;
        this._characterClass = character.CharacterClass!;
        this._starterItemLevel = starterItemLevel;
    }

    /// <summary>
    /// Equips a basic, class-appropriate weapon (mirrors the low-level test account gear), so the bot
    /// is not punching with its fists.
    /// </summary>
    public void EquipWeapon()
    {
        // Data-driven, so every class gets a weapon it is actually QUALIFIED to wield. We pick the most
        // basic option (lowest DropLevel) the class can use from the weapon groups (0 sword, 1 axe,
        // 2 mace, 3 spear, 4 bow, 5 staff).
        // The weapon type follows the bot's BUILD (BotProgression.IsPreferredWeaponGroup - the same rule the
        // later upgrades use), so an energy-specked Magic Gladiator starts with a staff instead of a blade.
        // The Small Axe is qualified for almost every class, so without this filter casters and archers would
        // all end up with one.
        bool IsPreferredWeapon(ItemDefinition definition)
            => BotProgression.IsPreferredWeaponGroup(this._characterClass, this._character.Name, (byte)definition.Group);

        // Ammunition shares the bow group (Bolt/Arrows have DropLevel 0), so without this filter every
        // archer would get a bolt stack as its "weapon" and end up punching with its fists.
        var weapon = this._configuration.Items
                .Where(d => IsPreferredWeapon(d) && !d.IsAmmunition && d.QualifiedCharacters.Contains(this._characterClass))
                .MinBy(d => d.DropLevel)
            ?? this._configuration.Items
                .Where(d => d.Group <= StaffGroup && !d.IsAmmunition && d.QualifiedCharacters.Contains(this._characterClass))
                .MinBy(d => d.DropLevel);
        if (weapon is null)
        {
            return;
        }

        if (weapon.Group == BowGroup)
        {
            // Bows need ammunition; the arrows go into the left hand.
            this.AddEquippedItem(InventoryConstants.RightHandSlot, weapon);
            this.AddAmmunition();
        }
        else
        {
            this.AddEquippedItem(InventoryConstants.LeftHandSlot, weapon);
        }
    }

    /// <summary>
    /// Equips a basic, class-appropriate armor set (mirrors the low-level test account gear).
    /// </summary>
    public void EquipArmorSet()
    {
        // Data-driven, so every class gets gear it is actually QUALIFIED to wear (a Dark Lord must never
        // end up in a Pad/wizard set). We pick the armor set whose chest piece (group 8) has the lowest
        // DropLevel; its NUMBER identifies the set, and the equipment type is the GROUP (7 helm, 8 armor,
        // 9 pants, 10 gloves, 11 boots).
        // Choose a thematically appropriate armor set the class can wear, tried in order (warriors -> Leather,
        // wizards -> Pad, elves -> Vine, summoners -> Mistery, then fallbacks). Each piece is added only if the
        // class is qualified for it, so e.g. the Magic Gladiator keeps the set but skips the helm it can't wear.
        foreach (var set in ArmorSetCandidates)
        {
            if (this._configuration.Items.FirstOrDefault(d => d.Group == ArmorGroup && d.Number == set) is not { } chest
                || !chest.QualifiedCharacters.Contains(this._characterClass))
            {
                continue;
            }

            this.EquipArmorPiece(InventoryConstants.HelmSlot, 7, set);
            this.EquipArmorPiece(InventoryConstants.ArmorSlot, 8, set);
            this.EquipArmorPiece(InventoryConstants.PantsSlot, 9, set);
            this.EquipArmorPiece(InventoryConstants.GlovesSlot, 10, set);
            this.EquipArmorPiece(InventoryConstants.BootsSlot, 11, set);
            break;
        }
    }

    /// <summary>
    /// Adds starting potion stacks to the backpack, so the offline HealingHandler has something to drink.
    /// </summary>
    public void AddPotions()
    {
        // A stack of Large Healing Potions so the offline HealingHandler has something to drink, and a
        // stack of Large Mana Potions so casters can keep casting instead of degrading to weak melee once
        // their mana runs dry. The BotNavigator tops both up at runtime, so the bot never runs out.
        // Durability holds the stack count.
        this.AddPotionStack(3, InventoryConstants.EquippableSlotsCount);      // Large Healing Potion, first backpack slot
        this.AddPotionStack(6, (byte)(InventoryConstants.EquippableSlotsCount + 1)); // Large Mana Potion, second backpack slot
    }

    private void EquipArmorPiece(byte slot, int group, int number)
    {
        var definition = this._configuration.Items.FirstOrDefault(d => d.Group == group && d.Number == number);
        if (definition is null || !definition.QualifiedCharacters.Contains(this._characterClass))
        {
            return;
        }

        this.AddEquippedItem(slot, definition);
    }

    private void AddEquippedItem(byte slot, ItemDefinition definition)
    {
        if (!definition.QualifiedCharacters.Contains(this._characterClass))
        {
            return;
        }

        var item = this._context.CreateNew<Item>();
        item.Definition = definition;
        item.Level = this._starterItemLevel;
        item.Durability = definition.Durability;
        item.ItemSlot = slot;
        this._inventory.Items.Add(item);
    }

    private void AddAmmunition()
    {
        var arrows = this._configuration.Items.FirstOrDefault(d => d.Group == 4 && d.Number == 15);
        if (arrows is null)
        {
            return;
        }

        var item = this._context.CreateNew<Item>();
        item.Definition = arrows;
        item.Durability = 255;
        item.ItemSlot = InventoryConstants.LeftHandSlot;
        this._inventory.Items.Add(item);
    }

    private void AddPotionStack(byte potionNumber, byte slot)
    {
        var potion = this._configuration.Items.FirstOrDefault(d => d.Group == 14 && d.Number == potionNumber);
        if (potion is null)
        {
            return;
        }

        var item = this._context.CreateNew<Item>();
        item.Definition = potion;

        // Only a handful of charges to start with: fresh bots head to the merchant right away and buy
        // their supplies with their starting Zen, kicking off the shopping economy from minute one
        // (kept just above the emergency top-up threshold, so the economy path - not the fallback - runs).
        item.Durability = Rand.NextInt(10, 16);
        item.ItemSlot = slot;
        this._inventory.Items.Add(item);
    }
}
