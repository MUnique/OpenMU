// <copyright file="IUpdateCharacterHeroStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Interface of a view whose implementation informs about a change in the <see cref="Character.State"/>.
/// </summary>
public interface IUpdateCharacterHeroStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the current hero state.
    /// </summary>
    /// <param name="affectedPlayer">The player whose status needs an update.</param>
    ValueTask UpdateCharacterHeroStateAsync(Player affectedPlayer);
}