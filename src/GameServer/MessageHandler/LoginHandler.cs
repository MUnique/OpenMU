// <copyright file="LoginHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for login packets.
    /// </summary>
    internal class LoginHandler : IPacketHandler
    {
        private readonly IDecryptor decryptor;

        private readonly LoginAction loginAction;

        private readonly LogoutAction logoutAction;

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public LoginHandler(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
            this.loginAction = new LoginAction(gameContext);
            this.logoutAction = new LogoutAction(gameContext);
            this.decryptor = new Xor3Decryptor(0);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            if (packet[3] == 1)
            {
                this.ReadLoginPacket(player, packet);
            }
            else if (packet[3] == 2)
            {
                this.ReadLogoutPacket(player, packet);
            }
        }

        private void ReadLogoutPacket(Player player, byte[] buffer)
        {
            if (buffer.Length < 5)
            {
                return;
            }

            var logoutType = buffer[4];

            this.logoutAction.Logout(player, (LogoutType)logoutType);
        }

        private void ReadLoginPacket(Player player, byte[] packet)
        {
            byte[] userbytes = new byte[10];
            byte[] pass = new byte[this.gameContext.Configuration.MaximumPasswordLength];
            Array.Copy(packet, 4, userbytes, 0, 10);
            Array.Copy(packet, 14, pass, 0, this.gameContext.Configuration.MaximumPasswordLength);

            this.decryptor.Decrypt(ref userbytes);
            this.decryptor.Decrypt(ref pass);
            string username = userbytes.ExtractString(0, 10, Encoding.UTF8);
            string password = pass.ExtractString(0, this.gameContext.Configuration.MaximumPasswordLength, Encoding.UTF8);

            int startTickCountIndex = 14 + this.gameContext.Configuration.MaximumPasswordLength;
            ////Player.StartTickCount = Utils.MakeDword(buffer[sti], buffer[sti + 1], buffer[sti + 2], buffer[sti + 3]);
            ////Player.StartTick = DateTime.Now;
            this.loginAction.Login(player, username, password);

            var gameServerContext = this.gameContext as GameServerContext;
            var remotePlayer = player as RemotePlayer;
            if (gameServerContext != null && remotePlayer != null)
            {
                var versionStartIndex = startTickCountIndex + 4;
                byte[] version = new byte[5];
                Array.Copy(packet, versionStartIndex, version, 0, 5);
                var mainPacketHandler = gameServerContext.PacketHandlers.FirstOrDefault(ph => ph.ClientVersion.IsEqual(version));
                if (mainPacketHandler != null)
                {
                    remotePlayer.MainPacketHandler = mainPacketHandler;
                }
            }
        }
    }
}
