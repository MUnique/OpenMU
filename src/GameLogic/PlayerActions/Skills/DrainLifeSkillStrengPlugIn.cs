// <copyright file="DrainLifeSkillStrengPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the drain life skill of the summoner class. Additionally to the attacked target, it regains life for damage dealt.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DrainLifeSkillStrengPlugIn_Name), Description = nameof(PlugInResources.DrainLifeSkillStrengPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E5A7D3C1-9B8F-4A62-B0D7-1F3E2C4A5B6D")]
public class DrainLifeSkillStrengPlugIn : DrainLifeSkillPlugIn
{
    /// <inheritdoc/>
    public override short Key => 458;
}