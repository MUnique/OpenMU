// <copyright file="SkillEffects.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System;

    /// <summary>
    /// Skill effect flags which were used in earlier versions, like 0.75.
    /// </summary>
    [Flags]
    internal enum SkillEffects
    {
        /// <summary>
        /// The undefined effect. No effect is active.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The object is poisoned.
        /// </summary>
        Poisoned = 1,

        /// <summary>
        /// The object is iced.
        /// </summary>
        Iced = 2,

        /// <summary>
        /// The object has a damage buff.
        /// </summary>
        DamageBuff = 4,

        /// <summary>
        /// The object has a defense buff.
        /// </summary>
        DefenseBuff = 8,
    }
}