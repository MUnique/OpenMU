// <copyright file="IUpdateCharacterBaseStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Interface of a view whose implementation informs about changed character base stats.
/// </summary>
public interface IUpdateCharacterBaseStatsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the character base stats.
    /// </summary>
    ValueTask UpdateCharacterBaseStatsAsync();
}