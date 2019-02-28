// <copyright file="IAttackableGotHitPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an <see cref="IAttackable"/> got hit.
    /// </summary>
    [Guid("4FD59298-0424-4F93-83E5-290C8E7EB5E5")]
    [PlugInPoint("Attackable got hit", "Plugins which will be executed when an attackable object got hit by an attacker.")]
    public interface IAttackableGotHitPlugIn
    {
        /// <summary>
        /// This method is called when an <see cref="IAttackable"/> got hit.
        /// </summary>
        /// <param name="attackable">The attackable.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="hitInfo">The hit information.</param>
        void AttackableGotHit(IAttackable attackable, IAttackable attacker, HitInfo hitInfo);
    }
}