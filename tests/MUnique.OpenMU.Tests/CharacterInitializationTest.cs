// <copyright file="CharacterInitializationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for <see cref="CharacterInitialization"/>.
/// </summary>
[TestFixture]
public class CharacterInitializationTest
{
    private IContext _context = null!;

    /// <summary>
    /// Sets up a new in-memory persistence context for each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._context = new InMemoryPersistenceContextProvider().CreateNewContext();
    }

    /// <summary>
    /// Disposes the persistence context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// Tests that a new character gets the stat attributes of its class with their base values.
    /// </summary>
    [Test]
    public void StatAttributesAreInitializedWithBaseValues()
    {
        var characterClass = this.CreateCharacterClass();
        var character = this._context.CreateNew<Character>();

        this._context.InitializeNewCharacter(character, characterClass);

        Assert.That(character.CharacterClass, Is.SameAs(characterClass));
        Assert.That(character.Attributes, Has.Count.EqualTo(2));
        Assert.That(character.Attributes.Single(a => a.Definition == Stats.Level).Value, Is.EqualTo(1));
        Assert.That(character.Attributes.Single(a => a.Definition == Stats.BaseStrength).Value, Is.EqualTo(28));
    }

    /// <summary>
    /// Tests that a stat attribute which is defined twice by the class is added only once.
    /// </summary>
    [Test]
    public void DuplicatedStatAttributeDefinitionIsAddedOnce()
    {
        var characterClass = this.CreateCharacterClass();
        characterClass.StatAttributes.Add(this._context.CreateNew<StatAttributeDefinition>(Stats.BaseStrength, 50f, true));
        var character = this._context.CreateNew<Character>();

        this._context.InitializeNewCharacter(character, characterClass);

        Assert.That(character.Attributes.Count(a => a.Definition == Stats.BaseStrength), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a stat attribute which the character already has is kept and not added a second time.
    /// </summary>
    [Test]
    public void ExistingStatAttributeIsKept()
    {
        var characterClass = this.CreateCharacterClass();
        var character = this._context.CreateNew<Character>();
        character.Attributes.Add(this._context.CreateNew<StatAttribute>(Stats.BaseStrength, 100f));

        this._context.InitializeNewCharacter(character, characterClass);

        Assert.That(character.Attributes.Single(a => a.Definition == Stats.BaseStrength).Value, Is.EqualTo(100));
        Assert.That(character.Attributes, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests that a new character starts on the home map of its class, at a spawn gate.
    /// </summary>
    [Test]
    public void CharacterStartsAtSpawnGateOfHomeMap()
    {
        var characterClass = this.CreateCharacterClass();
        var character = this._context.CreateNew<Character>();

        this._context.InitializeNewCharacter(character, characterClass);

        var spawnGate = characterClass.HomeMap!.ExitGates.Single();
        Assert.That(character.CurrentMap, Is.SameAs(characterClass.HomeMap));
        Assert.That(character.PositionX, Is.InRange(spawnGate.X1, spawnGate.X2));
        Assert.That(character.PositionY, Is.InRange(spawnGate.Y1, spawnGate.Y2));
    }

    /// <summary>
    /// Tests that a new character gets an inventory and the default key configuration.
    /// </summary>
    [Test]
    public void InventoryAndKeyConfigurationAreInitialized()
    {
        var characterClass = this.CreateCharacterClass();
        var character = this._context.CreateNew<Character>();

        this._context.InitializeNewCharacter(character, characterClass);

        Assert.That(character.Inventory, Is.Not.Null);
        Assert.That(character.KeyConfiguration, Is.EqualTo(CharacterInitialization.CreateDefaultKeyConfiguration()));
    }

    /// <summary>
    /// Tests that the first unused slot is returned as free character slot.
    /// </summary>
    [Test]
    public void FreeCharacterSlotIsFirstUnusedSlot()
    {
        var configuration = this._context.CreateNew<GameConfiguration>();
        configuration.MaximumCharactersPerAccount = 3;
        var account = this._context.CreateNew<Account>();
        this.AddCharacter(account, 0);
        this.AddCharacter(account, 2);

        Assert.That(account.GetFreeCharacterSlot(configuration), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that there is no free character slot when the account has the maximum number of characters.
    /// </summary>
    [Test]
    public void NoFreeCharacterSlotWhenAllSlotsAreUsed()
    {
        var configuration = this._context.CreateNew<GameConfiguration>();
        configuration.MaximumCharactersPerAccount = 2;
        var account = this._context.CreateNew<Account>();
        this.AddCharacter(account, 0);
        this.AddCharacter(account, 1);

        Assert.That(account.GetFreeCharacterSlot(configuration), Is.Null);
    }

    /// <summary>
    /// Tests that character names are checked against the configured pattern.
    /// </summary>
    /// <param name="name">The name of the character.</param>
    /// <param name="expectedResult">The expected result.</param>
    [TestCase("Hero", true)]
    [TestCase("Hero!", false)]
    [TestCase("ab", false)]
    public void CharacterNameIsCheckedAgainstPattern(string name, bool expectedResult)
    {
        var configuration = this._context.CreateNew<GameConfiguration>();
        configuration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";

        Assert.That(configuration.IsValidCharacterName(name), Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// Tests that any character name is valid when no pattern is configured.
    /// </summary>
    [Test]
    public void CharacterNameIsValidWithoutPattern()
    {
        var configuration = this._context.CreateNew<GameConfiguration>();

        Assert.That(configuration.IsValidCharacterName("Hero!"), Is.True);
    }

    private CharacterClass CreateCharacterClass()
    {
        var homeMap = this._context.CreateNew<GameMapDefinition>();
        var spawnGate = this._context.CreateNew<ExitGate>();
        spawnGate.IsSpawnGate = true;
        spawnGate.X1 = 130;
        spawnGate.X2 = 135;
        spawnGate.Y1 = 120;
        spawnGate.Y2 = 125;
        homeMap.ExitGates.Add(spawnGate);

        var characterClass = this._context.CreateNew<CharacterClass>();
        characterClass.HomeMap = homeMap;
        characterClass.StatAttributes.Add(this._context.CreateNew<StatAttributeDefinition>(Stats.Level, 1f, false));
        characterClass.StatAttributes.Add(this._context.CreateNew<StatAttributeDefinition>(Stats.BaseStrength, 28f, true));
        return characterClass;
    }

    private void AddCharacter(Account account, byte slot)
    {
        var character = this._context.CreateNew<Character>();
        character.CharacterSlot = slot;
        account.Characters.Add(character);
    }
}
