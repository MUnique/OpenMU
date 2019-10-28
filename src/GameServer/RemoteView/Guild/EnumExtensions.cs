// <copyright file="EnumExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using static MUnique.OpenMU.Network.Packets.ServerToClient.GuildJoinResponse;

    /// <summary>
    /// Extension methods for enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the view value.
        /// </summary>
        /// <param name="playerPosition">The player position.</param>
        /// <returns>The value which is used in the message for the corresponding enum value.</returns>
        public static GuildMemberRole Convert(this GuildPosition? playerPosition)
        {
            if (playerPosition.HasValue)
            {
                return playerPosition.Value.Convert();
            }

            return GuildMemberRole.Undefined;
        }

        /// <summary>
        /// Gets the view value.
        /// </summary>
        /// <param name="playerPosition">The player position.</param>
        /// <returns>The value which is used in the message for the corresponding enum value.</returns>
        public static GuildMemberRole Convert(this GuildPosition playerPosition)
        {
            return playerPosition switch
            {
                GuildPosition.GuildMaster => GuildMemberRole.GuildMaster,
                GuildPosition.NormalMember => GuildMemberRole.NormalMember,
                GuildPosition.BattleMaster => GuildMemberRole.BattleMaster,
                _ => GuildMemberRole.Undefined,
            };
        }

        /// <summary>
        /// Converts the <see cref="GuildRequestAnswerResult"/> into a <see cref="GuildJoinRequestResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="GuildRequestAnswerResult"/> which should be converted.</param>
        /// <returns>The converted value.</returns>
        public static GuildJoinRequestResult Convert(this GuildRequestAnswerResult result)
        {
            return result switch
            {
                GuildRequestAnswerResult.Refused => GuildJoinRequestResult.Refused,
                GuildRequestAnswerResult.Accepted => GuildJoinRequestResult.Accepted,
                GuildRequestAnswerResult.GuildFull => GuildJoinRequestResult.GuildFull,
                GuildRequestAnswerResult.Disconnected => GuildJoinRequestResult.Disconnected,
                GuildRequestAnswerResult.NotTheGuildMaster => GuildJoinRequestResult.NotTheGuildMaster,
                GuildRequestAnswerResult.AlreadyHaveGuild => GuildJoinRequestResult.AlreadyHaveGuild,
                GuildRequestAnswerResult.GuildMasterOrRequesterIsBusy => GuildJoinRequestResult.GuildMasterOrRequesterIsBusy,
                GuildRequestAnswerResult.MinimumLevel6 => GuildJoinRequestResult.MinimumLevel6,
                _ => throw new NotImplementedException($"The case {result} is not implemented."),
            };
        }

        /// <summary>
        /// Converts a <see cref="GuildKickSuccess"/> into a <see cref="GuildKickResponse.GuildKickSuccess"/>.
        /// </summary>
        /// <param name="success">The <see cref="GuildKickSuccess"/> which should be converted.</param>
        /// <returns>The converted <see cref="GuildKickResponse.GuildKickSuccess"/>.</returns>
        public static GuildKickResponse.GuildKickSuccess Convert(this GuildKickSuccess success)
        {
            return success switch
            {
                GuildKickSuccess.Failed => GuildKickResponse.GuildKickSuccess.Failed,
                GuildKickSuccess.KickSucceeded => GuildKickResponse.GuildKickSuccess.KickSucceeded,
                GuildKickSuccess.GuildDisband => GuildKickResponse.GuildKickSuccess.GuildDisband,
                _ => throw new NotImplementedException($"The case {success} is not implemented."),
            };
        }

        /// <summary>
        /// Converts a <see cref="GuildCreateErrorDetail"/> into a <see cref="GuildCreationResult.GuildCreationErrorType"/>.
        /// </summary>
        /// <param name="errorDetail">The <see cref="GuildCreateErrorDetail"/> which should be converted.</param>
        /// <returns>The converted <see cref="GuildCreationResult.GuildCreationErrorType"/>.</returns>
        public static GuildCreationResult.GuildCreationErrorType Convert(this GuildCreateErrorDetail errorDetail)
        {
            return errorDetail switch
            {
                GuildCreateErrorDetail.None => GuildCreationResult.GuildCreationErrorType.None,
                GuildCreateErrorDetail.GuildAlreadyExist => GuildCreationResult.GuildCreationErrorType.GuildNameAlreadyTaken,
                _ => throw new NotImplementedException($"The case {errorDetail} is not implemented."),
            };
        }
    }
}