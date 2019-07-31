// <copyright file="LogoutType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Login
{
    /// <summary>
    /// The type of the logout.
    /// </summary>
    public enum LogoutType : byte
    {
        /// <summary>
        /// This is sent when the player closes the game.
        /// </summary>
        CloseGame,

        /// <summary>
        /// This is sent by the client when the player wants to go back to the character selection screen.
        /// </summary>
        BackToCharacterSelection,

        /// <summary>
        /// This is sent by the client when the player wants to go back to the server selection screen.
        /// </summary>
        BackToServerSelection,
    }
}