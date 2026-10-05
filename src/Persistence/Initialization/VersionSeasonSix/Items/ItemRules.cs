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
/// Like in the client, the other pets (Guardian Angel, Imp, Horn of Uniria, Horn of Dinorant,
/// Horn of Fenrir) and the transformation rings can't be repaired, although they wear out:
/// a pet which can't be trained is destroyed when its durability reaches zero.
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
    /// An action which an item rule blocks.
    /// </summary>
    public enum Blocked
    {
        /// <summary>
        /// The item can't be traded.
        /// </summary>
        Trade,

        /// <summary>
        /// The item can't be dropped.
        /// </summary>
        Drop,

        /// <summary>
        /// The item can't be stored in the vault.
        /// </summary>
        Store,

        /// <summary>
        /// The item can't be sold to an NPC.
        /// </summary>
        SellToNpc,

        /// <summary>
        /// The item can't be offered in a personal store.
        /// </summary>
        PersonalStore,

        /// <summary>
        /// The item can't be repaired.
        /// </summary>
        Repair,
    }

    /// <summary>
    /// Gets the items which don't allow every action, with the actions they block.
    /// </summary>
    public static IReadOnlyList<ItemRule> Rules { get; } = new[]
    {
        Rule(4, 7, Blocked.Repair), // Bolt
        Rule(4, 15, Blocked.Repair), // Arrow
        Rule(12, 7, Blocked.Repair), // Orb of Twisting Slash
        Rule(12, 8, Blocked.Repair), // Healing Orb
        Rule(12, 9, Blocked.Repair), // Orb of Greater Fortitude
        Rule(12, 10, Blocked.Repair), // Orb of Greater Damage
        Rule(12, 11, Blocked.Repair), // Orb of Summoning
        Rule(12, 12, Blocked.Repair), // Orb of Rageful Blow
        Rule(12, 13, Blocked.Repair), // Orb of Impale
        Rule(12, 14, Blocked.Repair), // Orb of Greater Fortitude
        Rule(12, 15, Blocked.Repair), // Jewel of Chaos
        Rule(12, 16, Blocked.Repair), // Orb of Fire Slash
        Rule(12, 17, Blocked.Repair), // Orb of Penetration
        Rule(12, 18, Blocked.Repair), // Orb of Ice Arrow
        Rule(12, 19, Blocked.Repair), // Orb of Death Stab
        Rule(12, 130, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Small Cape of Lord
        Rule(12, 131, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Small Wing of Curse
        Rule(12, 132, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Small Wings of Elf
        Rule(12, 133, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Small Wings of Heaven
        Rule(12, 134, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Small Wings of Satan
        Rule(12, 135, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Little Warrior's Cloak
        Rule(13, 0, Blocked.Repair), // Guardian Angel
        Rule(13, 1, Blocked.Repair), // Imp
        Rule(13, 2, Blocked.Repair), // Horn of Uniria
        Rule(13, 3, Blocked.Repair), // Horn of Dinorant
        Rule(13, 10, Blocked.Repair), // Transformation Ring
        Rule(13, 11, Blocked.Repair), // Order (Guardian/Life Stone)
        Rule(13, 14, Blocked.Repair), // Loch's Feather
        Rule(13, 15, Blocked.Repair), // Fruits
        Rule(13, 16, Blocked.Repair), // Scroll of Archangel
        Rule(13, 17, Blocked.Repair), // Blood Bone
        Rule(13, 18, Blocked.Repair), // Invisibility Cloak
        Rule(13, 19, Blocked.Trade, Blocked.Store, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Absolute Weapon of Archangel
        Rule(13, 20, Blocked.Trade, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Wizards Ring (bound to the character, see the class remarks)
        Rule(13, 29, Blocked.Repair), // Armor of Guardsman
        Rule(13, 32, Blocked.Repair), // Splinter of Armor
        Rule(13, 33, Blocked.Repair), // Bless of Guardian
        Rule(13, 34, Blocked.Repair), // Claw of Beast
        Rule(13, 35, Blocked.Repair), // Fragment of Horn
        Rule(13, 36, Blocked.Repair), // Broken Horn
        Rule(13, 37, Blocked.Repair), // Horn of Fenrir
        Rule(13, 38, Blocked.Trade, Blocked.PersonalStore, Blocked.Repair), // Moonstone Pendant
        Rule(13, 39, Blocked.Trade, Blocked.PersonalStore, Blocked.Repair), // Eilte Transfer Skeleton Ring
        Rule(13, 40, Blocked.Repair), // Jack Olantern Ring
        Rule(13, 41, Blocked.Repair), // Transfer Christmas Ring
        Rule(13, 42, Blocked.Repair), // Ring of GM
        Rule(13, 49, Blocked.Repair), // Old Scroll
        Rule(13, 50, Blocked.Repair), // Illusion Sorcerer Covenant
        Rule(13, 51, Blocked.Repair), // Scroll of Blood
        Rule(13, 64, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Demon
        Rule(13, 65, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Spirit of Guardian
        Rule(13, 67, Blocked.Repair), // Pet Rudolf
        Rule(13, 68, Blocked.Repair), // Snowman Transformation Ring
        Rule(13, 76, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Panda Ring
        Rule(13, 80, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Pet Panda
        Rule(13, 106, Blocked.Trade, Blocked.Drop, Blocked.SellToNpc, Blocked.PersonalStore, Blocked.Repair), // Pet Unicorn
        Rule(13, 122, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Skeleton Transformation Ring
        Rule(13, 123, Blocked.Trade, Blocked.SellToNpc, Blocked.Repair), // Pet Skeleton
        Rule(14, 0, Blocked.Repair), // Apple
        Rule(14, 1, Blocked.Repair), // Small Healing Potion
        Rule(14, 2, Blocked.Repair), // Healing Potion
        Rule(14, 3, Blocked.Repair), // Large Healing Potion
        Rule(14, 4, Blocked.Repair), // Small Mana Potion
        Rule(14, 5, Blocked.Repair), // Mana Potion
        Rule(14, 6, Blocked.Repair), // Large Mana Potion
        Rule(14, 7, Blocked.Repair), // Siege Potion
        Rule(14, 8, Blocked.Repair), // Antidote
        Rule(14, 9, Blocked.Repair), // Ale
        Rule(14, 10, Blocked.Repair), // Town Portal Scroll
        Rule(14, 11, Blocked.SellToNpc, Blocked.Repair), // Box of Luck
        Rule(14, 13, Blocked.Repair), // Jewel of Bless
        Rule(14, 14, Blocked.Repair), // Jewel of Soul
        Rule(14, 16, Blocked.Repair), // Jewel of Life
        Rule(14, 17, Blocked.Repair), // Devil's Eye
        Rule(14, 18, Blocked.Repair), // Devil's Key
        Rule(14, 19, Blocked.Repair), // Devil's Invitation
        Rule(14, 21, Blocked.Trade, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Rena
        Rule(14, 22, Blocked.Repair), // Jewel of Creation
        Rule(14, 23, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Scroll of the Emperor
        Rule(14, 24, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Broken Sword
        Rule(14, 25, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Tear of Elf
        Rule(14, 26, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Soul Shard of Wizard
        Rule(14, 28, Blocked.Repair), // Lost Map
        Rule(14, 29, Blocked.Repair), // Symbol of Kundun
        Rule(14, 31, Blocked.Repair), // Jewel of Guardian
        Rule(14, 32, Blocked.Repair), // Pink Chocolate Box
        Rule(14, 33, Blocked.Repair), // Red Chocolate Box
        Rule(14, 34, Blocked.Repair), // Blue Chocolate Box
        Rule(14, 35, Blocked.Repair), // Small SD Potion
        Rule(14, 36, Blocked.Repair), // SD Potion
        Rule(14, 37, Blocked.Repair), // Large SD Potion
        Rule(14, 38, Blocked.Repair), // Small Complex Potion
        Rule(14, 39, Blocked.Repair), // Complex Potion
        Rule(14, 40, Blocked.Repair), // Large Complex Potion
        Rule(14, 41, Blocked.Repair), // Gemstone
        Rule(14, 42, Blocked.Repair), // Jewel of Harmony
        Rule(14, 43, Blocked.Repair), // Lower refining stone
        Rule(14, 44, Blocked.Repair), // Higher refining stone
        Rule(14, 45, Blocked.Repair), // Pumpkin of Luck
        Rule(14, 46, Blocked.Repair), // Jack O'Lantern Blessings
        Rule(14, 47, Blocked.Repair), // Jack O'Lantern Wrath
        Rule(14, 48, Blocked.Repair), // Jack O'Lantern Cry
        Rule(14, 49, Blocked.Repair), // Jack O'Lantern Food
        Rule(14, 50, Blocked.Repair), // Jack O'Lantern Drink
        Rule(14, 51, Blocked.Repair), // Chistmas Star
        Rule(14, 52, Blocked.Trade, Blocked.Repair), // GM Gift
        Rule(14, 63, Blocked.Repair), // Firecracker
        Rule(14, 65, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Death-beam Knight Flame
        Rule(14, 66, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Hell-Miner Horn
        Rule(14, 67, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Dark Phoenix Feather
        Rule(14, 68, Blocked.Trade, Blocked.Drop, Blocked.Store, Blocked.PersonalStore, Blocked.Repair), // Abyssal Eye
        Rule(14, 84, Blocked.Repair), // Cherry Blossom Play-Box
        Rule(14, 85, Blocked.Repair), // Cherry Blossom Wine
        Rule(14, 86, Blocked.Repair), // Cherry Blossom Rice Cake
        Rule(14, 87, Blocked.Repair), // Cherry Blossom Flower Petal
        Rule(14, 90, Blocked.Repair), // Golden Cherry Blossom Branch
        Rule(14, 99, Blocked.Repair), // Christmas Firecracker
        Rule(14, 101, Blocked.Repair), // Suspicious Scrap of Paper
        Rule(14, 102, Blocked.Repair), // Gaion's Order
        Rule(14, 103, Blocked.Repair), // First Secromicon Fragment
        Rule(14, 104, Blocked.Repair), // Second Secromicon Fragment
        Rule(14, 105, Blocked.Repair), // Third Secromicon Fragment
        Rule(14, 106, Blocked.Repair), // Fourth Secromicon Fragment
        Rule(14, 107, Blocked.Repair), // Fifth Secromicon Fragment
        Rule(14, 108, Blocked.Repair), // Sixth Secromicon Fragment
        Rule(14, 109, Blocked.Repair), // Complete Secromicon
        Rule(15, 0, Blocked.Repair), // Scroll of Poison
        Rule(15, 1, Blocked.Repair), // Scroll of Meteorite
        Rule(15, 2, Blocked.Repair), // Scroll of Lighting
        Rule(15, 3, Blocked.Repair), // Scroll of Fire Ball
        Rule(15, 4, Blocked.Repair), // Scroll of Flame
        Rule(15, 5, Blocked.Repair), // Scroll of Teleport
        Rule(15, 6, Blocked.Repair), // Scroll of Ice
        Rule(15, 7, Blocked.Repair), // Scroll of Twister
        Rule(15, 8, Blocked.Repair), // Scroll of Evil Spirit
        Rule(15, 9, Blocked.Repair), // Scroll of Hellfire
        Rule(15, 10, Blocked.Repair), // Scroll of Power Wave
        Rule(15, 11, Blocked.Repair), // Scroll of Aqua Beam
        Rule(15, 12, Blocked.Repair), // Scroll of Cometfall
        Rule(15, 13, Blocked.Repair), // Scroll of Inferno
        Rule(15, 14, Blocked.Repair), // Scroll of Teleport Ally
        Rule(15, 15, Blocked.Repair), // Scroll of Soul Barrier
        Rule(15, 16, Blocked.Repair), // Scroll of Decay
        Rule(15, 17, Blocked.Repair), // Scroll of Ice Storm
        Rule(15, 18, Blocked.Repair), // Scroll of Nova
        Rule(15, 19, Blocked.Repair), // Chain Lightning Parchment
        Rule(15, 20, Blocked.Repair), // Drain Life Parchment
        Rule(15, 21, Blocked.Repair), // Lightning Shock Parchment
        Rule(15, 22, Blocked.Repair), // Damage Reflection Parchment
        Rule(15, 23, Blocked.Repair), // Berserker Parchment
        Rule(15, 24, Blocked.Repair), // Sleep Parchment
        Rule(15, 26, Blocked.Repair), // Weakness Parchment
        Rule(15, 27, Blocked.Repair), // Innovation Parchment
        Rule(15, 28, Blocked.Repair), // Scroll of Wizardry Enhance
        Rule(15, 29, Blocked.Repair), // Scroll of Gigantic Storm
        Rule(15, 30, Blocked.Repair), // Chain Drive Parchment
        Rule(15, 31, Blocked.Repair), // Dark Side Parchment
        Rule(15, 32, Blocked.Repair), // Dragon Roar Parchment
        Rule(15, 33, Blocked.Repair), // Dragon Slasher Parchment
        Rule(15, 34, Blocked.Repair), // Ignore Defense Parchment
        Rule(15, 35, Blocked.Repair), // Increase Health Parchment
        Rule(15, 36, Blocked.Repair), // Increase Block Parchment
    };

    /// <inheritdoc />
    public override void Initialize()
    {
        Apply(this.GameConfiguration);
    }

    /// <summary>
    /// Applies the <see cref="Rules"/> to the items of the game configuration. Listed items which are
    /// not part of the configuration are skipped; an item which exists more than once (e.g. copied in
    /// the admin panel) gets the rule on every copy.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void Apply(GameConfiguration gameConfiguration)
    {
        Apply(gameConfiguration.Items);
    }

    /// <summary>
    /// Applies the <see cref="Rules"/> to the given items. Items which are not listed are skipped.
    /// </summary>
    /// <param name="items">The items.</param>
    internal static void Apply(IEnumerable<ItemDefinition> items)
    {
        var rules = Rules.ToDictionary(rule => (rule.Group, rule.Number), rule => rule.BlockedActions);
        foreach (var item in items)
        {
            if (!rules.TryGetValue((item.Group, item.Number), out var blocked))
            {
                continue;
            }

            item.IsTradable = !blocked.Contains(Blocked.Trade);
            item.IsDroppable = !blocked.Contains(Blocked.Drop);
            item.IsStorable = !blocked.Contains(Blocked.Store);
            item.IsSellableToNpc = !blocked.Contains(Blocked.SellToNpc);
            item.IsPersonalStoreSellable = !blocked.Contains(Blocked.PersonalStore);
            item.IsRepairable = !blocked.Contains(Blocked.Repair);
        }
    }

    private static ItemRule Rule(byte group, short number, params Blocked[] blockedActions)
    {
        return new ItemRule(group, number, blockedActions);
    }

    /// <summary>
    /// The actions which an item doesn't allow.
    /// </summary>
    /// <param name="Group">The group of the item.</param>
    /// <param name="Number">The number of the item.</param>
    /// <param name="BlockedActions">The actions which the item doesn't allow.</param>
    public sealed record ItemRule(byte Group, short Number, IReadOnlyCollection<Blocked> BlockedActions);
}
