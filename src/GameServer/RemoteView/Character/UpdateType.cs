// <copyright file="UpdateType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    /// <summary>
    /// Type of an attribute update.
    /// </summary>
    internal enum UpdateType
    {
        /// <summary>
        /// An item consumption failed, no value is updated.
        /// </summary>
        Failed = 0xFD,

        /// <summary>
        /// The maximum value is updated.
        /// </summary>
        Maximum = 0xFE,

        /// <summary>
        /// The current value is updated.
        /// </summary>
        Current = 0xFF,
    }
}