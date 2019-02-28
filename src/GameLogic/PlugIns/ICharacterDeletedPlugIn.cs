// <copyright file="ICharacterDeletedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when a character has been deleted.
    /// </summary>
    [Guid("566F8E98-D4A3-40AD-B2EF-1B8359259B7A")]
    [PlugInPoint("Character deleted", "Is called when a character got deleted.")]
    public interface ICharacterDeletedPlugIn
    {
        /// <summary>
        /// Is called when a character has been deleted.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="deletedCharacter">The deleted character.</param>
        void CharacterDeleted(Player player, Character deletedCharacter);
    }
}