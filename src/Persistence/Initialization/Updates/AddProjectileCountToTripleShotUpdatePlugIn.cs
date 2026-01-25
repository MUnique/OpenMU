// <copyright file="AddProjectileCountToTripleShotUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the Triple Shot skill to use 3 projectiles for proper arrow direction handling.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("E3A8F7C9-2D4B-4A1E-9F3C-8B5D7A6C1E4F")]
public class AddProjectileCountToTripleShotUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Projectile Count to Triple Shot";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the projectile count of 3 to the Triple Shot skill to properly handle arrow directions.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddProjectileCountToTripleShot;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 01, 04, 11, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var tripleShotSkill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TripleShot);
        if (tripleShotSkill?.AreaSkillSettings is { } areaSkillSettings)
        {
            areaSkillSettings.ProjectileCount = 3;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
