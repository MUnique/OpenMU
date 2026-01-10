// <copyright file="SoulBarrierProficieSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Soul Barrier Proficiency skill action.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SoulBarrierProficieSkillAction_Name), Description = nameof(PlugInResources.SoulBarrierProficieSkillAction_Description), ResourceType = typeof(PlugInResources))]
[Guid("9c56073f-1719-423c-8397-30d94793f929")]
public class SoulBarrierProficieSkillAction : SoulBarrierStrengSkillAction
{
    /// <inheritdoc/>
    public override short Key => 404;
}