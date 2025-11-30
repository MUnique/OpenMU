// <copyright file="IItemPostInfoViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.ItemPost;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Interface for showing item post information.
/// </summary>
public interface IItemPostInfoViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the item information for a posted item.
    /// </summary>
    /// <param name="postId">The post identifier.</param>
    /// <param name="item">The posted item.</param>
    ValueTask ShowItemPostInfoAsync(uint postId, Item item);
}
