// <copyright file="HitInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// The attributes of a damage.
    /// </summary>
    [Flags]
    public enum DamageAttributes
    {
        /// <summary>
        /// No defined attribute.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The damage is critical (means 100% of the possible damage between minimum and maximum).
        /// </summary>
        Critical = 1,

        /// <summary>
        /// The damage is excellent (20% higher base damage than critical).
        /// </summary>
        Excellent = 2,

        /// <summary>
        /// The damage ignored the defense of the victim.
        /// </summary>
        IgnoreDefense = 4,

        /// <summary>
        /// The damage is caused by poison.
        /// </summary>
        Poison = 8,

        /// <summary>
        /// The damage was doubled.
        /// </summary>
        Double = 16,

        /// <summary>
        /// The damage was tripled (e.g. combo skill).
        /// </summary>
        Triple = 32
    }

    /// <summary>
    /// The information about a hit.
    /// </summary>
    public struct HitInfo
    {
        /// <summary>
        /// The attributes of the damage.
        /// </summary>
        public DamageAttributes Attributes;

        /// <summary>
        /// The damage which reduces the health points.
        /// </summary>
        public uint DamageHP;

        /// <summary>
        /// The damage which reduces the shield points.
        /// </summary>
        public uint DamageSD;

        /// <summary>
        /// Initializes a new instance of the <see cref="HitInfo"/> struct.
        /// </summary>
        /// <param name="damageHP">The damage which reduces the health points.</param>
        /// <param name="damageSD"> The damage which reduces the shield points.</param>
        /// <param name="attributes">The attributes.</param>
        public HitInfo(uint damageHP, uint damageSD, DamageAttributes attributes)
        {
            this.DamageHP = damageHP;
            this.DamageSD = damageSD;
            this.Attributes = attributes;
        }
    }
}
