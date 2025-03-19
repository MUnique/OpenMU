// <copyright file="HitInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The information about a hit.
/// </summary>
public record struct HitInfo(uint HealthDamage, uint ShieldDamage, DamageAttributes Attributes, uint ManaToll = 0);