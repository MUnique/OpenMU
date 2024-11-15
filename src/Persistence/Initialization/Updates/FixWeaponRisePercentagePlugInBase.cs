// <copyright file="FixWeaponRisePercentagePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// This update fixes weapons (staff) rise percentage.
/// </summary>
public abstract class FixWeaponRisePercentagePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Weapon Rise Percentage";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes weapons (staff) rise percentage";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 11, 11, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var staffRiseBonusTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Staff Rise");

        // Modify existing table name and description
        staffRiseBonusTable.Name = "Staff Rise (even)";
        staffRiseBonusTable.Description = "The staff rise bonus per item level for even magic power staves.";

        // Add new staff odd increase table
        float[] staffRiseIncreaseByLevelOdd = { 0, 4, 7, 11, 14, 18, 21, 25, 28, 32, 36, 40, 45, 51, 57, 63 };

        var staffOddTable = context.CreateNew<ItemLevelBonusTable>();
        gameConfiguration.ItemLevelBonusTables.Add(staffOddTable);
        staffOddTable.Name = "Staff Rise (odd)";
        staffOddTable.Description = "The staff rise bonus per item level for odd magic power staves.";
        for (int level = 0; level < staffRiseIncreaseByLevelOdd.Length; level++)
        {
            var value = staffRiseIncreaseByLevelOdd[level];
            if (value != 0)
            {
                var levelBonus = context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = staffRiseIncreaseByLevelOdd[level];
                staffOddTable.BonusPerLevel.Add(levelBonus);
            }
        }

        // Fix Group 5 weapons (staves)
        var staves = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Staff && i.BasePowerUpAttributes.Any(pua => pua.TargetAttribute == Stats.StaffRise));
        foreach (var staff in staves)
        {
            if (staff.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.StaffRise) is { } staffRiseAttr)
            {
                if ((int)staffRiseAttr.BaseValue % 2 != 0)
                {
                    staffRiseAttr.BonusPerLevelTable = staffOddTable;
                }

                staffRiseAttr.BaseValue /= 2.0f;
            }
        }
    }
}