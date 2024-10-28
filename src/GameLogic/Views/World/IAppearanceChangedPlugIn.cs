// <copyright file="IAppearanceChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about the changed appearance of a player.
/// </summary>
public interface IAppearanceChangedPlugIn : IViewPlugIn
{
    /// <summary>
    /// The appearance of a player changed.
    /// </summary>
    /// <param name="changedPlayer">The changed player.</param>
    /// <param name="changedItem">The changed item.</param>
    /// <param name="isEquipped"></param>
    ValueTask AppearanceChangedAsync(Player changedPlayer, Item changedItem, bool isEquipped);
}