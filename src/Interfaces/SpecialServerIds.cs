// <copyright file="SpecialServerIds.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// Special server ids.
    /// </summary>
    /// <remarks>Ids from 0 to 0xFFFF are reserved to game servers.</remarks>
    public static class SpecialServerIds
    {
        /// <summary>
        /// The connect server special server id.
        /// </summary>
        public static readonly int ConnectServer = 0x10000;

        /// <summary>
        /// The chat server special server id.
        /// </summary>
        public static readonly int ChatServer = 0x20000;
    }
}