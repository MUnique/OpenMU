// <copyright file="BotGearPlannerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="BotGearPlanner"/> on the real Season 6 item data, together with the split of
/// <see cref="BotProgression.SplitPoints"/> which serves its minimums first.
/// </summary>
[TestFixture]
public class BotGearPlannerTests
{
    private const byte ArmorGroup = 8;
    private const byte ShieldGroup = 6;

    private GameConfiguration _configuration = null!;

    private IGameContext _gameContext = null!;

    /// <summary>
    /// Creates the Season 6 configuration in memory, once for all tests.
    /// </summary>
    /// <returns>The task.</returns>
    [OneTimeSetUp]
    public async Task CreateConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new Persistence.Initialization.VersionSeasonSix.DataInitialization(contextProvider, NullLoggerFactory.Instance)
            .CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        this._configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();

        var mapInitializer = new MapInitializer(this._configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(this._configuration, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        this._gameContext = gameContext;
    }

    /// <summary>
    /// Tests that a worn piece is planned one item level above its current one: the bot takes it off to
    /// apply a jewel and must be able to put it back on after the upgrade succeeded.
    /// </summary>
    [Test]
    public void WornPieceIsPlannedOneLevelUp()
    {
        var wizard = this.GetClass(BotClassNumbers.DarkWizardNumber);
        var armor = Wear(this.GetQualified(wizard, ArmorGroup).OrderBy(d => d.DropLevel).First(), 5);

        var minimums = BotGearPlanner.GetStatMinimums(this._configuration, wizard, "Aa", 1, [armor], 0);

        var atCurrent = this.InvestedRequirement(armor, wizard, Stats.BaseStrength);
        var upgraded = new TemporaryItem();
        upgraded.AssignValues(armor);
        upgraded.Level = 6;
        Assert.That(minimums[Stats.BaseStrength], Is.EqualTo(this.InvestedRequirement(upgraded, wizard, Stats.BaseStrength)));
        Assert.That(minimums[Stats.BaseStrength], Is.GreaterThan(atCurrent));
    }

    /// <summary>
    /// Tests that a generated veteran of every build can wear its starter set after the split - the
    /// starter gear went on without a requirement check, and a piece taken off for a jewel never came
    /// back on when the stats were too low.
    /// </summary>
    /// <param name="classNumber">The number of the character class.</param>
    /// <param name="variant">The build variant (see <see cref="BotBuild.GetVariant"/>).</param>
    [TestCase(BotClassNumbers.DarkWizardNumber, 0)]
    [TestCase(BotClassNumbers.DarkKnightNumber, 0)]
    [TestCase(BotClassNumbers.DarkKnightNumber, 1)]
    [TestCase(BotClassNumbers.FairyElfNumber, 0)]
    [TestCase(BotClassNumbers.FairyElfNumber, 1)]
    [TestCase(BotClassNumbers.MagicGladiatorNumber, 0)]
    [TestCase(BotClassNumbers.MagicGladiatorNumber, 1)]
    [TestCase(BotClassNumbers.DarkLordNumber, 0)]
    [TestCase(BotClassNumbers.SummonerNumber, 0)]
    [TestCase(BotClassNumbers.RageFighterNumber, 0)]
    public void GeneratedVeteranCanWearItsStarterSet(byte classNumber, int variant)
    {
        var characterClass = this.GetClass(classNumber);
        var name = NameOfVariant(variant);
        const int level = 100;
        var points = level * 5;

        // The lowest qualified piece of each armor kind, at the veteran's starter level +3.
        var worn = new[] { 7, 8, 9, 10, 11 }
            .Select(group => this.GetQualified(characterClass, (byte)group).OrderBy(d => d.DropLevel).FirstOrDefault())
            .OfType<ItemDefinition>()
            .Select(d => Wear(d, 3))
            .ToList();

        var invested = this.Split(characterClass, name, level, worn, points);

        foreach (var item in worn)
        {
            foreach (var (attribute, value) in item.GetRequirements(this._configuration))
            {
                if (BotProgression.TotalToBaseStat(attribute) is { } baseStat)
                {
                    Assert.That(BaseValue(characterClass, baseStat) + invested.GetValueOrDefault(baseStat), Is.GreaterThanOrEqualTo(value), $"{item.Definition!.Name} {baseStat.Designation}");
                }
            }
        }
    }

    /// <summary>
    /// Tests that a supporter elf puts the agility of an armor set aside, and that the gear takes no more
    /// from its build than the planner's budget of half its points.
    /// </summary>
    [Test]
    public void SupportElfPlansAgilityWithinTheBudget()
    {
        var elf = this.GetClass(BotClassNumbers.FairyElfNumber);
        var name = NameOfVariant(1);
        const int level = 150;
        const int points = level * 5;
        var weights = BotProgression.GetStatWeights(elf, name);
        var totalWeight = weights.Sum(w => w.Weight);

        var minimums = BotGearPlanner.GetStatMinimums(this._configuration, elf, name, level, [], points);
        var invested = this.Split(elf, name, level, [], points);
        var takenFromTheBuild = invested.Sum(i => Math.Max(0, i.Value - (points * weights.FirstOrDefault(w => w.Stat == i.Key).Weight / totalWeight)));

        Assert.That(minimums[Stats.BaseAgility], Is.GreaterThan(100));
        Assert.That(invested[Stats.BaseAgility], Is.GreaterThanOrEqualTo(minimums[Stats.BaseAgility]));
        Assert.That(minimums.Values.Sum(), Is.LessThanOrEqualTo(points));
        Assert.That(takenFromTheBuild, Is.LessThanOrEqualTo(points / 2));
    }

    /// <summary>
    /// Tests that a top set beyond the budget is skipped for the next best one: the Titan Armor's
    /// strength is out of reach for a Dark Knight at level 150 and would take its whole build.
    /// </summary>
    [Test]
    public void TopSetBeyondBudgetIsSkipped()
    {
        var knight = this.GetClass(BotClassNumbers.DarkKnightNumber);
        var titan = this.GetQualified(knight, ArmorGroup).OrderByDescending(d => this.InvestedRequirement(Wear(d, 0), knight, Stats.BaseStrength)).First();
        const int level = 150;

        var chosenArmor = this.ChosenOfGroup(knight, NameOfVariant(1), level, level * 5, ArmorGroup);

        Assert.That(titan.DropLevel, Is.LessThanOrEqualTo(level), "precondition: the set drops at this level");
        Assert.That(chosenArmor, Is.Not.Null);
        Assert.That(chosenArmor, Is.Not.SameAs(titan));
        Assert.That(chosenArmor!.DropLevel, Is.LessThan(titan.DropLevel));
    }

    /// <summary>
    /// Tests that the budget grows with the points: with enough of them, the same knight plans for the
    /// top set.
    /// </summary>
    [Test]
    public void TopSetWithinBudgetIsPlanned()
    {
        var knight = this.GetClass(BotClassNumbers.DarkKnightNumber);
        var titan = this.GetQualified(knight, ArmorGroup).OrderByDescending(d => this.InvestedRequirement(Wear(d, 0), knight, Stats.BaseStrength)).First();
        const int level = 150;

        var chosenArmor = this.ChosenOfGroup(knight, NameOfVariant(1), level, 5000, ArmorGroup);

        Assert.That(chosenArmor!.DropLevel, Is.GreaterThanOrEqualTo(titan.DropLevel));
    }

    /// <summary>
    /// Tests that an item whose level requirement the character doesn't meet yet is not planned for,
    /// although its drop level is low enough (the second class sets require level 380).
    /// </summary>
    [Test]
    public void ItemAboveCharacterLevelIsSkipped()
    {
        var bladeKnight = this.GetClass(BotClassNumbers.BladeKnightNumber);
        var levelGated = this.GetQualified(bladeKnight, ArmorGroup)
            .Where(d => this.LevelRequirement(d) > 300)
            .OrderByDescending(d => d.DropLevel)
            .First();
        var gatedLevel = this.LevelRequirement(levelGated);

        var below = BotGearPlanner.ChooseCandidates(this._configuration, bladeKnight, NameOfVariant(0), gatedLevel - 1, 100_000);
        var reached = BotGearPlanner.ChooseCandidates(this._configuration, bladeKnight, NameOfVariant(0), gatedLevel, 100_000);

        Assert.That(levelGated.DropLevel, Is.LessThan(gatedLevel), "precondition: the set drops below its level requirement");
        Assert.That(below.Select(c => c.Definition), Has.No.Member(levelGated));
        Assert.That(reached.Single(c => c.Definition.Group == ArmorGroup).Definition.DropLevel, Is.GreaterThanOrEqualTo(levelGated.DropLevel));
    }

    /// <summary>
    /// Tests that of two sets of a similar tier the one cut for the build is planned: an energy Magic
    /// Gladiator takes its energy set, not the strength set lying a few drop levels above it.
    /// </summary>
    [Test]
    public void SetOfTheBuildIsPreferredOverStrongerSetOfAnotherBuild()
    {
        var gladiator = this.GetClass(BotClassNumbers.MagicGladiatorNumber);
        const int level = 300;
        var points = level * 7;

        var forEnergy = this.ChosenOfGroup(gladiator, NameOfVariant(1), level, points, ArmorGroup)!;
        var forStrength = this.ChosenOfGroup(gladiator, NameOfVariant(0), level, points, ArmorGroup)!;

        Assert.That(forEnergy, Is.Not.SameAs(forStrength));
        Assert.That(
            this.InvestedRequirement(Wear(forEnergy, 0), gladiator, Stats.BaseStrength),
            Is.LessThan(this.InvestedRequirement(Wear(forStrength, 0), gladiator, Stats.BaseStrength)));
    }

    /// <summary>
    /// Tests that a shield counts only while it is worn - many builds have no hand for one.
    /// </summary>
    [Test]
    public void ShieldCountsOnlyWhileWorn()
    {
        var lord = this.GetClass(BotClassNumbers.DarkLordNumber);
        var shield = Wear(this.GetQualified(lord, ShieldGroup).OrderByDescending(d => d.DropLevel).First(), 9);
        const int level = 1;

        var without = BotGearPlanner.GetStatMinimums(this._configuration, lord, "Aa", level, [], 0);
        var with = BotGearPlanner.GetStatMinimums(this._configuration, lord, "Aa", level, [shield], 0);

        Assert.That(without.GetValueOrDefault(Stats.BaseStrength), Is.Zero);
        Assert.That(with[Stats.BaseStrength], Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that a Jewel of Bless is not spent on a worn piece the bot could not put back on one level up
    /// (it takes the piece off for the jewel) - and is, once the stats are there.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask BlessIsOnlySpentOnAPieceWhichStaysWearableAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var baseStrength = (long)player.Attributes![Stats.BaseStrength];
        var armorDefinition = this._configuration.Items
            .Where(d => d.Group == ArmorGroup && d.DropsFromMonsters)
            .OrderBy(d => d.DropLevel)
            .First(d => this.InvestedRequirement(Wear(d, 6), player.SelectedCharacter!.CharacterClass!, Stats.BaseStrength) > 0
                        && this.StrengthRequirement(Wear(d, 6)) > baseStrength);
        var armor = player.PersistenceContext.CreateNew<Item>();
        armor.Definition = armorDefinition;
        armor.Durability = armorDefinition.Durability;
        armor.Level = 5;
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, armor).ConfigureAwait(false);
        var bless = player.PersistenceContext.CreateNew<Item>();
        bless.Definition = this._configuration.Items.Single(d => d.Name.ValueInNeutralLanguage == "Jewel of Bless");
        bless.Durability = 1;
        await player.Inventory.AddItemAsync(bless).ConfigureAwait(false);

        Assert.That(BotJewelHandler.PlanNextUse(player, 1, 0, 0), Is.Null);

        player.Attributes[Stats.BaseStrength] = this.StrengthRequirement(Wear(armorDefinition, 6));

        Assert.That(BotJewelHandler.PlanNextUse(player, 1, 0, 0), Is.EqualTo(((Item, Item)?)(bless, armor)));
    }

    /// <summary>A character name which hits the given build variant (the variant is the parity of the character sum).</summary>
    private static string NameOfVariant(int variant) => variant == 0 ? "Aa" : "Ab";

    private static Item Wear(ItemDefinition definition, byte level) => new TemporaryItem { Definition = definition, Level = level };

    private static long BaseValue(CharacterClass characterClass, AttributeDefinition baseStat)
        => (long)(characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == baseStat)?.BaseValue ?? 0f);

    /// <summary>Plays the generator's split: all points at once, the gear minimums first.</summary>
    private Dictionary<AttributeDefinition, long> Split(CharacterClass characterClass, string name, int level, IReadOnlyList<Item> worn, int points)
    {
        var weights = BotProgression.GetStatWeights(characterClass, name);
        var minimums = BotGearPlanner.GetStatMinimums(this._configuration, characterClass, name, level, worn, points);
        return BotProgression.SplitPoints(points, weights, minimums: minimums).ToDictionary(r => r.Stat, r => (long)r.Amount);
    }

    private ItemDefinition? ChosenOfGroup(CharacterClass characterClass, string name, int level, long totalPoints, byte group)
        => BotGearPlanner.ChooseCandidates(this._configuration, characterClass, name, level, totalPoints)
            .SingleOrDefault(c => c.Definition.Group == group)?.Definition;

    private CharacterClass GetClass(byte number) => this._configuration.CharacterClasses.Single(c => c.Number == number);

    private IEnumerable<ItemDefinition> GetQualified(CharacterClass characterClass, byte group)
        => this._configuration.Items.Where(d => d.Group == group && d.QualifiedCharacters.Contains(characterClass));

    private long InvestedRequirement(Item item, CharacterClass characterClass, AttributeDefinition baseStat)
    {
        var requirement = item.GetRequirements(this._configuration)
            .Where(r => BotProgression.TotalToBaseStat(r.Attr) == baseStat)
            .Select(r => (long)r.Value)
            .DefaultIfEmpty(0)
            .Max();
        return Math.Max(0, requirement - BaseValue(characterClass, baseStat));
    }

    private int StrengthRequirement(Item item)
        => item.GetRequirements(this._configuration).Where(r => r.Attr == Stats.TotalStrength).Select(r => r.Value).DefaultIfEmpty(0).Max();

    private int LevelRequirement(ItemDefinition definition)
        => Wear(definition, 0).GetRequirements(this._configuration).Where(r => r.Attr == Stats.Level).Select(r => r.Value).DefaultIfEmpty(0).Max();
}
