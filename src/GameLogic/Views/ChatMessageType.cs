// <copyright file="ChatMessageType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    /// <summary>
    /// The type of a chat message.
    /// </summary>
    public enum ChatMessageType
    {
        /// <summary>
        /// A normal chat message which is sent to all observing players.
        /// </summary>
        Normal,

        /// <summary>
        /// A whispered chat message, which only the recipient can read.
        /// </summary>
        Whisper,

        /// <summary>
        /// A chat message which can only be read inside the same party.
        /// </summary>
        Party,

        /// <summary>
        /// A chat message which can only be read inside the same guild.
        /// </summary>
        Guild,

        /// <summary>
        /// A chat message which can only be read inside the same guild alliance.
        /// </summary>
        Alliance,

        /// <summary>
        /// A chat message which can only be read inside the same gens side.
        /// </summary>
        Gens,

        /// <summary>
        /// A command message.
        /// </summary>
        Command,
    }
}