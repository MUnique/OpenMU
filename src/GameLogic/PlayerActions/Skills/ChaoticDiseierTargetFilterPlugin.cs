// <copyright file="ChaoticDiseierTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the chaotic diseier skill.
/// </summary>
[Guid("5DDE3499-BD4A-48F1-98A6-A99E8CC347CA")]
[PlugIn(nameof(ChaoticDiseierTargetFilterPlugin), "Filters the targets for the chaotic diseier skill.")]
public class ChaoticDiseierTargetFilterPlugin : FrustrumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaoticDiseierTargetFilterPlugin"/> class.
    /// </summary>
    public ChaoticDiseierTargetFilterPlugin()
        : base(1.5f, 1.5f, 6.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 238;
}