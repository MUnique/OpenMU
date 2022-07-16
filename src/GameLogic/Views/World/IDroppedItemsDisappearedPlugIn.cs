// <copyright file="IDroppedItemsDisappearedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about disappearing dropped items.
/// </summary>
public interface IDroppedItemsDisappearedPlugIn : IViewPlugIn
{
    /// <summary>
    /// The item drops disappeared from the ground.
    /// </summary>
    /// <param name="disappearedItemIds">The ids of the disappeared items.</param>
    ValueTask DroppedItemsDisappearedAsync(IEnumerable<ushort> disappearedItemIds);
}