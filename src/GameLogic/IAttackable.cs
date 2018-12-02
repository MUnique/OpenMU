// <copyright file="IAttackable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface for an attackable object. An attackable object can also attack other attackable objects.
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
        bool Alive { get; }

        /// <summary>
        /// Gets the last received damage.
        /// </summary>
        uint LastReceivedDamage { get; }

        /// <summary>
        /// Attacks this object by the attacker with the specified skill.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="skill">The skill.</param>
        void AttackBy(IAttackable attacker, SkillEntry skill);

        /// <summary>
        /// Reflects the damage which was done previously with <see cref="AttackBy" /> or even <see cref="ReflectDamage" /> to the <paramref name="reflector" />.
        /// </summary>
        /// <param name="reflector">The reflector.</param>
        /// <param name="damage">The damage.</param>
        void ReflectDamage(IAttackable reflector, uint damage);
    }
}
