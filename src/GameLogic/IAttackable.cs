// <copyright file="IAttackable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Contains information about the last death of the object.
/// </summary>
/// <param name="KillerId">The id of the killer.</param>
/// <param name="KillerName">The name of the killer.</param>
/// <param name="FinalHit">The hit info of the final/lethal hit.</param>
/// <param name="SkillNumber">The number of the used skill.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "The names are affecting the property names of the record. We want upper case there.")]
public record DeathInformation(ushort KillerId, string KillerName, HitInfo FinalHit, short SkillNumber);

/// <summary>
/// Interface for an object which is attackable.
/// </summary>
public interface IAttackable : IIdentifiable, ILocateable
{
    /// <summary>
    /// Occurs when this instance died.
    /// </summary>
    event EventHandler<DeathInformation>? Died;

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
    /// <param name="isCombo">If set to <c>true</c>, the attacker did a combination of skills.</param>
    /// <param name="damageFactor">The damage factor.</param>
    ValueTask AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0);

    /// <summary>
    /// Reflects the damage which was done previously with <see cref="AttackByAsync" /> or even <see cref="ReflectDamageAsync" /> to the <paramref name="reflector" />.
    /// </summary>
    /// <param name="reflector">The reflector.</param>
    /// <param name="damage">The damage.</param>
    ValueTask ReflectDamageAsync(IAttacker reflector, uint damage);

    /// <summary>
    /// Applies the poison damage.
    /// </summary>
    /// <param name="initialAttacker">The initial attacker.</param>
    /// <param name="damage">The damage.</param>
    ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage);

    /// <summary>
    /// Kills the attackable instantly.
    /// </summary>
    ValueTask KillInstantlyAsync();
}