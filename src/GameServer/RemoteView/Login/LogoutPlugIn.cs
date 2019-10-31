// <copyright file="LogoutPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Login
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ILogoutPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("LogoutPlugIn", "The default implementation of the ILogoutPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f4a5185b-b422-463d-8df0-56f270a8831e")]
    public class LogoutPlugIn : ILogoutPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public LogoutPlugIn(RemotePlayer player) => this.player = player;

        /// <remarks>
        /// This packet is sent to the Client after a Logout Request, or by disconnection by the server.
        /// It will send the Client to the the Server Select, Character Select or Disconnects the User.
        /// </remarks>
        /// <inheritdoc />
        public void Logout(LogoutType logoutType)
        {
            using var writer = this.player.Connection.StartSafeWrite(LogoutResponse.HeaderType, LogoutResponse.Length);
            _ = new LogoutResponse(writer.Span)
            {
                Type = Convert(logoutType),
            };

            writer.Commit();
        }

        private static LogOutType Convert(LogoutType logoutType)
        {
            return logoutType switch
            {
                LogoutType.CloseGame => LogOutType.CloseGame,
                LogoutType.BackToServerSelection => LogOutType.BackToServerSelection,
                LogoutType.BackToCharacterSelection => LogOutType.BackToCharacterSelection,
                _ => throw new ArgumentException($"Unhandled enum value {logoutType}."),
            };
        }
    }
}