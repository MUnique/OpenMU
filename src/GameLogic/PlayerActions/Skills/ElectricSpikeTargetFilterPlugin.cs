// <copyright file="ElectricSpikeTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the electric spike skill.
/// </summary>
/// <remarks>
/// In the original server, this is not calculated by a frustrum, but by a hitbox-file (SkillElect.hit),
/// which defines a hitbox for each 10 degree angle.
/// I decided against the file, because calculating seems more precise and consistent with the other skills.
/// </remarks>
[Guid("67D30451-83EA-4C1F-A419-E720D1CF9484")]
[PlugIn(nameof(ElectricSpikeTargetFilterPlugin), "Filters the targets for the electric spike skill.")]
public class ElectricSpikeTargetFilterPlugin : FrustrumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElectricSpikeTargetFilterPlugin"/> class.
    /// </summary>
    public ElectricSpikeTargetFilterPlugin()
        : base(1.5f, 1.5f, 12.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 65;
}