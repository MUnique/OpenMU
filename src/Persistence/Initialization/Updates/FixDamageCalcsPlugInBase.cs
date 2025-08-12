// <copyright file="FixDamageCalcsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// This update fixes character stats, magic effects, items, and options related to damage.
/// </summary>
public abstract class FixDamageCalcsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Damage Calculations";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes character stats, magic effects, items, and options related to damage.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 08, 7, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update attributes
        var totalStrengthAndAgility = context.CreateNew<AttributeDefinition>(Stats.TotalStrengthAndAgility.Id, Stats.TotalStrengthAndAgility.Designation, Stats.TotalStrengthAndAgility.Description);
        gameConfiguration.Attributes.Add(totalStrengthAndAgility);
        var minPhysBaseDmgByRightWeapon = context.CreateNew<AttributeDefinition>(Stats.MinPhysBaseDmgByRightWeapon.Id, Stats.MinPhysBaseDmgByRightWeapon.Designation, Stats.MinPhysBaseDmgByRightWeapon.Description);
        gameConfiguration.Attributes.Add(minPhysBaseDmgByRightWeapon);
        var maxPhysBaseDmgByRightWeapon = context.CreateNew<AttributeDefinition>(Stats.MaxPhysBaseDmgByRightWeapon.Id, Stats.MaxPhysBaseDmgByRightWeapon.Designation, Stats.MaxPhysBaseDmgByRightWeapon.Description);
        gameConfiguration.Attributes.Add(maxPhysBaseDmgByRightWeapon);
        var minWizardryAndCurseDmgBonus = context.CreateNew<AttributeDefinition>(Stats.MinWizardryAndCurseDmgBonus.Id, Stats.MinWizardryAndCurseDmgBonus.Designation, Stats.MinWizardryAndCurseDmgBonus.Description);
        gameConfiguration.Attributes.Add(minWizardryAndCurseDmgBonus);
        var wizardryAndCurseBaseDmgBonus = context.CreateNew<AttributeDefinition>(Stats.WizardryAndCurseBaseDmgBonus.Id, Stats.WizardryAndCurseBaseDmgBonus.Designation, Stats.WizardryAndCurseBaseDmgBonus.Description);
        gameConfiguration.Attributes.Add(wizardryAndCurseBaseDmgBonus);
        var finalDamageBonus = context.CreateNew<AttributeDefinition>(Stats.FinalDamageBonus.Id, Stats.FinalDamageBonus.Designation, Stats.FinalDamageBonus.Description);
        gameConfiguration.Attributes.Add(finalDamageBonus);
        var hasDoubleWield = context.CreateNew<AttributeDefinition>(Stats.HasDoubleWield.Id, Stats.HasDoubleWield.Designation, Stats.HasDoubleWield.Description);
        gameConfiguration.Attributes.Add(hasDoubleWield);
        var doubleWieldWeaponCount = context.CreateNew<AttributeDefinition>(Stats.DoubleWieldWeaponCount.Id, Stats.DoubleWieldWeaponCount.Designation, Stats.DoubleWieldWeaponCount.Description);
        gameConfiguration.Attributes.Add(doubleWieldWeaponCount);
        var wizardryBaseDmgIncrease = context.CreateNew<AttributeDefinition>(Stats.WizardryBaseDmgIncrease.Id, Stats.WizardryBaseDmgIncrease.Designation, Stats.WizardryBaseDmgIncrease.Description);
        gameConfiguration.Attributes.Add(wizardryBaseDmgIncrease);
        var physicalBaseDmgIncrease = context.CreateNew<AttributeDefinition>(Stats.PhysicalBaseDmgIncrease.Id, Stats.PhysicalBaseDmgIncrease.Designation, Stats.PhysicalBaseDmgIncrease.Description);
        gameConfiguration.Attributes.Add(physicalBaseDmgIncrease);
        var crossBowMasteryBonusDamage = context.CreateNew<AttributeDefinition>(Stats.CrossBowMasteryBonusDamage.Id, Stats.CrossBowMasteryBonusDamage.Designation, Stats.CrossBowMasteryBonusDamage.Description);
        gameConfiguration.Attributes.Add(crossBowMasteryBonusDamage);
        var meleeAttackMode = context.CreateNew<AttributeDefinition>(Stats.MeleeAttackMode.Id, Stats.MeleeAttackMode.Designation, Stats.MeleeAttackMode.Description);
        gameConfiguration.Attributes.Add(meleeAttackMode);
        var meleeMinDmg = context.CreateNew<AttributeDefinition>(Stats.MeleeMinDmg.Id, Stats.MeleeMinDmg.Designation, Stats.MeleeMinDmg.Description);
        gameConfiguration.Attributes.Add(meleeMinDmg);
        var meleeMaxDmg = context.CreateNew<AttributeDefinition>(Stats.MeleeMaxDmg.Id, Stats.MeleeMaxDmg.Designation, Stats.MeleeMaxDmg.Description);
        gameConfiguration.Attributes.Add(meleeMaxDmg);
        var archeryAttackMode = context.CreateNew<AttributeDefinition>(Stats.ArcheryAttackMode.Id, Stats.ArcheryAttackMode.Designation, Stats.ArcheryAttackMode.Description);
        gameConfiguration.Attributes.Add(archeryAttackMode);
        var archeryMinDmg = context.CreateNew<AttributeDefinition>(Stats.ArcheryMinDmg.Id, Stats.ArcheryMinDmg.Designation, Stats.ArcheryMinDmg.Description);
        gameConfiguration.Attributes.Add(archeryMinDmg);
        var archeryMaxDmg = context.CreateNew<AttributeDefinition>(Stats.ArcheryMaxDmg.Id, Stats.ArcheryMaxDmg.Designation, Stats.ArcheryMaxDmg.Description);
        gameConfiguration.Attributes.Add(archeryMaxDmg);
        var greaterDamageBonus = context.CreateNew<AttributeDefinition>(Stats.GreaterDamageBonus.Id, Stats.GreaterDamageBonus.Designation, Stats.GreaterDamageBonus.Description);
        gameConfiguration.Attributes.Add(greaterDamageBonus);
        var isOneHandedSwordEquipped = Stats.IsOneHandedSwordEquipped.GetPersistent(gameConfiguration);
        isOneHandedSwordEquipped.MaximumValue = 1;
        var weaponMasteryAttackSpeed = context.CreateNew<AttributeDefinition>(Stats.WeaponMasteryAttackSpeed.Id, Stats.WeaponMasteryAttackSpeed.Designation, Stats.WeaponMasteryAttackSpeed.Description);
        gameConfiguration.Attributes.Add(weaponMasteryAttackSpeed);
        var twoHandedSwordMasteryBonusDamage = context.CreateNew<AttributeDefinition>(Stats.TwoHandedSwordMasteryBonusDamage.Id, Stats.TwoHandedSwordMasteryBonusDamage.Designation, Stats.TwoHandedSwordMasteryBonusDamage.Description);
        gameConfiguration.Attributes.Add(twoHandedSwordMasteryBonusDamage);
        var masterSkillPhysBonusDmg = context.CreateNew<AttributeDefinition>(Stats.MasterSkillPhysBonusDmg.Id, Stats.MasterSkillPhysBonusDmg.Designation, Stats.MasterSkillPhysBonusDmg.Description);
        gameConfiguration.Attributes.Add(masterSkillPhysBonusDmg);
        var isOneHandedStaffEquipped = gameConfiguration.Attributes.First(a => a.Id == Stats.IsOneHandedStaffEquipped.Id);
        isOneHandedStaffEquipped.MaximumValue = 1;
        var twoHandedStaffMasteryBonusDamage = context.CreateNew<AttributeDefinition>(Stats.TwoHandedStaffMasteryBonusDamage.Id, Stats.TwoHandedStaffMasteryBonusDamage.Designation, Stats.TwoHandedStaffMasteryBonusDamage.Description);
        gameConfiguration.Attributes.Add(twoHandedStaffMasteryBonusDamage);
        var isBookEquipped = context.CreateNew<AttributeDefinition>(Stats.IsBookEquipped.Id, Stats.IsBookEquipped.Designation, Stats.IsBookEquipped.Description);
        gameConfiguration.Attributes.Add(isBookEquipped);
        var bookBonusBaseDamage = context.CreateNew<AttributeDefinition>(Stats.BookBonusBaseDamage.Id, Stats.BookBonusBaseDamage.Designation, Stats.BookBonusBaseDamage.Description);
        gameConfiguration.Attributes.Add(bookBonusBaseDamage);
        var stickMasteryBonusDamage = context.CreateNew<AttributeDefinition>(Stats.StickMasteryBonusDamage.Id, Stats.StickMasteryBonusDamage.Designation, Stats.StickMasteryBonusDamage.Description);
        gameConfiguration.Attributes.Add(stickMasteryBonusDamage);
        var scepterMasteryBonusDamage = context.CreateNew<AttributeDefinition>(Stats.ScepterMasteryBonusDamage.Id, Stats.ScepterMasteryBonusDamage.Designation, Stats.ScepterMasteryBonusDamage.Description);
        gameConfiguration.Attributes.Add(scepterMasteryBonusDamage);
        var armorDamageDecrease = context.CreateNew<AttributeDefinition>(Stats.ArmorDamageDecrease.Id, Stats.ArmorDamageDecrease.Designation, Stats.ArmorDamageDecrease.Description);
        gameConfiguration.Attributes.Add(armorDamageDecrease);
        var berserkerManaMultiplier = context.CreateNew<AttributeDefinition>(Stats.BerserkerManaMultiplier.Id, Stats.BerserkerManaMultiplier.Designation, Stats.BerserkerManaMultiplier.Description);
        gameConfiguration.Attributes.Add(berserkerManaMultiplier);
        var berserkerHealthDecrement = context.CreateNew<AttributeDefinition>(Stats.BerserkerHealthDecrement.Id, Stats.BerserkerHealthDecrement.Designation, Stats.BerserkerHealthDecrement.Description);
        gameConfiguration.Attributes.Add(berserkerHealthDecrement);
        var berserkerCurseMultiplier = context.CreateNew<AttributeDefinition>(Stats.BerserkerCurseMultiplier.Id, Stats.BerserkerCurseMultiplier.Designation, Stats.BerserkerCurseMultiplier.Description);
        gameConfiguration.Attributes.Add(berserkerCurseMultiplier);
        var berserkerWizardryMultiplier = context.CreateNew<AttributeDefinition>(Stats.BerserkerWizardryMultiplier.Id, Stats.BerserkerWizardryMultiplier.Designation, Stats.BerserkerWizardryMultiplier.Description);
        gameConfiguration.Attributes.Add(berserkerWizardryMultiplier);
        var berserkerProficiencyMultiplier = context.CreateNew<AttributeDefinition>(Stats.BerserkerProficiencyMultiplier.Id, Stats.BerserkerProficiencyMultiplier.Designation, Stats.BerserkerProficiencyMultiplier.Description);
        gameConfiguration.Attributes.Add(berserkerProficiencyMultiplier);
        var berserkerMinPhysDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMinPhysDmgBonus.Id, Stats.BerserkerMinPhysDmgBonus.Designation, Stats.BerserkerMinPhysDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMinPhysDmgBonus);
        var berserkerMaxPhysDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMaxPhysDmgBonus.Id, Stats.BerserkerMaxPhysDmgBonus.Designation, Stats.BerserkerMaxPhysDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMaxPhysDmgBonus);
        var berserkerMinWizDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMinWizDmgBonus.Id, Stats.BerserkerMinWizDmgBonus.Designation, Stats.BerserkerMinWizDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMinWizDmgBonus);
        var berserkerMaxWizDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMaxWizDmgBonus.Id, Stats.BerserkerMaxWizDmgBonus.Designation, Stats.BerserkerMaxWizDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMaxWizDmgBonus);
        var berserkerMinCurseDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMinCurseDmgBonus.Id, Stats.BerserkerMinCurseDmgBonus.Designation, Stats.BerserkerMinCurseDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMinCurseDmgBonus);
        var berserkerMaxCurseDmgBonus = context.CreateNew<AttributeDefinition>(Stats.BerserkerMaxCurseDmgBonus.Id, Stats.BerserkerMaxCurseDmgBonus.Designation, Stats.BerserkerMaxCurseDmgBonus.Description);
        gameConfiguration.Attributes.Add(berserkerMaxCurseDmgBonus);
        var weaknessPhysDmgDecrement = context.CreateNew<AttributeDefinition>(Stats.WeaknessPhysDmgDecrement.Id, Stats.WeaknessPhysDmgDecrement.Designation, Stats.WeaknessPhysDmgDecrement.Description);
        gameConfiguration.Attributes.Add(weaknessPhysDmgDecrement);
        var iceDamageBonus = context.CreateNew<AttributeDefinition>(Stats.IceDamageBonus.Id, Stats.IceDamageBonus.Designation, Stats.IceDamageBonus.Description);
        gameConfiguration.Attributes.Add(iceDamageBonus);
        var fireDamageBonus = context.CreateNew<AttributeDefinition>(Stats.FireDamageBonus.Id, Stats.FireDamageBonus.Designation, Stats.FireDamageBonus.Description);
        gameConfiguration.Attributes.Add(fireDamageBonus);
        var waterDamageBonus = context.CreateNew<AttributeDefinition>(Stats.WaterDamageBonus.Id, Stats.WaterDamageBonus.Designation, Stats.WaterDamageBonus.Description);
        gameConfiguration.Attributes.Add(waterDamageBonus);
        var earthDamageBonus = context.CreateNew<AttributeDefinition>(Stats.EarthDamageBonus.Id, Stats.EarthDamageBonus.Designation, Stats.EarthDamageBonus.Description);
        gameConfiguration.Attributes.Add(earthDamageBonus);
        var windDamageBonus = context.CreateNew<AttributeDefinition>(Stats.WindDamageBonus.Id, Stats.WindDamageBonus.Designation, Stats.WindDamageBonus.Description);
        gameConfiguration.Attributes.Add(windDamageBonus);
        var poisonDamageBonus = context.CreateNew<AttributeDefinition>(Stats.PoisonDamageBonus.Id, Stats.PoisonDamageBonus.Designation, Stats.PoisonDamageBonus.Description);
        gameConfiguration.Attributes.Add(poisonDamageBonus);
        var lightningDamageBonus = context.CreateNew<AttributeDefinition>(Stats.LightningDamageBonus.Id, Stats.LightningDamageBonus.Designation, Stats.LightningDamageBonus.Description);
        gameConfiguration.Attributes.Add(lightningDamageBonus);
        var isDinorantEquipped = context.CreateNew<AttributeDefinition>(Stats.IsDinorantEquipped.Id, Stats.IsDinorantEquipped.Designation, Stats.IsDinorantEquipped.Description);
        gameConfiguration.Attributes.Add(isDinorantEquipped);
        var ammunitionDamageBonus = context.CreateNew<AttributeDefinition>(Stats.AmmunitionDamageBonus.Id, Stats.AmmunitionDamageBonus.Designation, Stats.AmmunitionDamageBonus.Description);
        gameConfiguration.Attributes.Add(ammunitionDamageBonus);
        var nearbyPartyMemberCount = context.CreateNew<AttributeDefinition>(Stats.NearbyPartyMemberCount.Id, Stats.NearbyPartyMemberCount.Designation, Stats.NearbyPartyMemberCount.Description);
        gameConfiguration.Attributes.Add(nearbyPartyMemberCount);
        var skillLevel = context.CreateNew<AttributeDefinition>(Stats.SkillLevel.Id, Stats.SkillLevel.Designation, Stats.SkillLevel.Description);
        gameConfiguration.Attributes.Add(skillLevel);

        var obsoleteNovaBonusDamage = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == new Guid("0ABE8432-7F23-4CE9-BBCC-B8A633DAC08B")); // we remove this later

        // Update attribute combinations
        var minimumPhysBaseDmg = Stats.MinimumPhysBaseDmg.GetPersistent(gameConfiguration);
        var maximumPhysBaseDmg = Stats.MaximumPhysBaseDmg.GetPersistent(gameConfiguration);
        var baseMinDamageBonus = Stats.BaseMinDamageBonus.GetPersistent(gameConfiguration);
        var baseMaxDamageBonus = Stats.BaseMaxDamageBonus.GetPersistent(gameConfiguration);
        var physicalBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);
        var baseDamageBonus = Stats.BaseDamageBonus.GetPersistent(gameConfiguration);
        var canFly = Stats.CanFly.GetPersistent(gameConfiguration);
        var minimumPhysBaseDmgByWeapon = Stats.MinimumPhysBaseDmgByWeapon.GetPersistent(gameConfiguration);
        var maximumPhysBaseDmgByWeapon = Stats.MaximumPhysBaseDmgByWeapon.GetPersistent(gameConfiguration);
        var attackSpeedAny = Stats.AttackSpeedAny.GetPersistent(gameConfiguration);

        // DL
        var isScepterEquipped = Stats.IsScepterEquipped.GetPersistent(gameConfiguration);
        var ravenBonusDamage = Stats.RavenBonusDamage.GetPersistent(gameConfiguration);
        var scepterPetBonusDamage = Stats.ScepterPetBonusDamage.GetPersistent(gameConfiguration);
        var bonusDamageWithScepter = Stats.BonusDamageWithScepter.GetPersistent(gameConfiguration);
        var totalStrength = Stats.TotalStrength.GetPersistent(gameConfiguration);
        var ravenCriticalDamageChance = Stats.RavenCriticalDamageChance.GetPersistent(gameConfiguration);

        // DW, MG, Summoner
        var minimumWizBaseDmg = Stats.MinimumWizBaseDmg.GetPersistent(gameConfiguration);
        var maximumWizBaseDmg = Stats.MaximumWizBaseDmg.GetPersistent(gameConfiguration);
        var wizardryBaseDmg = Stats.WizardryBaseDmg.GetPersistent(gameConfiguration);

        // Elf
        var totalAgility = Stats.TotalAgility.GetPersistent(gameConfiguration);
        var isBowEquipped = Stats.IsBowEquipped.GetPersistent(gameConfiguration);
        var isCrossBowEquipped = Stats.IsCrossBowEquipped.GetPersistent(gameConfiguration);

        // Summoner
        var minimumCurseBaseDmg = Stats.MinimumCurseBaseDmg.GetPersistent(gameConfiguration);
        var maximumCurseBaseDmg = Stats.MaximumCurseBaseDmg.GetPersistent(gameConfiguration);
        var curseBaseDmg = Stats.CurseBaseDmg.GetPersistent(gameConfiguration);
        var maximumMana = Stats.MaximumMana.GetPersistent(gameConfiguration);
        var maximumHealth = Stats.MaximumHealth.GetPersistent(gameConfiguration);
        var defensePvm = Stats.DefensePvm.GetPersistent(gameConfiguration);
        var defensePvp = Stats.DefensePvp.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            // Remove common base attribute
            if (charClass.BaseAttributeValues.FirstOrDefault(attr => attr.Definition == Stats.TwoHandedWeaponDamageIncrease) is { } twoHandedWeaponDamageIncrease)
            {
                charClass.BaseAttributeValues.Remove(twoHandedWeaponDamageIncrease);
            }

            // Update common physical damage attribute combinations
            var baseMinDmgBonusToMinPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                minimumPhysBaseDmg,
                1,
                baseMinDamageBonus,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var baseMaxDmgBonusToMaxPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                maximumPhysBaseDmg,
                1,
                baseMaxDamageBonus,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var baseDmgBonusToPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                physicalBaseDmg,
                1,
                baseDamageBonus,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var physBaseDmgIncToMinPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                minimumPhysBaseDmg,
                1,
                physicalBaseDmgIncrease,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            var physBaseDmgIncToMaxPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                maximumPhysBaseDmg,
                1,
                physicalBaseDmgIncrease,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            var isDinoEquippedToCanFly = context.CreateNew<AttributeRelationship>(
                canFly,
                1,
                isDinorantEquipped,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            charClass.AttributeCombinations.Add(baseMinDmgBonusToMinPhysBaseDmg);
            charClass.AttributeCombinations.Add(baseMaxDmgBonusToMaxPhysBaseDmg);
            charClass.AttributeCombinations.Add(baseDmgBonusToPhysBaseDmg);
            charClass.AttributeCombinations.Add(physBaseDmgIncToMinPhysBaseDmg);
            charClass.AttributeCombinations.Add(physBaseDmgIncToMaxPhysBaseDmg);
            charClass.AttributeCombinations.Add(isDinoEquippedToCanFly);
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1, physicalBaseDmgIncrease));
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(-1, hasDoubleWield));

            // Add double wield attribute combos
            if (charClass.Number == 4 || charClass.Number == 6 || charClass.Number == 7 // DK classes
                || charClass.Number == 12 || charClass.Number == 13 // MG classes
                || charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                var tempDoubleWield = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Double Wield multiplier", string.Empty);
                gameConfiguration.Attributes.Add(tempDoubleWield);

                var doubleWieldWeaponCountToHasDoubleWield = context.CreateNew<AttributeRelationship>(
                    hasDoubleWield,
                    1,
                    doubleWieldWeaponCount,
                    InputOperator.Maximum,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var hasDoubleWieldToTempDoubleWield = context.CreateNew<AttributeRelationship>(
                    tempDoubleWield,
                    -0.45f,
                    hasDoubleWield,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var tempDoubleWieldToPhysBaseDmgInc = context.CreateNew<AttributeRelationship>(
                    physicalBaseDmgIncrease,
                    1,
                    tempDoubleWield,
                    InputOperator.Add,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var minPhysBaseDmgByRightWeaponToMinPhysBaseDmgByWeapon = context.CreateNew<AttributeRelationship>(
                    minimumPhysBaseDmgByWeapon,
                    hasDoubleWield,
                    minPhysBaseDmgByRightWeapon,
                    AggregateType.AddRaw);

                var maxPhysBaseDmgByRightWeaponToMaxPhysBaseDmgByWeapon = context.CreateNew<AttributeRelationship>(
                    maximumPhysBaseDmgByWeapon,
                    hasDoubleWield,
                    maxPhysBaseDmgByRightWeapon,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(doubleWieldWeaponCountToHasDoubleWield);
                charClass.AttributeCombinations.Add(hasDoubleWieldToTempDoubleWield);
                charClass.AttributeCombinations.Add(tempDoubleWieldToPhysBaseDmgInc);
                charClass.AttributeCombinations.Add(minPhysBaseDmgByRightWeaponToMinPhysBaseDmgByWeapon);
                charClass.AttributeCombinations.Add(maxPhysBaseDmgByRightWeaponToMaxPhysBaseDmgByWeapon);
            }

            // Add wizardry damage attribute combos
            if (charClass.Number == 0 || charClass.Number == 2 || charClass.Number == 3 // DW classes
                || charClass.Number == 12 || charClass.Number == 13 // MG classes
                || charClass.Number == 20 || charClass.Number == 22 || charClass.Number == 23) // Summoner classes
            {
                var baseMinDmgBonusToMinWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumWizBaseDmg,
                    1,
                    baseMinDamageBonus,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var baseMaxDmgBonusToMaxWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumWizBaseDmg,
                    1,
                    baseMaxDamageBonus,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var baseDmgBonusToWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    wizardryBaseDmg,
                    1,
                    baseDamageBonus,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var wizBaseDmgIncToMinWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumWizBaseDmg,
                    1,
                    wizardryBaseDmgIncrease,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var wizBaseDmgIncreaseToMaxWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumWizBaseDmg,
                    1,
                    wizardryBaseDmgIncrease,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                charClass.AttributeCombinations.Add(baseMinDmgBonusToMinWizBaseDmg);
                charClass.AttributeCombinations.Add(baseMaxDmgBonusToMaxWizBaseDmg);
                charClass.AttributeCombinations.Add(baseDmgBonusToWizBaseDmg);
                charClass.AttributeCombinations.Add(wizBaseDmgIncToMinWizBaseDmg);
                charClass.AttributeCombinations.Add(wizBaseDmgIncreaseToMaxWizBaseDmg);
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1, wizardryBaseDmgIncrease));
            }

            var attrCombos = charClass.AttributeCombinations.ToList();
            if (charClass.Number == 4 || charClass.Number == 6 || charClass.Number == 7) // DK classes
            {
                foreach (var attrCombo in attrCombos)
                {
                    if (attrCombo.OperandAttribute == Stats.IsOneHandedSwordEquipped
                        || attrCombo.OperandAttribute == Stats.IsTwoHandedSwordEquipped
                        || attrCombo.OperandAttribute == Stats.IsSpearEquipped
                        || attrCombo.OperandAttribute == Stats.IsMaceEquipped)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }

                var weaponMasteryAttackSpeedToAttackSpeedAny = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isOneHandedSwordEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAny);
            }

            if (charClass.Number == 16 || charClass.Number == 17) // Lord classes
            {
                foreach (var attrCombo in attrCombos)
                {
                    if ((attrCombo.TargetAttribute == Stats.RavenMinimumDamage || attrCombo.TargetAttribute == Stats.RavenMaximumDamage)
                        && attrCombo.InputOperand == 1
                        && attrCombo.InputAttribute == Stats.RavenBonusDamage) // RavenBonusDamage is the old RavenBaseDamage
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }

                    if (attrCombo.TargetAttribute == Stats.RavenAttackRate && attrCombo.InputAttribute == Stats.RavenLevel)
                    {
                        attrCombo.InputOperand = 16f / 15;
                    }

                    if (attrCombo.OperandAttribute == Stats.IsScepterEquipped)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }

                var scepterPetBonusDmgToRavenBonusDmg = context.CreateNew<AttributeRelationship>(
                    ravenBonusDamage,
                    isScepterEquipped,
                    scepterPetBonusDamage,
                    AggregateType.AddRaw);

                var bonusDmgWithScepterToMasterSkillPhysBonusDmg = context.CreateNew<AttributeRelationship>(
                    masterSkillPhysBonusDmg,
                    isScepterEquipped,
                    bonusDamageWithScepter,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(scepterPetBonusDmgToRavenBonusDmg);
                charClass.AttributeCombinations.Add(bonusDmgWithScepterToMasterSkillPhysBonusDmg);
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.3f, ravenCriticalDamageChance));
            }

            if (charClass.Number == 0 || charClass.Number == 2 || charClass.Number == 3) // DW classes
            {
                var weaponMasteryAttackSpeedToAttackSpeedAny = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isOneHandedStaffEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAny);

                if (charClass.AttributeCombinations.FirstOrDefault(ac => ac.TargetAttribute == obsoleteNovaBonusDamage) is { } totalStrengthToNovaBonusDamage)
                {
                    charClass.AttributeCombinations.Remove(totalStrengthToNovaBonusDamage);
                }

                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, Stats.NovaStageDamage.GetPersistent(gameConfiguration)));
            }

            if (charClass.Number == 8 || charClass.Number == 10 || charClass.Number == 11) // Elf classes
            {
                var ammunitionDmgIncrease = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Ammunition damage increase", string.Empty);
                gameConfiguration.Attributes.Add(ammunitionDmgIncrease);

                foreach (var attrCombo in attrCombos)
                {
                    if (attrCombo.TargetAttribute == Stats.MinimumPhysBaseDmg
                        && (attrCombo.InputAttribute == Stats.TotalAgility || attrCombo.InputAttribute == Stats.TotalStrength))
                    {
                        attrCombo.TargetAttribute = archeryMinDmg;
                    }

                    if (attrCombo.TargetAttribute == Stats.MaximumPhysBaseDmg
                        && (attrCombo.InputAttribute == Stats.TotalAgility || attrCombo.InputAttribute == Stats.TotalStrength))
                    {
                        attrCombo.TargetAttribute = archeryMaxDmg;
                    }

                    if (attrCombo.OperandAttribute == Stats.IsBowEquipped || attrCombo.OperandAttribute == Stats.IsCrossBowEquipped)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }

                var totalStrengthToTotalStrengthAndAgility = context.CreateNew<AttributeRelationship>(
                    totalStrengthAndAgility,
                    0f,
                    totalStrength,
                    InputOperator.Add,
                    totalAgility,
                    AggregateType.AddRaw);

                var totalStrengthAndAgilityToMeleeMinDmg = context.CreateNew<AttributeRelationship>(
                    meleeMinDmg,
                    1.0f / 7,
                    totalStrengthAndAgility,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var totalStrengthAndAgilityToMeleeMaxDmg = context.CreateNew<AttributeRelationship>(
                    meleeMaxDmg,
                    1.0f / 4,
                    totalStrengthAndAgility,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var ammunitionDmgIncToPhysicalBaseDmgIncrease = context.CreateNew<AttributeRelationship>(
                    physicalBaseDmgIncrease,
                    1,
                    ammunitionDmgIncrease,
                    InputOperator.Add,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var isBowEquippedToArcheryAttackMode = context.CreateNew<AttributeRelationship>(
                    archeryAttackMode,
                    1,
                    isBowEquipped,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var isCrossBowEquippedToArcheryAttackMode = context.CreateNew<AttributeRelationship>(
                    archeryAttackMode,
                    1,
                    isCrossBowEquipped,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var archeryAttackModeToMeleeAttackMode = context.CreateNew<AttributeRelationship>(
                    meleeAttackMode,
                    0,
                    archeryAttackMode,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var archeryMinDmgToMinPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumPhysBaseDmg,
                    archeryAttackMode,
                    archeryMinDmg,
                    AggregateType.AddRaw);

                var archeryMaxDmgToMaxPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumPhysBaseDmg,
                    archeryAttackMode,
                    archeryMaxDmg,
                    AggregateType.AddRaw);

                var ammunitionDmgBonusToAmmunitionDmgInc = context.CreateNew<AttributeRelationship>(
                    ammunitionDmgIncrease,
                    archeryAttackMode,
                    ammunitionDamageBonus,
                    AggregateType.AddRaw);

                var meleeMinDmgToMinPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumPhysBaseDmg,
                    meleeAttackMode,
                    meleeMinDmg,
                    AggregateType.AddRaw);

                var meleeMaxDmgToMaxPhysBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumPhysBaseDmg,
                    meleeAttackMode,
                    meleeMaxDmg,
                    AggregateType.AddRaw);

                var weaponMasteryAttackSpeedToAttackSpeedAny = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isBowEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(totalStrengthToTotalStrengthAndAgility);
                charClass.AttributeCombinations.Add(totalStrengthAndAgilityToMeleeMinDmg);
                charClass.AttributeCombinations.Add(totalStrengthAndAgilityToMeleeMaxDmg);
                charClass.AttributeCombinations.Add(ammunitionDmgIncToPhysicalBaseDmgIncrease);
                charClass.AttributeCombinations.Add(isBowEquippedToArcheryAttackMode);
                charClass.AttributeCombinations.Add(isCrossBowEquippedToArcheryAttackMode);
                charClass.AttributeCombinations.Add(archeryAttackModeToMeleeAttackMode);
                charClass.AttributeCombinations.Add(archeryMinDmgToMinPhysBaseDmg);
                charClass.AttributeCombinations.Add(archeryMaxDmgToMaxPhysBaseDmg);
                charClass.AttributeCombinations.Add(ammunitionDmgBonusToAmmunitionDmgInc);
                charClass.AttributeCombinations.Add(meleeMinDmgToMinPhysBaseDmg);
                charClass.AttributeCombinations.Add(meleeMaxDmgToMaxPhysBaseDmg);
                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAny);
            }

            if (charClass.Number == 12 || charClass.Number == 13) // MG classes
            {
                foreach (var attrCombo in attrCombos)
                {
                    if (attrCombo.OperandAttribute == Stats.IsOneHandedSwordEquipped || attrCombo.OperandAttribute == Stats.IsTwoHandedSwordEquipped)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }

                var isOneHandedStaffEquippedToIsOneHandedSwordEquipped = context.CreateNew<AttributeRelationship>(
                    isOneHandedSwordEquipped,
                    0,
                    isOneHandedStaffEquipped,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var weaponMasteryAttackSpeedToAttackSpeedAnyStaff = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isOneHandedStaffEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                var weaponMasteryAttackSpeedToAttackSpeedAnySword = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isOneHandedSwordEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(isOneHandedStaffEquippedToIsOneHandedSwordEquipped);
                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAnyStaff);
                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAnySword);
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, isOneHandedSwordEquipped));
            }

            if (charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                foreach (var attrCombo in attrCombos)
                {
                    if (attrCombo.TargetAttribute == Stats.MinimumWizBaseDmg || attrCombo.TargetAttribute == Stats.MaximumWizBaseDmg)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }

                    if (attrCombo.OperandAttribute == Stats.IsGloveWeaponEquipped)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }
            }

            if (charClass.Number == 20 || charClass.Number == 22 || charClass.Number == 23) // Summoner classes
            {
                var statsDefense = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Stats defense", string.Empty);
                gameConfiguration.Attributes.Add(statsDefense);
                var statsMinWizAndCurseBaseDmg = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Stats min wiz and curse base dmg", string.Empty);
                gameConfiguration.Attributes.Add(statsMinWizAndCurseBaseDmg);
                var statsMaxWizAndCurseBaseDmg = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Stats max wiz and curse base dmg", string.Empty);
                gameConfiguration.Attributes.Add(statsMaxWizAndCurseBaseDmg);
                var minBerserkerHealthDecrement = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Min Berserker health decrement", string.Empty);
                gameConfiguration.Attributes.Add(minBerserkerHealthDecrement);
                var finalBerserkerHealthDecrement = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Final Berserker health decrement", string.Empty);
                gameConfiguration.Attributes.Add(finalBerserkerHealthDecrement);

                foreach (var attrCombo in attrCombos)
                {
                    if (attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility)
                    {
                        attrCombo.InputAttribute = statsDefense;
                        attrCombo.InputOperand = 1;
                    }

                    if ((attrCombo.TargetAttribute == Stats.MinimumPhysBaseDmg || attrCombo.TargetAttribute == Stats.MaximumPhysBaseDmg)
                        && attrCombo.InputAttribute == Stats.TotalStrength)
                    {
                        attrCombo.InputAttribute = totalStrengthAndAgility;
                    }

                    if (attrCombo.TargetAttribute == Stats.MinimumWizBaseDmg && attrCombo.InputAttribute == Stats.TotalEnergy)
                    {
                        attrCombo.TargetAttribute = statsMinWizAndCurseBaseDmg;
                    }

                    if (attrCombo.TargetAttribute == Stats.MaximumWizBaseDmg && attrCombo.InputAttribute == Stats.TotalEnergy)
                    {
                        attrCombo.TargetAttribute = statsMaxWizAndCurseBaseDmg;
                    }

                    if ((attrCombo.TargetAttribute == Stats.MinimumCurseBaseDmg || attrCombo.TargetAttribute == Stats.MaximumCurseBaseDmg)
                        && attrCombo.InputAttribute == Stats.TotalEnergy)
                    {
                        charClass.AttributeCombinations.Remove(attrCombo);
                    }
                }

                var totalStrengthToTotalStrengthAndAgility = context.CreateNew<AttributeRelationship>(
                    totalStrengthAndAgility,
                    0f,
                    totalStrength,
                    InputOperator.Add,
                    totalAgility,
                    AggregateType.AddRaw);

                var totalAgilityToStatsDefense = context.CreateNew<AttributeRelationship>(
                    statsDefense,
                    1.0f / 3,
                    totalAgility,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var statsMinWizAndCurseDmgToMinWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumWizBaseDmg,
                    1,
                    statsMinWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var statsMaxWizAndCurseDmgToMaxWizBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumWizBaseDmg,
                    1,
                    statsMaxWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var statsMinWizAndCurseDmgToMinCurseBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumCurseBaseDmg,
                    1,
                    statsMinWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var statsMaxWizAndCurseDmgToMaxCurseBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumCurseBaseDmg,
                    1,
                    statsMaxWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var curseBaseDmgToMinCurseBaseDmg = context.CreateNew<AttributeRelationship>(
                    minimumCurseBaseDmg,
                    1,
                    curseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var curseBaseDmgToMaxCurseBaseDmg = context.CreateNew<AttributeRelationship>(
                    maximumCurseBaseDmg,
                    1,
                    curseBaseDmg,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var bookBonusBaseDmgToCurseBaseDmg = context.CreateNew<AttributeRelationship>(
                    curseBaseDmg,
                    isBookEquipped,
                    bookBonusBaseDamage,
                    AggregateType.AddRaw);

                var weaponMasteryAttackSpeedToAttackSpeedAny = context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    isBookEquipped,
                    weaponMasteryAttackSpeed,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(totalStrengthToTotalStrengthAndAgility);
                charClass.AttributeCombinations.Add(totalAgilityToStatsDefense);
                charClass.AttributeCombinations.Add(statsMinWizAndCurseDmgToMinWizBaseDmg);
                charClass.AttributeCombinations.Add(statsMaxWizAndCurseDmgToMaxWizBaseDmg);
                charClass.AttributeCombinations.Add(statsMinWizAndCurseDmgToMinCurseBaseDmg);
                charClass.AttributeCombinations.Add(statsMaxWizAndCurseDmgToMaxCurseBaseDmg);
                charClass.AttributeCombinations.Add(curseBaseDmgToMinCurseBaseDmg);
                charClass.AttributeCombinations.Add(curseBaseDmgToMaxCurseBaseDmg);
                charClass.AttributeCombinations.Add(bookBonusBaseDmgToCurseBaseDmg);
                charClass.AttributeCombinations.Add(weaponMasteryAttackSpeedToAttackSpeedAny);

                // Berserker combos
                var berserkerManaMultiplierToMaxMana = context.CreateNew<AttributeRelationship>(
                    maximumMana,
                    1,
                    berserkerManaMultiplier,
                    InputOperator.Add,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var berserkerHealthDecToFinalBerserkerHealthDec = context.CreateNew<AttributeRelationship>(
                    finalBerserkerHealthDecrement,
                    -0.1f,
                    berserkerHealthDecrement,
                    InputOperator.Minimum,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var berserkerMinPhysDmgBonusToIsBerserkerBuffed = context.CreateNew<AttributeRelationship>(
                    finalBerserkerHealthDecrement,
                    1,
                    berserkerMinPhysDmgBonus,
                    InputOperator.Minimum,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var finalBerserkerHealthDecToMaxHealth = context.CreateNew<AttributeRelationship>(
                    maximumHealth,
                    1,
                    finalBerserkerHealthDecrement,
                    InputOperator.Add,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var finalBerserkerHealthDecToDefensePvm = context.CreateNew<AttributeRelationship>(
                    defensePvm,
                    statsDefense,
                    finalBerserkerHealthDecrement,
                    AggregateType.AddFinal);

                var finalBerserkerHealthDecToDefensePvp = context.CreateNew<AttributeRelationship>(
                    defensePvp,
                    statsDefense,
                    finalBerserkerHealthDecrement,
                    AggregateType.AddFinal);

                var berserkerManaMultiplierToBerserkerMinPhysDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMinPhysDmgBonus,
                    1,
                    berserkerManaMultiplier,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var berserkerManaMultiplierToBerserkerMaxPhysDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMaxPhysDmgBonus,
                    1,
                    berserkerManaMultiplier,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var berserkerManaMultiplierToBerserkerWizMultiplier = context.CreateNew<AttributeRelationship>(
                    berserkerWizardryMultiplier,
                    1,
                    berserkerManaMultiplier,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var berserkerManaMultiplierToBerserkerCurseMultiplier = context.CreateNew<AttributeRelationship>(
                    berserkerCurseMultiplier,
                    1,
                    berserkerManaMultiplier,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var berserkerProficMultiplierToBerserkerWizMultiplier = context.CreateNew<AttributeRelationship>(
                    berserkerWizardryMultiplier,
                    1,
                    berserkerProficiencyMultiplier,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var statsMinWizAndCurseBaseDmgToBerserkerMinWizDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMinWizDmgBonus,
                    0,
                    statsMinWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    berserkerWizardryMultiplier,
                    AggregateType.AddRaw);

                var statsMaxWizAndCurseBaseDmgToBerserkerMaxWizDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMaxWizDmgBonus,
                    0,
                    statsMaxWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    berserkerWizardryMultiplier,
                    AggregateType.AddRaw);

                var statsMinWizAndCurseBaseDmgToBerserkerMinCurseDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMinCurseDmgBonus,
                    0,
                    statsMinWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    berserkerCurseMultiplier,
                    AggregateType.AddRaw);

                var statsMaxWizAndCurseBaseDmgToBerserkerMaxCurseDmgBonus = context.CreateNew<AttributeRelationship>(
                    berserkerMaxCurseDmgBonus,
                    0,
                    statsMaxWizAndCurseBaseDmg,
                    InputOperator.Multiply,
                    berserkerCurseMultiplier,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(berserkerManaMultiplierToMaxMana);
                charClass.AttributeCombinations.Add(berserkerMinPhysDmgBonusToIsBerserkerBuffed);
                charClass.AttributeCombinations.Add(berserkerHealthDecToFinalBerserkerHealthDec);
                charClass.AttributeCombinations.Add(finalBerserkerHealthDecToMaxHealth);
                charClass.AttributeCombinations.Add(finalBerserkerHealthDecToDefensePvm);
                charClass.AttributeCombinations.Add(finalBerserkerHealthDecToDefensePvp);
                charClass.AttributeCombinations.Add(berserkerManaMultiplierToBerserkerMinPhysDmgBonus);
                charClass.AttributeCombinations.Add(berserkerManaMultiplierToBerserkerMaxPhysDmgBonus);
                charClass.AttributeCombinations.Add(berserkerManaMultiplierToBerserkerWizMultiplier);
                charClass.AttributeCombinations.Add(berserkerManaMultiplierToBerserkerCurseMultiplier);
                charClass.AttributeCombinations.Add(berserkerProficMultiplierToBerserkerWizMultiplier);
                charClass.AttributeCombinations.Add(statsMinWizAndCurseBaseDmgToBerserkerMinWizDmgBonus);
                charClass.AttributeCombinations.Add(statsMaxWizAndCurseBaseDmgToBerserkerMaxWizDmgBonus);
                charClass.AttributeCombinations.Add(statsMinWizAndCurseBaseDmgToBerserkerMinCurseDmgBonus);
                charClass.AttributeCombinations.Add(statsMaxWizAndCurseBaseDmgToBerserkerMaxCurseDmgBonus);
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, berserkerManaMultiplier));
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, berserkerHealthDecrement));
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, berserkerProficiencyMultiplier));
            }
        });

        if (obsoleteNovaBonusDamage is not null)
        {
            gameConfiguration.Attributes.Remove(obsoleteNovaBonusDamage);
        }

        // Update Greater Damage magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.GreaterDamage) is { } greaterDamageEffect
            && greaterDamageEffect.PowerUpDefinitions.FirstOrDefault() is { } greaterDmgPowerUp)
        {
            greaterDmgPowerUp.TargetAttribute = Stats.GreaterDamageBonus.GetPersistent(gameConfiguration);
        }

        // Add Stats.DoubleWieldWeaponCount attribute
        var items = gameConfiguration.Items;
        if (items.Where(i => i.Group < (byte)ItemGroups.Spears && i.Width == 1) is { } doubleWieldWeapons)
        {
            foreach (var doubleWieldWeapon in doubleWieldWeapons)
            {
                var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = doubleWieldWeaponCount;
                powerUpDefinition.BaseValue = 1;
                powerUpDefinition.AggregateType = AggregateType.AddRaw;
                doubleWieldWeapon.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        // Remove Stats.IsTwoHandedWeaponEquipped from staffs (this stat is for physical type damage weapons only)
        if (items.Where(i => i.Group == (byte)ItemGroups.Staff && i.Width == 2) is { } staffs)
        {
            foreach (var staff in staffs)
            {
                if (staff.BasePowerUpAttributes.FirstOrDefault(p => p.TargetAttribute == Stats.IsTwoHandedWeaponEquipped) is { } twoHandedWeaponPowerUp)
                {
                    staff.BasePowerUpAttributes.Remove(twoHandedWeaponPowerUp);
                }
            }
        }
    }

#pragma warning disable SA1600, CS1591 // Elements should be documented.
    protected void UpdateExcellentOptions(GameConfiguration gameConfiguration)
    {
        var excWizAttackOptionsId = new Guid("00000083-0014-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == excWizAttackOptionsId) is { } excWizAttackOptions)
        {
            if (excWizAttackOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg) is { } twoPercentDmgIncOption)
            {
                twoPercentDmgIncOption.PowerUpDefinition!.TargetAttribute = Stats.WizardryBaseDmgIncrease.GetPersistent(gameConfiguration);
            }

            if (excWizAttackOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.WizardryBaseDmg) is { } levelDmgIncOption
                && levelDmgIncOption.PowerUpDefinition!.Boost?.RelatedValues.FirstOrDefault() is { } relatedValue)
            {
                relatedValue.InputAttribute = Stats.TotalLevel.GetPersistent(gameConfiguration);
            }
        }

        var excPhysAttackOptionsId = new Guid("00000083-0013-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == excPhysAttackOptionsId) is { } excPhysAttackOptions)
        {
            if (excPhysAttackOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.MaximumPhysBaseDmg) is { } twoPercentDmgIncOption)
            {
                twoPercentDmgIncOption.PowerUpDefinition!.TargetAttribute = Stats.PhysicalBaseDmgIncrease.GetPersistent(gameConfiguration);
            }

            if (excPhysAttackOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmg) is { } levelDmgIncOption
                && levelDmgIncOption.PowerUpDefinition!.Boost?.RelatedValues.FirstOrDefault() is { } relatedValue)
            {
                relatedValue.InputAttribute = Stats.TotalLevel.GetPersistent(gameConfiguration);
            }
        }

        var excDefenseOptionsId = new Guid("00000083-0012-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == excDefenseOptionsId) is { } excDefenseOptions)
        {
            if (excDefenseOptions.PossibleOptions.FirstOrDefault(p => p.PowerUpDefinition?.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDecOption)
            {
                dmgDecOption.PowerUpDefinition!.TargetAttribute = Stats.ArmorDamageDecrease.GetPersistent(gameConfiguration);
                dmgDecOption.PowerUpDefinition.Boost!.ConstantValue.Value = 0.04f;
                dmgDecOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            }
        }
    }

    protected void UpdateAmmoItems(IContext context, GameConfiguration gameConfiguration, float[] ammunitionDamageIncreaseByLevel, bool isSeasonSix = false)
    {
        if (gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Bows && i.IsAmmunition) is { } ammoItems)
        {
            var ammoDmgTable = context.CreateNew<ItemLevelBonusTable>();
            gameConfiguration.ItemLevelBonusTables.Add(ammoDmgTable);
            ammoDmgTable.Name = "Damage Increase % (Bolts/Arrows)";
            ammoDmgTable.Description = "The damage increase % by ammunition item level.";
            for (int level = 0; level < ammunitionDamageIncreaseByLevel.Length; level++)
            {
                var value = ammunitionDamageIncreaseByLevel[level];
                if (value != 0)
                {
                    var levelBonus = context.CreateNew<LevelBonus>();
                    levelBonus.Level = level;
                    levelBonus.AdditionalValue = value;
                    ammoDmgTable.BonusPerLevel.Add(levelBonus);
                }
            }

            var ammunitionDamageBonus = Stats.AmmunitionDamageBonus.GetPersistent(gameConfiguration);
            var skillExtraManaCost = Stats.SkillExtraManaCost.GetPersistent(gameConfiguration);
            ItemLevelBonusTable? ammoManaLossTable = null;
            foreach (var ammoItem in ammoItems)
            {
                var ammoDmgPowerUp = context.CreateNew<ItemBasePowerUpDefinition>();
                ammoDmgPowerUp.TargetAttribute = ammunitionDamageBonus;
                ammoDmgPowerUp.BaseValue = 0f;
                ammoDmgPowerUp.AggregateType = AggregateType.AddRaw;
                ammoDmgPowerUp.BonusPerLevelTable = ammoDmgTable;
                ammoItem.BasePowerUpAttributes.Add(ammoDmgPowerUp);

                if (isSeasonSix)
                {
                    ammoManaLossTable ??= CreateManaLossAfterHitTable(context, gameConfiguration, ammunitionDamageIncreaseByLevel);
                    var manaLossPowerDown = context.CreateNew<ItemBasePowerUpDefinition>();
                    manaLossPowerDown.TargetAttribute = skillExtraManaCost;
                    manaLossPowerDown.BaseValue = 0f;
                    manaLossPowerDown.AggregateType = AggregateType.AddRaw;
                    manaLossPowerDown.BonusPerLevelTable = ammoManaLossTable;
                    ammoItem.BasePowerUpAttributes.Add(manaLossPowerDown);
                }
            }

            ItemLevelBonusTable CreateManaLossAfterHitTable(IContext context, GameConfiguration gameConfiguration, float[] ammunitionDamageIncreaseByLevel)
            {
                var ammoManaLossTable = context.CreateNew<ItemLevelBonusTable>();
                gameConfiguration.ItemLevelBonusTables.Add(ammoManaLossTable);
                ammoManaLossTable.Name = "Mana Loss After Hit (Bolts/Arrows)";
                ammoManaLossTable.Description = "The mana loss per skill hit per ammunition item level due to infinity arrow efect.";
                float[] manaLossAfterHitByLevel = [5, 7, 10, 15];
                for (int level = 0; level < ammunitionDamageIncreaseByLevel.Length; level++)
                {
                    var value = manaLossAfterHitByLevel[level];
                    if (value != 0)
                    {
                        var levelBonus = context.CreateNew<LevelBonus>();
                        levelBonus.Level = level;
                        levelBonus.AdditionalValue = value;
                        ammoManaLossTable.BonusPerLevel.Add(levelBonus);
                    }
                }

                return ammoManaLossTable;
            }
        }
    }

    protected void AddDinorantBasePowerUp(IContext context, GameConfiguration gameConfiguration)
    {
        var dinorantId = new Guid("00000080-000d-0003-0000-000000000000");
        if (gameConfiguration.Items.FirstOrDefault(i => i.GetId() == dinorantId) is { } dinorant)
        {
            var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUpDefinition.TargetAttribute = Stats.IsDinorantEquipped.GetPersistent(gameConfiguration);
            powerUpDefinition.BaseValue = 1;
            powerUpDefinition.AggregateType = AggregateType.AddRaw;
            dinorant.BasePowerUpAttributes.Add(powerUpDefinition);
        }
    }

    protected void UpdateMGStaffsItemSlot(GameConfiguration gameConfiguration)
    {
        var leftOrRightHandSlot = gameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(0) && t.ItemSlots.Contains(1));
        if (gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Staff && i.Width == 1 && i.QualifiedCharacters.Any(cc => cc.Number == 12)) is { } staffs)
        {
            foreach (var staff in staffs)
            {
                staff.ItemSlot = leftOrRightHandSlot;
            }
        }
    }

    protected void UpdateWeaponItems(IContext context, GameConfiguration gameConfiguration)
    {
        var equippedWeaponCount = Stats.EquippedWeaponCount.GetPersistent(gameConfiguration);
        foreach (var weapon in gameConfiguration.Items.Where(i => i.Group >= (byte)ItemGroups.Swords && i.Group <= (byte)ItemGroups.Staff && !i.IsAmmunition))
        {
            if (weapon.Group != (byte)ItemGroups.Bows && weapon.BasePowerUpAttributes.FirstOrDefault(bpu => bpu.TargetAttribute == Stats.AmmunitionConsumptionRate) is { } badPowerUp)
            {
                weapon.BasePowerUpAttributes.Remove(badPowerUp);
            }

            var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUpDefinition.TargetAttribute = equippedWeaponCount;
            powerUpDefinition.BaseValue = 1f;
            powerUpDefinition.AggregateType = AggregateType.AddRaw;
            weapon.BasePowerUpAttributes.Add(powerUpDefinition);
        }
    }
#pragma warning restore SA1600, CS1591 // Elements should be documented.
}