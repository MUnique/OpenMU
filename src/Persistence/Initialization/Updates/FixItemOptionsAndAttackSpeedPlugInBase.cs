// <copyright file="FixItemOptionsAndAttackSpeedPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// This update fixes some item options (damage, defense rate) and weapons attack speed.
/// It also refactors attack speed attributes for simplification.
/// </summary>
public abstract class FixItemOptionsAndAttackSpeedPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Item Options And Attack Speed";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes some item options (damage, defense rate) and weapons attack speed.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 03, 05, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var attackSpeedByWeapon = Stats.AttackSpeedByWeapon.GetPersistent(gameConfiguration);
        var physicalBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);
        var wizardryBaseDmg = Stats.WizardryBaseDmg.GetPersistent(gameConfiguration);

        // Add AttackSpeedAny stat
        var attackSpeedAny = context.CreateNew<AttributeDefinition>(Stats.AttackSpeedAny.Id, Stats.AttackSpeedAny.Designation, Stats.AttackSpeedAny.Description);
        gameConfiguration.Attributes.Add(attackSpeedAny);

        // Change attack speed-related combination class attributes
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            if (charClass.AttributeCombinations.FirstOrDefault(rel => rel.TargetAttribute == Stats.AttackSpeed && rel.InputAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeedByWeaponToAttackSpeed
                && charClass.AttributeCombinations.FirstOrDefault(rel => rel.TargetAttribute == Stats.MagicSpeed && rel.InputAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeedByWeaponToMagicSpeed
                && charClass.AttributeCombinations.FirstOrDefault(rel => rel.TargetAttribute == Stats.AttackSpeed && rel.OperandAttribute == Stats.AreTwoWeaponsEquipped) is { } areTwoWeaponsEquippedToAttackSpeedConditional
                && charClass.AttributeCombinations.FirstOrDefault(rel => rel.TargetAttribute == Stats.MagicSpeed && rel.OperandAttribute == Stats.AreTwoWeaponsEquipped) is { } areTwoWeaponsEquippedToMagicSpeedConditional)
            {
                attackSpeedByWeaponToAttackSpeed.InputAttribute = attackSpeedAny;
                attackSpeedByWeaponToMagicSpeed.InputAttribute = attackSpeedAny;

                charClass.AttributeCombinations.Add(context.CreateNew<AttributeRelationship>(
                    attackSpeedAny,
                    1,
                    attackSpeedByWeapon,
                    InputOperator.Multiply,
                    default(AttributeDefinition?)));

                areTwoWeaponsEquippedToAttackSpeedConditional.TargetAttribute = attackSpeedAny;
                charClass.AttributeCombinations.Remove(areTwoWeaponsEquippedToMagicSpeedConditional);
            }
        });

        // Change gloves attack speed stats
        var gloves = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Gloves);
        foreach (var glovesItem in gloves)
        {
            if (glovesItem.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.AttackSpeed) is { } glovesAttackSpeed
                && glovesItem.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.MagicSpeed) is { } glovesMagicSpeed)
            {
                glovesAttackSpeed.TargetAttribute = attackSpeedAny;
                glovesItem.BasePowerUpAttributes.Remove(glovesMagicSpeed);
            }
        }

        // Change alcohol attack speed magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(me => me.Number == (short)MagicEffectNumber.Alcohol) is { } alcoholMagicEffect
            && alcoholMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeed) is { } alcoholAttackSpeed
            && alcoholMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.MagicSpeed) is { } alcoholMagicSpeed)
        {
            alcoholAttackSpeed.TargetAttribute = attackSpeedAny;
            alcoholMagicEffect.PowerUpDefinitions.Remove(alcoholMagicSpeed);
        }

        // Fix Item Options (all weapon and wing damage options)
        var itemOptions = gameConfiguration.ItemOptions.Where(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Option));
        foreach (var itemOption in itemOptions)
        {
            foreach (var opt in itemOption.PossibleOptions)
            {
                if (opt.PowerUpDefinition?.TargetAttribute == Stats.MaximumPhysBaseDmg)
                {
                    opt.PowerUpDefinition.TargetAttribute = physicalBaseDmg;
                }
                else if (opt.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg)
                {
                    opt.PowerUpDefinition.TargetAttribute = wizardryBaseDmg;
                }
                else
                {
                    continue;
                }
            }
        }

        gameConfiguration.PhysicalDamageOption().Name = Stats.PhysicalBaseDmg.Designation + " Option";
        gameConfiguration.WizardryDamageOption().Name = Stats.WizardryBaseDmg.Designation + " Option";

        // Add shield defense rate item option
        gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(context, gameConfiguration, Stats.DefenseRatePvm, ItemOptionDefinitionNumbers.DefenseRateOption, 5));

        // Fix all shields item option
        var shields = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Shields);
        var defenseOption = gameConfiguration.GetDefenseOption();
        var defenseRateOption = gameConfiguration.GetDefenseRateOption();
        foreach (var shield in shields)
        {
            if (shield.PossibleItemOptions.FirstOrDefault(io => io == defenseOption) is { } defOpt)
            {
                shield.PossibleItemOptions.Remove(defOpt);
            }

            shield.PossibleItemOptions.Add(defenseRateOption);
        }

        // Fix 3rd wings defense bonus per level table
        var thirdWingsDefenseTable = gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name == "Defense Bonus (3rd Wings)");
        if (thirdWingsDefenseTable is not null)
        {
            var thirdWings = gameConfiguration.Items.Where(i =>
                i.Group == (int)ItemGroups.Orbs
                && i.BasePowerUpAttributes.Any(pua => pua.TargetAttribute == Stats.CanFly)
                && i.Requirements.Any(r => r.Attribute == Stats.Level && r.MinimumValue == 400));

            foreach (var wing in thirdWings)
            {
                if (wing.BasePowerUpAttributes.First(pua => pua.TargetAttribute == Stats.DefenseBase) is { } defensePowerUp)
                {
                    defensePowerUp.BonusPerLevelTable = thirdWingsDefenseTable;
                }
            }
        }
    }

#pragma warning disable SA1600, CS1591 // Elements should be documented.
    protected ItemOptionDefinition CreateOptionDefinition(IContext context, GameConfiguration gameConfiguration, AttributeDefinition attributeDefinition, short number, byte baseValue)
    {
        var definition = context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(number);
        definition.Name = attributeDefinition.Designation + " Option";
        definition.AddChance = 0.25f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        var itemOption = context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(number);
        itemOption.OptionType = gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
        itemOption.PowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = gameConfiguration.Attributes.First(a => a == attributeDefinition);
        itemOption.PowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue!.Value = baseValue;
        for (short level = 2; level <= 4; level++)
        {
            var levelDependentOption = context.CreateNew<ItemOptionOfLevel>();
            levelDependentOption.Level = level;
            var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = itemOption.PowerUpDefinition.TargetAttribute;
            powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue!.Value = level * baseValue;
            levelDependentOption.PowerUpDefinition = powerUpDefinition;
            itemOption.LevelDependentOptions.Add(levelDependentOption);
        }

        definition.PossibleOptions.Add(itemOption);

        return definition;
    }

    protected void FixWeaponsAttackSpeedStat(GameConfiguration gameConfiguration)
    {
        var attackSpeedByWeapon = Stats.AttackSpeedByWeapon.GetPersistent(gameConfiguration);
        var weapons = gameConfiguration.Items.Where(i => i.Group >= (int)ItemGroups.Swords && i.Group <= (int)ItemGroups.Staff);
        foreach (var weapon in weapons)
        {
            if (weapon.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.AttackSpeed) is { } weaponAttackSpeed)
            {
                weaponAttackSpeed.TargetAttribute = attackSpeedByWeapon;
            }
        }
    }

    protected void ChangeDinorantAttackSpeedOption(GameConfiguration gameConfiguration)
    {
        var dinorantOption = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0080-0000-0000-000000000000"));
        dinorantOption ??= gameConfiguration.ItemOptions.FirstOrDefault(io => io.Name == "Dinorant Options"); // 0.95d

        if (dinorantOption is not null
            && dinorantOption.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } dinoAttackSpeed)
        {
            dinoAttackSpeed.PowerUpDefinition!.TargetAttribute = Stats.AttackSpeedAny.GetPersistent(gameConfiguration);
        }
    }

    protected void UpdateExcellentAttackSpeedAndBaseDmgOptions(GameConfiguration gameConfiguration)
    {
        var excPhysAttackOpts = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0013-0000-0000-000000000000"));
        var excWizAttackOpts = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0014-0000-0000-000000000000"));
        var excCurseAttackOpts = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0015-0000-0000-000000000000"));
        var attackSpeedAny = Stats.AttackSpeedAny.GetPersistent(gameConfiguration);
        var physicalBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);
        var wizardryBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);

        if (excPhysAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } physOptAttackSpeed)
        {
            physOptAttackSpeed.PowerUpDefinition!.TargetAttribute = attackSpeedAny;
        }

        if (excPhysAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.MaximumPhysBaseDmg && opt.Number == 5) is { } maxPhysBaseDmg)
        {
            maxPhysBaseDmg.PowerUpDefinition!.TargetAttribute = physicalBaseDmg;
        }

        if (excWizAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } wizOptAttackSpeed)
        {
            wizOptAttackSpeed.PowerUpDefinition!.TargetAttribute = attackSpeedAny;
        }

        if (excWizAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg && opt.Number == 5) is { } maxWizBaseDmg)
        {
            maxWizBaseDmg.PowerUpDefinition!.TargetAttribute = wizardryBaseDmg;
        }

        if (excCurseAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } curseOptAttackSpeed)
        {
            curseOptAttackSpeed.PowerUpDefinition!.TargetAttribute = attackSpeedAny;
        }

        if (excCurseAttackOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.MaximumCurseBaseDmg && opt.Number == 5) is { } maxCurseBaseDmg)
        {
            maxCurseBaseDmg.PowerUpDefinition!.TargetAttribute = wizardryBaseDmg; // Yes, wizardry. This should not be in use, but just in case.
        }
    }
#pragma warning restore SA1600, CS1591 // Elements should be documented.
}