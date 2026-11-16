// <copyright file="MonsterResistancesUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests the elemental resistances of the monsters in the initialization data and the
/// <see cref="MonsterResistancesUpdatePlugInBase"/> variants, which bring them to existing databases.
/// </summary>
[TestFixture]
internal class MonsterResistancesUpdateTest
{
    private static readonly AttributeDefinition[] Resistances =
    [
        Stats.IceResistance,
        Stats.PoisonResistance,
        Stats.LightningResistance,
        Stats.FireResistance,
        Stats.WaterResistance,
        Stats.WindResistance,
        Stats.EarthResistance,
    ];

    /// <summary>
    /// Tests that a new database of each version has whole numbers as resistances of the monsters, no water or wind
    /// resistance, and the corrected resistances.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task NewDatabaseHasCorrectedResistancesAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        var resistances = gameConfiguration.Monsters.SelectMany(m => m.Attributes.Where(IsResistance).Select(a => (Monster: m, Attribute: a))).ToList();
        Assert.That(resistances, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var (monster, attribute) in resistances)
            {
                var name = $"monster {monster.Number} {attribute.AttributeDefinition?.Designation}";
                Assert.That(attribute.Value, Is.EqualTo(MathF.Round(attribute.Value)).And.InRange(1, 255), name);
                Assert.That(attribute.AttributeDefinition, Is.Not.EqualTo(Stats.WaterResistance).And.Not.EqualTo(Stats.WindResistance), name);
            }
        });

        var checkedMonsters = 0;
        foreach (var correction in MonsterResistancesUpdatePlugInBase.Corrections)
        {
            foreach (var monster in gameConfiguration.Monsters.Where(m => m.Number == correction.Number))
            {
                checkedMonsters++;
                Assert.That(GetResistances(monster), Is.EqualTo((correction.Corrected.Ice, correction.Corrected.Poison, correction.Corrected.Lightning, correction.Corrected.Fire, 0, 0)), $"monster {monster.Number}");
            }
        }

        Assert.That(checkedMonsters, Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that the update of each version brings a database with the former resistances (divided by 255, partly
    /// assigned to the wrong elements) to the state of a new database, and that applying it twice doesn't change anything.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task UpdateRestoresResistancesOfNewDatabaseAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;
        var expected = gameConfiguration.Monsters.ToDictionary(m => m, GetResistances);
        SetFormerResistances(context, gameConfiguration);
        Assert.That(gameConfiguration.Monsters.Count(m => GetResistances(m) != expected[m]), Is.GreaterThan(0));

        var update = CreateUpdate(version);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var monster in gameConfiguration.Monsters)
            {
                Assert.That(GetResistances(monster), Is.EqualTo(expected[monster]), $"monster {monster.Number}");
                Assert.That(monster.Attributes.Where(IsResistance).All(a => a.Value == MathF.Round(a.Value)), Is.True, $"monster {monster.Number}");
            }
        });
    }

    /// <summary>
    /// Tests that the update converts customized resistances of the former scale to whole numbers, but keeps them otherwise.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomizedResistancesAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var correction = MonsterResistancesUpdatePlugInBase.Corrections.First(c => gameConfiguration.Monsters.Any(m => m.Number == c.Number));
        var monster = gameConfiguration.Monsters.First(m => m.Number == correction.Number);
        SetResistance(context, gameConfiguration, monster, Stats.IceResistance, 42f / 255);
        SetResistance(context, gameConfiguration, monster, Stats.PoisonResistance, 0);
        SetResistance(context, gameConfiguration, monster, Stats.LightningResistance, 0);
        SetResistance(context, gameConfiguration, monster, Stats.FireResistance, 7f / 255);
        SetResistance(context, gameConfiguration, monster, Stats.WaterResistance, 0);
        SetResistance(context, gameConfiguration, monster, Stats.WindResistance, 0);

        await new MonsterResistancesUpdatePlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(GetResistances(monster), Is.EqualTo((42, 0, 0, 7, 0, 0)));
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

    private static UpdatePlugInBase CreateUpdate(string version)
    {
        return version switch
        {
            "0.75" => new MonsterResistancesUpdatePlugIn075(),
            "0.95d" => new MonsterResistancesUpdatePlugIn095D(),
            _ => new MonsterResistancesUpdatePlugInSeason6(),
        };
    }

    /// <summary>
    /// Brings the resistances of the monsters to the state of the former initialization data: the resistances of the
    /// corrected monsters are the previous ones, and all resistances are divided by 255.
    /// </summary>
    private static void SetFormerResistances(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var correction in MonsterResistancesUpdatePlugInBase.Corrections)
        {
            foreach (var monster in gameConfiguration.Monsters.Where(m => m.Number == correction.Number))
            {
                var previous = correction.Previous;
                SetResistance(context, gameConfiguration, monster, Stats.IceResistance, previous.Ice);
                SetResistance(context, gameConfiguration, monster, Stats.PoisonResistance, previous.Poison);
                SetResistance(context, gameConfiguration, monster, Stats.LightningResistance, previous.Lightning);
                SetResistance(context, gameConfiguration, monster, Stats.FireResistance, previous.Fire);
                SetResistance(context, gameConfiguration, monster, Stats.WaterResistance, previous.Water);
                SetResistance(context, gameConfiguration, monster, Stats.WindResistance, previous.Wind);
            }
        }

        foreach (var attribute in gameConfiguration.Monsters.SelectMany(m => m.Attributes).Where(IsResistance))
        {
            attribute.Value /= 255;
        }
    }

    private static void SetResistance(IContext context, GameConfiguration gameConfiguration, MonsterDefinition monster, AttributeDefinition attributeDefinition, float value)
    {
        var attribute = monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeDefinition.Id);
        if (value == 0)
        {
            if (attribute is not null)
            {
                monster.Attributes.Remove(attribute);
            }
        }
        else if (attribute is not null)
        {
            attribute.Value = value;
        }
        else
        {
            monster.AddAttributes(new Dictionary<AttributeDefinition, float> { { attributeDefinition, value } }, context, gameConfiguration);
        }
    }

    private static (int Ice, int Poison, int Lightning, int Fire, int Water, int Wind) GetResistances(MonsterDefinition monster)
    {
        int Get(AttributeDefinition definition) => monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == definition.Id) is { } attribute
            ? (int)MathF.Round(attribute.Value)
            : 0;

        return (Get(Stats.IceResistance), Get(Stats.PoisonResistance), Get(Stats.LightningResistance), Get(Stats.FireResistance), Get(Stats.WaterResistance), Get(Stats.WindResistance));
    }

    private static bool IsResistance(MonsterAttribute attribute)
    {
        return Resistances.Any(r => r.Id == attribute.AttributeDefinition?.Id);
    }
}
