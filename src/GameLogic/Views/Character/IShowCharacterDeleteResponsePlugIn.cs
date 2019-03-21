// <copyright file="IShowCharacterDeleteResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about the character deletion request result.
    /// </summary>
    public interface IShowCharacterDeleteResponsePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the character delete response result.
        /// </summary>
        /// <param name="result">The result.</param>
        void ShowCharacterDeleteResponse(CharacterDeleteResult result);
    }
}