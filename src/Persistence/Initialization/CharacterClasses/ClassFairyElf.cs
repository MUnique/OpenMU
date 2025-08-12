// <copyright file="ClassFairyElf.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initialization of character classes data.
/// </summary>
internal partial class CharacterClassInitialization
{
    /// <summary>
    /// Creates the fairy elf.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="isMaster">if set to <c>true</c>, the class is a master class which has access to the master skill tree.</param>
    /// <param name="nextGenerationClass">The next generation class.</param>
    /// <param name="canGetCreated">If set to <c>true</c>, it can get created by the player.</param>
    /// <returns>The created character class.</returns>
    protected CharacterClass CreateFairyElf(CharacterClassNumber number, string name, bool isMaster, CharacterClass? nextGenerationClass, bool canGetCreated)
    {
        var ammunitionDmgIncrease = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Ammunition damage increase", string.Empty);
        this.GameConfiguration.Attributes.Add(ammunitionDmgIncrease);

        var result = this.Context.CreateNew<CharacterClass>();
        result.SetGuid((byte)number);
        this.GameConfiguration.CharacterClasses.Add(result);
        result.CanGetCreated = canGetCreated;
        result.HomeMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == NoriaMapId);
        result.Number = (byte)number;
        result.Name = name;
        result.IsMasterClass = isMaster;
        result.NextGenerationClass = nextGenerationClass;
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.PointsPerLevelUp, 5, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 22, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 25, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 20, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 15, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 80, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 30, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.AmmunitionAmount, 0, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Resets, 0, false));

        this.AddCommonAttributeRelationships(result.AttributeCombinations);

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrengthAndAgility, Stats.TotalAgility, Stats.TotalStrength, InputOperator.Add));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 10, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 0.25f, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.TotalLevel));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackSpeed, 1.0f / 50, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MagicSpeed, 1.0f / 50, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, Stats.TotalLevel));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.TotalLevel));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryMinDmg, 1.0f / 7, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryMaxDmg, 1.0f / 4, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryMinDmg, 1.0f / 14, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryMaxDmg, 1.0f / 8, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MeleeMinDmg, 1.0f / 7, Stats.TotalStrengthAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MeleeMaxDmg, 1.0f / 4, Stats.TotalStrengthAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.PhysicalBaseDmgIncrease, 1, ammunitionDmgIncrease, InputOperator.Add, AggregateType.Multiplicate));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryAttackMode, 1, Stats.IsBowEquipped));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ArcheryAttackMode, 1, Stats.IsCrossBowEquipped));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MeleeAttackMode, 0, Stats.ArcheryAttackMode, InputOperator.ExponentiateByAttribute));

        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MinimumPhysBaseDmg, Stats.ArcheryAttackMode, Stats.ArcheryMinDmg));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MaximumPhysBaseDmg, Stats.ArcheryAttackMode, Stats.ArcheryMaxDmg));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(ammunitionDmgIncrease, Stats.ArcheryAttackMode, Stats.AmmunitionDamageBonus));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MinimumPhysBaseDmg, Stats.MeleeAttackMode, Stats.MeleeMinDmg));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MaximumPhysBaseDmg, Stats.MeleeAttackMode, Stats.MeleeMaxDmg));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.AttackSpeedAny, Stats.IsBowEquipped, Stats.WeaponMasteryAttackSpeed));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 3, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 7, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.TotalEnergy));

        if (!this.UseClassicPvp)
        {
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f, Stats.DefenseFinal));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.TotalLevel, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.1f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.TotalLevel));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 0.6f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.TotalLevel));
        }

        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(39, Stats.MaximumHealth));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(6, Stats.MaximumMana));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SkillMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));

        this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);

        return result;
    }

    private CharacterClass CreateMuseElf(CharacterClass highElf)
    {
        return this.CreateFairyElf(CharacterClassNumber.MuseElf, "Muse Elf", false, highElf, false);
    }

    private CharacterClass CreateHighElf()
    {
        var result = this.CreateFairyElf(CharacterClassNumber.HighElf, "High Elf", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }
}