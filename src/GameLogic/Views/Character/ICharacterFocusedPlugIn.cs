// <copyright file="ICharacterFocusedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about the focusing of the character, previously requested by the client.
    /// </summary>
    public interface ICharacterFocusedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A characters has been focused on the character selection screen.
        /// </summary>
        /// <param name="character">The character which has been focused.</param>
        void CharacterFocused(Character character);
    }
}