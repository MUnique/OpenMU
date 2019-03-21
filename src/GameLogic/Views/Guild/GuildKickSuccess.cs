﻿// <copyright file="GuildKickSuccess.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Type of the guild kick success.
    /// </summary>
    public enum GuildKickSuccess : byte
    {
        /// <summary>
        /// Kicking failed. Player stays at guild.
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Kicking succeeded. Player left the guild.
        /// </summary>
        KickSucceeded = 1,

        /// <summary>
        /// Kicking succeeded and guild got disbanded.
        /// </summary>
        GuildDisband = 4
    }
}