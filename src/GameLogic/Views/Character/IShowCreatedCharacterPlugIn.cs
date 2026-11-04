// <copyright file="IShowCreatedCharacterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Interface of a view whose implementation informs about the successfully created character.
/// </summary>
public interface IShowCreatedCharacterPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the created character.
    /// </summary>
    /// <param name="character">The character.</param>
    ValueTask ShowCreatedCharacterAsync(DataModel.Entities.Character character);
}