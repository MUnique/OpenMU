// <copyright file="ChainLightningUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the chain lightning skill.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("039D09CB-283C-4CBD-ABBC-FFD3F7D5C62F")]
public class ChainLightningUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Chain Lightning skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update sets the right settings for the chain lightning skill.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ChainLightningUpdate;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 07, 15, 18, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var chainLightning = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.ChainLightning);
        chainLightning.SkillType = SkillType.AreaSkillExplicitTarget;
        chainLightning.Target = SkillTarget.Explicit;

        var chainLightningStr = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.ChainLightningStr);
        chainLightningStr.SkillType = SkillType.AreaSkillExplicitTarget;
        chainLightningStr.Target = SkillTarget.Explicit;
    }
}