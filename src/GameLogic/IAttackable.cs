// <copyright file="IAttackable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Contains information about the last death of the object.
    /// </summary>
    /// <param name="KillerId">The id of the killer.</param>
    /// <param name="KillerName">The name of the killer.</param>
    /// <param name="FinalHit">The hit info of the final/lethal hit.</param>
    /// <param name="SkillNumber">The number of the used skill.</param>
    public record DeathInformation(ushort KillerId, string KillerName, HitInfo FinalHit, short SkillNumber);

    /// <summary>
    /// Interface for an object which is attackable.
    /// </summary>
    public interface IAttackable : IIdentifiable, ILocateable
    {
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        IAttributeSystem Attributes { get; }

        /// <summary>
        /// Gets the magic effect list which contains buffs and de-buffs.
        /// </summary>
        MagicEffectsList MagicEffectList { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IAttackable"/> is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IAttackable"/> is currently teleporting and can't be directly targeted.
        /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
        /// </value>
        bool IsTeleporting { get; }

        /// <summary>
        /// Gets the information about the last death.
        /// </summary>
        DeathInformation? LastDeath { get; }

        /// <summary>
        /// Attacks this object by the attacker with the specified skill.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="skill">The skill.</param>
        void AttackBy(IAttacker attacker, SkillEntry? skill);

        /// <summary>
        /// Reflects the damage which was done previously with <see cref="AttackBy" /> or even <see cref="ReflectDamage" /> to the <paramref name="reflector" />.
        /// </summary>
        /// <param name="reflector">The reflector.</param>
        /// <param name="damage">The damage.</param>
        void ReflectDamage(IAttacker reflector, uint damage);

        /// <summary>
        /// Applies the poison damage.
        /// </summary>
        /// <param name="initialAttacker">The initial attacker.</param>
        /// <param name="damage">The damage.</param>
        void ApplyPoisonDamage(IAttacker initialAttacker, uint damage);
    }
}
