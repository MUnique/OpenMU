// <copyright file="IllusionTempleDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The illusion temple update plugin. Brings an existing database up to date with everything the event
/// needs, without requiring a full reinstall: the mini game definitions, the statue/guardian/relic-box
/// spawns and the roaming arena monsters on the six temple maps, the sacred relic and ticket items, the
/// two special-skill magic effects, and a couple of fields on already-existing rows that a plain data
/// migration (EF migration) can add as a column but not populate with the right value.
/// </summary>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.IllusionTempleDataUpdatePlugIn_Name), Description = nameof(Properties.PlugInResources.IllusionTempleDataUpdatePlugIn_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("032FECC5-932E-4161-A50A-DF7D07AF3866")]
public class IllusionTempleDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The number of the Mirage NPC, which opens the illusion temple dialog.
    /// </summary>
    private const short MirageNpcNumber = 385;

    /// <summary>
    /// The number of Lumen the Barmaid, the merchant who sells the Illusion Sorcerer Covenant.
    /// </summary>
    private const short LumenTheBarmaidNumber = 255;

    /// <summary>
    /// The number of the Stone Statue NPC, which holds the sacred relic during a match.
    /// </summary>
    private const short StoneStatueNumber = 380;

    /// <summary>
    /// The number of the MU Allies General NPC (decorative team guardian).
    /// </summary>
    private const short AllianceGuardianNumber = 381;

    /// <summary>
    /// The number of the Illusion Elder NPC (decorative team guardian).
    /// </summary>
    private const short IllusionGuardianNumber = 382;

    /// <summary>
    /// The number of the Alliance Item Storage NPC, to which the allied forces carry the relic to score.
    /// </summary>
    private const short AllianceItemStorageNumber = 383;

    /// <summary>
    /// The number of the Illusion Item Storage NPC, to which the illusion forces carry the relic to score.
    /// </summary>
    private const short IllusionItemStorageNumber = 384;

    /// <summary>
    /// The lowest NPC number of the obsolete "Cursed Statue" / "Captured Stone Statue" spawns, which
    /// this update removes from the temple maps.
    /// </summary>
    private const short ObsoleteStatueRangeStart = 658;

    /// <summary>
    /// The highest NPC number of the obsolete "Cursed Statue" / "Captured Stone Statue" spawns.
    /// </summary>
    private const short ObsoleteStatueRangeEnd = 668;

    /// <summary>
    /// The lowest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeStart = 386;

    /// <summary>
    /// The highest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeEnd = 403;

    /// <summary>
    /// The default minimum player count for an illusion temple match, if the definition doesn't already
    /// have one configured through the admin panel.
    /// </summary>
    private const int DefaultMinimumPlayerCount = 2;

    /// <summary>
    /// The item group of the illusion temple ticket items.
    /// </summary>
    private const byte TicketItemGroup = 13;

    /// <summary>
    /// The number of the Illusion Sorcerer Covenant, which drops from monsters.
    /// </summary>
    private const short CovenantItemNumber = 50;

    /// <summary>
    /// The number of the Scroll of Blood, the ticket, which the chaos goblin combines from an Old Scroll and an
    /// Illusion Sorcerer Covenant.
    /// </summary>
    private const short ScrollOfBloodItemNumber = 51;

    /// <summary>
    /// The neutral name of the Illusion Sorcerer Covenant.
    /// </summary>
    private const string CovenantName = "Illusion Sorcerer Covenant";

    /// <summary>
    /// The neutral name of the Scroll of Blood.
    /// </summary>
    private const string ScrollOfBloodName = "Scroll of Blood";

    /// <summary>
    /// The original width of the Scroll of Blood.
    /// </summary>
    private const byte ScrollOfBloodWidth = 2;

    /// <summary>
    /// The original width of the Illusion Sorcerer Covenant.
    /// </summary>
    private const byte CovenantWidth = 1;

    /// <summary>
    /// The original height of both items.
    /// </summary>
    private const byte ItemHeight = 2;

    /// <summary>
    /// The map numbers of the six illusion temples, and the arena monster NPC number range (base id and
    /// how many ids to cycle through) which each of them roams with.
    /// </summary>
    private static readonly (byte MapNumber, short ArenaMonsterBase, int ArenaMonsterCycleLength)[] Temples =
    {
        (45, 386, 3),
        (46, 389, 3),
        (47, 392, 3),
        (48, 395, 3),
        (49, 398, 3),
        (50, 401, 3),
    };

    /// <summary>
    /// The pool of statue spawn positions, shared by all six illusion temples. Only one of them is
    /// active at a time, randomly picked by the game logic.
    /// </summary>
    private static readonly (byte X, byte Y)[] StatuePositions =
    {
        (207, 047),
        (134, 121),
    };

    /// <summary>
    /// The positions of the decorative team guardians, shared by all six illusion temples.
    /// </summary>
    private static readonly (byte X, byte Y) AllianceGuardianPosition = (139, 046);

    private static readonly (byte X, byte Y) IllusionGuardianPosition = (194, 123);

    /// <summary>
    /// The position of the Alliance Item Storage, close to the allied forces' own spawn area.
    /// </summary>
    private static readonly (byte X, byte Y) AllianceBoxPosition = (141, 059);

    /// <summary>
    /// The position of the Illusion Item Storage, close to the illusion forces' own spawn area.
    /// </summary>
    private static readonly (byte X, byte Y) IllusionBoxPosition = (194, 113);

    /// <summary>
    /// The 32 roaming arena monster spawn positions, shared by all temples that have them - the NPC
    /// number at each position cycles through the temple's arena monster range.
    /// </summary>
    private static readonly (byte X, byte Y)[] ArenaMonsterPositions =
    {
        (131, 93), (131, 89), (131, 85),
        (168, 123), (164, 123), (160, 123),
        (169, 48), (169, 52), (169, 56),
        (206, 85), (206, 81), (206, 77),
        (158, 85), (168, 94), (169, 75),
        (179, 85), (157, 66), (162, 66),
        (150, 78), (150, 73), (176, 103),
        (181, 103), (187, 98), (187, 92),
        (197, 57), (141, 113), (193, 61),
        (145, 109), (189, 65), (149, 105),
        (167, 87), (171, 83),
    };

    /// <summary>
    /// The stats of the 18 "Illusion Sorc. Spirit" arena monster variants (386 to 403), three per temple
    /// level.
    /// </summary>
    private static readonly (short Number, int Level, int Hp, int MinDmg, int MaxDmg, int Defense, int AttackRate, int DefenseRate, byte AttackRange, short ViewRange, int MoveDelayMs, int AttackDelayMs)[] ArenaMonsterStats =
    {
        (386, 65, 7150, 195, 245, 150, 340, 98, 4, 4, 800, 1600),
        (387, 65, 7150, 215, 265, 170, 380, 110, 4, 4, 800, 1600),
        (388, 67, 7370, 235, 285, 190, 440, 130, 1, 6, 1600, 2000),
        (389, 70, 8680, 280, 330, 210, 500, 150, 4, 4, 800, 1600),
        (390, 70, 8680, 300, 350, 230, 560, 170, 4, 4, 800, 1600),
        (391, 72, 8928, 320, 370, 250, 640, 200, 1, 6, 1600, 2000),
        (392, 75, 15000, 375, 395, 280, 460, 150, 4, 4, 800, 1600),
        (393, 75, 15000, 395, 415, 300, 520, 160, 4, 4, 800, 1600),
        (394, 77, 15400, 415, 435, 320, 580, 195, 1, 6, 1600, 2000),
        (395, 80, 19200, 480, 500, 360, 660, 230, 4, 4, 800, 1600),
        (396, 80, 19200, 500, 520, 380, 720, 260, 4, 4, 800, 1600),
        (397, 82, 19680, 520, 540, 400, 840, 280, 1, 6, 1600, 2000),
        (398, 85, 25500, 595, 615, 450, 760, 275, 4, 4, 800, 1600),
        (399, 85, 25500, 615, 635, 470, 820, 303, 4, 4, 800, 1600),
        (400, 87, 26100, 635, 655, 490, 900, 335, 1, 6, 1600, 2000),
        (401, 112, 36000, 720, 740, 550, 1015, 405, 4, 4, 800, 1600),
        (402, 112, 36000, 740, 760, 570, 1080, 440, 4, 4, 800, 1600),
        (403, 114, 36800, 760, 780, 590, 1190, 485, 1, 6, 1600, 2000),
    };

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.IllusionTempleDataUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.IllusionTempleDataUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override int Version => 5;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 29, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 11, 23, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// Version one swapped the numbers of the Illusion Sorcerer Covenant and the Scroll of Blood, believing the
    /// covenant was the ticket. It's the other way around, so version three leaves the original numbers as they
    /// are, and gives a database which got the swap the original item data back (see <see cref="RestoreTicketItems"/>).
    /// Version four also restores the data when the server owner gave both items their original names already, so
    /// it runs the restoration again on a database which has version three.
    /// The ticket binding repair of version two pointed the temples which reference 13/50 at 13/51, then named the
    /// covenant; the restoration still makes that move, as 13/51 becomes the Scroll of Blood.
    /// Versions one to four gave temple 5 only two arena monster variants and temple 6 none; version five adds the
    /// missing variants (400 to 403) and their spawns (see <see cref="AddMissingArenaMonsters"/>), and lets characters of
    /// level 400 without the master class enter temple 5 (see <see cref="AllowLevel400InTemple5"/>).
    /// </remarks>
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration, int? installedVersion)
    {
        // Replaying version one's content changes could overwrite owner-customized spawns or safezones.
        if (installedVersion is null or < 1)
        {
            await this.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        }

        RestoreTicketItems(gameConfiguration);

        if (installedVersion is null or < 5)
        {
            this.AddMissingArenaMonsters(context, gameConfiguration);
            AllowLevel400InTemple5(gameConfiguration);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddRelicItem(context, gameConfiguration);
        this.AddArenaMonsterDefinitions(context, gameConfiguration);
        this.AddMapSpawns(context, gameConfiguration);
        this.FixSafezoneMaps(gameConfiguration);

        if (gameConfiguration.MiniGameDefinitions.All(def => def.Type != MiniGameType.IllusionTemple))
        {
            var initializer = new IllusionTempleInitializer(context, gameConfiguration);
            initializer.Initialize();
        }
        else
        {
            foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(def => def.Type == MiniGameType.IllusionTemple && def.MinimumPlayerCount <= 0))
            {
                definition.MinimumPlayerCount = DefaultMinimumPlayerCount;
            }
        }

        // After the initializer: it creates the magic effects along with the definitions, so running
        // this first would add a second copy of each on a database which has neither yet.
        this.AddSpecialSkillEffects(context, gameConfiguration);

        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == MirageNpcNumber) is { } mirage)
        {
            mirage.NpcWindow = NpcWindow.IllusionTemple;
        }
    }

    /// <summary>
    /// Raises the maximum character level of temple 5 from 399 to 400. A character of level 400 without the master
    /// class goes to temple 5 (the master classes to temple 6), so with 399 he couldn't enter any temple.
    /// </summary>
    /// <remarks>Only the values which are still the old default are changed, so an owner's own range stays.</remarks>
    private static void AllowLevel400InTemple5(GameConfiguration gameConfiguration)
    {
        const int previousMaximumLevel = 399;
        const int maximumLevel = 400;
        foreach (var temple5 in gameConfiguration.MiniGameDefinitions.Where(d => d is { Type: MiniGameType.IllusionTemple, GameLevel: 5 }))
        {
            if (temple5.MaximumCharacterLevel == previousMaximumLevel)
            {
                temple5.MaximumCharacterLevel = maximumLevel;
            }

            if (temple5.MaximumSpecialCharacterLevel == previousMaximumLevel)
            {
                temple5.MaximumSpecialCharacterLevel = maximumLevel;
            }
        }
    }

    /// <summary>
    /// Gives the items 13/50 and 13/51 their original data back, if the configuration has the data of the
    /// two swapped, and points the mini game definitions which reference 13/50 at the ticket 13/51.
    /// </summary>
    /// <remarks>
    /// Version one of this update swapped the numbers of the two item definitions, and the data initialization of
    /// the same time created them with swapped data. The game client knows the items by their numbers, so the
    /// numbers stay and the data is swapped back instead: an owned item which the client shows as the Scroll of
    /// Blood stays the ticket, and the item 13/50 drops again. The swapped state is recognized by the names. When the
    /// server owner gave both items their original names already, it's recognized by what's left of it: the swapped
    /// sizes, Lumen the Barmaid selling 13/51 instead of 13/50, or a drop item group of the covenant with 13/51 instead
    /// of 13/50. A 13/51 next to a 13/50 is an addition of the owner. On a configuration without these, nothing
    /// changes, also when the owner changed the sizes or the temple tickets, or added the Scroll of Blood.
    /// </remarks>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void RestoreTicketItems(GameConfiguration gameConfiguration)
    {
        // The names are matched in the neutral language: item names became a LocalizedString, whose
        // serialized value carries the translations and the resource metadata, so comparing it to a
        // plain string would never match on a database which has been localized.
        // The items are named by their numbers, which is what they are after the restoration.
        var ticket = gameConfiguration.Items.FirstOrDefault(item => item.Group == TicketItemGroup && item.Number == ScrollOfBloodItemNumber);
        var covenant = gameConfiguration.Items.FirstOrDefault(item => item.Group == TicketItemGroup && item.Number == CovenantItemNumber);
        if (ticket is null || covenant is null)
        {
            return;
        }

        if (ticket.Name.ValueInNeutralLanguage == CovenantName && covenant.Name.ValueInNeutralLanguage == ScrollOfBloodName)
        {
            RestoreSwappedItems(gameConfiguration, ticket, covenant);
        }
        else if (ticket.Name.ValueInNeutralLanguage == ScrollOfBloodName && covenant.Name.ValueInNeutralLanguage == CovenantName
                 && HasRemainsOfTheSwap(gameConfiguration, ticket, covenant))
        {
            // A server owner may have given both items their original names already, but not the rest of their data.
            RestoreRenamedBackItems(gameConfiguration, ticket, covenant);
        }
        else
        {
            // Not swapped, or the server owner changed one of the two: the data of one item alone can't be swapped,
            // and the description of the update tells the owner to check the items.
        }
    }

    /// <summary>
    /// Restores the items while their names are swapped: the names and the rest of the data follow the number.
    /// The temples with the ticket 13/50 move to 13/51, and the store items of 13/51 to 13/50, as the owner chose
    /// them by the swapped name; a temple of 13/51 or a store item of 13/50 is right already, so it stays. The drop
    /// item groups swap the two items.
    /// </summary>
    private static void RestoreSwappedItems(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        RepairTicketBindings(gameConfiguration, ticket, covenant);
        RepairMerchantStoreItems(gameConfiguration.Monsters, ticket, covenant);
        (ticket.Name, covenant.Name) = (covenant.Name, ticket.Name);
        SwapData(ticket, covenant);
        SetOriginalSizes(ticket, covenant);
        SwapDropItemGroups(gameConfiguration, ticket, covenant);
    }

    /// <summary>
    /// Determines whether a configuration in which both items have their original names has remains of the swap:
    /// the swapped sizes, Lumen the Barmaid selling 13/51 instead of 13/50, or a drop item group of the covenant with
    /// 13/51 instead of 13/50. None of these is in the original data; a 13/51 which the owner added next to the 13/50
    /// isn't a remain.
    /// </summary>
    private static bool HasRemainsOfTheSwap(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        return (ticket is { Width: CovenantWidth, Height: ItemHeight } && covenant is { Width: ScrollOfBloodWidth, Height: ItemHeight })
               || GetLumenSellingTicketInsteadOfCovenant(gameConfiguration, ticket, covenant).Any()
               || GetCovenantGroupsWithTicketInsteadOfCovenant(gameConfiguration, ticket, covenant).Any();
    }

    /// <summary>
    /// Restores the items when the server owner gave both their original names already. The owner may have fixed the
    /// sizes, too, so the repairs only change what's still wrong.
    /// A reference which the owner added to 13/51 may be meant for the Scroll of Blood, so only the references of the
    /// original data move: the temple tickets, and the store items of Lumen the Barmaid and the drop item groups of the
    /// covenant which have 13/51 instead of 13/50. The rest of the data, besides the sizes, is the same for both items
    /// in the original data.
    /// </summary>
    private static void RestoreRenamedBackItems(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        RepairTicketBindings(gameConfiguration, ticket, covenant);
        RepairMerchantStoreItems(GetLumenSellingTicketInsteadOfCovenant(gameConfiguration, ticket, covenant).ToList(), ticket, covenant);
        SetOriginalSizes(ticket, covenant);
        RepairCovenantDropItemGroups(gameConfiguration, ticket, covenant);
    }

    /// <summary>
    /// Points the mini game definitions which reference the item 13/50 at the ticket 13/51.
    /// A database which had temple definitions before version one of this update renumbered the items kept
    /// them on the item definition which got the number 50, and a server owner who chose a ticket in the
    /// swapped state chose 13/50 as the Scroll of Blood. That item definition becomes the Illusion Sorcerer
    /// Covenant, so every definition follows the Scroll of Blood to 13/51, also one with an own id or ticket
    /// level. A ticket of another item, or of an item definition of another configuration, stays.
    /// </summary>
    private static void RepairTicketBindings(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(definition => ReferenceEquals(definition.TicketItem, covenant)))
        {
            definition.TicketItem = ticket;
        }
    }

    /// <summary>
    /// Points the items of the stores of the specified merchants which reference the ticket 13/51 at the item 13/50.
    /// No merchant sells the ticket, but a merchant of the original data, Lumen the Barmaid, sells the Illusion
    /// Sorcerer Covenant, whose item definition version one of this update renumbered to 51.
    /// </summary>
    private static void RepairMerchantStoreItems(IEnumerable<MonsterDefinition> merchants, ItemDefinition ticket, ItemDefinition covenant)
    {
        foreach (var item in GetStoreItems(merchants).Where(item => ReferenceEquals(item.Definition, ticket)))
        {
            item.Definition = covenant;
        }
    }

    /// <summary>
    /// Swaps the data which the data initialization sets for each event item, except the number, the name and the size.
    /// The other data, like the item rules, the price definition and the game version, is set by the number,
    /// so it already fits.
    /// </summary>
    private static void SwapData(ItemDefinition first, ItemDefinition second)
    {
        (first.Durability, second.Durability) = (second.Durability, first.Durability);
        (first.MaximumItemLevel, second.MaximumItemLevel) = (second.MaximumItemLevel, first.MaximumItemLevel);
        (first.DropsFromMonsters, second.DropsFromMonsters) = (second.DropsFromMonsters, first.DropsFromMonsters);
        (first.DropLevel, second.DropLevel) = (second.DropLevel, first.DropLevel);
        (first.MaximumDropLevel, second.MaximumDropLevel) = (second.MaximumDropLevel, first.MaximumDropLevel);
    }

    /// <summary>
    /// Sets the original sizes of the two items, which the game client knows by the number. They're set instead of
    /// swapped, as a server owner may have fixed them already: the client shows overlapping items otherwise.
    /// </summary>
    private static void SetOriginalSizes(ItemDefinition ticket, ItemDefinition covenant)
    {
        ticket.Width = ScrollOfBloodWidth;
        ticket.Height = ItemHeight;
        covenant.Width = CovenantWidth;
        covenant.Height = ItemHeight;
    }

    /// <summary>
    /// Swaps the two item definitions in the drop item groups which contain one of them, which are the groups of
    /// the Illusion Sorcerer Covenant, one for each item level.
    /// </summary>
    private static void SwapDropItemGroups(GameConfiguration gameConfiguration, ItemDefinition first, ItemDefinition second)
    {
        foreach (var group in gameConfiguration.DropItemGroups)
        {
            var containsFirst = group.PossibleItems.Contains(first);
            if (containsFirst == group.PossibleItems.Contains(second))
            {
                continue;
            }

            group.PossibleItems.Remove(containsFirst ? first : second);
            group.PossibleItems.Add(containsFirst ? second : first);
        }
    }

    /// <summary>
    /// Points the drop item groups of the Illusion Sorcerer Covenant, whose description the data initialization started
    /// with its name (the item level follows the translations), from the ticket 13/51 at the item 13/50.
    /// When the server owner gave the items their original names already, a drop item group of their own which
    /// contains 13/51 may be meant for the Scroll of Blood, and a covenant group which contains 13/50 is right, also
    /// when the owner added 13/51 to it, so these stay.
    /// </summary>
    private static void RepairCovenantDropItemGroups(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        foreach (var group in GetCovenantGroupsWithTicketInsteadOfCovenant(gameConfiguration, ticket, covenant).ToList())
        {
            group.PossibleItems.Remove(ticket);
            group.PossibleItems.Add(covenant);
        }
    }

    /// <summary>
    /// Gets Lumen the Barmaid, the merchant of the original data who sells the Illusion Sorcerer Covenant, when she
    /// sells the ticket 13/51 instead of the covenant 13/50. When she sells both, the owner added the Scroll of Blood.
    /// The detection of the swap and the repair use it both, so they stay in step.
    /// </summary>
    private static IEnumerable<MonsterDefinition> GetLumenSellingTicketInsteadOfCovenant(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        return gameConfiguration.Monsters.Where(monster =>
            monster.Number == LumenTheBarmaidNumber
            && monster.MerchantStore?.Items is { } items
            && items.Any(item => ReferenceEquals(item.Definition, ticket))
            && !items.Any(item => ReferenceEquals(item.Definition, covenant)));
    }

    /// <summary>
    /// Gets the items of the stores of the specified merchants.
    /// </summary>
    private static IEnumerable<Item> GetStoreItems(IEnumerable<MonsterDefinition> merchants)
    {
        return merchants.SelectMany(monster => monster.MerchantStore?.Items ?? Enumerable.Empty<Item>());
    }

    /// <summary>
    /// Gets the drop item groups of the Illusion Sorcerer Covenant which contain the ticket 13/51 instead of the
    /// covenant 13/50. One which contains both got the Scroll of Blood from the owner. The detection of the swap and
    /// the repair use it both, so they stay in step.
    /// </summary>
    private static IEnumerable<DropItemGroup> GetCovenantGroupsWithTicketInsteadOfCovenant(GameConfiguration gameConfiguration, ItemDefinition ticket, ItemDefinition covenant)
    {
        return gameConfiguration.DropItemGroups.Where(group =>
            IsCovenantGroup(group) && group.PossibleItems.Contains(ticket) && !group.PossibleItems.Contains(covenant));
    }

    /// <summary>
    /// Determines whether the drop item group is one of the Illusion Sorcerer Covenant, whose description the data
    /// initialization started with its name.
    /// </summary>
    private static bool IsCovenantGroup(DropItemGroup group)
    {
        return group.Description.ValueInNeutralLanguage.StartsWith(CovenantName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Adds the sacred relic item ("Cursed Castle Water", Group 14, Number 64), if it doesn't exist yet -
    /// <see cref="MUnique.OpenMU.GameLogic.MiniGames.IllusionTempleContext.TalkToNpcStoneStatueAsync"/>
    /// looks it up by Group/Number.
    /// </summary>
    private void AddRelicItem(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Items.Any(item => item.Group == 14 && item.Number == 64))
        {
            return;
        }

        var relic = context.CreateNew<ItemDefinition>();
        gameConfiguration.Items.Add(relic);
        relic.Group = 14;
        relic.Number = 64;
        relic.Name = LocalizedString.FromResource(() => ItemNames.CursedCastleWater);
        relic.Width = 1;
        relic.Height = 1;
        relic.Durability = 1;
        relic.DropsFromMonsters = false;
        relic.SetGuid(relic.Group, relic.Number);
    }

    /// <summary>
    /// Adds the 18 "Illusion Sorc. Spirit" arena monster definitions (386 to 403), if they don't exist
    /// yet.
    /// </summary>
    private void AddArenaMonsterDefinitions(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var spirit in ArenaMonsterStats)
        {
            if (gameConfiguration.Monsters.Any(monster => monster.Number == spirit.Number))
            {
                continue;
            }

            var def = context.CreateNew<MonsterDefinition>();
            def.Number = spirit.Number;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.IllusionSorcererSpirit);
            def.MoveRange = 3;
            def.AttackRange = spirit.AttackRange;
            def.ViewRange = spirit.ViewRange;
            def.MoveDelay = TimeSpan.FromMilliseconds(spirit.MoveDelayMs);
            def.AttackDelay = TimeSpan.FromMilliseconds(spirit.AttackDelayMs);
            def.RespawnDelay = TimeSpan.FromSeconds(10);
            def.Attribute = 2;
            def.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, spirit.Level },
                { Stats.MaximumHealth, spirit.Hp },
                { Stats.MinimumPhysBaseDmg, spirit.MinDmg },
                { Stats.MaximumPhysBaseDmg, spirit.MaxDmg },
                { Stats.DefenseBase, spirit.Defense },
                { Stats.AttackRatePvm, spirit.AttackRate },
                { Stats.DefenseRatePvm, spirit.DefenseRate },
            };
            def.AddAttributes(attributes, context, gameConfiguration);
            gameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }
    }

    /// <summary>
    /// Adds the stone statue, team guardian, relic delivery and arena monster spawn points to the six
    /// illusion temple maps, if they don't have them yet.
    /// </summary>
    /// <remarks>
    /// The maps themselves are usually already present in an existing database (they've been part of the
    /// game data since 2018), but this update is needed to add the previously missing spawns to
    /// them - the game maps aren't recreated by <see cref="IllusionTempleInitializer"/>, only its mini
    /// game definitions are.
    /// </remarks>
    /// <summary>
    /// Adds the arena monsters which the versions before five didn't have: the third variant of temple 5 (400) and
    /// the three variants of temple 6 (401 to 403), with their spawns.
    /// </summary>
    /// <remarks>
    /// A spawn of temple 5 only gets the new variant when it still has the monster which the old version put there,
    /// so a spawn which the server owner changed stays as it is. Temple 6 only gets the spawns which don't exist yet.
    /// </remarks>
    private void AddMissingArenaMonsters(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddArenaMonsterDefinitions(context, gameConfiguration);

        foreach (var temple in Temples.Where(t => t.MapNumber is 49 or 50))
        {
            if (gameConfiguration.Maps.FirstOrDefault(m => m.Number == temple.MapNumber) is not { } map)
            {
                continue;
            }

            for (var i = 0; i < ArenaMonsterPositions.Length; i++)
            {
                var monsterNumber = (short)(temple.ArenaMonsterBase + (i % temple.ArenaMonsterCycleLength));
                if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == monsterNumber) is not { } monsterDefinition)
                {
                    continue;
                }

                var spawnNumber = (short)(120 + i);
                var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawnNumber);
                if (map.MonsterSpawns.FirstOrDefault(spawn => spawn.GetId() == id) is { } existingSpawn)
                {
                    // Temple 5 used to alternate between 398 and 399.
                    var previousMonsterNumber = (short)(temple.ArenaMonsterBase + (i % 2));
                    if (temple.MapNumber == 49 && existingSpawn.MonsterDefinition?.Number == previousMonsterNumber)
                    {
                        existingSpawn.MonsterDefinition = monsterDefinition;
                    }

                    continue;
                }

                var (x, y) = ArenaMonsterPositions[i];
                this.AddSpawn(context, map, spawnNumber, monsterDefinition, x, y, SpawnTrigger.AutomaticDuringEvent);
            }
        }
    }

    private void AddMapSpawns(IContext context, GameConfiguration gameConfiguration)
    {
        var stoneStatue = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == StoneStatueNumber);
        var allianceGuardian = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == AllianceGuardianNumber);
        var illusionGuardian = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == IllusionGuardianNumber);
        var allianceBox = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == AllianceItemStorageNumber);
        var illusionBox = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == IllusionItemStorageNumber);
        if (stoneStatue is null || allianceGuardian is null || illusionGuardian is null || allianceBox is null || illusionBox is null)
        {
            return;
        }

        foreach (var temple in Temples)
        {
            var map = gameConfiguration.Maps.FirstOrDefault(m => m.Number == temple.MapNumber);
            if (map is null)
            {
                continue;
            }

            if (map.MonsterSpawns.Any(spawn => spawn.MonsterDefinition == stoneStatue))
            {
                this.RemoveObsoleteStatueSpawns(map);
                continue;
            }

            // The spawn numbers match the ones of the temple map initializers, so an updated database ends
            // up with the same deterministic ids as a fresh installation.
            short spawnNumber = 100;
            foreach (var (x, y) in StatuePositions)
            {
                // Only one of the pool positions is spawned at a time, randomly picked by the game logic.
                this.AddSpawn(context, map, spawnNumber++, stoneStatue, x, y, SpawnTrigger.ManuallyForEvent);
            }

            this.AddSpawn(context, map, 110, allianceGuardian, AllianceGuardianPosition.X, AllianceGuardianPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, 111, illusionGuardian, IllusionGuardianPosition.X, IllusionGuardianPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, 112, allianceBox, AllianceBoxPosition.X, AllianceBoxPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, 113, illusionBox, IllusionBoxPosition.X, IllusionBoxPosition.Y, SpawnTrigger.AutomaticDuringEvent);

            if (temple.ArenaMonsterCycleLength > 0)
            {
                for (var i = 0; i < ArenaMonsterPositions.Length; i++)
                {
                    var monsterNumber = (short)(temple.ArenaMonsterBase + (i % temple.ArenaMonsterCycleLength));
                    var monsterDefinition = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == monsterNumber);
                    if (monsterDefinition is null)
                    {
                        continue;
                    }

                    var (x, y) = ArenaMonsterPositions[i];
                    this.AddSpawn(context, map, (short)(120 + i), monsterDefinition, x, y, SpawnTrigger.AutomaticDuringEvent);
                }
            }

            // Only after adding the new spawns: the obsolete ones used the spawn numbers 100 to 110, so the
            // ones whose id got reused by a new spawn have already been overwritten in place.
            this.RemoveObsoleteStatueSpawns(map);
        }
    }

    /// <summary>
    /// Removes the previous "Cursed Statue" / "Captured Stone Statue" spawns (NPC 658 to 668) from a
    /// temple map. They were placeholders based on the wrong NPC numbers - the event actually uses a
    /// single "Stone Statue" (380), which is picked at random from a small pool of positions. Leaving
    /// the old ones in place would litter the arena with statues that have no function.
    /// </summary>
    /// <param name="map">The temple map to clean up.</param>
    private void RemoveObsoleteStatueSpawns(GameMapDefinition map)
    {
        var obsoleteSpawns = map.MonsterSpawns
            .Where(spawn => spawn.MonsterDefinition?.Number is >= ObsoleteStatueRangeStart and <= ObsoleteStatueRangeEnd)
            .ToList();
        foreach (var spawn in obsoleteSpawns)
        {
            map.MonsterSpawns.Remove(spawn);
        }
    }

    /// <summary>
    /// Adds a spawn with a deterministic id to the map. If the map already has a spawn with the same id
    /// (e.g. one of the obsolete statue spawns), it is overwritten instead - creating a second entity with
    /// the same id would fail when saving, because the existing one is already tracked by the context,
    /// even if it got removed from the map.
    /// </summary>
    private void AddSpawn(IContext context, GameMapDefinition map, short spawnNumber, MonsterDefinition monsterDefinition, byte x, byte y, SpawnTrigger spawnTrigger)
    {
        var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawnNumber);
        var area = map.MonsterSpawns.FirstOrDefault(spawn => spawn.GetId() == id);
        if (area is null)
        {
            area = context.CreateNew<MonsterSpawnArea>();
            area.SetGuid(id);
            map.MonsterSpawns.Add(area);
        }

        area.GameMap = map;
        area.MonsterDefinition = monsterDefinition;
        area.Quantity = 1;
        area.Direction = Direction.Undefined;
        area.SpawnTrigger = spawnTrigger;
        area.X1 = x;
        area.X2 = x;
        area.Y1 = y;
        area.Y2 = y;
        area.WaveNumber = 0;
    }

    /// <summary>
    /// The six illusion temple maps have their own spawn gate, so <see cref="BaseMapInitializer.SafezoneMapNumber"/>
    /// used to default to the temple map itself instead of Devias - a player who ends up warped to his
    /// "safezone" (e.g. because too few players entered) would just be sent right back into the arena
    /// instead of actually leaving it.
    /// </summary>
    private void FixSafezoneMaps(GameConfiguration gameConfiguration)
    {
        var devias = gameConfiguration.Maps.FirstOrDefault(map => map.Number == 2);
        if (devias is null)
        {
            return;
        }

        foreach (var temple in Temples)
        {
            var map = gameConfiguration.Maps.FirstOrDefault(m => m.Number == temple.MapNumber);
            if (map is null || map.SafezoneMap == devias)
            {
                continue;
            }

            map.SafezoneMap = devias;
        }
    }

    /// <summary>
    /// Adds the two magic effects used by the event's special skills (210 - Order of Protection and
    /// 211 - Restraint), if they don't exist yet. The other two special skills (212 - Tracking and
    /// 213 - Weaken) act instantly and don't need a magic effect of their own.
    /// </summary>
    private void AddSpecialSkillEffects(IContext context, GameConfiguration gameConfiguration)
    {
        const short protectionEffectNumber = 210;
        const short restraintEffectNumber = 211;

        if (gameConfiguration.MagicEffects.Any(effect => effect.Number == protectionEffectNumber))
        {
            return;
        }

        var protection = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(protection);
        protection.Number = protectionEffectNumber;
        protection.Name = LocalizedString.FromResource(() => MagicEffectNames.IllusionTempleOrderOfProtection);
        protection.InformObservers = true;
        protection.StopByDeath = true;
        protection.Duration = context.CreateNew<PowerUpDefinitionValue>();
        protection.Duration.ConstantValue!.Value = 15; // 15 seconds

        var protectionPowerUp = context.CreateNew<PowerUpDefinition>();
        protection.PowerUpDefinitions.Add(protectionPowerUp);
        protectionPowerUp.TargetAttribute = Stats.DamageReceiveDecrement.GetPersistent(gameConfiguration);
        protectionPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        protectionPowerUp.Boost.ConstantValue.Value = 0.50f; // 50 % damage reduction
        protectionPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var restraint = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(restraint);
        restraint.Number = restraintEffectNumber;
        restraint.Name = LocalizedString.FromResource(() => MagicEffectNames.IllusionTempleRestraint);
        restraint.InformObservers = true;
        restraint.StopByDeath = true;
        restraint.Duration = context.CreateNew<PowerUpDefinitionValue>();
        restraint.Duration.ConstantValue!.Value = 15; // 15 seconds

        var restraintPowerUp = context.CreateNew<PowerUpDefinition>();
        restraint.PowerUpDefinitions.Add(restraintPowerUp);
        restraintPowerUp.TargetAttribute = Stats.IsFrozen.GetPersistent(gameConfiguration);
        restraintPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        restraintPowerUp.Boost.ConstantValue.Value = 1;
    }
}
