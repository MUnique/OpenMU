// <copyright file="TestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.DataModel.Configuration.Items;

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Persistence.InMemory;
    using Rhino.Mocks;

    /// <summary>
    /// Some helper functions to create test objects.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <returns>The test player.</returns>
        public static Player GetPlayer()
        {
            var gameConfig = MockRepository.GenerateStub<GameConfiguration>();
            gameConfig.RecoveryInterval = int.MaxValue;
            gameConfig.Stub(c => c.Maps).Return(new List<GameMapDefinition>());
            gameConfig.Stub(c => c.Items).Return(new List<ItemDefinition>());
            var map = MockRepository.GenerateStub<GameMapDefinition>();
            map.Stub(m => m.DropItemGroups).Return(new List<DropItemGroup>());
            map.Stub(m => m.MonsterSpawns).Return(new List<MonsterSpawnArea>());
            gameConfig.Maps.Add(map);

            var mapInitializer = new MapInitializer(gameConfig, null);
            var gameContext = new GameContext(gameConfig, new InMemoryPersistenceContextProvider(), null, mapInitializer);
            return GetPlayer(gameContext);
        }

        /// <summary>
        /// Gets a test player.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <returns>
        /// The test player.
        /// </returns>
        public static Player GetPlayer(IGameContext gameContext)
        {
            var character = MockRepository.GenerateStub<Character>();
            character.Inventory = MockRepository.GenerateStub<ItemStorage>();
            character.Inventory.Stub(i => i.Items).Return(new List<Item>());
            character.Stub(c => c.LearnedSkills).Return(new List<SkillEntry>());
            character.CurrentMap = gameContext.GetMap(0)?.Definition;
            character.CharacterClass = MockRepository.GenerateStub<CharacterClass>();
            character.CharacterClass.Stub(c => c.StatAttributes).Return(
                new List<StatAttributeDefinition>
                {
                    new StatAttributeDefinition(Stats.Level, 0, false),
                    new StatAttributeDefinition(Stats.BaseStrength, 28, true),
                    new StatAttributeDefinition(Stats.BaseAgility, 20, true),
                    new StatAttributeDefinition(Stats.BaseVitality, 25, true),
                    new StatAttributeDefinition(Stats.BaseEnergy, 10, true),
                    new StatAttributeDefinition(Stats.CurrentHealth, 0, false),
                    new StatAttributeDefinition(Stats.CurrentMana, 0, false)
                });
            character.Stub(c => c.Attributes).Return(new List<StatAttribute>());
            foreach (var attributeDef in character.CharacterClass.StatAttributes)
            {
                character.Attributes.Add(new StatAttribute(attributeDef.Attribute, attributeDef.BaseValue));
            }

            character.CharacterClass.Stub(c => c.AttributeCombinations).Return(new List<AttributeRelationship>
            {
                // Params: TargetAttribute, Multiplier, SourceAttribute
                new AttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength),
                new AttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility),
                new AttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality),
                new AttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy),

                new AttributeRelationship(Stats.MaximumAbility, 1, Stats.TotalEnergy),
                new AttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality),
                new AttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility),
                new AttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalStrength),

                new AttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy),
                new AttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality),
                new AttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility),
                new AttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength),
                new AttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase),

                new AttributeRelationship(Stats.MaximumMana, 1, Stats.TotalEnergy),
                new AttributeRelationship(Stats.MaximumMana, 0.5f, Stats.Level),
                new AttributeRelationship(Stats.MaximumHealth, 2, Stats.Level),
                new AttributeRelationship(Stats.MaximumHealth, 3, Stats.TotalVitality),
            });
            character.CharacterClass.Stub(c => c.BaseAttributeValues).Return(new List<ConstValueAttribute>()
            {
                new ConstValueAttribute(10, Stats.MaximumMana),
                new ConstValueAttribute(35, Stats.MaximumHealth),
                new ConstValueAttribute(2, Stats.SkillMultiplier),
                new ConstValueAttribute(2, Stats.AbilityRecovery),
                new ConstValueAttribute(1, Stats.DamageReceiveDecrement),
                new ConstValueAttribute(1, Stats.AttackDamageIncrease),
                new ConstValueAttribute(1, Stats.MoneyAmountRate)
            });

            character.Stub(c => c.DropItemGroups).Return(new List<DropItemGroup>());

            var player = new Player(gameContext, MockRepository.GenerateMock<IPlayerView>()) { Account = new Account() };
            player.PlayerView.Stub(v => v.InventoryView).Return(MockRepository.GenerateMock<IInventoryView>());
            player.PlayerView.Stub(v => v.WorldView).Return(MockRepository.GenerateMock<IWorldView>());
            player.PlayerView.Stub(v => v.TradeView).Return(MockRepository.GenerateMock<ITradeView>());
            player.PlayerView.Stub(v => v.GuildView).Return(MockRepository.GenerateMock<IGuildView>());
            player.PlayerView.Stub(v => v.PartyView).Return(MockRepository.GenerateMock<IPartyView>());
            player.PlayerView.Stub(v => v.MessengerView).Return(MockRepository.GenerateMock<IMessengerView>());
            player.PlayerState.TryAdvanceTo(PlayerState.LoginScreen);
            player.PlayerState.TryAdvanceTo(PlayerState.Authenticated);
            player.PlayerState.TryAdvanceTo(PlayerState.CharacterSelection);
            player.SelectedCharacter = character;
            return player;
        }
    }
}
