// <copyright file="AdjustIceArrowFreezeDurationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adjusts the default Ice Arrow freeze duration for Season Six configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Name), Description = nameof(PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FEC39E1D-85AC-4768-B7E0-D5B0C8A83C26")]
public class AdjustIceArrowFreezeDurationUpdatePlugIn : UpdatePlugInBase
{
    private const float OriginalDurationInSeconds = 5;
    private const float UpdatedDurationInSeconds = 7;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AdjustIceArrowFreezeDurationUpdatePlugIn_Description;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var duration = gameConfiguration.Skills
            .FirstOrDefault(skill => skill.Number == (short)SkillNumber.IceArrow)?
            .MagicEffectDef?
            .Duration;

        if (duration?.ConstantValue.Value == OriginalDurationInSeconds)
        {
            duration.ConstantValue.Value = UpdatedDurationInSeconds;
        }

        return ValueTask.CompletedTask;
    }
}
