// <copyright file="Version097kDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the initialization of version 0.97k.
/// </summary>
[TestFixture]
internal class Version097kDataTest
{
    private GameConfiguration _gameConfiguration = null!;

    /// <summary>
    /// Initializes a new database of version 0.97k.
    /// </summary>
    [OneTimeSetUp]
    public async Task SetUpAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version097k.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        this._gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
    }

    /// <summary>
    /// Tests that the dark wizard, dark knight and fairy elf can evolve to their second class, which can't get created.
    /// </summary>
    /// <param name="firstClass">The first class.</param>
    /// <param name="secondClass">The second class.</param>
    [TestCase(CharacterClassNumber.DarkWizard, CharacterClassNumber.SoulMaster)]
    [TestCase(CharacterClassNumber.DarkKnight, CharacterClassNumber.BladeKnight)]
    [TestCase(CharacterClassNumber.FairyElf, CharacterClassNumber.MuseElf)]
    public void FirstClassEvolvesToSecondClass(CharacterClassNumber firstClass, CharacterClassNumber secondClass)
    {
        var first = this.GetClass(firstClass);
        var second = this.GetClass(secondClass);
        Assert.That(first.NextGenerationClass, Is.SameAs(second));
        Assert.That(first.CanGetCreated, Is.True);
        Assert.That(second.CanGetCreated, Is.False);
        Assert.That(second.NextGenerationClass, Is.Null);
        Assert.That(second.HomeMap, Is.SameAs(first.HomeMap));
    }

    /// <summary>
    /// Tests that the second classes can use every item and skill which their first classes can use.
    /// </summary>
    /// <param name="firstClass">The first class.</param>
    /// <param name="secondClass">The second class.</param>
    [TestCase(CharacterClassNumber.DarkWizard, CharacterClassNumber.SoulMaster)]
    [TestCase(CharacterClassNumber.DarkKnight, CharacterClassNumber.BladeKnight)]
    [TestCase(CharacterClassNumber.FairyElf, CharacterClassNumber.MuseElf)]
    public void SecondClassIsQualifiedLikeFirstClass(CharacterClassNumber firstClass, CharacterClassNumber secondClass)
    {
        var first = this.GetClass(firstClass);
        var second = this.GetClass(secondClass);
        var itemsWithoutSecondClass = this._gameConfiguration.Items
            .Where(item => item.QualifiedCharacters.Contains(first) && !item.QualifiedCharacters.Contains(second))
            .Select(item => item.Name.ValueInNeutralLanguage);
        var skillsWithoutSecondClass = this._gameConfiguration.Skills
            .Where(skill => skill.QualifiedCharacters.Contains(first) && !skill.QualifiedCharacters.Contains(second))
            .Select(skill => skill.Name.ValueInNeutralLanguage);

        Assert.That(itemsWithoutSecondClass, Is.Empty);
        Assert.That(skillsWithoutSecondClass, Is.Empty);
    }

    /// <summary>
    /// Tests that each second wing is usable by its second class and can be crafted with the Loch's Feather.
    /// </summary>
    /// <param name="wingNumber">The item number of the wing.</param>
    /// <param name="characterClass">The qualified character class.</param>
    [TestCase(3, CharacterClassNumber.MuseElf)]
    [TestCase(4, CharacterClassNumber.SoulMaster)]
    [TestCase(5, CharacterClassNumber.BladeKnight)]
    [TestCase(6, CharacterClassNumber.MagicGladiator)]
    public void SecondWingsCanBeCrafted(short wingNumber, CharacterClassNumber characterClass)
    {
        var wing = this._gameConfiguration.Items.Single(item => item.Group == 12 && item.Number == wingNumber);
        Assert.That(wing.QualifiedCharacters, Is.EquivalentTo(new[] { this.GetClass(characterClass) }));

        var crafting = this._gameConfiguration.Monsters.SelectMany(m => m.ItemCraftings).Single(c => c.Number == 7);
        var settings = crafting.SimpleCraftingSettings!;
        Assert.That(settings.ResultItems.Select(r => r.ItemDefinition), Does.Contain(wing));
        Assert.That(settings.RequiredItems.SelectMany(r => r.PossibleItems), Does.Contain(this._gameConfiguration.Items.Single(item => item.Group == 13 && item.Number == 14)));
    }

    /// <summary>
    /// Tests that the six levels of Blood Castle are defined with their maps, entrances and the Messenger of Archangel in Devias.
    /// </summary>
    [Test]
    public void BloodCastleIsDefined()
    {
        var bloodCastles = this._gameConfiguration.MiniGameDefinitions.Where(m => m.Type == MiniGameType.BloodCastle).OrderBy(m => m.GameLevel).ToList();

        Assert.That(bloodCastles.Select(m => (int)m.GameLevel), Is.EqualTo(Enumerable.Range(1, 6)));
        Assert.That(bloodCastles.Select(m => (int)m.Entrance!.Map!.Number), Is.EqualTo(Enumerable.Range(11, 6)));
        Assert.That(bloodCastles.Select(m => m.TicketItem), Has.All.Matches<ItemDefinition>(item => item is { Group: 13, Number: 18 }));
        Assert.That(bloodCastles.Last().MaximumCharacterLevel, Is.EqualTo(this._gameConfiguration.MaximumLevel));

        var devias = this._gameConfiguration.Maps.Single(m => m.Number == 2);
        Assert.That(devias.MonsterSpawns.Any(spawn => spawn.MonsterDefinition?.Number == 233), Is.True);
        Assert.That(this._gameConfiguration.Monsters.SelectMany(m => m.ItemCraftings).Any(c => c.Number == 8), Is.True);
    }

    /// <summary>
    /// Tests that Sevina the Priestess offers the quests for the evolution to the second class.
    /// </summary>
    /// <param name="characterClass">The first character class.</param>
    [TestCase(CharacterClassNumber.DarkWizard)]
    [TestCase(CharacterClassNumber.DarkKnight)]
    [TestCase(CharacterClassNumber.FairyElf)]
    public void SevinaOffersEvolutionQuests(CharacterClassNumber characterClass)
    {
        var sevina = this._gameConfiguration.Monsters.Single(m => m.Number == 235);
        var quests = sevina.Quests.Where(q => q.QualifiedCharacter == this.GetClass(characterClass)).OrderBy(q => q.Number).ToList();

        Assert.That(quests.Select(q => (int)q.Number), Is.EqualTo(new[] { 0, 1 }));
        Assert.That(quests.Last().Rewards.Select(r => r.RewardType), Does.Contain(QuestRewardType.CharacterEvolutionFirstToSecond));
        Assert.That(quests.SelectMany(q => q.RequiredItems).Select(r => r.Item), Has.All.Not.Null);
    }

    private CharacterClass GetClass(CharacterClassNumber number)
    {
        return this._gameConfiguration.CharacterClasses.Single(c => c.Number == (byte)number);
    }
}
