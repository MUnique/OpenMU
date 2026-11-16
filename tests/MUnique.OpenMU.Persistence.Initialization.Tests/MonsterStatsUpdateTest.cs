// <copyright file="MonsterStatsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests the Season 6 monster stats of the initialization data and the <see cref="MonsterStatsUpdatePlugInBase"/>
/// variants and <see cref="FixBloodCastleMonsterAttributesUpdatePlugIn"/>, which bring them to existing databases.
/// </summary>
[TestFixture]
internal class MonsterStatsUpdateTest
{
    /// <summary>
    /// Tests that a new database of each version contains the corrected stats of all monsters it has.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task NewDatabaseHasCorrectedStatsAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        var checkedMonsters = AssertStats(gameConfiguration, GetCorrections(version));

        Assert.That(checkedMonsters, Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that the update of each version sets the corrected stats on an existing database which has other values,
    /// and that applying it twice doesn't change anything.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task UpdateSetsCorrectedStatsAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;
        var corrections = GetCorrections(version);
        Spoil(gameConfiguration, corrections);

        var updates = new List<UpdatePlugInBase>
        {
            version switch
            {
                "0.75" => new MonsterStatsUpdatePlugIn075(),
                "0.95d" => new MonsterStatsUpdatePlugIn095D(),
                _ => new MonsterStatsUpdatePlugInSeason6(),
            },
        };
        if (version == "Season 6")
        {
            updates.Add(new FixBloodCastleMonsterAttributesUpdatePlugIn());
        }

        for (var i = 0; i < 2; i++)
        {
            foreach (var update in updates)
            {
                await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            }
        }

        AssertStats(gameConfiguration, corrections);
    }

    /// <summary>
    /// Tests that a new Season 6 database names the boss of Kalima 7 Kundun.
    /// </summary>
    [Test]
    public async Task NewDatabaseNamesKundunAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var kundun = gameConfiguration.Monsters.Single(m => m.Number == MonsterStatsUpdatePlugInBase.KundunNumber);

        Assert.That(kundun.Designation.ValueInNeutralLanguage, Is.EqualTo("Kundun"));
    }

    /// <summary>
    /// Tests that the update renames the boss of Kalima 7 from its previous name to Kundun,
    /// and keeps a name which was changed in the configuration.
    /// </summary>
    /// <param name="previousName">The name of the monster before the update.</param>
    /// <param name="expectedName">The expected name after the update.</param>
    [TestCase(MonsterStatsUpdatePlugInBase.PreviousKundunName, "Kundun")]
    [TestCase("Custom Kundun", "Custom Kundun")]
    public async Task UpdateRenamesKundunAsync(string previousName, string expectedName)
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var kundun = gameConfiguration.Monsters.Single(m => m.Number == MonsterStatsUpdatePlugInBase.KundunNumber);
        kundun.Designation = previousName;

        await new MonsterStatsUpdatePlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(kundun.Designation.ValueInNeutralLanguage, Is.EqualTo(expectedName));
        if (previousName == MonsterStatsUpdatePlugInBase.PreviousKundunName)
        {
            Assert.That(kundun.Designation.SourceKey, Is.EqualTo("MonsterNames/Kundun"));
        }
    }

    private static IReadOnlyList<MonsterStatsCorrection> GetCorrections(string version)
    {
        return version == "Season 6"
            ? [.. MonsterStatsUpdatePlugInBase.Corrections, .. FixBloodCastleMonsterAttributesUpdatePlugIn.Corrections]
            : MonsterStatsUpdatePlugInBase.Corrections;
    }

    private static Task<(IContext Context, GameConfiguration GameConfiguration)> CreateAsync(string version)
    {
        return version switch
        {
            "0.75" => TestGameConfiguration.Create075Async(),
            "0.95d" => TestGameConfiguration.Create095dAsync(),
            _ => TestGameConfiguration.CreateSeason6Async(),
        };
    }

    /// <summary>
    /// Changes every corrected stat of the monsters, like a database which was initialized before.
    /// </summary>
    private static void Spoil(GameConfiguration gameConfiguration, IReadOnlyList<MonsterStatsCorrection> corrections)
    {
        foreach (var correction in corrections)
        {
            if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == correction.Number) is not { } monster)
            {
                continue;
            }

            foreach (var attribute in monster.Attributes)
            {
                attribute.Value += 1;
            }

            monster.MoveRange += 1;
            monster.AttackRange += 1;
            monster.ViewRange += 1;
            monster.MoveDelay += TimeSpan.FromMilliseconds(1);
            monster.AttackDelay += TimeSpan.FromMilliseconds(1);
            monster.RespawnDelay += TimeSpan.FromSeconds(1);
        }
    }

    /// <summary>
    /// Asserts the stats of the corrections on the monsters which the configuration has.
    /// </summary>
    /// <returns>The number of the checked monsters.</returns>
    private static int AssertStats(GameConfiguration gameConfiguration, IReadOnlyList<MonsterStatsCorrection> corrections)
    {
        var count = 0;
        foreach (var correction in corrections)
        {
            if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == correction.Number) is not { } monster)
            {
                continue;
            }

            count++;
            var name = $"monster {correction.Number}";
            AssertAttribute(monster, Stats.Level, correction.Level, name);
            AssertAttribute(monster, Stats.MaximumHealth, correction.MaximumHealth, name);
            AssertAttribute(monster, Stats.MinimumPhysBaseDmg, correction.MinimumPhysicalDamage, name);
            AssertAttribute(monster, Stats.MaximumPhysBaseDmg, correction.MaximumPhysicalDamage, name);
            AssertAttribute(monster, Stats.DefenseBase, correction.Defense, name);
            AssertAttribute(monster, Stats.AttackRatePvm, correction.AttackRate, name);
            AssertAttribute(monster, Stats.DefenseRatePvm, correction.DefenseRate, name);
            Assert.That(monster.MoveRange, Is.EqualTo(correction.MoveRange ?? monster.MoveRange), name);
            Assert.That(monster.AttackRange, Is.EqualTo(correction.AttackRange ?? monster.AttackRange), name);
            Assert.That(monster.ViewRange, Is.EqualTo(correction.ViewRange ?? monster.ViewRange), name);
            Assert.That(monster.MoveDelay.TotalMilliseconds, Is.EqualTo(correction.MoveDelayMilliseconds ?? monster.MoveDelay.TotalMilliseconds), name);
            Assert.That(monster.AttackDelay.TotalMilliseconds, Is.EqualTo(correction.AttackDelayMilliseconds ?? monster.AttackDelay.TotalMilliseconds), name);
            Assert.That(monster.RespawnDelay.TotalSeconds, Is.EqualTo(correction.RespawnDelaySeconds ?? monster.RespawnDelay.TotalSeconds), name);
        }

        return count;
    }

    private static void AssertAttribute(MonsterDefinition monster, AttributeDefinition attributeDefinition, int? expected, string name)
    {
        if (expected is not { } value)
        {
            return;
        }

        var attribute = monster.Attributes.Single(a => a.AttributeDefinition?.Id == attributeDefinition.Id);
        Assert.That(attribute.Value, Is.EqualTo(value), $"{name} {attributeDefinition.Designation}");
    }
}
