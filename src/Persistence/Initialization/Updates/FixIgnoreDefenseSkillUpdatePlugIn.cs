// <copyright file="FixIgnoreDefenseSkillUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the ignore defense skill.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("8DEC7BC2-E6A0-4E46-B123-C92CB43B9ED5")]
public class FixIgnoreDefenseSkillUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fixed ignore defense skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the magic effect definition for the Ignore Defense skill.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixIgnoreDefenseSkill;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 08, 20, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.IgnoreDefense);
        skill.SkillType = SkillType.Buff;
        skill.Target = SkillTarget.ImplicitPlayer;
        skill.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.IgnoreDefense);
    }
}
