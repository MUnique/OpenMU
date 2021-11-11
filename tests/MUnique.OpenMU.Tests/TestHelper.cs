﻿// <copyright file="TestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Some helper functions to create test objects.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Gets the player.
    /// </summary>
    /// <returns>The test player.</returns>
    public static Player CreatePlayer()
    {
        var gameConfig = new Mock<GameConfiguration>();
        gameConfig.SetupAllProperties();
        gameConfig.Setup(c => c.Maps).Returns(new List<GameMapDefinition>());
        gameConfig.Setup(c => c.Items).Returns(new List<ItemDefinition>());
        gameConfig.Setup(c => c.Skills).Returns(new List<Skill>());
        gameConfig.Setup(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration>());
        var map = new Mock<GameMapDefinition>();
        map.SetupAllProperties();
        map.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        map.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        map.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Object.RecoveryInterval = int.MaxValue;
        gameConfig.Object.Maps.Add(map.Object);

        var mapInitializer = new MapInitializer(gameConfig.Object, new NullLogger<MapInitializer>(), NullDropGenerator.Instance);
        var gameContext = new GameContext(gameConfig.Object, new InMemoryPersistenceContextProvider(), mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null), NullDropGenerator.Instance);
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        return CreatePlayer(gameContext);
    }

    /// <summary>
    /// Gets a test player.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>
    /// The test player.
    /// </returns>
    public static Player CreatePlayer(IGameContext gameContext)
    {
        var characterMock = new Mock<Character>();
        characterMock.SetupAllProperties();
        characterMock.Setup(c => c.LearnedSkills).Returns(new List<SkillEntry>());
        characterMock.Setup(c => c.Attributes).Returns(new List<StatAttribute>());
        characterMock.Setup(c => c.DropItemGroups).Returns(new List<DropItemGroup>());

        var inventoryMock = new Mock<ItemStorage>();
        inventoryMock.Setup(i => i.Items).Returns(new List<Item>());

        var character = characterMock.Object;
        character.Inventory = inventoryMock.Object;
        character.CurrentMap = gameContext.GetMap(0)?.Definition;
        var characterClassMock = new Mock<CharacterClass>();
        characterClassMock.Setup(c => c.StatAttributes).Returns(
            new List<StatAttributeDefinition>
            {
                new (Stats.Level, 0, false),
                new (Stats.BaseStrength, 28, true),
                new (Stats.BaseAgility, 20, true),
                new (Stats.BaseVitality, 25, true),
                new (Stats.BaseEnergy, 10, true),
                new (Stats.CurrentHealth, 0, false),
                new (Stats.CurrentMana, 0, false),
                new (Stats.CurrentShield, 0, false),
            });
        characterClassMock.Setup(c => c.AttributeCombinations).Returns(new List<AttributeRelationship>
        {
            // Params: TargetAttribute, Multiplier, SourceAttribute
            new (Stats.TotalStrength, 1, Stats.BaseStrength),
            new (Stats.TotalAgility, 1, Stats.BaseAgility),
            new (Stats.TotalVitality, 1, Stats.BaseVitality),
            new (Stats.TotalEnergy, 1, Stats.BaseEnergy),

            new (Stats.MaximumAbility, 1, Stats.TotalEnergy),
            new (Stats.MaximumAbility, 0.3f, Stats.TotalVitality),
            new (Stats.MaximumAbility, 0.2f, Stats.TotalAgility),
            new (Stats.MaximumAbility, 0.15f, Stats.TotalStrength),

            new (Stats.MaximumShield, 1.2f, Stats.TotalEnergy),
            new (Stats.MaximumShield, 1.2f, Stats.TotalVitality),
            new (Stats.MaximumShield, 1.2f, Stats.TotalAgility),
            new (Stats.MaximumShield, 1.2f, Stats.TotalStrength),
            new (Stats.MaximumShield, 0.5f, Stats.DefenseBase),

            new (Stats.MaximumMana, 1, Stats.TotalEnergy),
            new (Stats.MaximumMana, 0.5f, Stats.Level),
            new (Stats.MaximumHealth, 2, Stats.Level),
            new (Stats.MaximumHealth, 3, Stats.TotalVitality),
        });
        characterClassMock.Setup(c => c.BaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new (10, Stats.MaximumMana),
            new (35, Stats.MaximumHealth),
            new (2, Stats.SkillMultiplier),
            new (2, Stats.AbilityRecoveryMultiplier),
            new (1, Stats.DamageReceiveDecrement),
            new (1, Stats.AttackDamageIncrease),
            new (1, Stats.MoneyAmountRate),
        });
        character.CharacterClass = characterClassMock.Object;

        foreach (var attributeDef in character.CharacterClass.StatAttributes)
        {
            character.Attributes.Add(new StatAttribute(attributeDef.Attribute, attributeDef.BaseValue));
        }

        var player = new TestPlayer(gameContext) { Account = new Account() };
        player.PlayerState.TryAdvanceTo(PlayerState.LoginScreen);
        player.PlayerState.TryAdvanceTo(PlayerState.Authenticated);
        player.PlayerState.TryAdvanceTo(PlayerState.CharacterSelection);
        player.SelectedCharacter = character;

        return player;
    }

    private class TestPlayer : Player
    {
        public TestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }
}