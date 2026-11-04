// <copyright file="IShowDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Interface of a view whose implementation informs about an effect at a coordinate.
/// </summary>
public interface IShowDialogPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the effect with the specified number at the specified coordinates.
    /// </summary>
    /// <param name="categoryNumber">The number of the dialog category.</param>
    /// <param name="dialogNumber">The number of the specific dialog.</param>
    ValueTask ShowDialogAsync(byte categoryNumber, byte dialogNumber);
}