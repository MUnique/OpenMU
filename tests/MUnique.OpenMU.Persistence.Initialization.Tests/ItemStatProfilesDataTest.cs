// <copyright file="ItemStatProfilesDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
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

    private readonly Dictionary<string, (IContext Context, GameConfiguration GameConfiguration)> _readOnlyData = [];

    /// <summary>
    /// Creates the data of each version once for the tests which only read it.
    /// The tests which change the data create their own.
    /// </summary>
    [OneTimeSetUp]
    public async Task CreateReadOnlyDataAsync()
    {
        foreach (var version in new[] { Version075, Version095d, Season6 })
        {
            this._readOnlyData[version] = await CreateAsync(version).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Disposes the contexts of the data which the tests only read.
    /// </summary>
    [OneTimeTearDown]
    public void DisposeReadOnlyData()
    {
        foreach (var (context, _) in this._readOnlyData.Values)
        {
            context.Dispose();
        }
    }

    /// <summary>
    /// Tests that a new database has the built-in profiles and the drop level bonuses, and that every item has a profile
    /// whose level bonus tables the configuration has (0.75 and 0.95d have no third wings and their table).
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase(Version075)]
    [TestCase(Version095d)]
    [TestCase(Season6)]
    public void NewDatabaseHasTheProfiles(string version)
    {
        var gameConfiguration = this._readOnlyData[version].GameConfiguration;

        var missingTables = gameConfiguration.ItemStatProfiles
            .SelectMany(profile => profile.StatTerms.Where(t => t.TableState == ItemStatTermState.Values && t.Table is null).Select(t => $"{profile.Name} {t.Attribute?.Designation}"))
            .ToList();
        Assert.Multiple(() =>
        {
            Assert.That(gameConfiguration.ExcellentDropLevelBonus, Is.EqualTo(25));
            Assert.That(gameConfiguration.AncientDropLevelBonus, Is.EqualTo(30));
            Assert.That(gameConfiguration.ItemStatProfiles.Select(p => p.Name), Is.EquivalentTo(BuiltInItemStatProfiles.Names));
            Assert.That(gameConfiguration.Items.Where(item => item.StatProfile is null || !gameConfiguration.ItemStatProfiles.Contains(item.StatProfile)), Is.Empty);
            Assert.That(missingTables, version == Season6 ? Is.Empty : Is.EquivalentTo(new[] { $"{BuiltInItemStatProfiles.ThirdTierWing} {Stats.DefenseBase.Designation}" }));
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
    public void ProfilesWhichComputeDifferentlyFollowTodaysChecks(string version)
    {
        var gameConfiguration = this._readOnlyData[version].GameConfiguration;

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
    public void PowerUpTablesAreTheProfileTables(string version)
    {
        var gameConfiguration = this._readOnlyData[version].GameConfiguration;

        Assert.That(GetTablesWhichDifferFromTheProfile(gameConfiguration), Is.Empty);
    }

    /// <summary>
    /// Tests that each profile is used by an item in the Season 6 data (the older versions have no sceptres, books, capes,
    /// small or third wings).
    /// </summary>
    [Test]
    public void EveryProfileIsUsedInSeason6()
    {
        var gameConfiguration = this._readOnlyData[Season6].GameConfiguration;

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
                table => new BasicModel.ItemLevelBonusTable { Name = table },
                attribute => attribute);
            return DescribeProfile(profile).ToList();
        }

        Assert.Multiple(() =>
        {
            // Requirements: 20 + multiplier * drop level * base / 100, with the multiplier 3 (energy 4), and 4 strength per
            // option level. Excellent: damage base * 25 / drop level + 5, magic power (base * 2 * 25 / drop level + 5) / 2,
            // block rate base * 25 / drop level + 5, defense base * 12 / drop level + drop level / 5 + 4.
            // Ancient: damage 5 + d / 40, magic power (2 + d / 60) / 2, defense 2 + value * 3 / d + d / 30.
            // The damage is the minimum physical damage, the magic power the staff, sceptre and book rise.
            Assert.That(DescribeBuiltIn(ItemStatProfile.DefaultName), Is.EqualTo(new[]
            {
                "default: level per item level 0, excellent level add 0, strength per option level 4",
                "  Base Defense: table DefenseIncreaseArmors, excellent 12/4/5, ancient 3/2/30",
                "  Book Rise Percentage: table None, excellent 25/5/0, ancient 0/2/60, per level after halving 2",
                "  Defense Rate (PvM): table DefenseRateIncreaseShields, excellent 25/5/0, ancient None",
                "  Minimum Physical Base Damage By Weapon: table DamageIncreaseWeapons, excellent 25/5/0, ancient 0/5/40",
                "  Scepter Rise Percentage: table None, excellent 25/5/0, ancient 0/2/60, per level after halving 2",
                "  Staff Rise Percentage: table None, excellent 25/5/0, ancient 0/2/60, per level after halving 2",
                "  Total Agility Requirement Value: add 20, level weight 3, percent 3",
                "  Total Energy Requirement Value: add 20, level weight 3, percent 4",
                "  Total Leadership Requirement Value: add 20, level weight 3, percent 3",
                "  Total Strength Requirement Value: add 20, level weight 3, percent 3",
                "  Total Vitality Requirement Value: add 20, level weight 3, percent 3",
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
                "  Total Energy Requirement Value: add 20, level weight 1, percent 3",
            }));

            // The shield's ancient defense: 2 + (base + level) * 20 / d, and no excellent defense; on its shield defense.
            Assert.That(DescribeBuiltIn(BuiltInItemStatProfiles.Shield), Is.EqualTo(new[]
            {
                "shield: level per item level -, excellent level add -, strength per option level -",
                "  Shield Defense (item): table DefenseIncreaseShields, excellent None, ancient 20/2/0 after level bonus",
            }));
        });
    }

    /// <summary>
    /// Tests how many items of the Season 6 data get each profile, so that an item doesn't move silently between profiles
    /// which compute alike (e.g. helper and default), which the other tests wouldn't notice.
    /// </summary>
    [Test]
    public void Season6ItemCountsPerProfile()
    {
        var gameConfiguration = this._readOnlyData[Season6].GameConfiguration;

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
    public void MadeUpItemsGetTheProfileOfTheirKind()
    {
        var (context, gameConfiguration) = this._readOnlyData[Season6];
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

        RemoveProfiles(gameConfiguration);
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

    /// <summary>
    /// Tests that the update finds the level bonus tables of the profiles by the items which use them,
    /// also when an owner renamed the tables in the admin panel.
    /// </summary>
    [Test]
    public async Task UpdateFindsRenamedTablesAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var expected = DescribeTables(gameConfiguration);
        foreach (var table in gameConfiguration.ItemLevelBonusTables)
        {
            table.Name = $"Renamed {table.Name.ValueInNeutralLanguage}";
        }

        RemoveProfiles(gameConfiguration);
        await CreateUpdate(Season6).ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(DescribeTables(gameConfiguration), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the update takes the level bonus table which the items use, when an owner gave them another table
    /// than the one with the built-in name.
    /// </summary>
    [Test]
    public async Task UpdateTakesTheTableWhichTheItemsUseAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var thirdWings = GetItemsOfProfile(gameConfiguration, BuiltInItemStatProfiles.ThirdTierWing);
        var ownTable = AddTable(context, gameConfiguration, "Own defense bonus of the third wings");
        foreach (var item in thirdWings)
        {
            GiveTable(item, ownTable, Stats.DefenseBase);
        }

        RemoveProfiles(gameConfiguration);
        await CreateUpdate(Season6).ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var profile = gameConfiguration.ItemStatProfiles.Single(p => p.Name == BuiltInItemStatProfiles.ThirdTierWing);
        Assert.That(profile.StatTerms.Single(t => t.Attribute == Stats.DefenseBase).Table, Is.SameAs(ownTable));
    }

    /// <summary>
    /// Tests that the update keeps a third wing which an owner gave another level bonus table than the other third wings:
    /// it gets a variant of the profile which names its table, the profile keeps the table of the others,
    /// and no item's table changes. A second run changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateGivesAnItemWithAnotherTableAVariantOfItsProfileAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var thirdWings = GetItemsOfProfile(gameConfiguration, BuiltInItemStatProfiles.ThirdTierWing);
        var builtInTable = GetProfile(gameConfiguration, BuiltInItemStatProfiles.ThirdTierWing).StatTerms.Single(t => t.Attribute == Stats.DefenseBase).Table!;
        var ownTable = AddTable(context, gameConfiguration, "Own defense bonus of one third wing");
        GiveTable(thirdWings[0], ownTable, Stats.DefenseBase);
        var powerUpTables = DescribePowerUpTables(gameConfiguration);

        RemoveProfiles(gameConfiguration);
        var update = CreateUpdate(Season6);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var afterFirstUpdate = Describe(gameConfiguration);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var profile = GetProfile(gameConfiguration, BuiltInItemStatProfiles.ThirdTierWing);
        var variantName = BuiltInItemStatProfiles.ThirdTierWing + "2";
        var expectedVariant = DescribeProfile(profile)
            .Select(line => line.Replace($"{BuiltInItemStatProfiles.ThirdTierWing}:", $"{variantName}:", StringComparison.Ordinal)
                .Replace(builtInTable.Name.ValueInNeutralLanguage, ownTable.Name.ValueInNeutralLanguage, StringComparison.Ordinal));
        Assert.Multiple(() =>
        {
            Assert.That(profile.StatTerms.Single(t => t.Attribute == Stats.DefenseBase).Table, Is.SameAs(builtInTable));
            Assert.That(thirdWings[0].StatProfile!.Name, Is.EqualTo(variantName));
            Assert.That(DescribeProfile(thirdWings[0].StatProfile!), Is.EqualTo(expectedVariant));
            Assert.That(thirdWings.Skip(1).Select(item => item.StatProfile), Is.All.SameAs(profile));
            Assert.That(DescribePowerUpTables(gameConfiguration), Is.EqualTo(powerUpTables));
            Assert.That(GetTablesWhichDifferFromTheProfile(gameConfiguration), Is.Empty);
            Assert.That(Describe(gameConfiguration), Is.EqualTo(afterFirstUpdate));
        });
    }

    /// <summary>
    /// Tests the same for a table which the items get from the default profile: a sword which an owner gave another
    /// damage table gets a variant of the weapon profile which names it, and the default keeps its table.
    /// </summary>
    [Test]
    public async Task UpdateGivesAWeaponWithAnotherDamageTableAVariantOfItsProfileAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var sword = GetItemsOfProfile(gameConfiguration, BuiltInItemStatProfiles.Weapon)
            .First(item => item.Group == 0 && item.BasePowerUpAttributes.Any(p => p.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon && p.BonusPerLevelTable is not null));
        var builtInTable = GetProfile(gameConfiguration, ItemStatProfile.DefaultName).StatTerms.Single(t => t.Attribute == Stats.MinimumPhysBaseDmgByWeapon).Table!;
        var ownTable = AddTable(context, gameConfiguration, "Own damage increase of one sword");
        GiveTable(sword, ownTable, Stats.MinimumPhysBaseDmgByWeapon, Stats.MaximumPhysBaseDmgByWeapon);
        var powerUpTables = DescribePowerUpTables(gameConfiguration);

        RemoveProfiles(gameConfiguration);
        await CreateUpdate(Season6).ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(GetProfile(gameConfiguration, ItemStatProfile.DefaultName).StatTerms.Single(t => t.Attribute == Stats.MinimumPhysBaseDmgByWeapon).Table, Is.SameAs(builtInTable));
            Assert.That(DescribeProfile(sword.StatProfile!), Is.EqualTo(new[]
            {
                "weapon2: level per item level -, excellent level add -, strength per option level -",
                "  Minimum Physical Base Damage By Weapon: table Own damage increase of one sword, excellent NotWritten, ancient NotWritten",
            }));
            Assert.That(DescribePowerUpTables(gameConfiguration), Is.EqualTo(powerUpTables));
            Assert.That(GetTablesWhichDifferFromTheProfile(gameConfiguration), Is.Empty);
        });
    }

    /// <summary>
    /// Tests that the update gives an item which has its own tables for two attributes, whose terms come from different
    /// profiles, one variant which names both: a shield with its own defense rate table (of the default profile) and its
    /// own shield defense table (of the shield profile). A second run changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateGivesAnItemWithTwoOwnTablesOneVariantAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var shield = GetItemsOfProfile(gameConfiguration, BuiltInItemStatProfiles.Shield)
            .OrderBy(item => item.Number)
            .First(item => HasTable(item, Stats.DefenseRatePvm) && HasTable(item, Stats.DefenseShield));
        GiveTable(shield, AddTable(context, gameConfiguration, "Own defense rate increase of one shield"), Stats.DefenseRatePvm);
        GiveTable(shield, AddTable(context, gameConfiguration, "Own defense increase of one shield"), Stats.DefenseShield);
        var powerUpTables = DescribePowerUpTables(gameConfiguration);

        RemoveProfiles(gameConfiguration);
        var update = CreateUpdate(Season6);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var differentTablesAfterFirstUpdate = GetTablesWhichDifferFromTheProfile(gameConfiguration);
        var afterFirstUpdate = Describe(gameConfiguration);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(DescribeProfile(shield.StatProfile!), Is.EqualTo(new[]
            {
                "shield2: level per item level -, excellent level add -, strength per option level -",
                "  Defense Rate (PvM): table Own defense rate increase of one shield, excellent NotWritten, ancient NotWritten",
                "  Shield Defense (item): table Own defense increase of one shield, excellent None, ancient 20/2/0 after level bonus",
            }));
            Assert.That(differentTablesAfterFirstUpdate, Is.Empty);
            Assert.That(GetTablesWhichDifferFromTheProfile(gameConfiguration), Is.Empty);
            Assert.That(DescribePowerUpTables(gameConfiguration), Is.EqualTo(powerUpTables));
            Assert.That(Describe(gameConfiguration), Is.EqualTo(afterFirstUpdate));
        });
    }

    /// <summary>
    /// Tests what the update does with a weapon whose maximum damage uses another table than its minimum damage, which a
    /// profile can't name: the weapon keeps its profile, which names the table of the minimum damage that the item
    /// formulas read, the maximum damage keeps its own table, and only that power-up differs from the profile.
    /// </summary>
    [Test]
    public async Task UpdateKeepsTheProfileOfAWeaponWithAnotherTableOfTheMaximumDamageAsync()
    {
        var (context, gameConfiguration) = await CreateAsync(Season6).ConfigureAwait(false);
        using var _ = context;
        var sword = GetItemsOfProfile(gameConfiguration, BuiltInItemStatProfiles.Weapon)
            .Where(item => item.Group == 0 && HasTable(item, Stats.MinimumPhysBaseDmgByWeapon) && HasTable(item, Stats.MaximumPhysBaseDmgByWeapon))
            .OrderBy(item => item.Number)
            .First();
        GiveTable(sword, AddTable(context, gameConfiguration, "Own maximum damage increase of one sword"), Stats.MaximumPhysBaseDmgByWeapon);
        var powerUpTables = DescribePowerUpTables(gameConfiguration);

        RemoveProfiles(gameConfiguration);
        await CreateUpdate(Season6).ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var differentTables = GetTablesWhichDifferFromTheProfile(gameConfiguration);
        Assert.Multiple(() =>
        {
            Assert.That(sword.StatProfile!.Name, Is.EqualTo(BuiltInItemStatProfiles.Weapon));
            Assert.That(differentTables, Has.Exactly(1).Items);
            Assert.That(differentTables, Has.All.Contains("Own maximum damage increase of one sword"));
            Assert.That(DescribePowerUpTables(gameConfiguration), Is.EqualTo(powerUpTables));
        });
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
            Version095d => new AddItemStatProfilesUpdatePlugIn095D(),
            _ => new AddItemStatProfilesUpdatePlugInSeasonSix(),
        };
    }

    /// <summary>
    /// Brings the configuration to the state of a database which didn't have the profiles yet.
    /// </summary>
    private static void RemoveProfiles(GameConfiguration gameConfiguration)
    {
        foreach (var item in gameConfiguration.Items)
        {
            item.StatProfile = null;
        }

        gameConfiguration.ItemStatProfiles.Clear();
    }

    private static List<ItemDefinition> GetItemsOfProfile(GameConfiguration gameConfiguration, string profileName)
    {
        return gameConfiguration.Items.Where(item => item.StatProfile?.Name == profileName).ToList();
    }

    private static ItemLevelBonusTable AddTable(IContext context, GameConfiguration gameConfiguration, string name)
    {
        var table = context.CreateNew<ItemLevelBonusTable>();
        table.Name = name;
        gameConfiguration.ItemLevelBonusTables.Add(table);
        return table;
    }

    private static void GiveTable(ItemDefinition item, ItemLevelBonusTable table, params AttributeDefinition[] attributes)
    {
        foreach (var powerUp in item.BasePowerUpAttributes.Where(p => attributes.Any(attribute => attribute == p.TargetAttribute) && p.BonusPerLevelTable is not null))
        {
            powerUp.BonusPerLevelTable = table;
        }
    }

    private static bool HasTable(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Any(p => p.TargetAttribute == attribute && p.BonusPerLevelTable is not null);
    }

    private static ItemStatProfile GetProfile(GameConfiguration gameConfiguration, string name)
    {
        return gameConfiguration.ItemStatProfiles.Single(p => p.Name == name);
    }

    /// <summary>
    /// Describes the level bonus table of each power-up of each item, by the table's id, in an order which doesn't depend on the loading.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The description.</returns>
    internal static List<string> DescribePowerUpTables(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items
            .OrderBy(item => item.Group).ThenBy(item => item.Number)
            .SelectMany(item => item.BasePowerUpAttributes
                .Select(p => $"{item.Group}/{item.Number} {p.TargetAttribute?.Designation}: {p.BonusPerLevelTable?.GetId()}")
                .OrderBy(line => line, StringComparer.Ordinal))
            .ToList();
    }

    /// <summary>
    /// Gets the damage, defense rate and defense power-ups whose level bonus table isn't the table of the item's profile.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The power-ups, described.</returns>
    internal static List<string> GetTablesWhichDifferFromTheProfile(GameConfiguration gameConfiguration)
    {
        // The maximum damage gets the table of the terms of the minimum damage.
        var termAttributes = new Dictionary<AttributeDefinition, AttributeDefinition>
        {
            { Stats.MinimumPhysBaseDmgByWeapon, Stats.MinimumPhysBaseDmgByWeapon },
            { Stats.MaximumPhysBaseDmgByWeapon, Stats.MinimumPhysBaseDmgByWeapon },
            { Stats.DefenseRatePvm, Stats.DefenseRatePvm },
            { Stats.DefenseBase, Stats.DefenseBase },
            { Stats.DefenseShield, Stats.DefenseShield },
        };
        var wrongTables = new List<string>();
        foreach (var item in gameConfiguration.Items)
        {
            foreach (var powerUp in item.BasePowerUpAttributes.Where(p => p.BonusPerLevelTable is not null && termAttributes.ContainsKey(p.TargetAttribute!)))
            {
                var profileTable = GetTable(gameConfiguration, item.StatProfile!, termAttributes[powerUp.TargetAttribute!]);
                if (profileTable != powerUp.BonusPerLevelTable)
                {
                    wrongTables.Add($"{item} ({item.StatProfile}) {powerUp.TargetAttribute}: {powerUp.BonusPerLevelTable!.Name}, profile: {profileTable?.Name}");
                }
            }
        }

        return wrongTables;
    }

    /// <summary>
    /// Describes the level bonus table of each stat of each profile by its id, so that a renamed table is the same.
    /// </summary>
    private static List<string> DescribeTables(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemStatProfiles
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .SelectMany(p => p.StatTerms.OrderBy(t => t.Attribute?.Designation, StringComparer.Ordinal).Select(t => $"{p.Name} {t.Attribute?.Designation}: {t.TableState} {t.Table?.GetId()}"))
            .ToList();
    }

    private static bool IsArmorItem(ItemDefinition item)
    {
        return item.ItemSlot?.ItemSlots.Any(slot => slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot) ?? false;
    }

    private static bool HasPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute == attribute);
    }

    private static ItemLevelBonusTable? GetTable(GameConfiguration gameConfiguration, ItemStatProfile profile, AttributeDefinition attribute)
    {
        var terms = profile.StatTerms.FirstOrDefault(t => t.Attribute == attribute && t.TableState != ItemStatTermState.NotWritten)
                    ?? gameConfiguration.ItemStatProfiles.Single(p => p.Name == ItemStatProfile.DefaultName).StatTerms.SingleOrDefault(t => t.Attribute == attribute);
        return terms?.TableState == ItemStatTermState.Values ? terms.Table : null;
    }

    /// <summary>
    /// Describes the profiles and the profile of each item, so that two configurations can be compared.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The description.</returns>
    internal static string Describe(GameConfiguration gameConfiguration)
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
        foreach (var terms in profile.StatTerms.OrderBy(t => t.Attribute?.Designation, StringComparer.Ordinal))
        {
            var table = terms.TableState == ItemStatTermState.Values
                ? terms.Table?.Name.ValueInNeutralLanguage ?? "missing"
                : terms.TableState + (terms.Table is { } stray ? $" ({stray.Name.ValueInNeutralLanguage})" : string.Empty);
            yield return $"  {terms.Attribute?.Designation}: table {table}, "
                         + $"excellent {Part(terms.ExcellentState, terms.ExcellentTimes, terms.ExcellentAdd, terms.ExcellentDropLevelDivisor)}, "
                         + $"ancient {Part(terms.AncientState, terms.AncientTimes, terms.AncientAdd, terms.AncientDropLevelDivisor)}"
                         + (terms.AncientAfterLevelBonus ? " after level bonus" : string.Empty)
                         + (terms.PerLevelAfterHalving is { } perLevel ? $", per level after halving {perLevel}" : string.Empty);
        }

        foreach (var formula in profile.RequirementFormulas.OrderBy(f => f.Attribute?.Designation, StringComparer.Ordinal))
        {
            yield return $"  {formula.Attribute?.Designation}: "
                         + (formula.KeepBase ? "base" : $"add {formula.Add}, level weight {formula.LevelWeight}, percent {formula.Percent}")
                         + (formula.LevelRequirementBasis ? ", level requirement basis" : string.Empty);
        }
    }
}
