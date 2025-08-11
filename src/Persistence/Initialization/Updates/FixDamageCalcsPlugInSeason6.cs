// <copyright file="FixDamageCalcsPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes character stats, skills, magic effects, items, and options related to damage. It also adds the Berserker magic effect.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("077BA63D-F201-41BE-8A65-CFB859482A1B")]
public class FixDamageCalcsPlugInSeason6 : FixDamageCalcsPlugInBase
{
    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update fixes character stats, skills, magic effects, items, and options related to damage. It also adds the Berserker magic effect.";

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDamageCalcsSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.UpdateExcellentOptions(gameConfiguration);
        this.UpdateAmmoItems(context, gameConfiguration, [0, 0.03f, 0.05f, 0.07f], true);
        this.AddDinorantBasePowerUp(context, gameConfiguration);
        this.UpdateMGStaffsItemSlot(gameConfiguration);

        // Create Berserker magic effect
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Berserker;
        magicEffect.Name = "Berserker Buff Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 30; // 30 Seconds

        var durationPerEnergy = context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 20f; // 20 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Mana (and damage) multiplier (buff)
        var manaPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaPowerUpDefinition);
        manaPowerUpDefinition.TargetAttribute = Stats.BerserkerManaMultiplier.GetPersistent(gameConfiguration);
        manaPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();

        var manaMultiplier = context.CreateNew<AttributeRelationship>();
        manaMultiplier.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        manaMultiplier.InputOperator = InputOperator.Multiply;
        manaMultiplier.InputOperand = 1f / 3000f;
        manaPowerUpDefinition.Boost.RelatedValues.Add(manaMultiplier);

        // Health (and defense) multiplier factor (debuff)
        var healthPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(healthPowerUpDefinition);
        healthPowerUpDefinition.TargetAttribute = Stats.BerserkerHealthDecrement.GetPersistent(gameConfiguration);
        healthPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        healthPowerUpDefinition.Boost.ConstantValue.Value = -0.4f;

        var healthMultiplier = context.CreateNew<AttributeRelationship>();
        healthMultiplier.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        healthMultiplier.InputOperator = InputOperator.Multiply;
        healthMultiplier.InputOperand = 1f / 6000f;
        healthPowerUpDefinition.Boost.RelatedValues.Add(healthMultiplier);

        // Min physical damage bonus (later gets multiplied by the mana multiplier)
        var minPhysPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(minPhysPowerUpDefinition);
        minPhysPowerUpDefinition.TargetAttribute = Stats.BerserkerMinPhysDmgBonus.GetPersistent(gameConfiguration);
        minPhysPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        minPhysPowerUpDefinition.Boost.ConstantValue.Value = 140f;

        var minDmgPerStrengthAndAgility = context.CreateNew<AttributeRelationship>();
        minDmgPerStrengthAndAgility.InputAttribute = Stats.TotalStrengthAndAgility.GetPersistent(gameConfiguration);
        minDmgPerStrengthAndAgility.InputOperator = InputOperator.Multiply;
        minDmgPerStrengthAndAgility.InputOperand = 1.0f / 50;
        minPhysPowerUpDefinition.Boost.RelatedValues.Add(minDmgPerStrengthAndAgility);

        // Max physical damage bonus (later gets multiplied by the mana multiplier)
        var maxPhysPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(maxPhysPowerUpDefinition);
        maxPhysPowerUpDefinition.TargetAttribute = Stats.BerserkerMaxPhysDmgBonus.GetPersistent(gameConfiguration);
        maxPhysPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        maxPhysPowerUpDefinition.Boost.ConstantValue.Value = 160f;

        var maxDmgPerStrengthAndAgility = context.CreateNew<AttributeRelationship>();
        maxDmgPerStrengthAndAgility.InputAttribute = Stats.TotalStrengthAndAgility.GetPersistent(gameConfiguration);
        maxDmgPerStrengthAndAgility.InputOperator = InputOperator.Multiply;
        maxDmgPerStrengthAndAgility.InputOperand = 1.0f / 30;
        maxPhysPowerUpDefinition.Boost.RelatedValues.Add(maxDmgPerStrengthAndAgility);

        // Placeholder for the Berserker Strengthener master skill
        var strengthenerCurseDmgMultiplier = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(strengthenerCurseDmgMultiplier);
        strengthenerCurseDmgMultiplier.TargetAttribute = Stats.BerserkerCurseMultiplier.GetPersistent(gameConfiguration);
        strengthenerCurseDmgMultiplier.Boost = context.CreateNew<PowerUpDefinitionValue>();

        // Placeholder for the Berserker Proficiency master skill
        var proficiencyDmgMultiplier = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(proficiencyDmgMultiplier);
        proficiencyDmgMultiplier.TargetAttribute = Stats.BerserkerProficiencyMultiplier.GetPersistent(gameConfiguration);
        proficiencyDmgMultiplier.Boost = context.CreateNew<PowerUpDefinitionValue>();

        var berserkers = gameConfiguration.Skills.Where(s =>
            s.Number == (short)SkillNumber.Berserker ||
            s.Number == (short)SkillNumber.BerserkerStrengthener ||
            s.Number == (short)SkillNumber.BerserkerProficiency);
        foreach (var berserker_ in berserkers)
        {
            berserker_.SkillType = SkillType.Buff;
            berserker_.TargetRestriction = SkillTargetRestriction.Self;
            berserker_.MagicEffectDef = magicEffect;
        }

        // Update Infinite Arrow magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.InfiniteArrow) is { } infiniteArrowEffect)
        {
            infiniteArrowEffect.Duration!.ConstantValue.Value = 600;

            var powerUps = infiniteArrowEffect.PowerUpDefinitions.ToList();
            foreach (var powerUp in powerUps)
            {
                if (powerUp.TargetAttribute == Stats.SkillExtraManaCost || powerUp.TargetAttribute == Stats.BaseDamageBonus) // SkillExtraManaCost is the old ManaLossAfterHit
                {
                    infiniteArrowEffect.PowerUpDefinitions.Remove(powerUp);
                }
            }
        }

        // Update options
        var ancientSetsOpts = gameConfiguration.ItemOptions.Where(io => io.Name.EndsWith("(Ancient Set)"));
        var finalDamageBonus = Stats.FinalDamageBonus.GetPersistent(gameConfiguration);
        var wizardryBaseDmgIncrease = Stats.WizardryBaseDmgIncrease.GetPersistent(gameConfiguration);
        foreach (var ancientSetOpts in ancientSetsOpts)
        {
            if (ancientSetOpts.PossibleOptions.FirstOrDefault(o => o.PowerUpDefinition?.TargetAttribute == Stats.BaseDamageBonus) is { } dmgBonusAncOpt)
            {
                dmgBonusAncOpt.PowerUpDefinition!.TargetAttribute = finalDamageBonus;
            }

            if (ancientSetOpts.PossibleOptions.FirstOrDefault(o => o.PowerUpDefinition?.TargetAttribute == Stats.WizardryAttackDamageIncrease) is { } wizDmgIncAncOpt)
            {
                wizDmgIncAncOpt.PowerUpDefinition!.TargetAttribute = wizardryBaseDmgIncrease;
                wizDmgIncAncOpt.PowerUpDefinition.Boost!.ConstantValue.Value += 1f;
                wizDmgIncAncOpt.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            }
        }

        var armorDamageDecrease = Stats.ArmorDamageDecrease.GetPersistent(gameConfiguration);
        var harmonyDefOptions = gameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == "Harmony Defense Options");
        if (harmonyDefOptions is not null
            && harmonyDefOptions.PossibleOptions.FirstOrDefault(o => o.Number == 7) is { } dmgDecOpt)
        {
            foreach (var level in dmgDecOpt.LevelDependentOptions)
            {
                level.PowerUpDefinition!.TargetAttribute = armorDamageDecrease;
                level.PowerUpDefinition.Boost!.ConstantValue.Value = 1f - level.PowerUpDefinition!.Boost!.ConstantValue.Value;
                level.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            }
        }

        var socketOptionsFireId = new Guid("00000083-0032-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(o => o.GetId() == socketOptionsFireId) is { } fireSocketOptions
            && fireSocketOptions.PossibleOptions.FirstOrDefault(o => o.Number == 0) is { } lvlDmgSockOpt)
        {
            var totalLevel = Stats.TotalLevel.GetPersistent(gameConfiguration);
            var baseDamageBonus = Stats.BaseDamageBonus.GetPersistent(gameConfiguration);
            foreach (var level in lvlDmgSockOpt.LevelDependentOptions)
            {
                level.PowerUpDefinition!.TargetAttribute = baseDamageBonus;

                if (level.PowerUpDefinition!.Boost!.RelatedValues.FirstOrDefault() is { } relatedValue)
                {
                    relatedValue.InputAttribute = totalLevel;
                }
            }
        }

        var socketOptionsWaterId = new Guid("00000083-0033-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(o => o.GetId() == socketOptionsWaterId) is { } waterSocketOptions
            && waterSocketOptions.PossibleOptions.FirstOrDefault(o => o.Number == 3) is { } dmgDecSockOpt)
        {
            foreach (var level in dmgDecSockOpt.LevelDependentOptions)
            {
                level.PowerUpDefinition!.TargetAttribute = armorDamageDecrease;
                level.PowerUpDefinition.Boost!.ConstantValue.Value = 1f - level.PowerUpDefinition.Boost.ConstantValue.Value;
                level.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            }
        }

        // Update RF glove weapons item slot
        var leftOrRightHandSlot = gameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(0) && t.ItemSlots.Contains(1));
        if (gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Swords && i.Width == 1 && i.Number >= 32) is { } gloveWeapons)
        {
            foreach (var gloveWeapon in gloveWeapons)
            {
                gloveWeapon.ItemSlot = leftOrRightHandSlot;
            }
        }

        // Add Stats.IsBookEquipped attribute to books
        if (gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Staff && i.Height == 2) is { } bookItems)
        {
            var isBookEquipped = Stats.IsBookEquipped.GetPersistent(gameConfiguration);
            foreach (var bookItem in bookItems)
            {
                if (bookItem.BasePowerUpAttributes.FirstOrDefault(p => p.TargetAttribute == isBookEquipped) is null)
                {
                    var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                    powerUpDefinition.TargetAttribute = isBookEquipped;
                    powerUpDefinition.BaseValue = 1;
                    powerUpDefinition.AggregateType = AggregateType.AddRaw;
                    bookItem.BasePowerUpAttributes.Add(powerUpDefinition);
                }
            }
        }

        // Update skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lance) is { } lance)
        {
            lance.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PowerSlash) is { } powerSlash
            && powerSlash.Requirements.FirstOrDefault(req => req.Attribute == Stats.TotalEnergy) is { } energyRequirement)
        {
            powerSlash.Requirements.Remove(energyRequirement);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DrainLife) is { } drainLife)
        {
            drainLife.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainLightning) is { } chainLightning)
        {
            chainLightning.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Berserker) is { } berserker)
        {
            berserker.DamageType = DamageType.None;
            berserker.SkillType = SkillType.Buff;
            berserker.TargetRestriction = SkillTargetRestriction.Self;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.LightningShock) is { } lightningShock)
        {
            lightningShock.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainLightningStr) is { } chainLightningStr)
        {
            chainLightningStr.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.LightningShockStr) is { } lightningShockStr)
        {
            lightningShockStr.DamageType = DamageType.Wizardry;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DrainLifeStrengthener) is { } drainLifeStr)
        {
            drainLifeStr.DamageType = DamageType.Wizardry;
        }

        // Create Skill attribute relationships
        AddAttributeRelationship(SkillNumber.Nova, Stats.SkillDamageBonus, 1.0f / 2, Stats.TotalStrength);
        AddAttributeRelationship(SkillNumber.Nova, Stats.SkillDamageBonus, 1, Stats.NovaStageDamage);

        AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillDamageBonus, 1.0f / 10, Stats.TotalStrength);
        AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillDamageBonus, 1.0f / 5, Stats.TotalLeadership);
        AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillDamageBonus, 10, Stats.HorseLevel);

        AddAttributeRelationship(SkillNumber.ElectricSpike, Stats.SkillDamageBonus, 50, Stats.NearbyPartyMemberCount);
        AddAttributeRelationship(SkillNumber.ElectricSpike, Stats.SkillDamageBonus, 1.0f / 10, Stats.TotalLeadership);

        AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillDamageBonus, 1.0f / 30, Stats.TotalStrength);
        AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillDamageBonus, 1.0f / 55, Stats.TotalEnergy);

        SkillNumber[] lordSkills = [SkillNumber.Force, SkillNumber.FireBlast, SkillNumber.FireBurst, SkillNumber.ForceWave, SkillNumber.FireScream];
        foreach (var lordSkillNumber in lordSkills)
        {
            AddAttributeRelationship(lordSkillNumber, Stats.SkillDamageBonus, 1.0f / 25, Stats.TotalStrength);
            AddAttributeRelationship(lordSkillNumber, Stats.SkillDamageBonus, 1.0f / 50, Stats.TotalEnergy);
        }

        AddAttributeRelationship(SkillNumber.MultiShot, Stats.SkillMultiplier, 0.8f, Stats.SkillMultiplier, AggregateType.Multiplicate);

        void AddAttributeRelationship(SkillNumber skillNumber, AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, AggregateType aggregateType = AggregateType.AddRaw)
        {
            var skill = gameConfiguration.Skills.First(s => s.Number == (int)skillNumber);
            var relationship = CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, multiplier, sourceAttribute, aggregateType: aggregateType);
            skill.AttributeRelationships.Add(relationship);
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WeaponMasteryBladeMaster)?.MasterDefinition is { } weaponMasteryBladeMaster)
        {
            weaponMasteryBladeMaster.TargetAttribute = Stats.MasterSkillPhysBonusDmg.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TwoHandedSwordMaster)?.MasterDefinition is { } twoHandedSwordMaster)
        {
            twoHandedSwordMaster.TargetAttribute = Stats.TwoHandedSwordMasteryBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.OneHandedSwordMaster)?.MasterDefinition is { } oneHandedSwordMaster)
        {
            oneHandedSwordMaster.TargetAttribute = Stats.WeaponMasteryAttackSpeed.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.OneHandedStaffStrengthener)?.MasterDefinition is { } oneHandedtaffStr)
        {
            oneHandedtaffStr.TargetAttribute = Stats.OneHandedStaffBonusBaseDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TwoHandedStaffStrengthener)?.MasterDefinition is { } twoHandedStaffStr)
        {
            twoHandedStaffStr.TargetAttribute = Stats.TwoHandedStaffBonusBaseDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.OneHandedStaffMaster)?.MasterDefinition is { } oneHandedStaffMaster)
        {
            oneHandedStaffMaster.TargetAttribute = Stats.WeaponMasteryAttackSpeed.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TwoHandedStaffMaster)?.MasterDefinition is { } twoHandedStaffMaster)
        {
            twoHandedStaffMaster.TargetAttribute = Stats.TwoHandedStaffMasteryBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WeaponMasteryHighElf)?.MasterDefinition is { } weaponMasteryHighElf)
        {
            weaponMasteryHighElf.TargetAttribute = Stats.MasterSkillPhysBonusDmg.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BowStrengthener)?.MasterDefinition is { } bowStr)
        {
            bowStr.TargetAttribute = Stats.BowStrBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.CrossbowStrengthener)?.MasterDefinition is { } crossbowStr)
        {
            crossbowStr.TargetAttribute = Stats.CrossBowStrBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BowMastery)?.MasterDefinition is { } bowMastery)
        {
            bowMastery.TargetAttribute = Stats.WeaponMasteryAttackSpeed.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.CrossbowMastery)?.MasterDefinition is { } crossbowMastery)
        {
            crossbowMastery.TargetAttribute = Stats.CrossBowMasteryBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.InfinityArrowStr)?.MasterDefinition is { } infinityArrowStr)
        {
            infinityArrowStr.ValueFormula = $"1 + {infinityArrowStr.ValueFormula}";
            infinityArrowStr.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MagicMasterySummoner)?.MasterDefinition is { } magicMasterySummoner)
        {
            magicMasterySummoner.TargetAttribute = Stats.WizardryAndCurseBaseDmgBonus.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.OtherWorldTomeStreng)?.MasterDefinition is { } otherWorldTomeStr)
        {
            otherWorldTomeStr.TargetAttribute = Stats.BookBonusBaseDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.StickMastery)?.MasterDefinition is { } stickMastery)
        {
            stickMastery.TargetAttribute = Stats.StickMasteryBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.OtherWorldTomeMastery)?.MasterDefinition is { } otherWorldTomeMastery)
        {
            otherWorldTomeMastery.TargetAttribute = Stats.WeaponMasteryAttackSpeed.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BerserkerStrengthener)?.MasterDefinition is { } berserkerStr)
        {
            berserkerStr.TargetAttribute = Stats.BerserkerCurseMultiplier.GetPersistent(gameConfiguration);
            berserkerStr.ValueFormula += " / 100";
            berserkerStr.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BerserkerProficiency)?.MasterDefinition is { } berserkerProf)
        {
            berserkerProf.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.BerserkerStrengthener);
            berserkerProf.TargetAttribute = Stats.BerserkerProficiencyMultiplier.GetPersistent(gameConfiguration);
            berserkerProf.ValueFormula += " / 100";
            berserkerProf.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MinimumWizCurseInc)?.MasterDefinition is { } minimumWizCurseInc)
        {
            minimumWizCurseInc.TargetAttribute = Stats.MinWizardryAndCurseDmgBonus.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WeaponMasteryDuelMaster)?.MasterDefinition is { } weaponMasteryDuelMaster)
        {
            weaponMasteryDuelMaster.TargetAttribute = Stats.MasterSkillPhysBonusDmg.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WeaponMasteryLordEmperor)?.MasterDefinition is { } weaponMasteryLordEmperor)
        {
            weaponMasteryLordEmperor.TargetAttribute = Stats.MasterSkillPhysBonusDmg.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ScepterMastery)?.MasterDefinition is { } scepterMastery)
        {
            scepterMastery.TargetAttribute = Stats.ScepterMasteryBonusDamage.GetPersistent(gameConfiguration);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WeaponMasteryFistMaster)?.MasterDefinition is { } weaponMasteryFistMaster)
        {
            weaponMasteryFistMaster.TargetAttribute = Stats.MasterSkillPhysBonusDmg.GetPersistent(gameConfiguration);
        }
    }
}