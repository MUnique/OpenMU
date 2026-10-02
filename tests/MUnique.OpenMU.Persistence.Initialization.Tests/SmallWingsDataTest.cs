// <copyright file="SmallWingsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the small wings and capes (12,130–135) of the season 6 data.
/// </summary>
[TestFixture]
internal class SmallWingsDataTest
{
    private static readonly short[] SmallWingNumbers = [130, 131, 132, 133, 134, 135];

    /// <summary>
    /// Tests that a new season 6 database contains the small wings with the values of the client.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsSmallWingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertSmallWing(gameConfiguration, 130, "Small Cape of Lord", 2, 2, 15, 20, CharacterClassNumber.DarkLord);
        AssertSmallWing(gameConfiguration, 131, "Small Wing of Curse", 3, 2, 10, 12, CharacterClassNumber.Summoner);
        AssertSmallWing(gameConfiguration, 132, "Small Wings of Elf", 3, 2, 10, 12, CharacterClassNumber.FairyElf);
        AssertSmallWing(gameConfiguration, 133, "Small Wings of Heaven", 3, 2, 10, 12, CharacterClassNumber.DarkWizard, CharacterClassNumber.MagicGladiator);
        AssertSmallWing(gameConfiguration, 134, "Small Wings of Satan", 3, 2, 20, 12, CharacterClassNumber.DarkKnight, CharacterClassNumber.MagicGladiator);
        AssertSmallWing(gameConfiguration, 135, "Little Warrior's Cloak", 2, 2, 15, 20, CharacterClassNumber.RageFighter);
    }

    /// <summary>
    /// Tests that the update adds the small wings to a database which was created before they existed,
    /// reusing the level bonus tables of the first wings, and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsSmallWingsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var smallWing in gameConfiguration.Items.Where(IsSmallWing).ToList())
        {
            gameConfiguration.Items.Remove(smallWing);
        }

        var bonusTableCount = gameConfiguration.ItemLevelBonusTables.Count;

        var update = new AddSmallWingsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsSmallWing), Is.EqualTo(SmallWingNumbers.Length));
        Assert.That(gameConfiguration.ItemLevelBonusTables, Has.Count.EqualTo(bonusTableCount));
        AssertSmallWing(gameConfiguration, 130, "Small Cape of Lord", 2, 2, 15, 20, CharacterClassNumber.DarkLord);
        AssertSmallWing(gameConfiguration, 131, "Small Wing of Curse", 3, 2, 10, 12, CharacterClassNumber.Summoner);
        AssertSmallWing(gameConfiguration, 132, "Small Wings of Elf", 3, 2, 10, 12, CharacterClassNumber.FairyElf);
        AssertSmallWing(gameConfiguration, 133, "Small Wings of Heaven", 3, 2, 10, 12, CharacterClassNumber.DarkWizard, CharacterClassNumber.MagicGladiator);
        AssertSmallWing(gameConfiguration, 134, "Small Wings of Satan", 3, 2, 20, 12, CharacterClassNumber.DarkKnight, CharacterClassNumber.MagicGladiator);
        AssertSmallWing(gameConfiguration, 135, "Little Warrior's Cloak", 2, 2, 15, 20, CharacterClassNumber.RageFighter);
    }

    private static bool IsSmallWing(ItemDefinition item)
    {
        return item.Group == 12 && SmallWingNumbers.Contains(item.Number);
    }

    private static void AssertSmallWing(GameConfiguration gameConfiguration, short number, string name, byte width, byte height, float defense, int damagePercent, params CharacterClassNumber[] classes)
    {
        var item = gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == number);
        var wingsOfElf = gameConfiguration.Items.Single(i => i is { Group: 12, Number: 0 });

        Assert.Multiple(() =>
        {
            Assert.That(item.Name.ValueInNeutralLanguage, Is.EqualTo(name));
            Assert.That((item.Width, item.Height), Is.EqualTo((width, height)));
            Assert.That(item.DropLevel, Is.EqualTo(1));
            Assert.That(item.Durability, Is.EqualTo(200));
            Assert.That(item.DropsFromMonsters, Is.False);
            Assert.That(item.ItemSlot, Is.SameAs(wingsOfElf.ItemSlot));
            Assert.That(item.Requirements.Single(r => r.Attribute == Stats.Level).MinimumValue, Is.EqualTo(1));
            Assert.That(item.QualifiedCharacters.Select(c => (CharacterClassNumber)c.Number), Is.SupersetOf(classes));
            Assert.That(item.PossibleItemOptions, Is.Empty);

            Assert.That(GetPowerUp(item, Stats.DefenseBase).BaseValue, Is.EqualTo(defense));
            Assert.That(GetPowerUp(item, Stats.AttackDamageIncrease).BaseValue, Is.EqualTo(1f + (damagePercent / 100f)));
            Assert.That(GetPowerUp(item, Stats.DamageReceiveDecrement).BaseValue, Is.EqualTo(1f - (damagePercent / 100f)));
            Assert.That(GetPowerUp(item, Stats.CanFly).BaseValue, Is.EqualTo(1));
            Assert.That(GetPowerUp(item, Stats.MovementSpeed).BaseValue, Is.EqualTo(GetPowerUp(wingsOfElf, Stats.MovementSpeed).BaseValue));

            // The bonus per level is the same as for the first wings.
            foreach (var attribute in new[] { Stats.DefenseBase, Stats.AttackDamageIncrease, Stats.DamageReceiveDecrement })
            {
                Assert.That(GetPowerUp(item, attribute).BonusPerLevelTable, Is.Not.Null.And.SameAs(GetPowerUp(wingsOfElf, attribute).BonusPerLevelTable), attribute.Designation);
            }

            Assert.That(item.IsTradable, Is.False);
            Assert.That(item.IsDroppable, Is.False);
            Assert.That(item.IsStorable, Is.True);
            Assert.That(item.IsSellableToNpc, Is.False);
            Assert.That(item.IsPersonalStoreSellable, Is.False);
            Assert.That(item.IsRepairable, Is.False);
        });
    }

    private static ItemBasePowerUpDefinition GetPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Single(p => p.TargetAttribute == attribute);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
