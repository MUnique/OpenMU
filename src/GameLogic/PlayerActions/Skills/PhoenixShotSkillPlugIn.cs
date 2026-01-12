// <copyright file="PhoenixShotSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the phoenix shot weapon skill of the rage fighter class. Additionally to the attacked target, it will hit up to seven additional targets.
/// </summary>
[PlugIn(nameof(PhoenixShotSkillPlugIn), "Handles the phoenix shot weapon skill of the rage fighter class. Additionally to the attacked target, it will hit up to seven additional targets.")]
[Guid("2c78e3db-ddc7-4bc6-8539-707f81638abf")]
public class PhoenixShotSkillPlugIn : DragonRoarSkillPlugIn
{
    /// <inheritdoc />
    public override short Key => 270;

    /// <inheritdoc />
    protected override short Range => 2;
}