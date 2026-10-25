// <copyright file="ItemStatProfilesDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests the item stat profiles of the initialization data and of the configuration update which adds them.
/// </summary>
[TestFixture]
internal class ItemStatProfilesDataTest
{
    private const string Version075 = "0.75";
    private const string Version095d = "0.95d";
    private const string Season6 = "Season 6";

    /// <summary>
    /// Tests that a new database has the built-in profiles and the drop level bonuses, and that every item has a profile
    /// whose level bonus tables the configuration has (0.75 and 0.95d have no third wings and their table).
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase(Version075)]
    [TestCase(Version095d)]
    [TestCase(Season6)]
    public async Task NewDatabaseHasTheProfilesAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        var missingTables = gameConfiguration.ItemStatProfiles
            .SelectMany(profile => profile.StatTerms.Where(t => t.TableState == ItemStatTermState.Values && t.Table is null).Select(t => $"{profile.Name} {t.Stat}"))
            .ToList();
        Assert.Multiple(() =>
        {
            Assert.That(gameConfiguration.ExcellentDropLevelBonus, Is.EqualTo(25));
            Assert.That(gameConfiguration.AncientDropLevelBonus, Is.EqualTo(30));
            Assert.That(gameConfiguration.ItemStatProfiles.Select(p => p.Name), Is.EquivalentTo(BuiltInItemStatProfiles.Names));
            Assert.That(gameConfiguration.Items.Where(item => item.StatProfile is null || !gameConfiguration.ItemStatProfiles.Contains(item.StatProfile)), Is.Empty);
            Assert.That(missingTables, version == Season6 ? Is.Empty : Is.EquivalentTo(new[] { $"{BuiltInItemStatProfiles.ThirdTierWing} {ItemStat.Defense}" }));
        });
    }

    /// <summary>
    /// Tests that the profiles which compute differently than the default are given exactly to the items for which
    /// the item formulas check it today: armor to items of the armor slots, book to items with a book rise, shield to group 6.
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase(Version075)]
    [TestCase(Version095d)]
    [TestCase(Season6)]
    public async Task ProfilesWhichComputeDifferentlyFollowTodaysChecksAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        var wrongItems = gameConfiguration.Items
            .Where(item => (item.StatProfile!.Name == BuiltInItemStatProfiles.Armor) != IsArmorItem(item)
                           || (item.StatProfile.Name == BuiltInItemStatProfiles.Book) != HasPowerUp(item, Stats.BookRise)
                           || (item.StatProfile.Name == BuiltInItemStatProfiles.Shield) != (item.Group == 6))
            .Select(item => $"{item} has {item.StatProfile}")
            .ToList();
        Assert.That(wrongItems, Is.Empty);
    }

    /// <summary>
    /// Tests that the level bonus table of each item's damage, block rate and defense power-up is the table of its profile.
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase(Version075)]
    [TestCase(Version095d)]
    [TestCase(Season6)]
    public async Task PowerUpTablesAreTheProfileTablesAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        var statsOfAttributes = new Dictionary<AttributeDefinition, ItemStat>
        {
            { Stats.MinimumPhysBaseDmgByWeapon, ItemStat.Damage },
            { Stats.MaximumPhysBaseDmgByWeapon, ItemStat.Damage },
            { Stats.DefenseRatePvm, ItemStat.BlockRate },
            { Stats.DefenseBase, ItemStat.Defense },
            { Stats.DefenseShield, ItemStat.Defense },
        };
        var wrongTables = new List<string>();
        foreach (var item in gameConfiguration.Items)
        {
            foreach (var powerUp in item.BasePowerUpAttributes.Where(p => p.BonusPerLevelTable is not null && statsOfAttributes.ContainsKey(p.TargetAttribute!)))
            {
                var profileTable = GetTable(gameConfiguration, item.StatProfile!, statsOfAttributes[powerUp.TargetAttribute!]);
                if (profileTable != powerUp.BonusPerLevelTable)
                {
                    wrongTables.Add($"{item} ({item.StatProfile}) {powerUp.TargetAttribute}: {powerUp.BonusPerLevelTable!.Name}, profile: {profileTable?.Name}");
                }
            }
        }

        Assert.That(wrongTables, Is.Empty);
    }

    /// <summary>
    /// Tests that each profile is used by an item in the Season 6 data (the older versions have no sceptres, books, capes,
    /// small or third wings).
    /// </summary>
    [Test]
    public async Task EveryProfileIsUsedInSeason6Async()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;

        var unused = gameConfiguration.ItemStatProfiles.Where(profile => gameConfiguration.Items.All(item => item.StatProfile != profile)).Select(p => p.Name);
        Assert.That(unused, Is.Empty);
    }

    /// <summary>
    /// Tests that the default profile and the profiles which compute differently hold the numbers of today's item formulas,
    /// so that a typo in them fails before the game logic reads them. The numbers are those of <c>ItemExtensions.GetRequirement</c>
    /// and its requirement calculations, and of <c>ItemPowerUpFactory.CreateExcellentAndAncientBasePowerUpWrappers</c>.
    /// </summary>
    [Test]
    public void BuiltInProfilesHoldTodaysNumbers()
    {
        static IEnumerable<string> DescribeBuiltIn(string name)
        {
            var profile = new BasicModel.ItemStatProfile { Name = name };
            BuiltInItemStatProfiles.Fill(
                profile,
                () => new BasicModel.ItemStatTerms(),
                () => new BasicModel.ItemRequirementFormula(),
                table => new BasicModel.ItemLevelBonusTable { Name = table.ToString() });
            return DescribeProfile(profile).ToList();
        }

        Assert.Multiple(() =>
        {
            // Requirements: 20 + multiplier * drop level * base / 100, with the multiplier 3 (energy 4), and 4 strength per
            // option level. Excellent: damage base * 25 / drop level + 5, magic power (base * 2 * 25 / drop level + 5) / 2,
            // block rate base * 25 / drop level + 5, defense base * 12 / drop level + drop level / 5 + 4.
            // Ancient: damage 5 + d / 40, magic power (2 + d / 60) / 2, defense 2 + value * 3 / d + d / 30.
            Assert.That(DescribeBuiltIn(ItemStatProfile.DefaultName), Is.EqualTo(new[]
            {
                "default: level per item level 0, excellent level add 0, strength per option level 4",
                "  Damage: table DamageIncreaseWeapons, excellent 25/5/0, ancient 0/5/40",
                "  MagicPower: table None, excellent 25/5/0, ancient 0/2/60, per level after halving 2",
                "  BlockRate: table DefenseRateIncreaseShields, excellent 25/5/0, ancient None",
                "  Defense: table DefenseIncreaseArmors, excellent 12/4/5, ancient 3/2/30",
                "  MagicDefense: table None, excellent None, ancient None",
                "  Strength: add 20, level weight 3, percent 3",
                "  Agility: add 20, level weight 3, percent 3",
                "  Energy: add 20, level weight 3, percent 4",
                "  Vitality: add 20, level weight 3, percent 3",
                "  Leadership: add 20, level weight 3, percent 3",
            }));

            // 5 strength per option level for armor pieces.
            Assert.That(DescribeBuiltIn(BuiltInItemStatProfiles.Armor), Is.EqualTo(new[]
            {
                "armor: level per item level -, excellent level add -, strength per option level 5",
            }));

            // CalculateBookEnergyRequirement: 20 + base * (drop level + level) * 3 / 100.
            Assert.That(DescribeBuiltIn(BuiltInItemStatProfiles.Book), Is.EqualTo(new[]
            {
                "book: level per item level -, excellent level add -, strength per option level -",
                "  Energy: add 20, level weight 1, percent 3",
            }));

            // The shield's ancient defense: 2 + (base + level) * 20 / d, and no excellent defense.
            Assert.That(DescribeBuiltIn(BuiltInItemStatProfiles.Shield), Is.EqualTo(new[]
            {
                "shield: level per item level -, excellent level add -, strength per option level -",
                "  Defense: table DefenseIncreaseShields, excellent None, ancient 20/2/0 after level bonus",
            }));
        });
    }

    /// <summary>
    /// Tests how many items of the Season 6 data get each profile, so that an item doesn't move silently between profiles
    /// which compute alike (e.g. helper and default), which the other tests wouldn't notice.
    /// </summary>
    [Test]
    public async Task Season6ItemCountsPerProfileAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;

        var counts = gameConfiguration.Items
            .GroupBy(item => item.StatProfile!.Name)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => $"{group.Key}: {group.Count()}");
        Assert.That(counts, Is.EqualTo(new[]
        {
            "armor: 274",
            "book: 3",
            "default: 173",
            "firstTierWing: 4",
            "helper: 27",
            "orb: 12",
            "sceptre: 10",
            "scroll: 36",
            "secondTierCape: 2",
            "secondTierWing: 5",
            "shield: 22",
            "smallWing: 6",
            "thirdTierWing: 7",
            "weapon: 113",
        }));
    }

    /// <summary>
    /// Tests the profile which made-up items get, e.g. items which a server owner adds in the admin panel.
    /// </summary>
    [Test]
    public async Task MadeUpItemsGetTheProfileOfTheirKindAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var handSlots = gameConfiguration.Items.First(item => item.Group == 0).ItemSlot;
        var helmSlot = gameConfiguration.Items.First(item => item.Group == 7).ItemSlot;
        var wingsSlot = gameConfiguration.ItemSlotTypes.First(type => type.ItemSlots.Contains(InventoryConstants.WingsSlot));
        var ringSlots = gameConfiguration.ItemSlotTypes.First(type => type.ItemSlots.Contains(InventoryConstants.Ring1Slot));

        ItemDefinition Create(byte group, short number, ItemSlotType? slot, AttributeDefinition? powerUp = null, params (AttributeDefinition Attribute, int Value, byte? FromLevel)[] requirements)
        {
            var item = context.CreateNew<ItemDefinition>();
            item.Group = group;
            item.Number = number;
            item.ItemSlot = slot;
            if (powerUp is not null)
            {
                var basePowerUp = context.CreateNew<ItemBasePowerUpDefinition>();
                basePowerUp.TargetAttribute = powerUp;
                item.BasePowerUpAttributes.Add(basePowerUp);
            }

            foreach (var (attribute, value, fromLevel) in requirements)
            {
                var requirement = context.CreateNew<AttributeRequirement>();
                requirement.Attribute = attribute;
                requirement.MinimumValue = value;
                requirement.MinimumItemLevel = fromLevel;
                item.Requirements.Add(requirement);
            }

            return item;
        }

        var ammunition = Create(4, 200, handSlots);
        ammunition.IsAmmunition = true;
        var cases = new (string Case, ItemDefinition Item, string ExpectedProfile)[]
        {
            ("sword", Create(0, 200, handSlots), BuiltInItemStatProfiles.Weapon),
            ("ammunition", ammunition, ItemStatProfile.DefaultName),
            ("sceptre in group 2", Create(2, 200, handSlots, Stats.ScepterRise), BuiltInItemStatProfiles.Sceptre),
            ("book in group 5", Create(5, 200, handSlots, Stats.BookRise), BuiltInItemStatProfiles.Book),
            ("shield", Create(6, 200, handSlots), BuiltInItemStatProfiles.Shield),
            ("copied helm", Create(7, 200, helmSlot), BuiltInItemStatProfiles.Armor),
            ("second wing", Create(12, 3, wingsSlot), BuiltInItemStatProfiles.SecondTierWing),
            ("unknown wing", Create(12, 200, wingsSlot), ItemStatProfile.DefaultName),
            ("orb", Create(12, 201, null, null, (Stats.TotalEnergy, 100, null)), BuiltInItemStatProfiles.Orb),
            ("jewel", Create(12, 202, null), ItemStatProfile.DefaultName),
            ("scroll", Create(15, 200, null, null, (Stats.Level, 50, null)), BuiltInItemStatProfiles.Scroll),
            ("ring with a level requirement", Create(13, 200, ringSlots, null, (Stats.Level, 50, null)), BuiltInItemStatProfiles.Helper),
            ("ring without a requirement", Create(13, 201, ringSlots), ItemStatProfile.DefaultName),
            ("ring with a level requirement by item level", Create(13, 202, ringSlots, null, (Stats.Level, 20, null), (Stats.Level, 50, 3)), ItemStatProfile.DefaultName),
            ("horn of fenrir", Create(13, 37, null, null, (Stats.Level, 300, null)), ItemStatProfile.DefaultName),
        };

        Assert.That(
            cases.Where(c => ItemStatProfiles.GetProfileName(c.Item) != c.ExpectedProfile).Select(c => $"{c.Case}: {ItemStatProfiles.GetProfileName(c.Item)}"),
            Is.Empty);
    }

    /// <summary>
    /// Tests that the update gives a database without profiles the same profiles and item profiles as a new database has,
    /// and that applying it a second time changes nothing.
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase(Version075)]
    [TestCase(Version095d)]
    [TestCase(Season6)]
    public async Task UpdateAddsTheProfilesAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;
        var expected = Describe(gameConfiguration);

        foreach (var item in gameConfiguration.Items)
        {
            item.StatProfile = null;
        }

        gameConfiguration.ItemStatProfiles.Clear();
        var update = CreateUpdate(version);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var afterFirstUpdate = Describe(gameConfiguration);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(afterFirstUpdate, Is.EqualTo(expected));
            Assert.That(Describe(gameConfiguration), Is.EqualTo(expected));
        });
    }

    /// <summary>
    /// Tests that the update keeps profiles which exist and the profiles which items have,
    /// and only adds a missing profile and gives it to the items without one.
    /// </summary>
    [Test]
    public async Task UpdateKeepsExistingProfilesAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var expected = Describe(gameConfiguration);
        var armor = gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.Armor);
        var shield = gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.Shield);
        var orb = gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.Orb);
        var sword = gameConfiguration.Items.First(item => item.Group == 0);
        armor.StrengthPerOptionLevel = 9;
        sword.StatProfile = shield;
        foreach (var item in gameConfiguration.Items.Where(item => item.StatProfile == orb))
        {
            item.StatProfile = null;
        }

        gameConfiguration.ItemStatProfiles.Remove(orb);

        await CreateUpdate(Season6).ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.Armor), Is.SameAs(armor));
            Assert.That(armor.StrengthPerOptionLevel, Is.EqualTo(9));
            Assert.That(sword.StatProfile, Is.SameAs(shield));
        });
        armor.StrengthPerOptionLevel = 5;
        sword.StatProfile = gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.Weapon);
        Assert.That(Describe(gameConfiguration), Is.EqualTo(expected));
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateAsync(string version)
    {
        return version switch
        {
            Version075 => await TestGameConfiguration.Create075Async().ConfigureAwait(false),
            Version095d => await TestGameConfiguration.Create095dAsync().ConfigureAwait(false),
            _ => await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false),
        };
    }

    private static AddItemStatProfilesUpdatePlugInBase CreateUpdate(string version)
    {
        return version switch
        {
            Version075 => new AddItemStatProfilesUpdatePlugIn075(),
            Version095d => new AddItemStatProfilesUpdatePlugIn095d(),
            _ => new AddItemStatProfilesUpdatePlugInSeasonSix(),
        };
    }

    private static bool IsArmorItem(ItemDefinition item)
    {
        return item.ItemSlot?.ItemSlots.Any(slot => slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot) ?? false;
    }

    private static bool HasPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute == attribute);
    }

    private static ItemLevelBonusTable? GetTable(GameConfiguration gameConfiguration, ItemStatProfile profile, ItemStat stat)
    {
        var terms = profile.StatTerms.FirstOrDefault(t => t.Stat == stat && t.TableState != ItemStatTermState.NotWritten)
                    ?? gameConfiguration.ItemStatProfiles.Single(p => p.Name == ItemStatProfile.DefaultName).StatTerms.Single(t => t.Stat == stat);
        return terms.TableState == ItemStatTermState.Values ? terms.Table : null;
    }

    private static string Describe(GameConfiguration gameConfiguration)
    {
        var profiles = gameConfiguration.ItemStatProfiles
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .SelectMany(DescribeProfile);
        var items = gameConfiguration.Items
            .OrderBy(item => item.Group).ThenBy(item => item.Number)
            .Select(item => $"{item.Group}/{item.Number}: {item.StatProfile?.Name}");
        return string.Join(Environment.NewLine, profiles.Concat(items));
    }

    /// <summary>
    /// Describes a profile: a line with its own parts, then a line per stat and per requirement formula it writes.
    /// A part which isn't written shows "-".
    /// </summary>
    private static IEnumerable<string> DescribeProfile(ItemStatProfile profile)
    {
        static string Part(ItemStatTermState state, int times, int add, int divisor) =>
            state == ItemStatTermState.Values ? $"{times}/{add}/{divisor}" : state.ToString();

        yield return $"{profile.Name}: level per item level {profile.LevelPerItemLevel?.ToString() ?? "-"}, "
                     + $"excellent level add {profile.ExcellentLevelAdd?.ToString() ?? "-"}, "
                     + $"strength per option level {profile.StrengthPerOptionLevel?.ToString() ?? "-"}";
        foreach (var terms in profile.StatTerms.OrderBy(t => t.Stat))
        {
            var table = terms.TableState == ItemStatTermState.Values
                ? terms.Table?.Name.ValueInNeutralLanguage ?? "missing"
                : terms.TableState + (terms.Table is { } stray ? $" ({stray.Name.ValueInNeutralLanguage})" : string.Empty);
            yield return $"  {terms.Stat}: table {table}, "
                         + $"excellent {Part(terms.ExcellentState, terms.ExcellentTimes, terms.ExcellentAdd, terms.ExcellentDropLevelDivisor)}, "
                         + $"ancient {Part(terms.AncientState, terms.AncientTimes, terms.AncientAdd, terms.AncientDropLevelDivisor)}"
                         + (terms.AncientAfterLevelBonus ? " after level bonus" : string.Empty)
                         + (terms.PerLevelAfterHalving is { } perLevel ? $", per level after halving {perLevel}" : string.Empty);
        }

        foreach (var formula in profile.RequirementFormulas.OrderBy(f => f.Stat))
        {
            yield return $"  {formula.Stat}: "
                         + (formula.KeepBase ? "base" : $"add {formula.Add}, level weight {formula.LevelWeight}, percent {formula.Percent}")
                         + (formula.LevelRequirementBasis ? ", level requirement basis" : string.Empty);
        }
    }
}
