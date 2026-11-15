// <copyright file="AdjustIceArrowFreezeDurationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extends the default Ice Arrow freeze duration, and adds its reduction on successful hits.
/// Also aligns Ice Arrow Str, Chain Drive Str, Strike of Destruction Str and Poison Str with their base skills magic effects.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Name), Description = nameof(PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FEC39E1D-85AC-4768-B7E0-D5B0C8A83C26")]
public class AdjustIceArrowFreezeDurationUpdatePlugIn : UpdatePlugInBase
{
    private const float OriginalDurationInSeconds = 5;
    private const float UpdatedDurationInSeconds = 7;
    private const float OriginalIcedOrPoisonedDurationInSeconds = 10;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Description;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 9, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var iceArrow = gameConfiguration.Skills.FirstOrDefault(skill => skill.Number == (short)SkillNumber.IceArrow);
        if (iceArrow?.MagicEffectDef?.Duration is { } duration && duration.ConstantValue.Value == OriginalDurationInSeconds)
        {
            duration.ConstantValue.Value = UpdatedDurationInSeconds;
        }

        // Fix misaligned magic effects
        UpdateMasterSkillEffect(gameConfiguration, SkillNumber.IceArrow, SkillNumber.IceArrowStrengthener);
        UpdateMasterSkillEffect(gameConfiguration, SkillNumber.ChainDrive, SkillNumber.ChainDriveStrengthener);
        UpdateMasterSkillEffect(gameConfiguration, SkillNumber.StrikeofDestruction, SkillNumber.StrikeofDestrStr);
        UpdateMasterSkillEffect(gameConfiguration, SkillNumber.Poison, SkillNumber.PoisonStrengthener);

        return ValueTask.CompletedTask;
    }

    private static void UpdateMasterSkillEffect(GameConfiguration gameConfiguration, SkillNumber baseSkillNumber, SkillNumber masterSkillNumber)
    {
        var baseSkill = gameConfiguration.Skills.FirstOrDefault(skill => skill.Number == (short)baseSkillNumber);
        var masterSkill = gameConfiguration.Skills.FirstOrDefault(skill => skill.Number == (short)masterSkillNumber);
        if (baseSkill?.MagicEffectDef is { } baseMagicEffect
            && masterSkill is not null
            && IsDefaultIcedOrPoisonedEffect(masterSkill.MagicEffectDef, gameConfiguration))
        {
            masterSkill.MagicEffectDef = baseMagicEffect;
        }
    }

    private static bool IsDefaultIcedOrPoisonedEffect(MagicEffectDefinition? effect, GameConfiguration gameConfiguration)
    {
        return effect is not null
            && effect.Chance is null
            && ((effect.Number == (short)MagicEffectNumber.Iced
                    && effect.SubType == (byte)(0xFF - ElementalType.Ice)
                    && effect.Duration?.ConstantValue.Value == OriginalIcedOrPoisonedDurationInSeconds
                    && effect.PowerUpDefinitions.Any(pu => pu.TargetAttribute == Stats.IsIced.GetPersistent(gameConfiguration)))
                || (effect.Number == (short)MagicEffectNumber.Poisoned
                    && effect.SubType == (byte)(0xFF - ElementalType.Poison)
                    && effect.Duration?.ConstantValue.Value == OriginalIcedOrPoisonedDurationInSeconds
                    && effect.PowerUpDefinitions.Any(pu => pu.TargetAttribute == Stats.IsPoisoned.GetPersistent(gameConfiguration))));
    }
}
