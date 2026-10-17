// <copyright file="AddDefenseReductionChancePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Removes the energy requirement from Power Slash Strengthener and sets Fire Slash's defense reduction chance to 45%.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("404412A4-6217-44AC-941B-F7F25242DCA4")]
public class AddDefenseReductionChancePlugIn : UpdatePlugInBase
{
    private const string PlugInName = "Add Defense Reduction Chance";

    private const string PlugInDescription = "Removes the energy requirement from Power Slash Strengthener and sets Fire Slash's defense reduction chance to 45%.";

    private const float DefenseReductionChance = 0.45f;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 13, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Skills.FirstOrDefault(skill => skill.Number == (short)SkillNumber.PowerSlashStreng) is { } powerSlashStrengthener)
        {
            foreach (var requirement in powerSlashStrengthener.Requirements
                         .Where(requirement => requirement.Attribute?.Id == Stats.TotalEnergy.Id)
                         .ToList())
            {
                powerSlashStrengthener.Requirements.Remove(requirement);
            }
        }

        if (gameConfiguration.MagicEffects.FirstOrDefault(effect => effect.Number == (short)MagicEffectNumber.DefenseReduction) is { } defenseReduction)
        {
            defenseReduction.Chance ??= context.CreateNew<PowerUpDefinitionValue>();
            defenseReduction.Chance.ConstantValue.Value = DefenseReductionChance;
        }

        return ValueTask.CompletedTask;
    }
}
