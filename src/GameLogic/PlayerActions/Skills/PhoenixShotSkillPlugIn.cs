// <copyright file="PhoenixShotSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the phoenix shot weapon skill of the rage fighter class. Additionally to the attacked target, it will hit up to seven additional targets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.PhoenixShotSkillPlugIn_Name), Description = nameof(PlugInResources.PhoenixShotSkillPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2C78E3DB-DDC7-4BC6-8539-707F81638ABF")]
public class PhoenixShotSkillPlugIn : DragonRoarSkillPlugIn
{
    /// <inheritdoc />
    public override short Key => 270;

    /// <inheritdoc />
    protected override short Range => 2;
}