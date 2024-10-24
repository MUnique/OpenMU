// <copyright file="PowerSlashTargetFilterPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Filters the targets for the power slash skill.
/// </summary>
[Guid("DCC9E368-541E-4BA1-8863-AF9798BC4377")]
[PlugIn(nameof(PowerSlashTargetFilterPlugin), "Filters the targets for the power slash skill.")]
public class PowerSlashTargetFilterPlugin : FrustumBasedAreaSkillFilterPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PowerSlashTargetFilterPlugin"/> class.
    /// </summary>
    public PowerSlashTargetFilterPlugin()
        : base(1.0f, 6.0f, 6.0f)
    {
    }

    /// <inheritdoc />
    public override short Key => 56;
}