// <copyright file="GuildPositionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.Packets.ServerToClient;

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
        public static GuildMemberRole GetViewValue(this GuildPosition? playerPosition)
        {
            if (playerPosition.HasValue)
            {
                return playerPosition.Value.GetViewValue();
            }

            return GuildMemberRole.Undefined;
        }

        /// <summary>
        /// Gets the view value.
        /// </summary>
        /// <param name="playerPosition">The player position.</param>
        /// <returns>The value which is used in the message for the corresponding enum value.</returns>
        public static GuildMemberRole GetViewValue(this GuildPosition playerPosition)
        {
            return playerPosition switch
            {
                GuildPosition.GuildMaster => GuildMemberRole.GuildMaster,
                GuildPosition.NormalMember => GuildMemberRole.NormalMember,
                GuildPosition.BattleMaster => GuildMemberRole.BattleMaster,
                _ => GuildMemberRole.Undefined,
            };
        }
    }
}