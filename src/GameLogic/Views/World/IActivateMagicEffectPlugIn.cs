// <copyright file="IActivateMagicEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    /// <summary>
    /// Interface of a view whose implementation informs about an activated magic effect.
    /// </summary>
    public interface IActivateMagicEffectPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Activates the magic effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="affectedObject">The affected object.</param>
        void ActivateMagicEffect(MagicEffect effect, IAttackable affectedObject);
    }
}