// <copyright file="CharacterCreatedPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="ICharacterCreatedPlugIn"/>s, invoked like the admin panel does it:
/// through the plugin point of a <see cref="PlugInManager"/>, without a player in the game.
/// </summary>
[TestFixture]
public class CharacterCreatedPlugInTest
{
    private const byte DarkKnightNumber = 4;
    private const byte FairyElfNumber = 8;

    private IContext _context = null!;
    private GameConfiguration _gameConfiguration = null!;
    private Account _account = null!;

    /// <summary>
    /// Sets up a game configuration with the items and skills which the tested plugins add.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._context = new InMemoryPersistenceContextProvider().CreateNewContext();
        this._gameConfiguration = this._context.CreateNew<GameConfiguration>();

        var darkKnight = this.AddCharacterClass(DarkKnightNumber);
        this.AddCharacterClass(FairyElfNumber);
        this.AddItemDefinition(group: 1, number: 0); // Small Axe
        this.AddItemDefinition(group: 4, number: 15); // Arrows

        var crescentMoonSlash = this._context.CreateNew<Skill>();
        crescentMoonSlash.Number = 44;
        crescentMoonSlash.QualifiedCharacters.Add(darkKnight);
        this._gameConfiguration.Skills.Add(crescentMoonSlash);

        this._account = this._context.CreateNew<Account>();
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
    /// Tests that the active plugins give a new character its initial item and skill.
    /// </summary>
    [Test]
    public void ActivePlugInsAddInitialItemAndSkill()
    {
        var plugInManager = CreatePlugInManager(typeof(AddSmallAxeForDarkKnight), typeof(AddCrescentMoonSlashForDarkKnight));
        var character = this.CreateCharacter(DarkKnightNumber);

        this.InvokePlugIns(plugInManager, character);

        var axe = character.Inventory!.Items.Single();
        Assert.That(axe.Definition, Is.SameAs(this._gameConfiguration.Items.Single(i => i.Group == 1)));
        Assert.That(axe.ItemSlot, Is.EqualTo(0));
        Assert.That(character.LearnedSkills.Single().Skill?.Number, Is.EqualTo(44));
    }

    /// <summary>
    /// Tests that a deactivated plugin doesn't add its item.
    /// </summary>
    [Test]
    public void DeactivatedPlugInAddsNothing()
    {
        var plugInManager = CreatePlugInManager(typeof(AddSmallAxeForDarkKnight));
        plugInManager.DeactivatePlugIn<AddSmallAxeForDarkKnight>();
        var character = this.CreateCharacter(DarkKnightNumber);

        this.InvokePlugIns(plugInManager, character);

        Assert.That(character.Inventory!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that a plugin for another character class doesn't add its item.
    /// </summary>
    [Test]
    public void PlugInOfOtherCharacterClassAddsNothing()
    {
        var plugInManager = CreatePlugInManager(typeof(AddSmallAxeForDarkKnight));
        var character = this.CreateCharacter(FairyElfNumber);

        this.InvokePlugIns(plugInManager, character);

        Assert.That(character.Inventory!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that a plugin which overrides the item creation is applied, too.
    /// The arrows of the fairy elf get the full durability, which is their amount.
    /// </summary>
    [Test]
    public void ArrowsAreAddedWithFullDurability()
    {
        var plugInManager = CreatePlugInManager(typeof(AddArrowsForFairyElf));
        var character = this.CreateCharacter(FairyElfNumber);

        this.InvokePlugIns(plugInManager, character);

        Assert.That(character.Inventory!.Items.Single().Durability, Is.EqualTo(255));
    }

    private static PlugInManager CreatePlugInManager(params Type[] activePlugIns)
    {
        // Like in the admin panel, all known plugins are registered and active, unless their configuration says otherwise.
        var plugInManager = new PlugInManager(new List<PlugInConfiguration>(), NullLoggerFactory.Instance, null, null);
        foreach (var plugInType in plugInManager.GetKnownPlugInsOf<ICharacterCreatedPlugIn>().Except(activePlugIns).ToList())
        {
            plugInManager.DeactivatePlugIn(plugInType);
        }

        return plugInManager;
    }

    private void InvokePlugIns(PlugInManager plugInManager, Character character)
    {
        plugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()!
            .CharacterCreated(this._account, character, this._context, this._gameConfiguration, NullLogger.Instance);
    }

    private Character CreateCharacter(byte characterClassNumber)
    {
        var character = this._context.CreateNew<Character>();
        character.CharacterClass = this._gameConfiguration.CharacterClasses.Single(c => c.Number == characterClassNumber);
        character.Inventory = this._context.CreateNew<ItemStorage>();
        this._account.Characters.Add(character);
        return character;
    }

    private CharacterClass AddCharacterClass(byte number)
    {
        var characterClass = this._context.CreateNew<CharacterClass>();
        characterClass.Number = number;
        this._gameConfiguration.CharacterClasses.Add(characterClass);
        return characterClass;
    }

    private void AddItemDefinition(byte group, short number)
    {
        var itemDefinition = this._context.CreateNew<ItemDefinition>();
        itemDefinition.Group = group;
        itemDefinition.Number = number;
        itemDefinition.Durability = 20;
        this._gameConfiguration.Items.Add(itemDefinition);
    }
}
