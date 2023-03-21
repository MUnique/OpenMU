// <copyright file="MultiShotTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the multi shot skill.
/// </summary>
[Guid("6A863656-973C-414C-BBA0-FC3B79DF97E0")]
[PlugIn(nameof(MultiShotTargetFilterPlugin), "Filters the targets for the multi shot skill.")]
public class MultiShotTargetFilterPlugin : FrustrumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiShotTargetFilterPlugin"/> class.
    /// </summary>
    public MultiShotTargetFilterPlugin()
        : base(1.0f, 6.0f, 7.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 235;
}