using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MigrateConfigurationUpdatesToGuidKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Key",
                schema: "config",
                table: "ConfigurationUpdate",
                type: "uuid",
                nullable: true);

            // NOTE: This CASE list was captured from the pre-refactor codebase (Phase 0).
            // If new IConfigurationUpdatePlugIn implementations were added in the meantime,
            // re-run the Phase 0 capture and extend the list before applying.
            migrationBuilder.Sql("""
                UPDATE config."ConfigurationUpdate"
                SET "Key" = CASE "Version"
                WHEN 1 THEN '13059991-F3C8-4050-A201-6D6A67E57541'::uuid -- ChaosCastleDataUpdatePlugIn
                WHEN 2 THEN '7231172F-51AD-4129-9003-C1ACC7E04147'::uuid -- SystemConfigurationAddedPlugInSeason6
                WHEN 3 THEN 'EC9FE71E-5C6C-456A-AC75-428EBA3FF626'::uuid -- SystemConfigurationAddedPlugIn095d
                WHEN 4 THEN 'F1151FDE-14F7-4945-AEE9-57DAB6449CFF'::uuid -- SystemConfigurationAddedPlugIn075
                WHEN 5 THEN '15FB42DE-A032-49B5-98B8-4CF34744B3A6'::uuid -- SpawnFixesUpdatePlugIn
                WHEN 6 THEN 'B0F275DC-B3C2-4826-8263-FFDC8A8AFAEA'::uuid -- FixLevelDiv20ExcOptionUpdatePlugIn
                WHEN 7 THEN 'F858E471-B76D-4AAF-8886-8DEB45BC1AB8'::uuid -- Season6
                WHEN 8 THEN '3D0201C3-D956-4BDD-9D57-3F6FD921EDF7'::uuid -- V095d
                WHEN 9 THEN '2E009ADF-1580-4E03-BA59-C9C51DC109BA'::uuid -- V075
                WHEN 10 THEN 'F4342D86-7042-477A-BC3B-475C1F2A79FF'::uuid -- FixWarpLevelUpdatePlugIn
                WHEN 11 THEN '48D40F2E-2844-4058-B1FA-710EEE55157B'::uuid -- AddQuestItemLimitPlugIn
                WHEN 12 THEN '1EF50759-0A5F-4301-A5E9-B68A8B7D29F9'::uuid -- AddGuardsDataPlugIn
                WHEN 13 THEN '65BA79B5-1DBF-4C97-9628-0D8A429A8C88'::uuid -- FixWarriorMorningStarPlugIn
                WHEN 15 THEN 'A78B7540-75AC-494C-9AEC-BC943D929C98'::uuid -- InfinityArrowSkillOnQuestCompletionPlugIn
                WHEN 16 THEN '6011A1B8-7FA5-48EB-935D-EEAF83017799'::uuid -- AddPointsPerResetAttributePlugIn
                WHEN 17 THEN '0C99155F-1289-4E73-97F0-47CB67C3716F'::uuid -- AddKalimaPlugIn
                WHEN 18 THEN '5DC5638E-581E-4ACC-81E4-D565C625649B'::uuid -- AddDuelConfigurationPlugIn
                WHEN 19 THEN '039D09CB-283C-4CBD-ABBC-FFD3F7D5C62F'::uuid -- ChainLightningUpdatePlugIn
                WHEN 20 THEN '426497DB-A1D7-4EC5-BE6A-C8AEABC288E2'::uuid -- WizEnhanceAndRagefighterSkillsUpdatePlugIn
                WHEN 21 THEN '8DEC7BC2-E6A0-4E46-B123-C92CB43B9ED5'::uuid -- FixIgnoreDefenseSkillUpdatePlugIn
                WHEN 22 THEN '618A53AF-ED2A-4C78-A103-BAD061FFB0D2'::uuid -- FixWingsAndCapesCraftingsUpdatePlugIn
                WHEN 23 THEN '280ACE93-2B96-476C-A4AF-4FDA7611D5D5'::uuid -- FixDamageAbsorbItemsUpdatePlugIn
                WHEN 24 THEN 'C802EFC2-1D42-4218-871E-8886D115F3ED'::uuid -- FixSocketSeedCraftingUpdatePlugIn
                WHEN 25 THEN 'FD521A61-D5B4-4CF2-B203-6FFF12C80E51'::uuid -- FixLifeSwellEffectUpdatePlugIn
                WHEN 26 THEN 'AB664421-1CA6-4FCE-A150-0007971017E1'::uuid -- FixAncientDiscriminatorsUpdatePlugIn
                WHEN 27 THEN 'A8827A3C-7F52-47CF-9EA5-562A9C06B986'::uuid -- FixDrainLifeSkillUpdate
                WHEN 28 THEN 'DCF14924-BB19-4CA2-93EC-397A89AA3EB3'::uuid -- AddItemDropGroupForJewelsUpdate075
                WHEN 29 THEN 'D21056E6-E912-416B-A076-3C2D17DA517B'::uuid -- AddItemDropGroupForJewelsUpdate095D
                WHEN 30 THEN 'F958CC5B-C1E6-4F67-B48D-4BF75EC5CAA8'::uuid -- AddItemDropGroupForJewelsUpdateSeason6
                WHEN 31 THEN 'EAC7C809-D4B8-443F-BE52-E56560003483'::uuid -- FixMaxManaAndAbilityJewelryOptionsUpdateSeason6
                WHEN 32 THEN '3821267A-9C37-40E5-B023-BAB1A8E4DAB7'::uuid -- FixWingsDmgRatesUpdatePlugIn075
                WHEN 33 THEN 'F45FA4D0-B19B-48E2-9592-A37F3B36348A'::uuid -- FixWingsDmgRatesUpdatePlugIn095D
                WHEN 34 THEN '03F49890-CB0E-40B7-A590-174BBA1962F4'::uuid -- FixWingsDmgRatesUpdatePlugInSeason6
                WHEN 35 THEN 'E94DE59E-5B3A-4498-A4AF-E7F4F173B754'::uuid -- AddHarmonyOptionWeightsUpdateSeason6
                WHEN 36 THEN '27714BB3-43F9-4D90-920F-98EF0EC20232'::uuid -- FixDuelArenaSafezoneMapUpdate
                WHEN 37 THEN 'F9977AA7-F52A-4F42-BD6C-98DE700B5980'::uuid -- FixAttackSpeedCalculationUpdate
                WHEN 38 THEN 'D01DA745-BF72-40C4-BD90-D2D637AEDF99'::uuid -- AddAreaSkillSettingsUpdatePlugIn
                WHEN 39 THEN '9E8DB2CB-1972-40D3-9129-6964ABFEB4DC'::uuid -- FixItemRequirementsPlugIn
                WHEN 40 THEN '5B63534D-E5DF-46B1-992D-C1637B197EE1'::uuid -- FixWeaponRisePercentagePlugIn075
                WHEN 41 THEN '33259706-F3DF-4F4D-9935-3DEF7E53BF81'::uuid -- FixWeaponRisePercentagePlugIn095D
                WHEN 42 THEN '58740F26-6496-4CCA-8C90-C4749E09DDB2'::uuid -- FixWeaponRisePercentagePlugInSeason6
                WHEN 43 THEN '04A5F236-117F-422A-8C38-28D09DE911D7'::uuid -- FixChaosMixesUpdatePlugIn075
                WHEN 44 THEN '68BC1F35-FC9A-468F-89FB-0940485AC107'::uuid -- FixChaosMixesPlugIn095D
                WHEN 45 THEN 'EFD7EA69-56AE-48A3-ACE2-1C3B5B87780A'::uuid -- FixChaosMixesPlugInSeason6
                WHEN 46 THEN '7733CDA9-6F4B-48D2-94F1-796C937F032A'::uuid -- FixItemOptionsAndAttackSpeedPlugIn075
                WHEN 47 THEN 'C7F90EDB-EC00-467D-826F-9DEFFEA1206A'::uuid -- FixItemOptionsAndAttackSpeedPlugIn095D
                WHEN 48 THEN 'EEEAA884-4704-48DE-825A-8E588A47E2CC'::uuid -- FixItemOptionsAndAttackSpeedPlugInSeason6
                WHEN 49 THEN '3E362629-AAF3-40E0-BC6D-32230285FB03'::uuid -- FixHorseFenrirOptionsSoulBarrierPlugIn
                WHEN 50 THEN 'D7CD05B7-06EE-4D9F-BAD0-65267F3A9FE8'::uuid -- FixCharStatsForceWavePlugIn075
                WHEN 51 THEN '14DFF317-B4E6-424A-A8D1-6D1D5195E970'::uuid -- FixCharStatsForceWavePlugIn095D
                WHEN 52 THEN '0C1995AB-A1CC-42A8-9EFC-E5FE8F360C53'::uuid -- FixCharStatsForceWavePlugInSeason6
                WHEN 53 THEN '683A8F8F-EFE9-4EF4-B536-21048E195A87'::uuid -- FixDefenseCalcsPlugIn075
                WHEN 54 THEN 'C6945ADC-0313-47AC-AFAE-61D0544C8935'::uuid -- FixDefenseCalcsPlugIn095D
                WHEN 55 THEN '447FA95B-091B-4950-B1F3-F4EB6D20DE19'::uuid -- FixDefenseCalcsPlugInSeason6
                WHEN 56 THEN '42B1582B-667F-4098-A339-DDA8560157E3'::uuid -- FixDamageCalcsPlugIn075
                WHEN 57 THEN 'A4410B7B-7E5F-409C-9F6F-4E216208829A'::uuid -- FixDamageCalcsPlugIn095D
                WHEN 58 THEN '077BA63D-F201-41BE-8A65-CFB859482A1B'::uuid -- FixDamageCalcsPlugInSeason6
                WHEN 59 THEN 'A8B5C2D1-3E4F-5A6B-7C8D-9E0F1A2B3C4D'::uuid -- FixEventItemsDropFromMonstersUpdatePlugInSeason6
                WHEN 60 THEN 'B9C6D3E2-4F5A-6B7C-8D9E-0F1A2B3C4D5E'::uuid -- FixEventItemsDropFromMonstersUpdatePlugIn095d
                WHEN 61 THEN 'A7C9D4E1-8F2B-4A3C-9E6D-7B8F9A0E1C2D'::uuid -- FixItemRequirementsPlugIn2
                WHEN 62 THEN '46F50226-B0A2-4FE7-B708-AEB3F306A7C0'::uuid -- FixJeweleryPetsDamageCalcsPlugIn075
                WHEN 63 THEN 'BF56A0E2-D7B3-4456-81F7-249440489607'::uuid -- FixJeweleryPetsDamageCalcsPlugIn095D
                WHEN 64 THEN 'DD5B0424-89DB-4DE4-A1BB-B294F2C1FCE6'::uuid -- FixJeweleryPetsDamageCalcsPlugInSeason6
                WHEN 65 THEN '753F01BA-5FCA-42FA-9587-7055631C27B7'::uuid -- FixSkillMultipliersPlugIn
                WHEN 66 THEN 'B1E2D6C3-1F4A-4D7C-8C2E-3F6D9A7B8E2F'::uuid -- AddSummonerBuffSkillsPlugIn
                WHEN 67 THEN 'E3A8F7C9-2D4B-4A1E-9F3C-8B5D7A6C1E4F'::uuid -- AddProjectileCountToTripleShotUpdatePlugIn
                WHEN 68 THEN 'CD958BC1-F17A-4C60-B66D-BD29D49B6ADA'::uuid -- RemoveJewelDropLevelGapPlugIn075
                WHEN 69 THEN '6614E91E-5749-478A-96A4-3240E7C1280E'::uuid -- RemoveJewelDropLevelGapPlugIn095D
                WHEN 70 THEN 'AB1C9F8B-5E3B-4F2A-BDCD-9C0F1E5A6B7C'::uuid -- RemoveJewelDropLevelGapPlugInSeason6
                WHEN 71 THEN 'EDDD17F9-BEA5-40F0-A653-8567566C40E7'::uuid -- FixRageFighterMultipleHitSkillsPlugIn
                WHEN 72 THEN 'FF14A478-3EA8-4C41-A298-8E6698D5973D'::uuid -- AddCrestOfMonarchDropGroupUpdateSeason6
                WHEN 73 THEN 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890'::uuid -- AddGlobalMoneyAmountRateAttributePlugIn075
                WHEN 74 THEN 'B2C3D4E5-F6A7-8901-BCDE-F12345678901'::uuid -- AddGlobalMoneyAmountRateAttributePlugIn095d
                WHEN 75 THEN 'C3D4E5F6-A7B8-9012-CDEF-123456789012'::uuid -- AddGlobalMoneyAmountRateAttributePlugInSeason6
                WHEN 76 THEN '2C2743B0-1305-47BF-85D9-09F6CA64AD54'::uuid -- AddMaximumAllianceSizeUpdatePlugInSeason6
                WHEN 77 THEN 'A3B4C8DB-2F39-4C81-A2D9-5E4FA5B9E004'::uuid -- FixSummonerCurseSkillsPlugIn
                WHEN 78 THEN 'B9938E4D-8F63-48DF-AE45-6739D1E2A8C7'::uuid -- FixAreaSkillsUpdatePlugIn
                WHEN 79 THEN '1A2B3C4D-5E6F-7890-ABCD-EF1234567890'::uuid -- FinishDarkLordMasterTreePlugIn
                WHEN 80 THEN 'D4E5F6A0-1B2C-3D4E-5F6A-7B8C9D0E1F2A'::uuid -- FixBloodCastleMonsterAttributesUpdatePlugIn
                WHEN 81 THEN '5F412933-CC0F-483B-B6AE-7B358A6257FD'::uuid -- AddRandomExperienceConfigAttributesPlugIn075
                WHEN 82 THEN '9A166583-C3E7-4E04-924C-F01FF9840974'::uuid -- AddRandomExperienceConfigAttributesPlugIn095d
                WHEN 83 THEN 'D1DC70A2-2614-4CC0-81C0-6C8253781019'::uuid -- AddRandomExperienceConfigAttributesPlugInSeason6
                WHEN 84 THEN '890E2FCB-EC93-4CC1-84FC-67A1B398D5C8'::uuid -- AddMovementSpeedAttributesPlugIn075
                WHEN 85 THEN '7C38C30F-163B-4625-A82D-5C3A0A9ED883'::uuid -- AddMovementSpeedAttributesPlugIn095D
                WHEN 86 THEN '1D4968DA-9C9C-42A7-AF80-D4811535EC63'::uuid -- AddMovementSpeedAttributesPlugInSeason6
                WHEN 87 THEN 'F78D6E1D-1CB5-45F7-912D-54B2CB1220EB'::uuid -- AddMissingMerchantStoresPlugIn
                WHEN 88 THEN 'B8F3E2C1-4D5A-6F78-9B0C-2E7D1A3F5B6C'::uuid -- FinishDarkKnightMasterTreePlugIn075
                WHEN 89 THEN 'D4F7A9C2-1B3E-56D8-9F0A-7C2E4B1D5A8F'::uuid -- FinishDarkKnightMasterTreePlugIn095D
                WHEN 90 THEN 'F7B2C9E4-1A3D-56F8-9B0C-4E2D7A1F8B3C'::uuid -- FinishDarkKnightMasterTreePlugInSeason6
                WHEN 91 THEN 'D8F4E2C0-5A6B-4C3D-9E7F-1B2A4C6D8E0F'::uuid -- AddWhiteWizardInvasionMobsUpdatePlugIn
                WHEN 92 THEN 'A7B3E5F1-8C2D-4E6F-9A1B-3D5C7E8F2A4B'::uuid -- LimitWhiteWizardDropsUpdatePlugIn
                WHEN 93 THEN 'ED2F1728-B35C-4A3D-810E-EAB5B6E12A82'::uuid -- AddLorenMarketJuliaWarpPlugIn
                WHEN 94 THEN '9374E428-CF5C-44B1-AAB9-0369C77AF7C6'::uuid -- AddIsQuestItemFlagPlugIn
                WHEN 95 THEN '9BCFC8B1-6A6E-48F9-AE7C-0D34FA6D706B'::uuid -- AddElfSoldierBuffPlugIn
                WHEN 96 THEN 'C1D2E3F4-5A6B-7C8D-9E0F-1A2B3C4D5E6F'::uuid -- FinishDarkWizardMasterTreePlugIn
                WHEN 97 THEN 'D1E2F3A4-B5C6-7D8E-9F0A-1B2C3D4E5F6A'::uuid -- FinishElfMasterTreePlugIn
                WHEN 98 THEN '182FC652-3277-4CDB-8BA8-DE70311E67C9'::uuid -- AddItemRegistrationAttributesUpdatePlugIn
                WHEN 99 THEN '6A1B7C3D-2E5F-4A7B-8C9D-1E1F2A3B4C6E'::uuid -- AddRenaItemUpdatePlugIn
                WHEN 100 THEN 'CD201E33-37C9-4C85-95CC-16042B28E974'::uuid -- AddCastleSiegeDataUpdatePlugIn
                WHEN 101 THEN '4A1F9B2C-3D6E-4F70-A8B9-1C2D3E4F5A6B'::uuid -- FinishSummonerMasterTreePlugIn
                WHEN 102 THEN 'D91757B1-0C3D-4336-8DEC-20438EDA7F09'::uuid -- ConfigureCastleSiegeRegistrationUpdatePlugIn
                WHEN 103 THEN '2DAE95BC-AE08-45E8-942A-9F61AE1C277B'::uuid -- FinishRageFighterMasterTreePlugIn
                WHEN 104 THEN '31F81908-548B-482E-A018-1ED0D1B8D89B'::uuid -- ConfigureCastleSiegeParticipationUpdatePlugIn
                WHEN 105 THEN 'F079402E-F557-423C-B376-80A1B87D842D'::uuid -- RegenerationsRefactorPlugIn075
                WHEN 106 THEN 'A7C2E9F4-5B1D-4E8A-9C6F-3D2B7A1E5F90'::uuid -- RegenerationsRefactorPlugIn095D
                WHEN 107 THEN 'E6A3B9F1-7C4D-48E2-A5B8-1F9D3C6E2A7B'::uuid -- RegenerationsRefactorPlugInSeason6
                WHEN 108 THEN '7ED67868-5C82-4B10-9BDA-732F51704DB9'::uuid -- ConfigureCastleSiegeEconomyUpdatePlugIn
                WHEN 109 THEN '6B0A9D2C-6E4F-4C1B-9B5A-2E7F8D4C0A31'::uuid -- RemoveDuplicateStatAttributesPlugIn075
                WHEN 110 THEN 'D1F4C7A8-3B62-4E05-8A9C-5C1B6E3F7D24'::uuid -- RemoveDuplicateStatAttributesPlugIn095D
                WHEN 111 THEN '9C3E5B71-8D04-42A6-BF19-7A2D6C8E5B03'::uuid -- RemoveDuplicateStatAttributesPlugInSeason6
                WHEN 112 THEN '654C871C-6BC2-41C4-BAC1-8DA2D9399B54'::uuid -- ConfigureCastleSiegeLifeStoneUpdatePlugIn
                WHEN 113 THEN '3F1B7A64-9C2E-4D58-B0A7-5E6C8D19F204'::uuid -- AddKanturuDataUpdatePlugIn
                WHEN 114 THEN '7B4E9D26-1A83-4F5C-9E70-2C8B6D41A395'::uuid -- AddKanturuMapContentUpdatePlugIn
                WHEN 115 THEN '8E4D2B17-6A3F-4C95-9D02-B7E15A6C3F48'::uuid -- AddDoppelgangerDataUpdatePlugIn
                WHEN 116 THEN 'C0F83AEA-C8E1-4E40-9B85-D344B7F93238'::uuid -- AddItemRuleFlagsPlugIn
                WHEN 117 THEN 'B2A18B26-5BF6-4EBC-80D9-421CB8B6BFCE'::uuid -- AddDarkHorseCanFlyPlugIn
                WHEN 118 THEN '2B7E9C41-5D86-4A13-B0F2-9C4A7E1D3B65'::uuid -- AddRaklionEventUpdatePlugIn
                WHEN 119 THEN 'E5AFDD7A-3DE8-4955-8BB5-4231F6A87749'::uuid -- RefreshKanturuDataUpdatePlugIn
                WHEN 120 THEN '8F3C6A2D-1E74-4B95-9D08-5A7E2C4B1F93'::uuid -- AddSelupanFallSkillUpdatePlugIn
                WHEN 121 THEN '4D8E2A17-B63C-4F95-A0E1-7C2B9F5D3E68'::uuid -- AddImperialGuardianDataUpdatePlugIn
                END;
                """);

            // Fail loudly instead of silently if any row wasn't covered by the CASE above.
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM config."ConfigurationUpdate" WHERE "Key" IS NULL) THEN
                        RAISE EXCEPTION 'Unmapped ConfigurationUpdate.Version found - update the CASE list.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "Key",
                schema: "config",
                table: "ConfigurationUpdate",
                type: "uuid",
                nullable: false);

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "config",
                table: "ConfigurationUpdate");

            migrationBuilder.DropColumn(
                name: "CurrentInstalledVersion",
                schema: "config",
                table: "ConfigurationUpdateState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not reversible: the numeric UpdateVersion enum this depends on is deleted in the same migration.
            throw new NotSupportedException("Restore from a backup taken before this migration instead.");
        }
    }
}
