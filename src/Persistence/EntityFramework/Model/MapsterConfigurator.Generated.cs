// <copyright file="MapsterConfigurator.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using MUnique.OpenMU.Persistence;
using Mapster;

/// <summary>
/// Configures Mapster to properly map these classes to the Persistence.BasicModel.
/// </summary>
public static class MapsterConfigurator
{
    private static bool isConfigured;

    /// <summary>
    /// Ensures that Mapster is configured to properly map these EF-Core persistence classes to the Persistence.BasicModel.
    /// </summary>
    public static void EnsureConfigured()
    {
        if (isConfigured)
        {
            return;
        }

        Mapster.TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        Mapster.TypeAdapterConfig.GlobalSettings.Default.IgnoreMember((member, side) => member.Name.StartsWith("Raw"));

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Statistics.MiniGameRankingEntry, MUnique.OpenMU.DataModel.Statistics.MiniGameRankingEntry>()
            .Include<MiniGameRankingEntry, BasicModel.MiniGameRankingEntry>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.Account, MUnique.OpenMU.DataModel.Entities.Account>()
            .Include<Account, BasicModel.Account>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.AppearanceData, MUnique.OpenMU.DataModel.Entities.AppearanceData>()
            .Include<AppearanceData, BasicModel.AppearanceData>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.Character, MUnique.OpenMU.DataModel.Entities.Character>()
            .Include<Character, BasicModel.Character>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.CharacterQuestState, MUnique.OpenMU.DataModel.Entities.CharacterQuestState>()
            .Include<CharacterQuestState, BasicModel.CharacterQuestState>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.Guild, MUnique.OpenMU.DataModel.Entities.Guild>()
            .Include<Guild, BasicModel.Guild>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.GuildMember, MUnique.OpenMU.DataModel.Entities.GuildMember>()
            .Include<GuildMember, BasicModel.GuildMember>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.Item, MUnique.OpenMU.DataModel.Entities.Item>()
            .Include<Item, BasicModel.Item>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.ItemAppearance, MUnique.OpenMU.DataModel.Entities.ItemAppearance>()
            .Include<ItemAppearance, BasicModel.ItemAppearance>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.ItemOptionLink, MUnique.OpenMU.DataModel.Entities.ItemOptionLink>()
            .Include<ItemOptionLink, BasicModel.ItemOptionLink>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.ItemStorage, MUnique.OpenMU.DataModel.Entities.ItemStorage>()
            .Include<ItemStorage, BasicModel.ItemStorage>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.LetterBody, MUnique.OpenMU.DataModel.Entities.LetterBody>()
            .Include<LetterBody, BasicModel.LetterBody>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.QuestMonsterKillRequirementState, MUnique.OpenMU.DataModel.Entities.QuestMonsterKillRequirementState>()
            .Include<QuestMonsterKillRequirementState, BasicModel.QuestMonsterKillRequirementState>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Entities.SkillEntry, MUnique.OpenMU.DataModel.Entities.SkillEntry>()
            .Include<SkillEntry, BasicModel.SkillEntry>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.BattleZoneDefinition, MUnique.OpenMU.DataModel.Configuration.BattleZoneDefinition>()
            .Include<BattleZoneDefinition, BasicModel.BattleZoneDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.CharacterClass, MUnique.OpenMU.DataModel.Configuration.CharacterClass>()
            .Include<CharacterClass, BasicModel.CharacterClass>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ChatServerDefinition, MUnique.OpenMU.DataModel.Configuration.ChatServerDefinition>()
            .Include<ChatServerDefinition, BasicModel.ChatServerDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ChatServerEndpoint, MUnique.OpenMU.DataModel.Configuration.ChatServerEndpoint>()
            .Include<ChatServerEndpoint, BasicModel.ChatServerEndpoint>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ConfigurationUpdate, MUnique.OpenMU.DataModel.Configuration.ConfigurationUpdate>()
            .Include<ConfigurationUpdate, BasicModel.ConfigurationUpdate>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ConfigurationUpdateState, MUnique.OpenMU.DataModel.Configuration.ConfigurationUpdateState>()
            .Include<ConfigurationUpdateState, BasicModel.ConfigurationUpdateState>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ConnectServerDefinition, MUnique.OpenMU.DataModel.Configuration.ConnectServerDefinition>()
            .Include<ConnectServerDefinition, BasicModel.ConnectServerDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.DropItemGroup, MUnique.OpenMU.DataModel.Configuration.DropItemGroup>()
            .Include<DropItemGroup, BasicModel.DropItemGroup>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.EnterGate, MUnique.OpenMU.DataModel.Configuration.EnterGate>()
            .Include<EnterGate, BasicModel.EnterGate>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ExitGate, MUnique.OpenMU.DataModel.Configuration.ExitGate>()
            .Include<ExitGate, BasicModel.ExitGate>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameClientDefinition, MUnique.OpenMU.DataModel.Configuration.GameClientDefinition>()
            .Include<GameClientDefinition, BasicModel.GameClientDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameConfiguration, MUnique.OpenMU.DataModel.Configuration.GameConfiguration>()
            .Include<GameConfiguration, BasicModel.GameConfiguration>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameMapDefinition, MUnique.OpenMU.DataModel.Configuration.GameMapDefinition>()
            .Include<GameMapDefinition, BasicModel.GameMapDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameServerConfiguration, MUnique.OpenMU.DataModel.Configuration.GameServerConfiguration>()
            .Include<GameServerConfiguration, BasicModel.GameServerConfiguration>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameServerDefinition, MUnique.OpenMU.DataModel.Configuration.GameServerDefinition>()
            .Include<GameServerDefinition, BasicModel.GameServerDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.GameServerEndpoint, MUnique.OpenMU.DataModel.Configuration.GameServerEndpoint>()
            .Include<GameServerEndpoint, BasicModel.GameServerEndpoint>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Gate, MUnique.OpenMU.DataModel.Configuration.Gate>()
            .Include<Gate, BasicModel.Gate>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ItemDropItemGroup, MUnique.OpenMU.DataModel.Configuration.ItemDropItemGroup>()
            .Include<ItemDropItemGroup, BasicModel.ItemDropItemGroup>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.JewelMix, MUnique.OpenMU.DataModel.Configuration.JewelMix>()
            .Include<JewelMix, BasicModel.JewelMix>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.LevelDependentDamage, MUnique.OpenMU.DataModel.Configuration.LevelDependentDamage>()
            .Include<LevelDependentDamage, BasicModel.LevelDependentDamage>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition, MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition>()
            .Include<MagicEffectDefinition, BasicModel.MagicEffectDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MasterSkillDefinition, MUnique.OpenMU.DataModel.Configuration.MasterSkillDefinition>()
            .Include<MasterSkillDefinition, BasicModel.MasterSkillDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MasterSkillRoot, MUnique.OpenMU.DataModel.Configuration.MasterSkillRoot>()
            .Include<MasterSkillRoot, BasicModel.MasterSkillRoot>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MiniGameChangeEvent, MUnique.OpenMU.DataModel.Configuration.MiniGameChangeEvent>()
            .Include<MiniGameChangeEvent, BasicModel.MiniGameChangeEvent>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition, MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition>()
            .Include<MiniGameDefinition, BasicModel.MiniGameDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MiniGameReward, MUnique.OpenMU.DataModel.Configuration.MiniGameReward>()
            .Include<MiniGameReward, BasicModel.MiniGameReward>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MiniGameSpawnWave, MUnique.OpenMU.DataModel.Configuration.MiniGameSpawnWave>()
            .Include<MiniGameSpawnWave, BasicModel.MiniGameSpawnWave>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MiniGameTerrainChange, MUnique.OpenMU.DataModel.Configuration.MiniGameTerrainChange>()
            .Include<MiniGameTerrainChange, BasicModel.MiniGameTerrainChange>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MonsterAttribute, MUnique.OpenMU.DataModel.Configuration.MonsterAttribute>()
            .Include<MonsterAttribute, BasicModel.MonsterAttribute>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MonsterDefinition, MUnique.OpenMU.DataModel.Configuration.MonsterDefinition>()
            .Include<MonsterDefinition, BasicModel.MonsterDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.MonsterSpawnArea, MUnique.OpenMU.DataModel.Configuration.MonsterSpawnArea>()
            .Include<MonsterSpawnArea, BasicModel.MonsterSpawnArea>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Rectangle, MUnique.OpenMU.DataModel.Configuration.Rectangle>()
            .Include<Rectangle, BasicModel.Rectangle>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Skill, MUnique.OpenMU.DataModel.Configuration.Skill>()
            .Include<Skill, BasicModel.Skill>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.SkillComboDefinition, MUnique.OpenMU.DataModel.Configuration.SkillComboDefinition>()
            .Include<SkillComboDefinition, BasicModel.SkillComboDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.SkillComboStep, MUnique.OpenMU.DataModel.Configuration.SkillComboStep>()
            .Include<SkillComboStep, BasicModel.SkillComboStep>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.StatAttributeDefinition, MUnique.OpenMU.DataModel.Configuration.StatAttributeDefinition>()
            .Include<StatAttributeDefinition, BasicModel.StatAttributeDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.SystemConfiguration, MUnique.OpenMU.DataModel.Configuration.SystemConfiguration>()
            .Include<SystemConfiguration, BasicModel.SystemConfiguration>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.WarpInfo, MUnique.OpenMU.DataModel.Configuration.WarpInfo>()
            .Include<WarpInfo, BasicModel.WarpInfo>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Quests.QuestDefinition, MUnique.OpenMU.DataModel.Configuration.Quests.QuestDefinition>()
            .Include<QuestDefinition, BasicModel.QuestDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Quests.QuestItemRequirement, MUnique.OpenMU.DataModel.Configuration.Quests.QuestItemRequirement>()
            .Include<QuestItemRequirement, BasicModel.QuestItemRequirement>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Quests.QuestMonsterKillRequirement, MUnique.OpenMU.DataModel.Configuration.Quests.QuestMonsterKillRequirement>()
            .Include<QuestMonsterKillRequirement, BasicModel.QuestMonsterKillRequirement>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward, MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward>()
            .Include<QuestReward, BasicModel.QuestReward>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement, MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement>()
            .Include<AttributeRequirement, BasicModel.AttributeRequirement>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.CombinationBonusRequirement, MUnique.OpenMU.DataModel.Configuration.Items.CombinationBonusRequirement>()
            .Include<CombinationBonusRequirement, BasicModel.CombinationBonusRequirement>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.IncreasableItemOption, MUnique.OpenMU.DataModel.Configuration.Items.IncreasableItemOption>()
            .Include<IncreasableItemOption, BasicModel.IncreasableItemOption>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemBasePowerUpDefinition, MUnique.OpenMU.DataModel.Configuration.Items.ItemBasePowerUpDefinition>()
            .Include<ItemBasePowerUpDefinition, BasicModel.ItemBasePowerUpDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition, MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>()
            .Include<ItemDefinition, BasicModel.ItemDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable, MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable>()
            .Include<ItemLevelBonusTable, BasicModel.ItemLevelBonusTable>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOfItemSet, MUnique.OpenMU.DataModel.Configuration.Items.ItemOfItemSet>()
            .Include<ItemOfItemSet, BasicModel.ItemOfItemSet>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOption, MUnique.OpenMU.DataModel.Configuration.Items.ItemOption>()
            .Include<ItemOption, BasicModel.ItemOption>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionCombinationBonus, MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionCombinationBonus>()
            .Include<ItemOptionCombinationBonus, BasicModel.ItemOptionCombinationBonus>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition, MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition>()
            .Include<ItemOptionDefinition, BasicModel.ItemOptionDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionOfLevel, MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionOfLevel>()
            .Include<ItemOptionOfLevel, BasicModel.ItemOptionOfLevel>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionType, MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionType>()
            .Include<ItemOptionType, BasicModel.ItemOptionType>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemSetGroup, MUnique.OpenMU.DataModel.Configuration.Items.ItemSetGroup>()
            .Include<ItemSetGroup, BasicModel.ItemSetGroup>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType, MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType>()
            .Include<ItemSlotType, BasicModel.ItemSlotType>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus, MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus>()
            .Include<LevelBonus, BasicModel.LevelBonus>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting, MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>()
            .Include<ItemCrafting, BasicModel.ItemCrafting>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCraftingRequiredItem, MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCraftingRequiredItem>()
            .Include<ItemCraftingRequiredItem, BasicModel.ItemCraftingRequiredItem>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCraftingResultItem, MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCraftingResultItem>()
            .Include<ItemCraftingResultItem, BasicModel.ItemCraftingResultItem>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.SimpleCraftingSettings, MUnique.OpenMU.DataModel.Configuration.ItemCrafting.SimpleCraftingSettings>()
            .Include<SimpleCraftingSettings, BasicModel.SimpleCraftingSettings>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition, MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition>()
            .Include<PowerUpDefinition, BasicModel.PowerUpDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinitionValue, MUnique.OpenMU.DataModel.Attributes.PowerUpDefinitionValue>()
            .Include<PowerUpDefinitionValue, BasicModel.PowerUpDefinitionValue>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.AttributeSystem.AttributeDefinition, MUnique.OpenMU.AttributeSystem.AttributeDefinition>()
            .Include<AttributeDefinition, BasicModel.AttributeDefinition>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.AttributeSystem.StatAttribute, MUnique.OpenMU.AttributeSystem.StatAttribute>()
            .Include<StatAttribute, BasicModel.StatAttribute>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.AttributeSystem.ConstValueAttribute, MUnique.OpenMU.AttributeSystem.ConstValueAttribute>()
            .Include<ConstValueAttribute, BasicModel.ConstValueAttribute>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.AttributeSystem.AttributeRelationship, MUnique.OpenMU.AttributeSystem.AttributeRelationship>()
            .Include<AttributeRelationship, BasicModel.AttributeRelationship>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.Interfaces.LetterHeader, MUnique.OpenMU.Interfaces.LetterHeader>()
            .Include<LetterHeader, BasicModel.LetterHeader>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.Interfaces.Friend, MUnique.OpenMU.Interfaces.Friend>()
            .Include<Friend, BasicModel.Friend>();

        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<MUnique.OpenMU.PlugIns.PlugInConfiguration, MUnique.OpenMU.PlugIns.PlugInConfiguration>()
            .Include<PlugInConfiguration, BasicModel.PlugInConfiguration>();


        isConfigured = true;
    }
}
