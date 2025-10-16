// <copyright file="UpdateVersion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Enum which keeps track of the <see cref="IConfigurationUpdatePlugIn.Version"/>,
/// so that it's easier to know which number is next.
/// </summary>
public enum UpdateVersion
{
    /// <summary>
    /// Undefined version.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The version of the <see cref="ChaosCastleDataUpdatePlugIn"/>.
    /// </summary>
    ChaosCastleDataUpdate = 1,

    /// <summary>
    /// The version of the <see cref="SystemConfigurationAddedPlugInSeason6"/>.
    /// </summary>
    SystemConfigurationAddedSeason6 = 2,

    /// <summary>
    /// The version of the <see cref="SystemConfigurationAddedPlugIn095d"/>.
    /// </summary>
    SystemConfigurationAdded095d = 3,

    /// <summary>
    /// The version of the <see cref="SystemConfigurationAddedPlugIn075"/>.
    /// </summary>
    SystemConfigurationAdded075 = 4,

    /// <summary>
    /// The version of the <see cref="SpawnFixesUpdatePlugIn"/>.
    /// </summary>
    SpawnFixesUpdate = 5,

    /// <summary>
    /// The version of the <see cref="FixLevelDiv20ExcOptionUpdatePlugIn"/>.
    /// </summary>
    FixLevelDiv20ExcOptionUpdate = 6,

    /// <summary>
    /// The version of the <see cref="FixSetBonusesPlugIn.Season6"/>.
    /// </summary>
    FixSetBonusesSeason6 = 7,

    /// <summary>
    /// The version of the <see cref="FixSetBonusesPlugIn.V095d"/>.
    /// </summary>
    FixSetBonuses095d = 8,

    /// <summary>
    /// The version of the <see cref="FixSetBonusesPlugIn.V075"/>.
    /// </summary>
    FixSetBonuses075 = 9,

    /// <summary>
    /// The version of the <see cref="FixWarpLevelUpdatePlugIn"/>.
    /// </summary>
    FixWarpLevelUpdate = 10,

    /// <summary>
    /// The version of the <see cref="AddQuestItemLimitPlugIn"/>.
    /// </summary>
    AddQuestItemLimit = 11,

    /// <summary>
    /// The version of the <see cref="AddGuardsDataPlugIn"/>.
    /// </summary>
    AddGuardsData = 12,

    /// <summary>
    /// The version of the <see cref="FixWarriorMorningStarPlugIn"/>.
    /// </summary>
    FixWarriorMorningStar = 13,

    /// <summary>
    /// The version of the <see cref="InfinityArrowSkillOnQuestCompletionPlugIn"/>.
    /// </summary>
    InfinityArrowSkillOnQuestCompletion = 15,

    /// <summary>
    /// The version of the <see cref="AddPointsPerResetAttributePlugIn"/>.
    /// </summary>
    AddPointsPerResetByClassAttribute = 16,

    /// <summary>
    /// The version of the <see cref="AddKalimaPlugIn"/>.
    /// </summary>
    AddKalima = 17,

    /// <summary>
    /// The version of the <see cref="AddDuelConfigurationPlugIn"/>.
    /// </summary>
    AddDuelConfiguration = 18,

    /// <summary>
    /// The version of the <see cref="ChainLightningUpdatePlugIn"/>.
    /// </summary>
    ChainLightningUpdate = 19,

    /// <summary>
    /// The version of the <see cref="WizEnhanceAndRagefighterSkillsUpdatePlugIn"/>.
    /// </summary>
    WizEnhanceAndRagefighterSkills = 20,

    /// <summary>
    /// The version of the <see cref="FixIgnoreDefenseSkillUpdatePlugIn"/>.
    /// </summary>
    FixIgnoreDefenseSkill = 21,

    /// <summary>
    /// The version of the <see cref="FixWingsAndCapesCraftingsUpdatePlugIn"/>.
    /// </summary>
    FixWingsAndCapesCraftings = 22,

    /// <summary>
    /// The version of the <see cref="FixDamageAbsorbItemsUpdatePlugIn"/>.
    /// </summary>
    FixDamageAbsorbItems = 23,

    /// <summary>
    /// The version of the <see cref="FixSocketSeedCraftingUpdatePlugIn"/>.
    /// </summary>
    FixSocketSeedCrafting = 24,

    /// <summary>
    /// The version of the <see cref="FixLifeSwellEffectUpdatePlugIn"/>.
    /// </summary>
    FixLifeSwellEffect = 25,

    /// <summary>
    /// The version of the <see cref="FixAncientDiscriminatorsUpdatePlugIn"/>.
    /// </summary>
    FixAncientDiscriminators = 26,

    /// <summary>
    /// The version of the <see cref="FixDrainLifeSkillUpdate"/>.
    /// </summary>
    FixDrainLifeSkill = 27,

    /// <summary>
    /// The version of the <see cref="AddItemDropGroupForJewelsUpdate075"/>.
    /// </summary>
    AddItemDropGroupForJewels075 = 28,

    /// <summary>
    /// The version of the <see cref="AddItemDropGroupForJewelsUpdate095D"/>.
    /// </summary>
    AddItemDropGroupForJewels095d = 29,

    /// <summary>
    /// The version of the <see cref="AddItemDropGroupForJewelsUpdateSeason6"/>.
    /// </summary>
    AddItemDropGroupForJewelsSeason6 = 30,

    /// <summary>
    /// The version of the <see cref="FixMaxManaAndAbilityJewelryOptionsUpdateSeason6"/>.
    /// </summary>
    FixMaxManaAndAbilityJewelryOptionsSeason6 = 31,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugIn075"/>.
    /// </summary>
    FixWingsDmgRatesPlugIn075 = 32,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugIn095D"/>.
    /// </summary>
    FixWingsDmgRatesPlugIn095d = 33,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugInSeason6"/>.
    /// </summary>
    FixWingsDmgRatesPlugInSeason6 = 34,

    /// <summary>
    /// The version of the <see cref="AddHarmonyOptionWeightsUpdateSeason6"/>.
    /// </summary>
    AddHarmonyOptionWeightsSeason6 = 35,

    /// <summary>
    /// The version of the <see cref="FixDuelArenaSafezoneMapUpdate"/>.
    /// </summary>
    FixDuelArenaSafezoneMap = 36,

    /// <summary>
    /// The version of the <see cref="FixAttackSpeedCalculationUpdate"/>.
    /// </summary>
    FixAttackSpeedCalculation = 37,

    /// <summary>
    /// The version of the <see cref="AddAreaSkillSettingsUpdatePlugIn"/>.
    /// </summary>
    AddAreaSkillSettings = 38,

    /// <summary>
    /// The version of the <see cref="FixItemRequirementsPlugIn"/>.
    /// </summary>
    FixItemRequirements = 39,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugIn075"/>.
    /// </summary>
    FixWeaponRisePercentage075 = 40,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugIn095D"/>.
    /// </summary>
    FixWeaponRisePercentage095d = 41,

    /// <summary>
    /// The version of the <see cref="FixWingsDmgRatesUpdatePlugInSeason6"/>.
    /// </summary>
    FixWeaponRisePercentageSeason6 = 42,

    /// <summary>
    /// The version of the <see cref="FixChaosMixesUpdatePlugIn075"/>.
    /// </summary>
    FixChaosMixes075 = 43,

    /// <summary>
    /// The version of the <see cref="FixChaosMixesPlugIn095D"/>.
    /// </summary>
    FixChaosMixes095d = 44,

    /// <summary>
    /// The version of the <see cref="FixChaosMixesPlugInSeason6"/>.
    /// </summary>
    FixChaosMixesSeason6 = 45,

    /// <summary>
    /// The version of the <see cref="ElfSummonDefaultsUpdatePlugIn"/>.
    /// </summary>
    ElfSummonDefaults = 250,

    /// <summary>
    /// The version of the <see cref="FixItemOptionsAndAttackSpeedPlugIn075"/>.
    /// </summary>
    FixItemOptionsAndAttackSpeed075 = 46,

    /// <summary>
    /// The version of the <see cref="FixItemOptionsAndAttackSpeedPlugIn095D"/>.
    /// </summary>
    FixItemOptionsAndAttackSpeed095d = 47,

    /// <summary>
    /// The version of the <see cref="FixItemOptionsAndAttackSpeedPlugInSeason6"/>.
    /// </summary>
    FixItemOptionsAndAttackSpeedSeason6 = 48,

    /// <summary>
    /// The version of the <see cref="Updates.FixHorseFenrirOptionsSoulBarrierPlugIn"/>.
    /// </summary>
    FixHorseFenrirOptionsSoulBarrierPlugIn = 49,

    /// <summary>
    /// The version of the <see cref="FixCharStatsForceWavePlugIn075"/>.
    /// </summary>
    FixCharStatsForceWave075 = 50,

    /// <summary>
    /// The version of the <see cref="FixCharStatsForceWavePlugIn095D"/>.
    /// </summary>
    FixCharStatsForceWave095d = 51,

    /// <summary>
    /// The version of the <see cref="FixCharStatsForceWavePlugInSeason6"/>.
    /// </summary>
    FixCharStatsForceWaveSeason6 = 52,

    /// <summary>
    /// The version of the <see cref="FixDefenseCalcsPlugIn075"/>.
    /// </summary>
    FixDefenseCalcs075 = 53,

    /// <summary>
    /// The version of the <see cref="FixDefenseCalcsPlugIn095D"/>.
    /// </summary>
    FixDefenseCalcs095d = 54,

    /// <summary>
    /// The version of the <see cref="FixDefenseCalcsPlugInSeason6"/>.
    /// </summary>
    FixDefenseCalcsSeason6 = 55,

    /// <summary>
    /// The version of the <see cref="FixDamageCalcsPlugIn075"/>.
    /// </summary>
    FixDamageCalcs075 = 56,

    /// <summary>
    /// The version of the <see cref="FixDamageCalcsPlugIn095D"/>.
    /// </summary>
    FixDamageCalcs095d = 57,

    /// <summary>
    /// The version of the <see cref="FixDamageCalcsPlugInSeason6"/>.
    /// </summary>
    FixDamageCalcsSeason6 = 58,

    /// <summary>
    /// The version of the <see cref="FixEventItemsDropFromMonstersUpdatePlugInSeason6"/>.
    /// </summary>
    FixEventItemsDropFromMonstersSeason6 = 59,

    /// <summary>
    /// The version of the <see cref="FixEventItemsDropFromMonstersUpdatePlugIn095d"/>.
    /// </summary>
    FixEventItemsDropFromMonsters095d = 60,

    /// <summary>
    /// The version of the <see cref="FixItemRequirementsPlugIn2"/>.
    /// </summary>
    FixItemRequirements2 = 61,

    /// <summary>
    /// The version of the White Wizard monster and default invasion drop groups update.
    /// </summary>
    WhiteWizardAndInvasionDropsSeason6 = 62,

    /// <summary>
    /// The version of the Golden Archer data update (adds Rena item and a default reward drop group).
    /// </summary>
    GoldenArcherDataSeason6 = 63,

    /// <summary>
    /// The version of the Rena global drop on Season 1 maps (Version 0.75).
    /// </summary>
    RenaGlobalDrop075 = 64,

    /// <summary>
    /// The version of the Rena global drop on Season 1 maps (Version 0.95d).
    /// </summary>
    RenaGlobalDrop095d = 65,

    /// <summary>
    /// The version of the Rena global drop on Season 1 maps (Season 6 data).
    /// </summary>
    RenaGlobalDropSeason6 = 66,
}
