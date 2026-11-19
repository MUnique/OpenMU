// <copyright file="IUpdateMagicEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about a changed time or changed values
/// of an active magic effect of the own player, e.g. after a recast or a shortened duration.
/// </summary>
public interface IUpdateMagicEffectPlugIn : IViewPlugIn
{
    /// <summary>
    /// Informs about the new time left and values of the active magic effect.
    /// </summary>
    /// <remarks>
    /// The effect may end meanwhile, e.g. removed by an antidote while the update waits for the connection.
    /// So an implementation writes nothing for an effect which is disposing or disposed, checked when the packet
    /// is written (under the output lock): the disposal starts before the deactivation is sent.
    /// The effect list informs about every change of an effect which hasn't ended yet; an implementation may skip
    /// updates shortly before the end, which its client counts down by itself.
    /// </remarks>
    /// <param name="effect">The active effect.</param>
    /// <param name="affectedObject">The affected object.</param>
    ValueTask UpdateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject);
}