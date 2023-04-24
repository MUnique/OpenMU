// <copyright file="FixWarpLevelUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
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
    public override int Version => 10;

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
        var characterClasses = gameConfiguration.CharacterClasses
            .Where(cc => cc.LevelWarpRequirementReductionPercent == 33)
            .ToList();
        foreach (var cc in characterClasses)
        {
            cc.LevelWarpRequirementReductionPercent = 34;
        }
    }
}