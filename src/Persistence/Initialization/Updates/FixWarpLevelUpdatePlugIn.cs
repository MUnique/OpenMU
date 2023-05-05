// <copyright file="FixWarpLevelUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// updating LevelWarpRequirementReductionPercent plugin.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("F4342D86-7042-477A-BC3B-475C1F2A79FF")]
public class FixWarpLevelUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Warp Level Reduction";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This plugin updates the LevelWarpRequirementReductionPercent for MG, DL, and RF.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWarpLevelUpdate;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 04, 24, 02, 05, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // warp requirement reduction
        var characterClasses = gameConfiguration.CharacterClasses
            .Where(cc => cc.LevelWarpRequirementReductionPercent == 33)
            .ToList();
        foreach (var cc in characterClasses)
        {
            cc.LevelWarpRequirementReductionPercent = 34;
        }

        // warp list
        (string Name, string? NewName, int Costs, int Level)[] warps =
        {
            ("KanturuRuins", "KanturuRuins1", -1, 160),
            ("KanturuRelics", null, 12000, -1),
            ("Elbeland", "Elveland", -1, -1),
            ("Elbeland2", "Elveland2", -1, -1),
            ("Elbeland3", "Elveland3", -1, -1),
            ("Vulcan", "Vulcanus", -1, -1),
            ("KanturuRuins3", null, 15000, -1),
            ("Karutan2", null, -1, 170),
        };
        foreach (var (name, newName, costs, level) in warps)
        {
            var warpInfo = gameConfiguration.WarpList.FirstOrDefault(w => w.Name == name);
            if (warpInfo is not null)
            {
                if (newName is not null)
                {
                    warpInfo.Name = newName;
                }

                if (costs > 0)
                {
                    warpInfo.Costs = costs;
                }

                if (level > 0)
                {
                    warpInfo.LevelRequirement = level;
                }
            }
        }

        // add LaCleon
        var laCleon = gameConfiguration.WarpList.FirstOrDefault(w => w.Name == "LaCleon");
        if (laCleon is null)
        {
            laCleon = context.CreateNew<WarpInfo>();
            laCleon.Index = 48;
            laCleon.Name = "LaCleon";
            laCleon.Costs = 15000;
            laCleon.LevelRequirement = 280;
            laCleon.Gate = gameConfiguration.Maps
                .FirstOrDefault(m => m.Name == "LaCleon")
                ?.ExitGates.FirstOrDefault(g => g.X1 == 222);
            gameConfiguration.WarpList.Add(laCleon);
        }
    }
}