// <copyright file="IAttackableGotHitWithSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an <see cref="IAttackable"/> got hit, including the skill which caused the hit.
/// </summary>
[Guid("7B2E9D41-5C3A-4F68-A1D0-3E8C6B9F2A57")]
[PlugInPoint("Attackable got hit with skill", "Plugins which will be executed when an attackable object got hit by an attacker, with the used skill (null for a basic attack).")]
public interface IAttackableGotHitWithSkillPlugIn
{
    /// <summary>
    /// This method is called when an <see cref="IAttackable"/> got hit.
    /// </summary>
    /// <param name="attackable">The attackable.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="hitInfo">The hit information.</param>
    /// <param name="skill">The skill which caused the hit; <c>null</c> for a basic attack.</param>
    void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo, Skill? skill);
}
