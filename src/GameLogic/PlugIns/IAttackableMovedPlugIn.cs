// <copyright file="IAttackableMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which Is called when a <see cref="IAttackable"/> moved on the game map.
    /// </summary>
    [Guid("ABD191BC-AEA2-4308-8EAA-CAF0A0D6B46B")]
    [PlugInPoint("Attackable moved", "Plugins which are called when an attackable object moved on the game map.")]
    public interface IAttackableMovedPlugIn
    {
        /// <summary>
        /// Is called when a <see cref="IAttackable"/> moved on the game map.
        /// </summary>
        /// <param name="attackable">The <see cref="IAttackable"/>.</param>
        void AttackableMoved(IAttackable attackable);
    }
}