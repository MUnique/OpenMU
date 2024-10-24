// <copyright file="StunTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the stun skill.
/// </summary>
/// <remarks>
/// In the original server, this is not calculated by a frustrum, but by a hitbox-file (SkillElect.hit),
/// which defines a hitbox for each 10 degree angle.
/// I decided against the file, because calculating seems more precise and consistent with the other skills.
/// </remarks>
[Guid("4E53F211-B225-4807-9FC7-1D65C1E1456A")]
[PlugIn(nameof(StunTargetFilterPlugin), "Filters the targets for the stun skill.")]
public class StunTargetFilterPlugin : FrustumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StunTargetFilterPlugin"/> class.
    /// </summary>
    public StunTargetFilterPlugin()
        : base(1.5f, 1.5f, 3.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 67;
}