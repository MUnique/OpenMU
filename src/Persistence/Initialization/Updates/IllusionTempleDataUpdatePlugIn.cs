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
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("032FECC5-932E-4161-A50A-DF7D07AF3866")]
public class IllusionTempleDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The number of the Mirage NPC, which opens the illusion temple dialog.
    /// </summary>
    private const short MirageNpcNumber = 385;

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
    /// The lowest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeStart = 386;

    /// <summary>
    /// The highest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeEnd = 399;

    /// <summary>
    /// The default minimum player count for an illusion temple match, if the definition doesn't already
    /// have one configured through the admin panel.
    /// </summary>
    private const int DefaultMinimumPlayerCount = 2;

    /// <summary>
    /// The map numbers of the six illusion temples, and the arena monster NPC number range (base id and
    /// how many ids to cycle through) which each of them roams with - temple 6 has none.
    /// </summary>
    private static readonly (byte MapNumber, short ArenaMonsterBase, int ArenaMonsterCycleLength)[] Temples =
    {
        (45, 386, 3),
        (46, 389, 3),
        (47, 392, 3),
        (48, 395, 3),
        (49, 398, 2),
        (50, 0, 0),
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
    /// The stats of the 14 "Illusion Sorc. Spirit" arena monster variants (386 to 399), one per temple
    /// level (temples 1 to 4 have three variants each, temple 5 has two, temple 6 has none).
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
    };

    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Illusion Temple Data";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update creates the configuration data for the illusion temple event and assigns the event dialog to the Mirage NPC.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.IllusionTempleData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 29, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        this.FixTicketItemNumbers(gameConfiguration);
        this.AddRelicItem(context, gameConfiguration);
        this.AddArenaMonsterDefinitions(context, gameConfiguration);
        this.AddMapSpawns(context, gameConfiguration);
        this.FixSafezoneMaps(gameConfiguration);
        this.AddSpecialSkillEffects(context, gameConfiguration);

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

        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == MirageNpcNumber) is { } mirage)
        {
            mirage.NpcWindow = NpcWindow.IllusionTemple;
        }
    }

    /// <summary>
    /// The illusion temple ticket ("Illusion Sorcerer Covenant") and the "Scroll of Blood" item used to
    /// have their numbers swapped (50/51) - <see cref="IllusionTempleInitializer"/> looks the ticket up
    /// by Group 13, Number 51, so an existing database still using the old numbering needs the two
    /// numbers corrected. Only the Number field is touched, so already-owned instances of either item
    /// keep referencing the same, unchanged, item definition entity.
    /// </summary>
    private void FixTicketItemNumbers(GameConfiguration gameConfiguration)
    {
        var covenant = gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Name == "Illusion Sorcerer Covenant");
        var scrollOfBlood = gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Name == "Scroll of Blood");
        if (covenant is { Number: 50 } && scrollOfBlood is { Number: 51 })
        {
            covenant.Number = 51;
            scrollOfBlood.Number = 50;
        }
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
        relic.Name = "Cursed Castle Water";
        relic.Width = 1;
        relic.Height = 1;
        relic.Durability = 1;
        relic.DropsFromMonsters = false;
        relic.SetGuid(relic.Group, relic.Number);
    }

    /// <summary>
    /// Adds the 14 "Illusion Sorc. Spirit" arena monster definitions (386 to 399), if they don't exist
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
            def.Designation = "Illusion Sorc. Spirit";
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
                continue;
            }

            short spawnNumber = 100;
            foreach (var (x, y) in StatuePositions)
            {
                // Only one of the pool positions is spawned at a time, randomly picked by the game logic.
                this.AddSpawn(context, map, spawnNumber++, stoneStatue, x, y, SpawnTrigger.ManuallyForEvent);
            }

            this.AddSpawn(context, map, spawnNumber++, allianceGuardian, AllianceGuardianPosition.X, AllianceGuardianPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, spawnNumber++, illusionGuardian, IllusionGuardianPosition.X, IllusionGuardianPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, spawnNumber++, allianceBox, AllianceBoxPosition.X, AllianceBoxPosition.Y, SpawnTrigger.AutomaticDuringEvent);
            this.AddSpawn(context, map, spawnNumber++, illusionBox, IllusionBoxPosition.X, IllusionBoxPosition.Y, SpawnTrigger.AutomaticDuringEvent);

            if (temple.ArenaMonsterCycleLength <= 0)
            {
                continue;
            }

            for (var i = 0; i < ArenaMonsterPositions.Length; i++)
            {
                var monsterNumber = (short)(temple.ArenaMonsterBase + (i % temple.ArenaMonsterCycleLength));
                var monsterDefinition = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == monsterNumber);
                if (monsterDefinition is null)
                {
                    continue;
                }

                var (x, y) = ArenaMonsterPositions[i];
                this.AddSpawn(context, map, spawnNumber++, monsterDefinition, x, y, SpawnTrigger.AutomaticDuringEvent);
            }
        }
    }

    private void AddSpawn(IContext context, GameMapDefinition map, short spawnNumber, MonsterDefinition monsterDefinition, byte x, byte y, SpawnTrigger spawnTrigger)
    {
        var area = context.CreateNew<MonsterSpawnArea>();
        area.SetGuid(map.Number, spawnNumber);
        area.GameMap = map;
        area.MonsterDefinition = monsterDefinition;
        area.Quantity = 1;
        area.Direction = Direction.Undefined;
        area.SpawnTrigger = spawnTrigger;
        area.X1 = x;
        area.X2 = x;
        area.Y1 = y;
        area.Y2 = y;
        map.MonsterSpawns.Add(area);
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
        protection.Name = "Illusion Temple - Order of Protection";
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
        restraint.Name = "Illusion Temple - Restraint";
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
