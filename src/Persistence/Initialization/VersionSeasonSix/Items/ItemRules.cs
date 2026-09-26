// <copyright file="ItemRules.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Sets the item rules (<see cref="ItemDefinition.IsTradable"/>, <see cref="ItemDefinition.IsDroppable"/>,
/// <see cref="ItemDefinition.IsStorable"/>, <see cref="ItemDefinition.IsSellableToNpc"/>,
/// <see cref="ItemDefinition.IsPersonalStoreSellable"/> and <see cref="ItemDefinition.IsRepairable"/>)
/// of the Season 6 items.
/// </summary>
/// <remarks>
/// The values are the ones of the MuMain client item data (src/bin/Data/Items), so the client and the
/// server allow the same actions. Items which are not listed allow everything.
/// Two exceptions:
/// <list type="bullet">
///   <item>The Wizards Ring: the client allows trading and storing it at level 0, but here it is
///   <see cref="ItemDefinition.IsBoundToCharacter"/>, so its flags follow that.</item>
///   <item>The Dark Horse and the Dark Raven stay repairable: the server repairs trainable pets
///   (with their own repair price), while the client has no way to repair them.</item>
/// </list>
/// </remarks>
public class ItemRules : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRules"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ItemRules(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// The actions an item rule blocks.
    /// </summary>
    [Flags]
    public enum Blocked
    {
        /// <summary>
        /// The item can't be traded.
        /// </summary>
        Trade = 1,

        /// <summary>
        /// The item can't be dropped.
        /// </summary>
        Drop = 2,

        /// <summary>
        /// The item can't be stored in the vault.
        /// </summary>
        Store = 4,

        /// <summary>
        /// The item can't be sold to an NPC.
        /// </summary>
        SellToNpc = 8,

        /// <summary>
        /// The item can't be offered in a personal store.
        /// </summary>
        PersonalStore = 16,

        /// <summary>
        /// The item can't be repaired.
        /// </summary>
        Repair = 32,
    }

    /// <summary>
    /// Gets the items which don't allow every action, with the actions they block.
    /// </summary>
    public static IReadOnlyList<(byte Group, short Number, Blocked Blocked)> Rules { get; } = new (byte, short, Blocked)[]
    {
        (4, 7, Blocked.Repair), // Bolt
        (4, 15, Blocked.Repair), // Arrow
        (12, 7, Blocked.Repair), // Orb of Twisting Slash
        (12, 8, Blocked.Repair), // Healing Orb
        (12, 9, Blocked.Repair), // Orb of Greater Fortitude
        (12, 10, Blocked.Repair), // Orb of Greater Damage
        (12, 11, Blocked.Repair), // Orb of Summoning
        (12, 12, Blocked.Repair), // Orb of Rageful Blow
        (12, 13, Blocked.Repair), // Orb of Impale
        (12, 14, Blocked.Repair), // Orb of Greater Fortitude
        (12, 15, Blocked.Repair), // Jewel of Chaos
        (12, 16, Blocked.Repair), // Orb of Fire Slash
        (12, 17, Blocked.Repair), // Orb of Penetration
        (12, 18, Blocked.Repair), // Orb of Ice Arrow
        (12, 19, Blocked.Repair), // Orb of Death Stab
        (13, 0, Blocked.Repair), // Guardian Angel
        (13, 1, Blocked.Repair), // Imp
        (13, 2, Blocked.Repair), // Horn of Uniria
        (13, 3, Blocked.Repair), // Horn of Dinorant
        (13, 10, Blocked.Repair), // Transformation Ring
        (13, 11, Blocked.Repair), // Order (Guardian/Life Stone)
        (13, 14, Blocked.Repair), // Loch's Feather
        (13, 15, Blocked.Repair), // Fruits
        (13, 16, Blocked.Repair), // Scroll of Archangel
        (13, 17, Blocked.Repair), // Blood Bone
        (13, 18, Blocked.Repair), // Invisibility Cloak
        (13, 19, Blocked.Trade | Blocked.Store | Blocked.SellToNpc | Blocked.PersonalStore | Blocked.Repair), // Absolute Weapon of Archangel
        (13, 20, Blocked.Trade | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Wizards Ring (bound to the character, see the class remarks)
        (13, 29, Blocked.Repair), // Armor of Guardsman
        (13, 32, Blocked.Repair), // Splinter of Armor
        (13, 33, Blocked.Repair), // Bless of Guardian
        (13, 34, Blocked.Repair), // Claw of Beast
        (13, 35, Blocked.Repair), // Fragment of Horn
        (13, 36, Blocked.Repair), // Broken Horn
        (13, 37, Blocked.Repair), // Horn of Fenrir
        (13, 38, Blocked.Trade | Blocked.PersonalStore | Blocked.Repair), // Moonstone Pendant
        (13, 39, Blocked.Trade | Blocked.PersonalStore | Blocked.Repair), // Eilte Transfer Skeleton Ring
        (13, 40, Blocked.Repair), // Jack Olantern Ring
        (13, 41, Blocked.Repair), // Transfer Christmas Ring
        (13, 42, Blocked.Repair), // Ring of GM
        (13, 49, Blocked.Repair), // Old Scroll
        (13, 50, Blocked.Repair), // Illusion Sorcerer Covenant
        (13, 51, Blocked.Repair), // Scroll of Blood
        (13, 64, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Demon
        (13, 65, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Spirit of Guardian
        (13, 67, Blocked.Repair), // Pet Rudolf
        (13, 68, Blocked.Repair), // Snowman Transformation Ring
        (13, 76, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Panda Ring
        (13, 80, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Pet Panda
        (13, 106, Blocked.Trade | Blocked.Drop | Blocked.SellToNpc | Blocked.PersonalStore | Blocked.Repair), // Pet Unicorn
        (13, 122, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Skeleton Transformation Ring
        (13, 123, Blocked.Trade | Blocked.SellToNpc | Blocked.Repair), // Pet Skeleton
        (14, 0, Blocked.Repair), // Apple
        (14, 1, Blocked.Repair), // Small Healing Potion
        (14, 2, Blocked.Repair), // Healing Potion
        (14, 3, Blocked.Repair), // Large Healing Potion
        (14, 4, Blocked.Repair), // Small Mana Potion
        (14, 5, Blocked.Repair), // Mana Potion
        (14, 6, Blocked.Repair), // Large Mana Potion
        (14, 7, Blocked.Repair), // Siege Potion
        (14, 8, Blocked.Repair), // Antidote
        (14, 9, Blocked.Repair), // Ale
        (14, 10, Blocked.Repair), // Town Portal Scroll
        (14, 11, Blocked.SellToNpc | Blocked.Repair), // Box of Luck
        (14, 13, Blocked.Repair), // Jewel of Bless
        (14, 14, Blocked.Repair), // Jewel of Soul
        (14, 16, Blocked.Repair), // Jewel of Life
        (14, 17, Blocked.Repair), // Devil's Eye
        (14, 18, Blocked.Repair), // Devil's Key
        (14, 19, Blocked.Repair), // Devil's Invitation
        (14, 21, Blocked.Trade | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Rena
        (14, 22, Blocked.Repair), // Jewel of Creation
        (14, 23, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Scroll of the Emperor
        (14, 24, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Broken Sword
        (14, 25, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Tear of Elf
        (14, 26, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Soul Shard of Wizard
        (14, 28, Blocked.Repair), // Lost Map
        (14, 29, Blocked.Repair), // Symbol of Kundun
        (14, 31, Blocked.Repair), // Jewel of Guardian
        (14, 32, Blocked.Repair), // Pink Chocolate Box
        (14, 33, Blocked.Repair), // Red Chocolate Box
        (14, 34, Blocked.Repair), // Blue Chocolate Box
        (14, 35, Blocked.Repair), // Small SD Potion
        (14, 36, Blocked.Repair), // SD Potion
        (14, 37, Blocked.Repair), // Large SD Potion
        (14, 38, Blocked.Repair), // Small Complex Potion
        (14, 39, Blocked.Repair), // Complex Potion
        (14, 40, Blocked.Repair), // Large Complex Potion
        (14, 41, Blocked.Repair), // Gemstone
        (14, 42, Blocked.Repair), // Jewel of Harmony
        (14, 43, Blocked.Repair), // Lower refining stone
        (14, 44, Blocked.Repair), // Higher refining stone
        (14, 45, Blocked.Repair), // Pumpkin of Luck
        (14, 46, Blocked.Repair), // Jack O'Lantern Blessings
        (14, 47, Blocked.Repair), // Jack O'Lantern Wrath
        (14, 48, Blocked.Repair), // Jack O'Lantern Cry
        (14, 49, Blocked.Repair), // Jack O'Lantern Food
        (14, 50, Blocked.Repair), // Jack O'Lantern Drink
        (14, 51, Blocked.Repair), // Chistmas Star
        (14, 52, Blocked.Trade | Blocked.Repair), // GM Gift
        (14, 63, Blocked.Repair), // Firecracker
        (14, 65, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Death-beam Knight Flame
        (14, 66, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Hell-Miner Horn
        (14, 67, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Dark Phoenix Feather
        (14, 68, Blocked.Trade | Blocked.Drop | Blocked.Store | Blocked.PersonalStore | Blocked.Repair), // Abyssal Eye
        (14, 84, Blocked.Repair), // Cherry Blossom Play-Box
        (14, 85, Blocked.Repair), // Cherry Blossom Wine
        (14, 86, Blocked.Repair), // Cherry Blossom Rice Cake
        (14, 87, Blocked.Repair), // Cherry Blossom Flower Petal
        (14, 90, Blocked.Repair), // Golden Cherry Blossom Branch
        (14, 99, Blocked.Repair), // Christmas Firecracker
        (14, 101, Blocked.Repair), // Suspicious Scrap of Paper
        (14, 102, Blocked.Repair), // Gaion's Order
        (14, 103, Blocked.Repair), // First Secromicon Fragment
        (14, 104, Blocked.Repair), // Second Secromicon Fragment
        (14, 105, Blocked.Repair), // Third Secromicon Fragment
        (14, 106, Blocked.Repair), // Fourth Secromicon Fragment
        (14, 107, Blocked.Repair), // Fifth Secromicon Fragment
        (14, 108, Blocked.Repair), // Sixth Secromicon Fragment
        (14, 109, Blocked.Repair), // Complete Secromicon
        (15, 0, Blocked.Repair), // Scroll of Poison
        (15, 1, Blocked.Repair), // Scroll of Meteorite
        (15, 2, Blocked.Repair), // Scroll of Lighting
        (15, 3, Blocked.Repair), // Scroll of Fire Ball
        (15, 4, Blocked.Repair), // Scroll of Flame
        (15, 5, Blocked.Repair), // Scroll of Teleport
        (15, 6, Blocked.Repair), // Scroll of Ice
        (15, 7, Blocked.Repair), // Scroll of Twister
        (15, 8, Blocked.Repair), // Scroll of Evil Spirit
        (15, 9, Blocked.Repair), // Scroll of Hellfire
        (15, 10, Blocked.Repair), // Scroll of Power Wave
        (15, 11, Blocked.Repair), // Scroll of Aqua Beam
        (15, 12, Blocked.Repair), // Scroll of Cometfall
        (15, 13, Blocked.Repair), // Scroll of Inferno
        (15, 14, Blocked.Repair), // Scroll of Teleport Ally
        (15, 15, Blocked.Repair), // Scroll of Soul Barrier
        (15, 16, Blocked.Repair), // Scroll of Decay
        (15, 17, Blocked.Repair), // Scroll of Ice Storm
        (15, 18, Blocked.Repair), // Scroll of Nova
        (15, 19, Blocked.Repair), // Chain Lightning Parchment
        (15, 20, Blocked.Repair), // Drain Life Parchment
        (15, 21, Blocked.Repair), // Lightning Shock Parchment
        (15, 22, Blocked.Repair), // Damage Reflection Parchment
        (15, 23, Blocked.Repair), // Berserker Parchment
        (15, 24, Blocked.Repair), // Sleep Parchment
        (15, 26, Blocked.Repair), // Weakness Parchment
        (15, 27, Blocked.Repair), // Innovation Parchment
        (15, 28, Blocked.Repair), // Scroll of Wizardry Enhance
        (15, 29, Blocked.Repair), // Scroll of Gigantic Storm
        (15, 30, Blocked.Repair), // Chain Drive Parchment
        (15, 31, Blocked.Repair), // Dark Side Parchment
        (15, 32, Blocked.Repair), // Dragon Roar Parchment
        (15, 33, Blocked.Repair), // Dragon Slasher Parchment
        (15, 34, Blocked.Repair), // Ignore Defense Parchment
        (15, 35, Blocked.Repair), // Increase Health Parchment
        (15, 36, Blocked.Repair), // Increase Block Parchment
    };

    /// <inheritdoc />
    public override void Initialize()
    {
        Apply(this.GameConfiguration);
    }

    /// <summary>
    /// Applies the <see cref="Rules"/> to the items of the game configuration.
    /// Items which are not part of the configuration are skipped.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void Apply(GameConfiguration gameConfiguration)
    {
        var items = gameConfiguration.Items.ToDictionary(item => (item.Group, item.Number));
        foreach (var (group, number, blocked) in Rules)
        {
            if (!items.TryGetValue((group, number), out var item))
            {
                continue;
            }

            item.IsTradable = !blocked.HasFlag(Blocked.Trade);
            item.IsDroppable = !blocked.HasFlag(Blocked.Drop);
            item.IsStorable = !blocked.HasFlag(Blocked.Store);
            item.IsSellableToNpc = !blocked.HasFlag(Blocked.SellToNpc);
            item.IsPersonalStoreSellable = !blocked.HasFlag(Blocked.PersonalStore);
            item.IsRepairable = !blocked.HasFlag(Blocked.Repair);
        }
    }
}
