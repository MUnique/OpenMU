// <copyright file="UpdateIsInSafezoneAfterPlayerMoved.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Updates the <see cref="Stats.IsInSafezone"/>. For example, this activates the automatic health and shield recover.
    /// </summary>
    [PlugIn(nameof(UpdateIsInSafezoneAfterPlayerMoved), "Updates the attribute if a player is located in the safezone. For example, this activates the automatic health and shield recover.")]
    [Guid("396617CA-954A-45A1-A6DB-1AA65309A03D")]
    public class UpdateIsInSafezoneAfterPlayerMoved : IAttackableMovedPlugIn
    {
        /// <inheritdoc />
        public void AttackableMoved(IAttackable attackable)
        {
            if (attackable is Player player)
            {
                player.Attributes.SetStatAttribute(Stats.IsInSafezone, attackable.IsAtSafezone() ? 1.0f : 0.0f);
            }
        }
    }
}
