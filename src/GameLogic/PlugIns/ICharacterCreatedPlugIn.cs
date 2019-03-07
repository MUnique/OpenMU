// <copyright file="ICharacterCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when a character has been created.
    /// </summary>
    [Guid("B5588572-A324-4A94-9644-4DC3C8FEA4A4")]
    [PlugInPoint("Character created", "Is called when a character got created.")]
    public interface ICharacterCreatedPlugIn
    {
        /// <summary>
        /// Is called when a new character has been created.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="createdCharacter">The created character.</param>
        void CharacterCreated(Player player, Character createdCharacter);
    }
}