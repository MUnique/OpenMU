// <copyright file="ConfigurationNameTranslations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Globalization;
using System.Resources;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Maps built-in entity numbers to resource keys shared by all initialization versions and languages.
/// </summary>
internal static class ConfigurationNameTranslations
{
    /// <summary>Gets the CharacterClassNames entity-to-resource mappings.</summary>
    internal static IReadOnlyList<(short Number, string Key)> CharacterClassMappings { get; } =
    [
        (0, nameof(CharacterClassNames.DarkWizard)),
        (2, nameof(CharacterClassNames.SoulMaster)),
        (3, nameof(CharacterClassNames.GrandMaster)),
        (4, nameof(CharacterClassNames.DarkKnight)),
        (6, nameof(CharacterClassNames.BladeKnight)),
        (7, nameof(CharacterClassNames.BladeMaster)),
        (8, nameof(CharacterClassNames.FairyElf)),
        (10, nameof(CharacterClassNames.MuseElf)),
        (11, nameof(CharacterClassNames.HighElf)),
        (12, nameof(CharacterClassNames.MagicGladiator)),
        (13, nameof(CharacterClassNames.DuelMaster)),
        (16, nameof(CharacterClassNames.DarkLord)),
        (17, nameof(CharacterClassNames.LordEmperor)),
        (20, nameof(CharacterClassNames.Summoner)),
        (22, nameof(CharacterClassNames.BloodySummoner)),
        (23, nameof(CharacterClassNames.DimensionMaster)),
        (24, nameof(CharacterClassNames.RageFighter)),
        (25, nameof(CharacterClassNames.FistMaster)),
    ];

    /// <summary>Gets the MapNames entity-to-resource mappings.</summary>
    internal static IReadOnlyList<(short Number, string Key)> MapMappings { get; } =
    [
        (0, nameof(MapNames.Lorencia)),
        (1, nameof(MapNames.Dungeon)),
        (2, nameof(MapNames.Devias)),
        (3, nameof(MapNames.Noria)),
        (4, nameof(MapNames.LostTower)),
        (7, nameof(MapNames.Atlans)),
        (8, nameof(MapNames.Tarkan)),
        (9, nameof(MapNames.DevilSquare1)),
        (9, nameof(MapNames.DevilSquare2)),
        (9, nameof(MapNames.DevilSquare3)),
        (9, nameof(MapNames.DevilSquare4)),
        (10, nameof(MapNames.Icarus)),
        (11, nameof(MapNames.BloodCastle1)),
        (12, nameof(MapNames.BloodCastle2)),
        (13, nameof(MapNames.BloodCastle3)),
        (14, nameof(MapNames.BloodCastle4)),
        (15, nameof(MapNames.BloodCastle5)),
        (16, nameof(MapNames.BloodCastle6)),
        (17, nameof(MapNames.BloodCastle7)),
        (18, nameof(MapNames.ChaosCastle1)),
        (19, nameof(MapNames.ChaosCastle2)),
        (20, nameof(MapNames.ChaosCastle3)),
        (21, nameof(MapNames.ChaosCastle4)),
        (22, nameof(MapNames.ChaosCastle5)),
        (23, nameof(MapNames.ChaosCastle6)),
        (24, nameof(MapNames.Kalima1)),
        (25, nameof(MapNames.Kalima2)),
        (26, nameof(MapNames.Kalima3)),
        (27, nameof(MapNames.Kalima4)),
        (28, nameof(MapNames.Kalima5)),
        (29, nameof(MapNames.Kalima6)),
        (30, nameof(MapNames.ValleyOfLoren)),
        (31, nameof(MapNames.LandOfTrials)),
        (32, nameof(MapNames.DevilSquare5)),
        (32, nameof(MapNames.DevilSquare6)),
        (32, nameof(MapNames.DevilSquare7)),
        (33, nameof(MapNames.Aida)),
        (34, nameof(MapNames.CrywolfFortress)),
        (36, nameof(MapNames.Kalima7)),
        (37, nameof(MapNames.KanturuI)),
        (38, nameof(MapNames.KanturuIII)),
        (39, nameof(MapNames.KanturuEvent)),
        (41, nameof(MapNames.BarracksOfBalgass)),
        (42, nameof(MapNames.BalgassRefuge)),
        (45, nameof(MapNames.IllusionTemple1)),
        (46, nameof(MapNames.IllusionTemple2)),
        (47, nameof(MapNames.IllusionTemple3)),
        (48, nameof(MapNames.IllusionTemple4)),
        (49, nameof(MapNames.IllusionTemple5)),
        (50, nameof(MapNames.IllusionTemple6)),
        (51, nameof(MapNames.Elvenland)),
        (52, nameof(MapNames.BloodCastle8)),
        (53, nameof(MapNames.ChaosCastle7)),
        (56, nameof(MapNames.SwampOfCalmness)),
        (57, nameof(MapNames.LaCleon)),
        (58, nameof(MapNames.LaCleonBoss)),
        (63, nameof(MapNames.Vulcanus)),
        (64, nameof(MapNames.DuelArena)),
        (65, nameof(MapNames.Doppelgaenger1)),
        (66, nameof(MapNames.Doppelgaenger2)),
        (67, nameof(MapNames.Doppelgaenger3)),
        (68, nameof(MapNames.Doppelgaenger4)),
        (69, nameof(MapNames.FortressOfImperialGuardian1)),
        (70, nameof(MapNames.FortressOfImperialGuardian2)),
        (71, nameof(MapNames.FortressOfImperialGuardian3)),
        (72, nameof(MapNames.FortressOfImperialGuardian4)),
        (79, nameof(MapNames.LorenMarket)),
        (80, nameof(MapNames.Karutan1)),
        (81, nameof(MapNames.Karutan2)),
    ];

    /// <summary>Gets the MerchantNames entity-to-resource mappings.</summary>
    internal static IReadOnlyList<(short Number, string Key)> MerchantMappings { get; } =
    [
        (230, nameof(MerchantNames.Alex)),
        (231, nameof(MerchantNames.ThompsonTheMerchant)),
        (242, nameof(MerchantNames.ElfLala)),
        (243, nameof(MerchantNames.EoTheCraftsman)),
        (244, nameof(MerchantNames.CarenTheBarmaid)),
        (245, nameof(MerchantNames.IzabelTheWizard)),
        (246, nameof(MerchantNames.ZiennaTheWeaponsMerchant)),
        (248, nameof(MerchantNames.WanderingMerchantMartin)),
        (250, nameof(MerchantNames.WanderingMerchantHarold)),
        (251, nameof(MerchantNames.HanzoTheBlacksmith)),
        (253, nameof(MerchantNames.PotionGirlAmy)),
        (254, nameof(MerchantNames.PasiTheMage)),
        (255, nameof(MerchantNames.LumenTheBarmaid)),
        (259, nameof(MerchantNames.OracleLayla)),
        (415, nameof(MerchantNames.Silvia)),
        (416, nameof(MerchantNames.Rhea)),
        (417, nameof(MerchantNames.Marce)),
        (577, nameof(MerchantNames.LeinaTheGeneralGoodsMerchant)),
        (578, nameof(MerchantNames.WeaponsMerchantBolo)),
    ];

    /// <summary>Gets the MonsterNames entity-to-resource mappings.</summary>
    internal static IReadOnlyList<(short Number, string Key)> MonsterMappings { get; } =
    [
        (0, nameof(MonsterNames.BullFighter)),
        (1, nameof(MonsterNames.Hound)),
        (2, nameof(MonsterNames.BudgeDragon)),
        (3, nameof(MonsterNames.Spider)),
        (4, nameof(MonsterNames.EliteBullFighter)),
        (5, nameof(MonsterNames.HellHound)),
        (6, nameof(MonsterNames.Lich)),
        (7, nameof(MonsterNames.Giant)),
        (8, nameof(MonsterNames.PoisonBull)),
        (9, nameof(MonsterNames.ThunderLich)),
        (10, nameof(MonsterNames.DarkKnight)),
        (11, nameof(MonsterNames.Ghost)),
        (12, nameof(MonsterNames.Larva)),
        (13, nameof(MonsterNames.HellSpider)),
        (14, nameof(MonsterNames.SkeletonWarrior)),
        (15, nameof(MonsterNames.SkeletonArcher)),
        (16, nameof(MonsterNames.EliteSkeleton)),
        (17, nameof(MonsterNames.Cyclops)),
        (18, nameof(MonsterNames.Gorgon)),
        (19, nameof(MonsterNames.Yeti)),
        (20, nameof(MonsterNames.EliteYeti)),
        (21, nameof(MonsterNames.Assassin)),
        (22, nameof(MonsterNames.IceMonster)),
        (23, nameof(MonsterNames.Hommerd)),
        (24, nameof(MonsterNames.Worm)),
        (25, nameof(MonsterNames.IceQueen)),
        (26, nameof(MonsterNames.Goblin)),
        (27, nameof(MonsterNames.ChainScorpion)),
        (28, nameof(MonsterNames.BeetleMonster)),
        (29, nameof(MonsterNames.Hunter)),
        (30, nameof(MonsterNames.ForestMonster)),
        (31, nameof(MonsterNames.Agon)),
        (32, nameof(MonsterNames.StoneGolem)),
        (33, nameof(MonsterNames.EliteGoblin)),
        (34, nameof(MonsterNames.CursedWizard)),
        (35, nameof(MonsterNames.DeathGorgon)),
        (36, nameof(MonsterNames.Shadow)),
        (37, nameof(MonsterNames.Devil)),
        (38, nameof(MonsterNames.Balrog)),
        (39, nameof(MonsterNames.PoisonShadow)),
        (40, nameof(MonsterNames.DeathKnight)),
        (41, nameof(MonsterNames.DeathCow)),
        (45, nameof(MonsterNames.Bahamut)),
        (46, nameof(MonsterNames.Vepar)),
        (47, nameof(MonsterNames.Valkyrie)),
        (48, nameof(MonsterNames.LizardKing)),
        (49, nameof(MonsterNames.Hydra)),
        (51, nameof(MonsterNames.GreatBahamut)),
        (52, nameof(MonsterNames.SilverValkyrie)),
        (57, nameof(MonsterNames.IronWheel)),
        (58, nameof(MonsterNames.Tantallos)),
        (59, nameof(MonsterNames.Zaikan)),
        (60, nameof(MonsterNames.BloodyWolf)),
        (61, nameof(MonsterNames.BeamKnight)),
        (62, nameof(MonsterNames.Mutant)),
        (63, nameof(MonsterNames.DeathBeamKnight)),
        (69, nameof(MonsterNames.Alquamos)),
        (70, nameof(MonsterNames.QueenRainer)),
        (71, nameof(MonsterNames.MegaCrust)),
        (72, nameof(MonsterNames.PhantomKnight)),
        (73, nameof(MonsterNames.Drakan)),
        (74, nameof(MonsterNames.AlphaCrust)),
        (75, nameof(MonsterNames.GreatDrakan)),
        (76, nameof(MonsterNames.DarkPhoenixShield)),
        (77, nameof(MonsterNames.DarkPhoenix)),
        (144, nameof(MonsterNames.DeathAngel1)),
        (145, nameof(MonsterNames.DeathCenturion1)),
        (146, nameof(MonsterNames.BloodSoldier1)),
        (147, nameof(MonsterNames.Aegis1)),
        (148, nameof(MonsterNames.RogueCenturion1)),
        (149, nameof(MonsterNames.Necron1)),
        (152, nameof(MonsterNames.GateToKalima1OfPlayer)),
        (153, nameof(MonsterNames.GateToKalima2OfPlayer)),
        (154, nameof(MonsterNames.GateToKalima3OfPlayer)),
        (155, nameof(MonsterNames.GateToKalima4OfPlayer)),
        (156, nameof(MonsterNames.GateToKalima5OfPlayer)),
        (157, nameof(MonsterNames.GateToKalima6OfPlayer)),
        (158, nameof(MonsterNames.GateToKalima7OfPlayer)),
        (160, nameof(MonsterNames.Schriker1)),
        (161, nameof(MonsterNames.IllusionOfKundun1)),
        (174, nameof(MonsterNames.DeathAngel2)),
        (175, nameof(MonsterNames.DeathCenturion2)),
        (176, nameof(MonsterNames.BloodSoldier2)),
        (177, nameof(MonsterNames.Aegis2)),
        (178, nameof(MonsterNames.RogueCenturion2)),
        (179, nameof(MonsterNames.Necron2)),
        (180, nameof(MonsterNames.Schriker2)),
        (181, nameof(MonsterNames.IllusionOfKundun2)),
        (182, nameof(MonsterNames.DeathAngel3)),
        (183, nameof(MonsterNames.DeathCenturion3)),
        (184, nameof(MonsterNames.BloodSoldier3)),
        (185, nameof(MonsterNames.Aegis3)),
        (186, nameof(MonsterNames.RogueCenturion3)),
        (187, nameof(MonsterNames.Necron3)),
        (188, nameof(MonsterNames.Schriker3)),
        (189, nameof(MonsterNames.IllusionOfKundun3)),
        (190, nameof(MonsterNames.DeathAngel4)),
        (191, nameof(MonsterNames.DeathCenturion4)),
        (192, nameof(MonsterNames.BloodSoldier4)),
        (193, nameof(MonsterNames.Aegis4)),
        (194, nameof(MonsterNames.RogueCenturion4)),
        (195, nameof(MonsterNames.Necron4)),
        (196, nameof(MonsterNames.Schriker4)),
        (197, nameof(MonsterNames.IllusionOfKundun4)),
        (204, nameof(MonsterNames.WolfStatus)),
        (205, nameof(MonsterNames.WolfAltar1)),
        (206, nameof(MonsterNames.WolfAltar2)),
        (207, nameof(MonsterNames.WolfAltar3)),
        (208, nameof(MonsterNames.WolfAltar4)),
        (209, nameof(MonsterNames.WolfAltar5)),
        (223, nameof(MonsterNames.Senior)),
        (224, nameof(MonsterNames.Guardsman)),
        (226, nameof(MonsterNames.PetTrainer)),
        (229, nameof(MonsterNames.Marlon)),
        (233, nameof(MonsterNames.MessengerOfArch)),
        (235, nameof(MonsterNames.SevinaThePriestess)),
        (237, nameof(MonsterNames.Charon)),
        (238, nameof(MonsterNames.ChaosGoblin)),
        (240, nameof(MonsterNames.BazTheVaultKeeper)),
        (241, nameof(MonsterNames.GuildMaster)),
        (256, nameof(MonsterNames.Lahap)),
        (257, nameof(MonsterNames.ElfSoldier)),
        (260, nameof(MonsterNames.DeathAngel5)),
        (261, nameof(MonsterNames.DeathCenturion5)),
        (262, nameof(MonsterNames.BloodSoldier5)),
        (263, nameof(MonsterNames.Aegis5)),
        (264, nameof(MonsterNames.RogueCenturion5)),
        (265, nameof(MonsterNames.Necron5)),
        (266, nameof(MonsterNames.Schriker5)),
        (267, nameof(MonsterNames.IllusionOfKundun5)),
        (268, nameof(MonsterNames.DeathAngel6)),
        (269, nameof(MonsterNames.DeathCenturion6)),
        (270, nameof(MonsterNames.BloodSoldier6)),
        (271, nameof(MonsterNames.Aegis6)),
        (272, nameof(MonsterNames.RogueCenturion6)),
        (273, nameof(MonsterNames.Necron6)),
        (274, nameof(MonsterNames.Schriker6)),
        (275, nameof(MonsterNames.IllusionOfKundun7)),
        (277, nameof(MonsterNames.CastleGate1)),
        (278, nameof(MonsterNames.LifeStone)),
        (283, nameof(MonsterNames.GuardianStatue)),
        (288, nameof(MonsterNames.CanonTower)),
        (290, nameof(MonsterNames.LizardWarrior)),
        (291, nameof(MonsterNames.FireGolem)),
        (292, nameof(MonsterNames.QueenBee)),
        (293, nameof(MonsterNames.PoisonGolem)),
        (294, nameof(MonsterNames.AxeWarrior)),
        (295, nameof(MonsterNames.Erohim)),
        (304, nameof(MonsterNames.WitchQueen)),
        (305, nameof(MonsterNames.BlueGolem)),
        (306, nameof(MonsterNames.DeathRider)),
        (307, nameof(MonsterNames.ForestOrc)),
        (308, nameof(MonsterNames.DeathTree)),
        (309, nameof(MonsterNames.HellMaine)),
        (310, nameof(MonsterNames.HammerScout)),
        (311, nameof(MonsterNames.LanceScout)),
        (312, nameof(MonsterNames.BowScout)),
        (313, nameof(MonsterNames.Werewolf)),
        (314, nameof(MonsterNames.ScoutHero)),
        (315, nameof(MonsterNames.WerewolfHero)),
        (316, nameof(MonsterNames.Balram)),
        (317, nameof(MonsterNames.Soram)),
        (331, nameof(MonsterNames.Aegis7)),
        (332, nameof(MonsterNames.RogueCenturion7)),
        (333, nameof(MonsterNames.BloodSoldier7)),
        (334, nameof(MonsterNames.DeathAngel7)),
        (335, nameof(MonsterNames.Necron7)),
        (336, nameof(MonsterNames.DeathCenturion7)),
        (337, nameof(MonsterNames.Schriker7)),
        (338, nameof(MonsterNames.IllusionOfKundun6)),
        (350, nameof(MonsterNames.Berserker)),
        (351, nameof(MonsterNames.SplinterWolf)),
        (352, nameof(MonsterNames.IronRider)),
        (353, nameof(MonsterNames.Satyros)),
        (354, nameof(MonsterNames.BladeHunter)),
        (355, nameof(MonsterNames.Kentauros)),
        (356, nameof(MonsterNames.Gigantis)),
        (357, nameof(MonsterNames.Genocider)),
        (358, nameof(MonsterNames.Persona)),
        (359, nameof(MonsterNames.TwinTale)),
        (360, nameof(MonsterNames.Dreadfear)),
        (361, nameof(MonsterNames.Nightmare)),
        (362, nameof(MonsterNames.MayaHandLeft)),
        (363, nameof(MonsterNames.MayaHandRight)),
        (364, nameof(MonsterNames.Maya)),
        (367, nameof(MonsterNames.GatewayMachine)),
        (368, nameof(MonsterNames.Elphis)),
        (369, nameof(MonsterNames.Osbourne)),
        (370, nameof(MonsterNames.Jerridon)),
        (375, nameof(MonsterNames.ChaosCardMaster)),
        (385, nameof(MonsterNames.Mirage)),
        (406, nameof(MonsterNames.PriestDevin)),
        (407, nameof(MonsterNames.WerewolfQuarrel)),
        (409, nameof(MonsterNames.BalramTraineeSoldier)),
        (410, nameof(MonsterNames.DeathSpiritTraineeSoldier)),
        (411, nameof(MonsterNames.SoramTraineeSoldier)),
        (412, nameof(MonsterNames.DarkElfTraineeSoldier)),
        (418, nameof(MonsterNames.StrangeRabbit)),
        (419, nameof(MonsterNames.PollutedButterfly)),
        (420, nameof(MonsterNames.HideousRabbit)),
        (421, nameof(MonsterNames.Werewolf421)),
        (422, nameof(MonsterNames.CursedLich)),
        (423, nameof(MonsterNames.TotemGolem)),
        (424, nameof(MonsterNames.Grizzly)),
        (425, nameof(MonsterNames.CaptainGrizzly)),
        (434, nameof(MonsterNames.Gigantis434)),
        (435, nameof(MonsterNames.Berserk)),
        (436, nameof(MonsterNames.BalramTrainee)),
        (437, nameof(MonsterNames.SoramTrainee)),
        (438, nameof(MonsterNames.Persona438)),
        (439, nameof(MonsterNames.Dreadfear439)),
        (440, nameof(MonsterNames.DarkElf)),
        (441, nameof(MonsterNames.SapiUnus)),
        (442, nameof(MonsterNames.SapiDuo)),
        (443, nameof(MonsterNames.SapiTres)),
        (444, nameof(MonsterNames.ShadowPawn)),
        (445, nameof(MonsterNames.ShadowKnight)),
        (446, nameof(MonsterNames.ShadowLook)),
        (447, nameof(MonsterNames.ThunderNapin)),
        (448, nameof(MonsterNames.GhostNapin)),
        (449, nameof(MonsterNames.BlazeNapin)),
        (452, nameof(MonsterNames.SeedMaster)),
        (453, nameof(MonsterNames.SeedResearcher)),
        (454, nameof(MonsterNames.IceWalker)),
        (455, nameof(MonsterNames.GiantMammoth)),
        (456, nameof(MonsterNames.IceGiant)),
        (457, nameof(MonsterNames.Coolutin)),
        (458, nameof(MonsterNames.IronKnight)),
        (459, nameof(MonsterNames.Selupan)),
        (480, nameof(MonsterNames.ZombieFighter)),
        (481, nameof(MonsterNames.ZombieFighter481)),
        (482, nameof(MonsterNames.ResurrectedGladiator)),
        (483, nameof(MonsterNames.ResurrectedGladiator483)),
        (484, nameof(MonsterNames.AshSlaughterer)),
        (485, nameof(MonsterNames.AshSlaughterer485)),
        (486, nameof(MonsterNames.BloodAssassin)),
        (487, nameof(MonsterNames.CruelBloodAssassin)),
        (488, nameof(MonsterNames.CruelBloodAssassin488)),
        (489, nameof(MonsterNames.BurningLavaGiant)),
        (490, nameof(MonsterNames.RuthlessLavaGiant)),
        (491, nameof(MonsterNames.RuthlessLavaGiant491)),
        (492, nameof(MonsterNames.MossTheMerchant)),
        (522, nameof(MonsterNames.AdviserJerinteu)),
        (529, nameof(MonsterNames.TerribleButcher)),
        (530, nameof(MonsterNames.MadButcher)),
        (531, nameof(MonsterNames.IceWalker531)),
        (532, nameof(MonsterNames.Larva532)),
        (534, nameof(MonsterNames.DoppelgangerElf)),
        (535, nameof(MonsterNames.DoppelgangerKnight)),
        (536, nameof(MonsterNames.DoppelgangerWizard)),
        (537, nameof(MonsterNames.DoppelgangerMagicGladiator)),
        (538, nameof(MonsterNames.DoppelgangerDarkLord)),
        (539, nameof(MonsterNames.DoppelgangerSummoner)),
        (540, nameof(MonsterNames.Lugard)),
        (549, nameof(MonsterNames.BloodyOrc)),
        (550, nameof(MonsterNames.BloodyDeathRider)),
        (551, nameof(MonsterNames.BloodyGolem)),
        (552, nameof(MonsterNames.BloodyWitchQueen)),
        (553, nameof(MonsterNames.BerserkerWarrior)),
        (554, nameof(MonsterNames.KentaurosWarrior)),
        (555, nameof(MonsterNames.GigantisWarrior)),
        (556, nameof(MonsterNames.GenociderWarrior)),
        (557, nameof(MonsterNames.SapiQueen)),
        (562, nameof(MonsterNames.DarkMammoth)),
        (563, nameof(MonsterNames.DarkGiant)),
        (564, nameof(MonsterNames.DarkCoolutin)),
        (565, nameof(MonsterNames.DarkIronKnight)),
        (569, nameof(MonsterNames.VenomousChainScorpion)),
        (570, nameof(MonsterNames.BoneScorpion)),
        (571, nameof(MonsterNames.Orcus)),
        (572, nameof(MonsterNames.Gollock)),
        (573, nameof(MonsterNames.Crypta)),
        (574, nameof(MonsterNames.Crypos)),
        (575, nameof(MonsterNames.Condra)),
        (576, nameof(MonsterNames.Narcondra)),
        (579, nameof(MonsterNames.David)),
    ];

    /// <summary>Completes available translations without overwriting customized values.</summary>
    /// <param name="configuration">The configuration to update.</param>
    internal static void Apply(GameConfiguration configuration)
    {
        UpdateNames(configuration.CharacterClasses, CharacterClassNames.ResourceManager, CharacterClassMappings, c => c.Number, c => c.Name, (c, name) => c.Name = name);
        UpdateNames(configuration.Maps, MapNames.ResourceManager, MapMappings, m => m.Number, m => m.Name, (m, name) => m.Name = name);
        UpdateNames(configuration.Monsters.Where(m => m.MerchantStore is not null), MerchantNames.ResourceManager, MerchantMappings, m => m.Number, m => m.Designation, (m, name) => m.Designation = name);
        UpdateNames(configuration.Monsters, MonsterNames.ResourceManager, MonsterMappings, m => m.Number, m => m.Designation, (m, name) => m.Designation = name);
    }

    private static void UpdateNames<T>(IEnumerable<T> entities, ResourceManager resources, IReadOnlyList<(short Number, string Key)> mappings, Func<T, int> getNumber, Func<T, LocalizedString> getName, Action<T, LocalizedString> setName)
    {
        var keysByNumber = mappings.ToLookup(mapping => (int)mapping.Number, mapping => mapping.Key);
        foreach (var entity in entities)
        {
            var current = getName(entity);
            foreach (var key in keysByNumber[getNumber(entity)])
            {
                var translated = resources.GetLocalizedString(key);
                if (current.ValueInNeutralLanguage != translated.ValueInNeutralLanguage)
                {
                    continue;
                }

                foreach (var culture in resources.AvailableCultures)
                {
                    var text = resources.GetResourceSet(culture, true, tryParents: false)?.GetString(key);
                    if (!string.IsNullOrEmpty(text) && !HasCustomTranslation(current, culture))
                    {
                        current = current.WithTranslation(culture, text);
                    }
                }

                setName(entity, current);
                break;
            }
        }
    }

    private static bool HasCustomTranslation(LocalizedString name, CultureInfo culture)
    {
        // Preserve legacy language-only customizations, but do not mistake a sibling culture
        // (for example zh-TW) for an existing translation of zh-CN.
        var entries = (name.Value ?? string.Empty).Split(LocalizedString.Separator, StringSplitOptions.None).Skip(1);
        for (var candidate = culture; !string.IsNullOrEmpty(candidate.Name); candidate = candidate.Parent)
        {
            var prefix = candidate.Name + "=";
            var entry = entries.FirstOrDefault(part => part.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (entry is not null)
            {
                var text = entry[prefix.Length..];
                return !string.IsNullOrEmpty(text) && text != name.ValueInNeutralLanguage;
            }
        }

        return false;
    }
}
