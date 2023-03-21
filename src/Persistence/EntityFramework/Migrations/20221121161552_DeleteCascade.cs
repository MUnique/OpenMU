using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ItemStorage_VaultId",
                schema: "data",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_GroundId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_Character_ItemStorage_InventoryId",
                schema: "data",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterQuestState_Character_CharacterId",
                schema: "data",
                table: "CharacterQuestState");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                schema: "config",
                table: "ChatServerEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_CombinationBonusRequirement_ItemOptionCombinationBonus_Item~",
                schema: "config",
                table: "CombinationBonusRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "EnterGate");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitGate_GameMapDefinition_MapId",
                schema: "config",
                table: "ExitGate");

            migrationBuilder.DropForeignKey(
                name: "FK_GameMapDefinition_BattleZoneDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                schema: "config",
                table: "GameServerEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemStorage_ItemStorageId",
                schema: "data",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                schema: "data",
                table: "ItemAppearance");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                schema: "config",
                table: "ItemCraftingRequiredItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                schema: "config",
                table: "ItemCraftingResultItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDropItemGroup_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLevelBonusTable_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemLevelBonusTable");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "ItemOfItemSet");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionCombinationBonus_GameConfiguration_GameConfigurat~",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionCombinationBonus_PowerUpDefinition_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionLink_Item_ItemId",
                schema: "data",
                table: "ItemOptionLink");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionType");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSetGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSlotType");

            migrationBuilder.DropForeignKey(
                name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "JewelMix");

            migrationBuilder.DropForeignKey(
                name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                schema: "data",
                table: "LetterBody");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelBonus_ItemLevelBonusTable_ItemLevelBonusTableId",
                schema: "config",
                table: "LevelBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameChangeEvent_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameChangeEvent_MonsterSpawnArea_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameReward_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameSpawnWave_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameSpawnWave");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameTerrainChange_MiniGameChangeEvent_MiniGameChangeEve~",
                schema: "config",
                table: "MiniGameTerrainChange");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "MonsterAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "PlugInConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_MagicEffectDefinition_MagicEffectDefiniti~",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestDefinition_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "QuestDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestItemRequirement_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestItemRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestMonsterKillRequirement_QuestDefinition_QuestDefinition~",
                schema: "config",
                table: "QuestMonsterKillRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestReward_Item_ItemRewardId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestReward_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillComboStep_SkillComboDefinition_SkillComboDefinitionId",
                schema: "config",
                table: "SkillComboStep");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillEntry_Character_CharacterId",
                schema: "data",
                table: "SkillEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_StatAttribute_Character_CharacterId",
                schema: "data",
                table: "StatAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                schema: "config",
                table: "StatAttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "WarpInfo");

            migrationBuilder.DropIndex(
                name: "IX_Skill_MasterDefinitionId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_QuestReward_ItemRewardId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropIndex(
                name: "IX_PowerUpDefinition_BoostId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MonsterDefinition_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MiniGameChangeEvent_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropIndex(
                name: "IX_MagicEffectDefinition_DurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropIndex(
                name: "IX_LetterBody_SenderAppearanceId",
                schema: "data",
                table: "LetterBody");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptionOfLevel_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptionCombinationBonus_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropIndex(
                name: "IX_ItemCrafting_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropIndex(
                name: "IX_IncreasableItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropIndex(
                name: "IX_GameMapDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropIndex(
                name: "IX_Character_InventoryId",
                schema: "data",
                table: "Character");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_GroundId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_Account_VaultId",
                schema: "data",
                table: "Account");

            migrationBuilder.AddColumn<Guid>(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                schema: "data",
                table: "LetterHeader",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ItemOption",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PowerUpDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    SubOptionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOption_ItemOptionType_OptionTypeId",
                        column: x => x.OptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOption_PowerUpDefinition_PowerUpDefinitionId",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MasterDefinitionId",
                schema: "config",
                table: "Skill",
                column: "MasterDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_ItemRewardId",
                schema: "config",
                table: "QuestReward",
                column: "ItemRewardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_BoostId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "BoostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawnArea_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinition_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition",
                column: "MerchantStoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameChangeEvent_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "SpawnAreaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_DurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LetterBody_SenderAppearanceId",
                schema: "data",
                table: "LetterBody",
                column: "SenderAppearanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionOfLevel_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "PowerUpDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionCombinationBonus_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "BonusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemCrafting_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting",
                column: "SimpleCraftingSettingsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "PowerUpDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition",
                column: "BattleZoneId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Character_InventoryId",
                schema: "data",
                table: "Character",
                column: "InventoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_GroundId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "GroundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "LeftGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "RightGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_VaultId",
                schema: "data",
                table: "Account",
                column: "VaultId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOption_OptionTypeId",
                schema: "config",
                table: "ItemOption",
                column: "OptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOption",
                column: "PowerUpDefinitionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ItemStorage_VaultId",
                schema: "data",
                table: "Account",
                column: "VaultId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                schema: "config",
                table: "AttributeRelationship",
                column: "CharacterClassId",
                principalSchema: "config",
                principalTable: "CharacterClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                schema: "config",
                table: "AttributeRelationship",
                column: "PowerUpDefinitionValueId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId1",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_GroundId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "GroundId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "LeftGoalId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "RightGoalId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Character_ItemStorage_InventoryId",
                schema: "data",
                table: "Character",
                column: "InventoryId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "CharacterClass",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId",
                principalSchema: "config",
                principalTable: "SkillComboDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterQuestState_Character_CharacterId",
                schema: "data",
                table: "CharacterQuestState",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                schema: "config",
                table: "ChatServerEndpoint",
                column: "ChatServerDefinitionId",
                principalSchema: "config",
                principalTable: "ChatServerDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CombinationBonusRequirement_ItemOptionCombinationBonus_Item~",
                schema: "config",
                table: "CombinationBonusRequirement",
                column: "ItemOptionCombinationBonusId",
                principalSchema: "config",
                principalTable: "ItemOptionCombinationBonus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "EnterGate",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitGate_GameMapDefinition_MapId",
                schema: "config",
                table: "ExitGate",
                column: "MapId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameMapDefinition_BattleZoneDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition",
                column: "BattleZoneId",
                principalSchema: "config",
                principalTable: "BattleZoneDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "GameMapDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                schema: "config",
                table: "GameServerEndpoint",
                column: "GameServerDefinitionId",
                principalSchema: "config",
                principalTable: "GameServerDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemOptionDefinitionId",
                principalSchema: "config",
                principalTable: "ItemOptionDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "PowerUpDefinitionId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemStorage_ItemStorageId",
                schema: "data",
                table: "Item",
                column: "ItemStorageId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                schema: "data",
                table: "ItemAppearance",
                column: "AppearanceDataId",
                principalSchema: "data",
                principalTable: "AppearanceData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                schema: "config",
                table: "ItemCraftingRequiredItem",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                schema: "config",
                table: "ItemCraftingResultItem",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDropItemGroup_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLevelBonusTable_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemLevelBonusTable",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "ItemOfItemSet",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionCombinationBonus_GameConfiguration_GameConfigurat~",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionCombinationBonus_PowerUpDefinition_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "BonusId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionLink_Item_ItemId",
                schema: "data",
                table: "ItemOptionLink",
                column: "ItemId",
                principalSchema: "data",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "IncreasableItemOptionId",
                principalSchema: "config",
                principalTable: "IncreasableItemOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "PowerUpDefinitionId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionType",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSetGroup",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSlotType",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "JewelMix",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                schema: "data",
                table: "LetterBody",
                column: "SenderAppearanceId",
                principalSchema: "data",
                principalTable: "AppearanceData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelBonus_ItemLevelBonusTable_ItemLevelBonusTableId",
                schema: "config",
                table: "LevelBonus",
                column: "ItemLevelBonusTableId",
                principalSchema: "config",
                principalTable: "ItemLevelBonusTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameChangeEvent_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameChangeEvent_MonsterSpawnArea_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "SpawnAreaId",
                principalSchema: "config",
                principalTable: "MonsterSpawnArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameReward_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameReward",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameSpawnWave_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameSpawnWave",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameTerrainChange_MiniGameChangeEvent_MiniGameChangeEve~",
                schema: "config",
                table: "MiniGameTerrainChange",
                column: "MiniGameChangeEventId",
                principalSchema: "config",
                principalTable: "MiniGameChangeEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "MonsterAttribute",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MonsterDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition",
                column: "MerchantStoreId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "PlugInConfiguration",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_MagicEffectDefinition_MagicEffectDefiniti~",
                schema: "config",
                table: "PowerUpDefinition",
                column: "MagicEffectDefinitionId",
                principalSchema: "config",
                principalTable: "MagicEffectDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "BoostId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestDefinition_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "QuestDefinition",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestItemRequirement_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestItemRequirement",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestMonsterKillRequirement_QuestDefinition_QuestDefinition~",
                schema: "config",
                table: "QuestMonsterKillRequirement",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestReward_Item_ItemRewardId",
                schema: "config",
                table: "QuestReward",
                column: "ItemRewardId",
                principalSchema: "data",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestReward_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestReward",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "Skill",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                schema: "config",
                table: "Skill",
                column: "MasterDefinitionId",
                principalSchema: "config",
                principalTable: "MasterSkillDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillComboStep_SkillComboDefinition_SkillComboDefinitionId",
                schema: "config",
                table: "SkillComboStep",
                column: "SkillComboDefinitionId",
                principalSchema: "config",
                principalTable: "SkillComboDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillEntry_Character_CharacterId",
                schema: "data",
                table: "SkillEntry",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatAttribute_Character_CharacterId",
                schema: "data",
                table: "StatAttribute",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                schema: "config",
                table: "StatAttributeDefinition",
                column: "CharacterClassId",
                principalSchema: "config",
                principalTable: "CharacterClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "WarpInfo",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ItemStorage_VaultId",
                schema: "data",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_GroundId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_Character_ItemStorage_InventoryId",
                schema: "data",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterQuestState_Character_CharacterId",
                schema: "data",
                table: "CharacterQuestState");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                schema: "config",
                table: "ChatServerEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_CombinationBonusRequirement_ItemOptionCombinationBonus_Item~",
                schema: "config",
                table: "CombinationBonusRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "EnterGate");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitGate_GameMapDefinition_MapId",
                schema: "config",
                table: "ExitGate");

            migrationBuilder.DropForeignKey(
                name: "FK_GameMapDefinition_BattleZoneDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                schema: "config",
                table: "GameServerEndpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemStorage_ItemStorageId",
                schema: "data",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                schema: "data",
                table: "ItemAppearance");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                schema: "config",
                table: "ItemCraftingRequiredItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                schema: "config",
                table: "ItemCraftingResultItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDropItemGroup_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLevelBonusTable_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemLevelBonusTable");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "ItemOfItemSet");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionCombinationBonus_GameConfiguration_GameConfigurat~",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionCombinationBonus_PowerUpDefinition_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionLink_Item_ItemId",
                schema: "data",
                table: "ItemOptionLink");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionType");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSetGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSlotType");

            migrationBuilder.DropForeignKey(
                name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "JewelMix");

            migrationBuilder.DropForeignKey(
                name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                schema: "data",
                table: "LetterBody");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelBonus_ItemLevelBonusTable_ItemLevelBonusTableId",
                schema: "config",
                table: "LevelBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameChangeEvent_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameChangeEvent_MonsterSpawnArea_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameReward_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameSpawnWave_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameSpawnWave");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameTerrainChange_MiniGameChangeEvent_MiniGameChangeEve~",
                schema: "config",
                table: "MiniGameTerrainChange");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "MonsterAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropForeignKey(
                name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "PlugInConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_MagicEffectDefinition_MagicEffectDefiniti~",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestDefinition_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "QuestDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestItemRequirement_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestItemRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestMonsterKillRequirement_QuestDefinition_QuestDefinition~",
                schema: "config",
                table: "QuestMonsterKillRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestReward_Item_ItemRewardId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestReward_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillComboStep_SkillComboDefinition_SkillComboDefinitionId",
                schema: "config",
                table: "SkillComboStep");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillEntry_Character_CharacterId",
                schema: "data",
                table: "SkillEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_StatAttribute_Character_CharacterId",
                schema: "data",
                table: "StatAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                schema: "config",
                table: "StatAttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "WarpInfo");

            migrationBuilder.DropTable(
                name: "ItemOption",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_Skill_MasterDefinitionId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_QuestReward_ItemRewardId",
                schema: "config",
                table: "QuestReward");

            migrationBuilder.DropIndex(
                name: "IX_PowerUpDefinition_BoostId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MonsterSpawnArea_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropIndex(
                name: "IX_MonsterDefinition_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MiniGameChangeEvent_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent");

            migrationBuilder.DropIndex(
                name: "IX_MagicEffectDefinition_DurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropIndex(
                name: "IX_LetterBody_SenderAppearanceId",
                schema: "data",
                table: "LetterBody");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptionOfLevel_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptionCombinationBonus_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus");

            migrationBuilder.DropIndex(
                name: "IX_ItemCrafting_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting");

            migrationBuilder.DropIndex(
                name: "IX_IncreasableItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropIndex(
                name: "IX_GameMapDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropIndex(
                name: "IX_Character_InventoryId",
                schema: "data",
                table: "Character");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_GroundId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_BattleZoneDefinition_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition");

            migrationBuilder.DropIndex(
                name: "IX_Account_VaultId",
                schema: "data",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                schema: "data",
                table: "LetterHeader");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MasterDefinitionId",
                schema: "config",
                table: "Skill",
                column: "MasterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_ItemRewardId",
                schema: "config",
                table: "QuestReward",
                column: "ItemRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_BoostId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "BoostId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinition_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition",
                column: "MerchantStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameChangeEvent_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "SpawnAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_DurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterBody_SenderAppearanceId",
                schema: "data",
                table: "LetterBody",
                column: "SenderAppearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionOfLevel_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "PowerUpDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionCombinationBonus_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "BonusId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCrafting_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting",
                column: "SimpleCraftingSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "PowerUpDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition",
                column: "BattleZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_InventoryId",
                schema: "data",
                table: "Character",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_GroundId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "GroundId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "LeftGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "RightGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_VaultId",
                schema: "data",
                table: "Account",
                column: "VaultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ItemStorage_VaultId",
                schema: "data",
                table: "Account",
                column: "VaultId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                schema: "config",
                table: "AttributeRelationship",
                column: "CharacterClassId",
                principalSchema: "config",
                principalTable: "CharacterClass",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                schema: "config",
                table: "AttributeRelationship",
                column: "PowerUpDefinitionValueId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId1",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_GroundId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "GroundId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "LeftGoalId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleZoneDefinition_Rectangle_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "RightGoalId",
                principalSchema: "config",
                principalTable: "Rectangle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Character_ItemStorage_InventoryId",
                schema: "data",
                table: "Character",
                column: "InventoryId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "CharacterClass",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId",
                principalSchema: "config",
                principalTable: "SkillComboDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterQuestState_Character_CharacterId",
                schema: "data",
                table: "CharacterQuestState",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                schema: "config",
                table: "ChatServerEndpoint",
                column: "ChatServerDefinitionId",
                principalSchema: "config",
                principalTable: "ChatServerDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CombinationBonusRequirement_ItemOptionCombinationBonus_Item~",
                schema: "config",
                table: "CombinationBonusRequirement",
                column: "ItemOptionCombinationBonusId",
                principalSchema: "config",
                principalTable: "ItemOptionCombinationBonus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "EnterGate",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExitGate_GameMapDefinition_MapId",
                schema: "config",
                table: "ExitGate",
                column: "MapId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameMapDefinition_BattleZoneDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition",
                column: "BattleZoneId",
                principalSchema: "config",
                principalTable: "BattleZoneDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "GameMapDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                schema: "config",
                table: "GameServerEndpoint",
                column: "GameServerDefinitionId",
                principalSchema: "config",
                principalTable: "GameServerDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemOptionDefinitionId",
                principalSchema: "config",
                principalTable: "ItemOptionDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "PowerUpDefinitionId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemStorage_ItemStorageId",
                schema: "data",
                table: "Item",
                column: "ItemStorageId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                schema: "data",
                table: "ItemAppearance",
                column: "AppearanceDataId",
                principalSchema: "data",
                principalTable: "AppearanceData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                schema: "config",
                table: "ItemCraftingRequiredItem",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                schema: "config",
                table: "ItemCraftingResultItem",
                column: "SimpleCraftingSettingsId",
                principalSchema: "config",
                principalTable: "SimpleCraftingSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDropItemGroup_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLevelBonusTable_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemLevelBonusTable",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "ItemOfItemSet",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionCombinationBonus_GameConfiguration_GameConfigurat~",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionCombinationBonus_PowerUpDefinition_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "BonusId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionLink_Item_ItemId",
                schema: "data",
                table: "ItemOptionLink",
                column: "ItemId",
                principalSchema: "data",
                principalTable: "Item",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "IncreasableItemOptionId",
                principalSchema: "config",
                principalTable: "IncreasableItemOption",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "PowerUpDefinitionId",
                principalSchema: "config",
                principalTable: "PowerUpDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemOptionType",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSetGroup",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ItemSlotType",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "JewelMix",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                schema: "data",
                table: "LetterBody",
                column: "SenderAppearanceId",
                principalSchema: "data",
                principalTable: "AppearanceData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LevelBonus_ItemLevelBonusTable_ItemLevelBonusTableId",
                schema: "config",
                table: "LevelBonus",
                column: "ItemLevelBonusTableId",
                principalSchema: "config",
                principalTable: "ItemLevelBonusTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameChangeEvent_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameChangeEvent_MonsterSpawnArea_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "SpawnAreaId",
                principalSchema: "config",
                principalTable: "MonsterSpawnArea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameReward_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameReward",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameSpawnWave_MiniGameDefinition_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameSpawnWave",
                column: "MiniGameDefinitionId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameTerrainChange_MiniGameChangeEvent_MiniGameChangeEve~",
                schema: "config",
                table: "MiniGameTerrainChange",
                column: "MiniGameChangeEventId",
                principalSchema: "config",
                principalTable: "MiniGameChangeEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "MonsterAttribute",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MonsterDefinition",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition",
                column: "MerchantStoreId",
                principalSchema: "data",
                principalTable: "ItemStorage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "PlugInConfiguration",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_MagicEffectDefinition_MagicEffectDefiniti~",
                schema: "config",
                table: "PowerUpDefinition",
                column: "MagicEffectDefinitionId",
                principalSchema: "config",
                principalTable: "MagicEffectDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "BoostId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestDefinition_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "QuestDefinition",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestItemRequirement_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestItemRequirement",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestMonsterKillRequirement_QuestDefinition_QuestDefinition~",
                schema: "config",
                table: "QuestMonsterKillRequirement",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestReward_Item_ItemRewardId",
                schema: "config",
                table: "QuestReward",
                column: "ItemRewardId",
                principalSchema: "data",
                principalTable: "Item",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestReward_QuestDefinition_QuestDefinitionId",
                schema: "config",
                table: "QuestReward",
                column: "QuestDefinitionId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "Skill",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                schema: "config",
                table: "Skill",
                column: "MasterDefinitionId",
                principalSchema: "config",
                principalTable: "MasterSkillDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillComboStep_SkillComboDefinition_SkillComboDefinitionId",
                schema: "config",
                table: "SkillComboStep",
                column: "SkillComboDefinitionId",
                principalSchema: "config",
                principalTable: "SkillComboDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillEntry_Character_CharacterId",
                schema: "data",
                table: "SkillEntry",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StatAttribute_Character_CharacterId",
                schema: "data",
                table: "StatAttribute",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                schema: "config",
                table: "StatAttributeDefinition",
                column: "CharacterClassId",
                principalSchema: "config",
                principalTable: "CharacterClass",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "WarpInfo",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");
        }
    }
}
