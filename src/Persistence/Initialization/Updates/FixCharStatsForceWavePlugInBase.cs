// <copyright file="FixCharStatsForceWavePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update fixes DW agility to defense multiplier stat.
/// </summary>
public abstract class FixCharStatsForceWavePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix DW Defense Multiplier";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes DW agility to defense multiplier stat.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 03, 24, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var wizardClasses = gameConfiguration.CharacterClasses.Where(charClass => charClass.Number == 0 || charClass.Number == 2 || charClass.Number == 3);
        foreach (var wizardClass in wizardClasses)
        {
            if (wizardClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseBase)
            {
                totalAgilityToDefenseBase.InputOperand = 0.25f;
            }
        }
    }

#pragma warning disable SA1600, CS1591 // Elements should be documented.
    protected void UpdateMagicGladiatorClassesStats(GameConfiguration gameConfiguration)
    {
        var magicGladiatorClasses = gameConfiguration.CharacterClasses.Where(charClass => charClass.Number == 12 || charClass.Number == 13);
        foreach (var magicGladiatorClass in magicGladiatorClasses)
        {
            magicGladiatorClass.StatAttributes.First(attr => attr.Attribute == Stats.BaseEnergy).BaseValue = 26;
        }
    }
#pragma warning restore SA1600, CS1591 // Elements should be documented.
}