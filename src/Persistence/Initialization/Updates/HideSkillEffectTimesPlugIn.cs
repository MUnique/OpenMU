// <copyright file="HideSkillEffectTimesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update switches off <see cref="MagicEffectDefinition.SendDuration"/> for the skill effects which the Season 6 data sent it for:
/// Infinite Arrow and the defense reductions of Fire Slash and Beast Uppercut, so that the open source client doesn't show their time.
/// </summary>
/// <remarks>
/// The original clients never got the time of these effects: they only get it for the effects of the attributes of consumed items.
/// The original game shows no time for skill effects, also in later seasons (GMO Season 21: Greater Damage, Greater Defense,
/// Shadow Phantom, Poison and Ice show none, see https://github.com/MUnique/OpenMU/pull/1476#issuecomment-6865505421;
/// the tooltip of Infinite Arrow shows only its name and "Arrows do not become consumed.", no time, see
/// https://github.com/MUnique/OpenMU/pull/1476#issuecomment-6945524150).
/// The update overwrites the current value of the flag, also one which an admin set, e.g. on the Effect times page;
/// as it's optional, an admin who wants these times shown can skip it.
/// The Beast Uppercut effect of existing databases comes from the released <see cref="FixRageFighterMultipleHitSkillsPlugIn"/>,
/// which stays as it is; this update depends on it, so it runs after it and switches that effect off, too.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.HideSkillEffectTimesPlugIn_Name), Description = nameof(PlugInResources.HideSkillEffectTimesPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C60DB57F-60D4-4A9A-85A6-1CDD27A21E1A")]
public class HideSkillEffectTimesPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Hide Skill Effect Times";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Switches off sending the duration of the skill effects which the Season 6 data sent it for: Infinite Arrow and the defense reductions of Fire Slash and Beast Uppercut. The original game shows no time for skill effects. It overwrites the current setting, also one made on the Effect times page.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(FixRageFighterMultipleHitSkillsPlugIn).GUID];

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 15, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Both defense reductions have the number which the client knows, the one of the Fire Slash effect.
        foreach (var effect in gameConfiguration.MagicEffects.Where(e => e.Number is (short)MagicEffectNumber.DefenseReduction or (short)MagicEffectNumber.InfiniteArrow))
        {
            effect.SendDuration = false;
        }

        return ValueTask.CompletedTask;
    }
}
