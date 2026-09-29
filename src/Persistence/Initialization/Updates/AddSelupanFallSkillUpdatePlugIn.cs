// <copyright file="AddSelupanFallSkillUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the skill of the fall of Selupan, with which it hits the players around it when it appears.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("8F3C6A2D-1E74-4B95-9D08-5A7E2C4B1F93")]
public class AddSelupanFallSkillUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Selupan Fall skill";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the skill of the fall of Selupan, with which it hits the players around it when it appears.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddSelupanFallSkill;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 27, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        AddRaklionEventUpdatePlugIn.CreateSkill(context, gameConfiguration, SkillNumber.SelupanFall, "Selupan Fall", 2.5f);
        return ValueTask.CompletedTask;
    }
}
