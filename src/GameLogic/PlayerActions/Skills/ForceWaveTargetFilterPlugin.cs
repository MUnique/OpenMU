// <copyright file="ForceWaveTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the force wave skill.
/// </summary>
/// <remarks>
/// In the original server, this is not calculated by a frustrum, but by a hitbox-file (SkillSpear.hit),
/// which defines a hitbox for each 10 degree angle.
/// I decided against the file, because calculating seems more precise and consistent with the other skills.
/// </remarks>
[Guid("87D19A8A-B656-43D8-87A4-F05BFE12D691")]
[PlugIn(nameof(ForceWaveTargetFilterPlugin), "Filters the targets for the force wave skill.")]
public class ForceWaveTargetFilterPlugin : FrustumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForceWaveTargetFilterPlugin"/> class.
    /// </summary>
    public ForceWaveTargetFilterPlugin()
        : base(1f, 1f, 4f)
    {
    }

    /// <inheritdoc />
    public override short Key => 66;
}