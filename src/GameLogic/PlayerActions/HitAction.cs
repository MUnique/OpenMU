// <copyright file="HitAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// Action to hit targets without a skill with pure melee damage.
    /// </summary>
    public class HitAction
    {
        /// <summary>
        /// Hits the specified target by the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="target">The target.</param>
        /// <param name="attackAnimation">The attack animation.</param>
        /// <param name="lookingDirection">The looking direction.</param>
        public void Hit(Player player, IAttackable target, byte attackAnimation, Direction lookingDirection)
        {
            if (target is IObservable targetAsObservable)
            {
                targetAsObservable.ObserverLock.EnterReadLock();
                try
                {
                    if (!targetAsObservable.Observers.Contains(player))
                    {
                        // Target out of range
                        return;
                    }
                }
                finally
                {
                    targetAsObservable.ObserverLock.ExitReadLock();
                }
            }

            player.Rotation = lookingDirection;
            target.AttackBy(player, null);
            player.ObserverLock.EnterReadLock();
            try
            {
                player.Observers.ForEach(observer => observer.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowAnimation(player, attackAnimation, target, lookingDirection));
            }
            finally
            {
                player.ObserverLock.ExitReadLock();
            }
        }
    }
}
