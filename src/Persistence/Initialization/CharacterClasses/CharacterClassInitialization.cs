// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initialization of character classes data.
/// </summary>
internal partial class CharacterClassInitialization : InitializerBase
{
    private const int LorenciaMapId = 0;
    private const int NoriaMapId = 3;
    private const int ElvenlandMapId = 51;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterClassInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets a value indicating whether to use classic PVP, which uses no shield stats and the same attack/defense rate as PvM.
    /// </summary>
    protected virtual bool UseClassicPvp => false;

    /// <summary>
    /// Creates the character classes.
    /// </summary>
    public override void Initialize()
    {
        var bladeMaster = this.CreateBladeMaster();
        var bladeKnight = this.CreateBladeKnight(bladeMaster);
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", false, bladeKnight, true);

        var grandMaster = this.CreateGrandMaster();
        var soulMaster = this.CreateSoulMaster(grandMaster);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", false, soulMaster, true);

        var highElf = this.CreateHighElf();
        var museElf = this.CreateMuseElf(highElf);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", false, museElf, true);

        var dimensionMaster = this.CreateDimensionMaster();
        var bloodySummoner = this.CreateBloodySummoner(dimensionMaster);
        this.CreateSummoner(CharacterClassNumber.Summoner, "Summoner", false, bloodySummoner, true);

        var duelMaster = this.CreateDuelMaster();
        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

        var lordEmperor = this.CreateLordEmperor();
        this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", false, lordEmperor, true);

        var fistMaster = this.CreateFistMaster();
        this.CreateRageFighter(CharacterClassNumber.RageFighter, "Rage Fighter", false, fistMaster, true);
    }

    private StatAttributeDefinition CreateStatAttributeDefinition(AttributeDefinition attribute, int value, bool increasableByPlayer)
    {
        var definition = this.Context.CreateNew<StatAttributeDefinition>(attribute.GetPersistent(this.GameConfiguration), value, increasableByPlayer);
        return definition;
    }

    private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return CharacterClassHelper.CreateAttributeRelationship(this.Context, this.GameConfiguration, targetAttribute, multiplier, sourceAttribute, inputOperator, aggregateType);
    }

    private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, AttributeDefinition multiplierAttribute, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return CharacterClassHelper.CreateAttributeRelationship(this.Context, this.GameConfiguration, targetAttribute, multiplierAttribute, sourceAttribute, inputOperator, aggregateType);
    }

    private AttributeRelationship CreateConditionalRelationship(AttributeDefinition targetAttribute, AttributeDefinition conditionalAttribute, AttributeDefinition sourceAttribute, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return CharacterClassHelper.CreateConditionalRelationship(this.Context, this.GameConfiguration, targetAttribute, conditionalAttribute, sourceAttribute, aggregateType);
    }

    private ConstValueAttribute CreateConstValueAttribute(float value, AttributeDefinition attribute, AggregateType aggregateType = AggregateType.AddRaw)
    {
        return CharacterClassHelper.CreateConstValueAttribute(this.Context, this.GameConfiguration, value, attribute, aggregateType);
    }

    private void AddCommonAttributeRelationships(ICollection<AttributeRelationship> attributeRelationships)
    {
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalLevel, 1, Stats.Level));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalLevel, 1, Stats.MasterLevel));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1, Stats.DefenseShield));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseFinal, 0.5f, Stats.DefenseBase));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseFinal));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseFinal));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AttackSpeedAny, 1, Stats.AttackSpeedByWeapon));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AttackSpeed, 1, Stats.AttackSpeedAny));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MagicSpeed, 1, Stats.AttackSpeedAny));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.BaseMinDamageBonus));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.BaseMaxDamageBonus));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmg, 1, Stats.BaseDamageBonus, aggregateType: AggregateType.AddFinal));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.PhysicalBaseDmgIncrease, aggregateType: AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.PhysicalBaseDmgIncrease, aggregateType: AggregateType.Multiplicate));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.SwellLifeHealthIncrease, aggregateType: AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, Stats.SwellLifeManaIncrease, aggregateType: AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.IncreaseBlockBonus, 1, Stats.DefenseRatePvm, aggregateType: AggregateType.Multiplicate));

        // If two weapons are equipped (DK, MG, Sum, RF) we average the weapon attack speed
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AreTwoWeaponsEquipped, 1, Stats.EquippedWeaponCount, InputOperator.Maximum));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AttackSpeedByWeapon, 0.5f, Stats.AreTwoWeaponsEquipped, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));

        var tempDefense = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Defense Bonus multiplier with Shield", string.Empty);
        this.GameConfiguration.Attributes.Add(tempDefense);
        attributeRelationships.Add(this.CreateConditionalRelationship(tempDefense, Stats.IsShieldEquipped, Stats.DefenseIncreaseWithEquippedShield));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseFinal, 1, tempDefense, InputOperator.Add, AggregateType.Multiplicate));

        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.DefenseFinal, Stats.DefenseShield, Stats.ShieldItemDefenseIncrease));
        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.DefenseFinal, Stats.IsShieldEquipped, Stats.BonusDefenseWithShield, AggregateType.AddFinal));
        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.DefenseRatePvm, Stats.IsShieldEquipped, Stats.BonusDefenseRateWithShield, AggregateType.AddFinal));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseDecrement, -1, Stats.InnovationDefDecrement));

        // The health and mana recovery rise the longer a character rests, until Stats.RestingRecoveryBonus reaches its maximum value.
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.RestingRecoveryBonus, 0.003f, Stats.RestingDuration));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.HealthRecoveryMultiplier, 1, Stats.RestingRecoveryBonus));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.ManaRecoveryMultiplier, 1, Stats.RestingRecoveryBonus));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AbilityRecoveryAbsolute, 3, Stats.IsInSafezone));

        // The shield recovery is only active at the safezone, except the character has the attribute which enables it everywhere.
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryActive, Stats.ShieldRecoveryEverywhere, Stats.IsInSafezone, InputOperator.Maximum));

        if (this.UseClassicPvp)
        {
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 1, Stats.DefenseRatePvm));
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 1, Stats.AttackRatePvm));
        }
        else
        {
            // The longer the shield recovers without interruption, the higher its recovery rate gets.
            // With the default values, the rate rises linearly from its base value up to the triple of it after 25 seconds.
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryRampBonus, 2f / 25f, Stats.ShieldRecoveryDuration));
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryMultiplier, 1, Stats.ShieldRecoveryRampBonus, InputOperator.Add, AggregateType.Multiplicate));
        }

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.CanFly, 1, Stats.IsDinorantEquipped));
    }

    private void AddCommonBaseAttributeValues(ICollection<ConstValueAttribute> baseAttributeValues, bool isMaster)
    {
        baseAttributeValues.Add(this.CreateConstValueAttribute(0.037f, Stats.ManaRecoveryMultiplier));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ExperienceRate));
        baseAttributeValues.Add(this.CreateConstValueAttribute(0.03f, Stats.PoisonDamageMultiplier));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ItemDurationIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.AbilityRecoveryAbsolute));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.PhysicalBaseDmgIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(-1, Stats.AreTwoWeaponsEquipped));
        baseAttributeValues.Add(this.CreateConstValueAttribute(-1, Stats.HasDoubleWield));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DefenseDecrement));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SwellLifeHealthIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SwellLifeManaIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(0.1f, Stats.DurabilityReductionFactor));
        baseAttributeValues.Add(this.CreateConstValueAttribute(0, Stats.IncreaseBlockBonus));   // Nullify the Multiplicate values until DefSuccessRateIncPowUp master Skill is learned

        if (isMaster)
        {
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MasterPointsPerLevelUp));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MasterExperienceRate));
        }

        if (!this.UseClassicPvp)
        {
            baseAttributeValues.Add(this.CreateConstValueAttribute(100, Stats.ShieldRecoveryMultiplier));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1f / 75000, Stats.ShieldRecoveryMultiplier, AggregateType.Multiplicate)); // 1 / (30 * 100 * 25)
        }
    }

    /// <summary>
    /// Adds double wield attribute relationships applicable to characters that can double wield (DK, MG, and RF).
    /// A double wield grants 110% physical attack damage (55% base damage, later doubled on damage calculations).
    /// </summary>
    private void AddDoubleWieldAttributeRelationships(ICollection<AttributeRelationship> attributeRelationships)
    {
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.HasDoubleWield, 1, Stats.DoubleWieldWeaponCount, InputOperator.Maximum));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmgIncrease, 0.55f, Stats.HasDoubleWield, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.MinimumPhysBaseDmgByWeapon, Stats.HasDoubleWield, Stats.MinPhysBaseDmgByRightWeapon));
        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.MaximumPhysBaseDmgByWeapon, Stats.HasDoubleWield, Stats.MaxPhysBaseDmgByRightWeapon));

        // We need to average the base damage of the two weapons and their item option and excellent options.
        // For PhysicalBaseDmgIncrease, to avoid using extra attributes, we use ad-hoc AggregateTypes (see ItemPowerUpFactory.GetPowerUpsOfItemOptions())
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmgByWeapon, 0.5f, Stats.HasDoubleWield, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmgByWeapon, 0.5f, Stats.HasDoubleWield, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmg, 0.5f, Stats.HasDoubleWield, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmgIncrease, 0.5f, Stats.HasDoubleWield, InputOperator.ExponentiateByAttribute, AggregateType.Multiplicate));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmgIncrease, 1, Stats.HasDoubleWield));
    }
}