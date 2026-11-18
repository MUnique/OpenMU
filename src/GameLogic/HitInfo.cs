// <copyright file="HitInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The information about a hit.
/// </summary>
/// <param name="HealthDamage">The health damage.</param>
/// <param name="ShieldDamage">The shield damage.</param>
/// <param name="Attributes">The damage attributes.</param>
/// <param name="ManaToll">The mana toll.</param>
/// <param name="PetDamage">The pet damage.</param>
/// <param name="SkillEntry">The skill entry.</param>
/// <param name="TriggersHitShock">Whether hit shock is triggered.</param>
public record struct HitInfo(
    uint HealthDamage,
    uint ShieldDamage,
    DamageAttributes Attributes,
    uint ManaToll = 0,
    uint PetDamage = 0,
    SkillEntry? SkillEntry = null,
    bool? TriggersHitShock = false)
{
    /// <summary>
    /// Gets the total damage.
    /// </summary>
    public readonly uint TotalDamage => this.HealthDamage + this.ShieldDamage;
}