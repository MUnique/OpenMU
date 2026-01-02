// <copyright file="AddSummonerBuffSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Sleep, Innovation, Damage Reflection and Weakness Summoner buff skills. It also fixes the 3rd wing full reflect option.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("B1E2D6C3-1F4A-4D7C-8C2E-3F6D9A7B8E2F")]
public class AddSummonerBuffSkillsPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Summoner Buff Skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Sleep, Innovation, Damage Reflection and Weakness Summoner buff skills. It also fixes the 3rd wing full reflect option.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddSummonerBuffSkills;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 12, 29, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Add new attributes
        var innovationDefDecrement = context.CreateNew<AttributeDefinition>(Stats.InnovationDefDecrement.Id, Stats.InnovationDefDecrement.Designation, Stats.InnovationDefDecrement.Description);
        gameConfiguration.Attributes.Add(innovationDefDecrement);
        var isAsleep = context.CreateNew<AttributeDefinition>(Stats.IsAsleep.Id, Stats.IsAsleep.Designation, Stats.IsAsleep.Description);
        gameConfiguration.Attributes.Add(isAsleep);
        var fullyReflectDamageAfterHitChance = context.CreateNew<AttributeDefinition>(Stats.FullyReflectDamageAfterHitChance.Id, Stats.FullyReflectDamageAfterHitChance.Designation, Stats.FullyReflectDamageAfterHitChance.Description);
        gameConfiguration.Attributes.Add(fullyReflectDamageAfterHitChance);

        // Fix reflect excellent option
        var excDefenseOptionsId = new Guid("00000083-0012-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == excDefenseOptionsId) is { } excDefenseOptions)
        {
            if (excDefenseOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.DamageReflection) is { } dmgReflection)
            {
                dmgReflection.PowerUpDefinition!.Boost!.ConstantValue.Value = 0.05f;
            }
        }

        // Update Weakness magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.Weakness) is { } weaknessEffect)
        {
            weaknessEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
            weaknessEffect.Chance.ConstantValue.Value = 0.1f; // 10%
        }

        var innovationEffect = this.CreateInnovationMagicEffect(context, gameConfiguration);
        var reflectionEffect = this.CreateReflectionMagicEffect(context, gameConfiguration);
        var sleepEffect = this.CreateSleepMagicEffect(context, gameConfiguration);
        var weaknessSummonerEffect = this.CreateWeaknessSummonerMagicEffect(context, gameConfiguration);

        // Update 3rd wing reflect option
        var thirWingOptionDefId = new Guid("00000083-0067-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == thirWingOptionDefId) is { } thirWingOptionDef
            && thirWingOptionDef.PossibleOptions.FirstOrDefault(po => po.PowerUpDefinition?.TargetAttribute == Stats.DamageReflection) is { } reflectOpt
            && reflectOpt.PowerUpDefinition is not null)
        {
            reflectOpt.PowerUpDefinition.TargetAttribute = fullyReflectDamageAfterHitChance;
        }

        // Update existing skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Innovation) is { } innovation)
        {
            innovation.MagicEffectDef = innovationEffect;
            innovation.SkillType = SkillType.Buff;
            innovation.AreaSkillSettings =
                this.AddAreaSkillSettings(context, false, 0, 0, 0, maximumHitsPerAttack: 5, useTargetAreaFilter: true, targetAreaDiameter: 10);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DamageReflection) is { } damageReflection)
        {
            damageReflection.MagicEffectDef = reflectionEffect;
            damageReflection.SkillType = SkillType.Buff;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Sleep) is { } sleep)
        {
            sleep.MagicEffectDef = sleepEffect;
            sleep.SkillType = SkillType.Buff;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Weakness) is { } weakness)
        {
            weakness.MagicEffectDef = weaknessSummonerEffect;
            weakness.SkillType = SkillType.Buff;
            weakness.AreaSkillSettings =
                this.AddAreaSkillSettings(context, false, 0, 0, 0, maximumHitsPerAttack: 5, useTargetAreaFilter: true, targetAreaDiameter: 10);
        }
    }

    private MagicEffectDefinition CreateInnovationMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Innovation;
        magicEffect.Name = "Innovation Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.DurationDependsOnTargetLevel = true;
        magicEffect.MonsterTargetLevelDivisor = 20;
        magicEffect.PlayerTargetLevelDivisor = 150;

        // Chance % = 32 + (Energy / 50) + (Book Rise / 6)
        magicEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.32f; // 32%

        var chancePerEnergy = context.CreateNew<AttributeRelationship>();
        chancePerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergy.InputOperator = InputOperator.Multiply;
        chancePerEnergy.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerEnergy);

        var chancePerBookRise = context.CreateNew<AttributeRelationship>();
        chancePerBookRise.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRise.InputOperator = InputOperator.Multiply;
        chancePerBookRise.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerBookRise);

        // Chance PvP % = 17 + (Energy / 50) + (Book Rise / 6)
        magicEffect.ChancePvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.ChancePvp.ConstantValue.Value = 0.17f; // 17%

        var chancePerEnergyPvp = context.CreateNew<AttributeRelationship>();
        chancePerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergyPvp.InputOperator = InputOperator.Multiply;
        chancePerEnergyPvp.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerEnergyPvp);

        var chancePerBookRisePvp = context.CreateNew<AttributeRelationship>();
        chancePerBookRisePvp.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRisePvp.InputOperator = InputOperator.Multiply;
        chancePerBookRisePvp.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerBookRisePvp);

        // Duration = 4 + (Energy / 100)
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 4; // 4 Seconds
        magicEffect.Duration.MaximumValue = 44; // 44 Seconds (based on 4k total energy cap)

        var durationPerEnergy = context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        magicEffect.DurationPvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.DurationPvp.MaximumValue = 18; // 18 Seconds (based on 4k total energy cap)

        // Duration PvP = 5 + (Energy / 300) + ((Level - Target's Level) / 150)
        var durationPerEnergyPvp = context.CreateNew<AttributeRelationship>();
        durationPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergyPvp.InputOperator = InputOperator.Multiply;
        durationPerEnergyPvp.InputOperand = 1f / 300f; // 300 energy adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerEnergyPvp);

        var durationPerLevelPvp = context.CreateNew<AttributeRelationship>();
        durationPerLevelPvp.InputAttribute = Stats.Level.GetPersistent(gameConfiguration);
        durationPerLevelPvp.InputOperator = InputOperator.Multiply;
        durationPerLevelPvp.InputOperand = 1f / 150f; // 150 levels adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerLevelPvp);

        // Defense decrease % (applies last) = 20 + (Energy / 90)
        var decDefPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDefPowerUpDefinition);
        decDefPowerUpDefinition.TargetAttribute = Stats.InnovationDefDecrement.GetPersistent(gameConfiguration);
        decDefPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDefPowerUpDefinition.Boost.ConstantValue.Value = 0.20f; // 20% decrease
        decDefPowerUpDefinition.Boost.MaximumValue = 0.64f; // 64% decrease (based on 4k total energy cap)

        var decDefPerEnergy = context.CreateNew<AttributeRelationship>();
        decDefPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        decDefPerEnergy.InputOperator = InputOperator.Multiply;
        decDefPerEnergy.InputOperand = 1f / 9000f; // 90 energy further decreases 0.01
        decDefPowerUpDefinition.Boost.RelatedValues.Add(decDefPerEnergy);

        // Defense decrease PvP % (applies last) = 12 + (Energy / 110)
        var decDefPowerUpDefinitionPvp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDefPowerUpDefinitionPvp);
        decDefPowerUpDefinitionPvp.TargetAttribute = Stats.InnovationDefDecrement.GetPersistent(gameConfiguration);
        decDefPowerUpDefinitionPvp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDefPowerUpDefinitionPvp.Boost.ConstantValue.Value = 0.12f; // 12% decrease
        decDefPowerUpDefinitionPvp.Boost.MaximumValue = 0.48f; // 48% decrease (based on 4k total energy cap)

        var decDefPerEnergyPvp = context.CreateNew<AttributeRelationship>();
        decDefPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        decDefPerEnergyPvp.InputOperator = InputOperator.Multiply;
        decDefPerEnergyPvp.InputOperand = 1f / 11000f; // 110 energy further decreases 0.01
        decDefPowerUpDefinitionPvp.Boost.RelatedValues.Add(decDefPerEnergyPvp);

        return magicEffect;
    }

    private MagicEffectDefinition CreateReflectionMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Reflection;
        magicEffect.Name = "Reflection Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;

        // Duration = 30 + (Energy / 24)
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 30; // 30 Seconds
        magicEffect.Duration.MaximumValue = 180;

        var durationPerEnergy = context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 24; // 24 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Reflection % = 30 + (Energy / 42)
        var incReflectPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(incReflectPowerUpDefinition);
        incReflectPowerUpDefinition.TargetAttribute = Stats.DamageReflection.GetPersistent(gameConfiguration);
        incReflectPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        incReflectPowerUpDefinition.Boost.ConstantValue.Value = 0.3f; // 30% increase
        incReflectPowerUpDefinition.Boost.MaximumValue = 0.6f;

        var incReflectPerEnergy = context.CreateNew<AttributeRelationship>();
        incReflectPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        incReflectPerEnergy.InputOperator = InputOperator.Multiply;
        incReflectPerEnergy.InputOperand = 1f / 4200f; // 42 energy further increases 0.01
        incReflectPowerUpDefinition.Boost.RelatedValues.Add(incReflectPerEnergy);

        return magicEffect;
    }

    private MagicEffectDefinition CreateSleepMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Sleep;
        magicEffect.Name = "Sleep Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.DurationDependsOnTargetLevel = true;
        magicEffect.MonsterTargetLevelDivisor = 20;
        magicEffect.PlayerTargetLevelDivisor = 100;

        // Chance % = 20 + (Energy / 30) + (Book Rise / 6)
        magicEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.2f; // 20%

        var chancePerEnergy = context.CreateNew<AttributeRelationship>();
        chancePerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergy.InputOperator = InputOperator.Multiply;
        chancePerEnergy.InputOperand = 1f / 3000f; // 30 energy adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerEnergy);

        var chancePerBookRise = context.CreateNew<AttributeRelationship>();
        chancePerBookRise.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRise.InputOperator = InputOperator.Multiply;
        chancePerBookRise.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerBookRise);

        // Chance PvP % = 15 + (Energy / 37) + (Book Rise / 6)
        magicEffect.ChancePvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.ChancePvp.ConstantValue.Value = 0.15f; // 15%

        var chancePerEnergyPvp = context.CreateNew<AttributeRelationship>();
        chancePerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergyPvp.InputOperator = InputOperator.Multiply;
        chancePerEnergyPvp.InputOperand = 1f / 3700f; // 37 energy adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerEnergyPvp);

        var chancePerBookRisePvp = context.CreateNew<AttributeRelationship>();
        chancePerBookRisePvp.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRisePvp.InputOperator = InputOperator.Multiply;
        chancePerBookRisePvp.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerBookRisePvp);

        // Duration = 5 + (Energy / 100)
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.Duration.MaximumValue = 20; // 20 Seconds

        var durationPerEnergy = context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Duration = 4 + (Energy / 250) + ((Level - Target's Level) / 100)
        magicEffect.DurationPvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 4; // 4 Seconds
        magicEffect.DurationPvp.MaximumValue = 10; // 10 Seconds

        var durationPerEnergyPvp = context.CreateNew<AttributeRelationship>();
        durationPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergyPvp.InputOperator = InputOperator.Multiply;
        durationPerEnergyPvp.InputOperand = 1f / 250f; // 250 energy adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerEnergyPvp);

        var durationPerLevelPvp = context.CreateNew<AttributeRelationship>();
        durationPerLevelPvp.InputAttribute = Stats.Level.GetPersistent(gameConfiguration);
        durationPerLevelPvp.InputOperator = InputOperator.Multiply;
        durationPerLevelPvp.InputOperand = 1f / 100f; // 100 levels adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerLevelPvp);

        var isAsleep = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(isAsleep);
        isAsleep.TargetAttribute = Stats.IsAsleep.GetPersistent(gameConfiguration);
        isAsleep.Boost = context.CreateNew<PowerUpDefinitionValue>();
        isAsleep.Boost.ConstantValue.Value = 1;

        return magicEffect;
    }

    private MagicEffectDefinition CreateWeaknessSummonerMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Weakness; // We will map skill to effect by hand in this update, so we use this number instead of WeaknessSummoner
        magicEffect.Name = "Weakness Effect (Summoner)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.DurationDependsOnTargetLevel = true;
        magicEffect.MonsterTargetLevelDivisor = 20;
        magicEffect.PlayerTargetLevelDivisor = 150;

        // Chance % = 32 + (Energy / 50) + (Book Rise / 6)
        magicEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.32f; // 32%

        var chancePerEnergy = context.CreateNew<AttributeRelationship>();
        chancePerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergy.InputOperator = InputOperator.Multiply;
        chancePerEnergy.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerEnergy);

        var chancePerBookRise = context.CreateNew<AttributeRelationship>();
        chancePerBookRise.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRise.InputOperator = InputOperator.Multiply;
        chancePerBookRise.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerBookRise);

        // Chance % = 17 + (Energy / 50) + (Book Rise / 6)
        magicEffect.ChancePvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.ChancePvp.ConstantValue.Value = 0.17f; // 17%

        var chancePerEnergyPvp = context.CreateNew<AttributeRelationship>();
        chancePerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        chancePerEnergyPvp.InputOperator = InputOperator.Multiply;
        chancePerEnergyPvp.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerEnergyPvp);

        var chancePerBookRisePvp = context.CreateNew<AttributeRelationship>();
        chancePerBookRisePvp.InputAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
        chancePerBookRisePvp.InputOperator = InputOperator.Multiply;
        chancePerBookRisePvp.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerBookRisePvp);

        // Duration = 4 + (Energy / 100)
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 4; // 4 Seconds
        magicEffect.Duration.MaximumValue = 44; // 44 Seconds (based on 4k total energy cap)

        var durationPerEnergy = context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Duration = 5 + (Energy / 300) + ((Level - Target's Level) / 150)
        magicEffect.DurationPvp = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.DurationPvp.MaximumValue = 18; // 18 Seconds (based on 4k total energy cap)

        var durationPerEnergyPvp = context.CreateNew<AttributeRelationship>();
        durationPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        durationPerEnergyPvp.InputOperator = InputOperator.Multiply;
        durationPerEnergyPvp.InputOperand = 1f / 300f; // 300 energy adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerEnergyPvp);

        var durationPerLevelPvp = context.CreateNew<AttributeRelationship>();
        durationPerLevelPvp.InputAttribute = Stats.Level.GetPersistent(gameConfiguration);
        durationPerLevelPvp.InputOperator = InputOperator.Multiply;
        durationPerLevelPvp.InputOperand = 1f / 150f; // 150 levels adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerLevelPvp);

        // Phys damage decrease % = 4 + (Energy / 58)
        var decDmgPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDmgPowerUpDefinition);
        decDmgPowerUpDefinition.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(gameConfiguration);
        decDmgPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinition.Boost.ConstantValue.Value = 0.04f; // 4% decrease
        decDmgPowerUpDefinition.Boost.MaximumValue = 0.73f; // 73% decrease (based on 4k total energy cap)

        var decDmgPerEnergy = context.CreateNew<AttributeRelationship>();
        decDmgPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        decDmgPerEnergy.InputOperator = InputOperator.Multiply;
        decDmgPerEnergy.InputOperand = 1f / 5800f; // 58 energy further decreases 0.01
        decDmgPowerUpDefinition.Boost.RelatedValues.Add(decDmgPerEnergy);

        // Phys damage decrease PvP % = 3 + (Energy / 93)
        var decDmgPowerUpDefinitionPvp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDmgPowerUpDefinitionPvp);
        decDmgPowerUpDefinitionPvp.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(gameConfiguration);
        decDmgPowerUpDefinitionPvp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinitionPvp.Boost.ConstantValue.Value = 0.03f; // 3% decrease
        decDmgPowerUpDefinitionPvp.Boost.MaximumValue = 0.46f; // 46% decrease (based on 4k total energy cap)

        var decDmgPerEnergyPvp = context.CreateNew<AttributeRelationship>();
        decDmgPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        decDmgPerEnergyPvp.InputOperator = InputOperator.Multiply;
        decDmgPerEnergyPvp.InputOperand = 1f / 9300f; // 93 energy further decreases 0.01
        decDmgPowerUpDefinitionPvp.Boost.RelatedValues.Add(decDmgPerEnergyPvp);

        return magicEffect;
    }

    private AreaSkillSettings AddAreaSkillSettings(
        IContext context,
        bool useFrustumFilter,
        float frustumStartWidth,
        float frustumEndWidth,
        float frustumDistance,
        bool useDeferredHits = false,
        TimeSpan delayPerOneDistance = default,
        TimeSpan delayBetweenHits = default,
        int minimumHitsPerTarget = 1,
        int maximumHitsPerTarget = 1,
        int maximumHitsPerAttack = default,
        float hitChancePerDistanceMultiplier = 1.0f,
        bool useTargetAreaFilter = false,
        float targetAreaDiameter = default)
    {
        var areaSkillSettings = context.CreateNew<AreaSkillSettings>();

        areaSkillSettings.UseFrustumFilter = useFrustumFilter;
        areaSkillSettings.FrustumStartWidth = frustumStartWidth;
        areaSkillSettings.FrustumEndWidth = frustumEndWidth;
        areaSkillSettings.FrustumDistance = frustumDistance;
        areaSkillSettings.UseTargetAreaFilter = useTargetAreaFilter;
        areaSkillSettings.TargetAreaDiameter = targetAreaDiameter;
        areaSkillSettings.UseDeferredHits = useDeferredHits;
        areaSkillSettings.DelayPerOneDistance = delayPerOneDistance;
        areaSkillSettings.DelayBetweenHits = delayBetweenHits;
        areaSkillSettings.MinimumNumberOfHitsPerTarget = minimumHitsPerTarget;
        areaSkillSettings.MaximumNumberOfHitsPerTarget = maximumHitsPerTarget;
        areaSkillSettings.MaximumNumberOfHitsPerAttack = maximumHitsPerAttack;
        areaSkillSettings.HitChancePerDistanceMultiplier = hitChancePerDistanceMultiplier;

        return areaSkillSettings;
    }
}