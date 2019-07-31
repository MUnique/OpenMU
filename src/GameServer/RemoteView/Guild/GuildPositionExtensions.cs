// <copyright file="GuildPositionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Extension method for <see cref="GuildPosition"/>.
    /// </summary>
    public static class GuildPositionExtensions
    {
        /// <summary>
        /// Gets the view value.
        /// </summary>
        /// <param name="playerPosition">The player position.</param>
        /// <returns>The value which is used in the message for the corresponding enum value.</returns>
        public static byte GetViewValue(this GuildPosition playerPosition)
        {
            switch (playerPosition)
            {
                case GuildPosition.GuildMaster:
                    return 0x80;
                case GuildPosition.NormalMember:
                    return 0x00;
                case GuildPosition.BattleMaster:
                    return 0x20;
                default:
                    throw new ArgumentException(nameof(playerPosition));
            }
        }
    }
}