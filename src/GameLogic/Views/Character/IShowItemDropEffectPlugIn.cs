// <copyright file="IShowItemDropEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Interface of a view whose implementation informs about an item drop effect at a coordinate.
/// </summary>
public interface IShowItemDropEffectPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the effect with the specified number at the specified coordinates.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="targetCoordinates">The target coordinates.</param>
    ValueTask ShowEffectAsync(ItemDropEffect effect, Point targetCoordinates);
}