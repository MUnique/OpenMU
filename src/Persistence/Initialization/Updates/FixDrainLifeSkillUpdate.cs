// <copyright file="FixDrainLifeSkillUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This adds the items required to enter the kalima map.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("A8827A3C-7F52-47CF-9EA5-562A9C06B986")]
public class FixDrainLifeSkillUpdate : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Drain Life Skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Updates the attributes of the summoner's Drain Life skill to make it work properly.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDrainLifeSkill;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 29, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var drainLife = gameConfiguration.Skills.First(x => x.Number == (short)SkillNumber.DrainLife);
        drainLife.SkillType = SkillType.AreaSkillExplicitTarget;
    }
}