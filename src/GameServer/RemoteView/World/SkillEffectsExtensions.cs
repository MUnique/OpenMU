// <copyright file="SkillEffectsExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Extensions for <see cref="SkillEffects"/>.
    /// </summary>
    internal static class SkillEffectsExtensions
    {
        /// <summary>
        /// Gets the skill effect flags for the specified magic effects.
        /// </summary>
        /// <param name="visibleEffects">The visible effects.</param>
        /// <returns>The skill effect flags.</returns>
        internal static SkillEffects GetSkillFlags(this IList<MagicEffect> visibleEffects)
        {
            SkillEffects effectFlags = SkillEffects.Undefined;
            foreach (var visibleEffect in visibleEffects)
            {
                switch (visibleEffect.Id)
                {
                    case 0x01:
                        effectFlags |= SkillEffects.DamageBuff;
                        break;
                    case 0x02:
                        effectFlags |= SkillEffects.DefenseBuff;
                        break;
                    case 0x37:
                        effectFlags |= SkillEffects.Poisoned;
                        break;
                    case 0x38:
                        effectFlags |= SkillEffects.Iced;
                        break;
                    default:
                        // others are not supported, so we ignore them.
                        break;
                }
            }

            return effectFlags;
        }
    }
}