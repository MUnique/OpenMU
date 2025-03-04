// <copyright file="FixItemOptionsAndAttackSpeedPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes some item options (damage, defense, defense rate), weapon attributes, and some AA weapon values.
/// It also refactors attack speed attributes for simplification.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("EEEAA884-4704-48DE-825A-8E588A47E2CC")]
public class FixItemOptionsAndAttackSpeedPlugInSeason6 : FixItemOptionsAndAttackSpeedPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixItemOptionsAndAttackSpeedSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.ChangeDinorantAttackSpeedOption(gameConfiguration);
        this.UpdateExcellentAttackSpeedAndBaseDmgOptions(gameConfiguration);

        // Change attack speed magic effects
        if (gameConfiguration.MagicEffects.FirstOrDefault(me => me.Number == (short)MagicEffectNumber.PotionOfSoul) is { } soulPotionMagicEffect
            && soulPotionMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeed) is { } soulPotionAttackSpeed
            && soulPotionMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.MagicSpeed) is { } soulPotionMagicSpeed)
        {
            soulPotionAttackSpeed.TargetAttribute = Stats.AttackSpeedAny;
            soulPotionMagicEffect.PowerUpDefinitions.Remove(soulPotionMagicSpeed);
        }

        if (gameConfiguration.MagicEffects.FirstOrDefault(me => me.Number == (short)MagicEffectNumber.JackOlanternBlessing) is { } jackMagicEffect
            && jackMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeed) is { } jackAttackSpeed
            && jackMagicEffect.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.MagicSpeed) is { } jackMagicSpeed)
        {
            jackAttackSpeed.TargetAttribute = Stats.AttackSpeedAny;
            jackMagicEffect.PowerUpDefinitions.Remove(jackMagicSpeed);
        }

        // Change wizard's ring, and fire socket attack speed option
        var wizardsRingOption = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0073-0000-0000-000000000000"));
        if (wizardsRingOption is not null
            && wizardsRingOption.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } wizardsRingAttackSpeed)
        {
            wizardsRingAttackSpeed.PowerUpDefinition!.TargetAttribute = Stats.AttackSpeedAny;
        }

        var fireSocketOption = gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == new Guid("00000083-0032-0000-0000-000000000000"));
        if (fireSocketOption is not null
            && fireSocketOption.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == Stats.AttackSpeed) is { } fireSocketAttackSpeed)
        {
            fireSocketAttackSpeed.PowerUpDefinition!.TargetAttribute = Stats.AttackSpeedAny;
        }

        // Add missing magic swords wiz item option
        var magicSwords = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Swords
            && (i.Number == 21 // Dark Reign Blade
                || i.Number == 23 // Explosion Blade
                || i.Number == 25 // Sword Dancer
                || i.Number == 28 // Imperial Sword
                || i.Number == 31)); // Rune Blade

        var wizItemOption = gameConfiguration.ItemOptions.FirstOrDefault(io =>
            io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Option && po.PowerUpDefinition?.TargetAttribute == Stats.WizardryBaseDmg));
        if (wizItemOption is not null)
        {
            foreach (var magicSword in magicSwords)
            {
                magicSword.PossibleItemOptions.Add(wizItemOption);
            }
        }

        // Remove staffs two handed weapon powerup & add missing powerup for soul master/mg staffs
        var staffs = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Staff && i.Width == 2);
        foreach (var staff in staffs)
        {
            if (staff.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.IsTwoHandedWeaponEquipped) is { } twoHandedWeapon)
            {
                staff.BasePowerUpAttributes.Remove(twoHandedWeapon);
            }

            if (staff.Number == 9 // Dragon Soul Staff
                || staff.Number == 11 // Kundun Staff
                || staff.Number == 12 // Grand Viper Staff
                || staff.Number == 13 // Platina Staff
                || staff.Number == 30 // Deadly Staff
                || staff.Number == 31 // Imperial Staff
                || staff.Number == 33) // Chromatic Staff
            {
                staff.BasePowerUpAttributes.Add(CreateNewBasePowerUpDefinition(Stats.IsOneHandedStaffEquipped));
            }
        }

        ItemBasePowerUpDefinition CreateNewBasePowerUpDefinition(AttributeDefinition attribute)
        {
            var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUpDefinition.TargetAttribute = attribute.GetPersistent(gameConfiguration);
            powerUpDefinition.BaseValue = 1;
            powerUpDefinition.AggregateType = AggregateType.AddRaw;
            return powerUpDefinition;
        }

        // Add crystal sword two handed sword powerup
        var crystalSword = gameConfiguration.Items.FirstOrDefault(i => i.Group == (int)ItemGroups.Swords && i.Number == 5);
        if (crystalSword is not null)
        {
            crystalSword.BasePowerUpAttributes.Add(CreateNewBasePowerUpDefinition(Stats.IsTwoHandedSwordEquipped));
        }

        // Fix AA weapons values
        var archangelSword = gameConfiguration.Items.FirstOrDefault(i => i.Group == (int)ItemGroups.Swords && i.Number == 19);
        var archangelScepter = gameConfiguration.Items.FirstOrDefault(i => i.Group == (int)ItemGroups.Scepters && i.Number == 13);
        var archangelCrossbow = gameConfiguration.Items.FirstOrDefault(i => i.Group == (int)ItemGroups.Bows && i.Number == 18);

        if (archangelSword is not null)
        {
            if (archangelSword.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MinimumPhysBaseDmg) is { } minPhysDmg)
            {
                minPhysDmg.BaseValue = 220;
            }

            if (archangelSword.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MaximumPhysBaseDmg) is { } maxPhysDmg)
            {
                maxPhysDmg.BaseValue = 230;
            }

            if (archangelSword.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeed)
            {
                attackSpeed.BaseValue = 45;
            }
        }

        if (archangelScepter is not null)
        {
            if (archangelScepter.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MinimumPhysBaseDmg) is { } minPhysDmg)
            {
                minPhysDmg.BaseValue = 200;
            }

            if (archangelScepter.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MaximumPhysBaseDmg) is { } maxPhysDmg)
            {
                maxPhysDmg.BaseValue = 233;
            }

            if (archangelScepter.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.ScepterRise) is { } rise)
            {
                rise.BaseValue = 138 / 2.0f;
            }
        }

        if (archangelCrossbow is not null)
        {
            if (archangelCrossbow.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MinimumPhysBaseDmg) is { } minPhysDmg)
            {
                minPhysDmg.BaseValue = 224;
            }

            if (archangelCrossbow.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.MaximumPhysBaseDmg) is { } maxPhysDmg)
            {
                maxPhysDmg.BaseValue = 246;
            }

            if (archangelCrossbow.BasePowerUpAttributes.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeed)
            {
                attackSpeed.BaseValue = 45;
            }
        }
    }
}