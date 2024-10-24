// <copyright file="FlameStrikeTargetFilterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the flame strike skill.
/// </summary>
[Guid("250E194E-F9AA-48DB-8F5F-9A15DC8E0706")]
[PlugIn(nameof(FlameStrikeTargetFilterPlugIn), "Filters the targets for the flame strike skill.")]
public class FlameStrikeTargetFilterPlugIn : FrustumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlameStrikeTargetFilterPlugIn"/> class.
    /// </summary>
    public FlameStrikeTargetFilterPlugIn()
        : base(5.0f, 2.0f, 4.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 236;
}