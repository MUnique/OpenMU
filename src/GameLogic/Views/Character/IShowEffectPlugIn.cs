// <copyright file="IShowEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Interface of a view whose implementation informs about an effect for a target object.
/// </summary>
public interface IShowEffectPlugIn : IViewPlugIn
{
    /// <summary>
    /// Defines the effect which is shown for the object.
    /// </summary>
    public enum EffectType
    {
        /// <summary>
        /// Undefined effect.
        /// </summary>
        Undefined,

        /// <summary>
        /// Shield was gained by a potion.
        /// </summary>
        ShieldPotion,

        /// <summary>
        /// A level up was achieved by the player.
        /// </summary>
        LevelUp,

        /// <summary>
        /// The players <see cref="Stats.CurrentShield"/> reached 0.
        /// </summary>
        ShieldLost,

        /// <summary>
        /// A swirl is shown around the player, e.g. after dropping event items.
        /// </summary>
        Swirl,
    }

    /// <summary>
    /// Shows the effect for the target object.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="effectType">The effect type.</param>
    ValueTask ShowEffectAsync(IIdentifiable target, EffectType effectType);
}