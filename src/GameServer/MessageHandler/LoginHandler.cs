// <copyright file="LoginHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Linq;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Xor;

    /// <summary>
    /// Handler for login packets.
    /// </summary>
    internal class LoginHandler : IPacketHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginHandler));

        private readonly ISpanDecryptor decryptor;

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
            this.logoutAction = new LogoutAction();
            this.decryptor = new Xor3Decryptor(0);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet[3] == 1)
            {
                this.ReadLoginPacket(player, packet);
            }
            else if (packet[3] == 2)
            {
                this.ReadLogoutPacket(player, packet);
            }
            else
            {
                Log.Warn($"Player: {player}, Unknown login action: {packet[3] :X}");
            }
        }

        private void ReadLogoutPacket(Player player, Span<byte> buffer)
        {
            if (buffer.Length < 5)
            {
                return;
            }

            var logoutType = buffer[4];
            this.logoutAction.Logout(player, (LogoutType)logoutType);
        }

        private void ReadLoginPacket(Player player, Span<byte> packet)
        {
            var usernameBytes = packet.Slice(4, 10);
            var passwordBytes = packet.Slice(14, this.gameContext.Configuration.MaximumPasswordLength);

            this.decryptor.Decrypt(usernameBytes);
            this.decryptor.Decrypt(passwordBytes);
            var username = usernameBytes.ExtractString(0, 10, Encoding.UTF8);
            var password = passwordBytes.ExtractString(0, this.gameContext.Configuration.MaximumPasswordLength, Encoding.UTF8);

            int startTickCountIndex = 14 + this.gameContext.Configuration.MaximumPasswordLength;
            ////Player.StartTickCount = Utils.MakeDword(buffer[sti], buffer[sti + 1], buffer[sti + 2], buffer[sti + 3]);
            ////Player.StartTick = DateTime.Now;
            this.loginAction.Login(player, username, password);

            if (this.gameContext is GameServerContext gameServerContext && player is RemotePlayer remotePlayer)
            {
                var versionStartIndex = startTickCountIndex + 4;
                var version = packet.Slice(versionStartIndex, 5).ToArray();

                var mainPacketHandler = gameServerContext.PacketHandlers.FirstOrDefault(ph => ph.ClientVersion.AsSpan().IsEqual(version));
                if (mainPacketHandler != null)
                {
                    remotePlayer.MainPacketHandler = mainPacketHandler;
                }
            }
        }
    }
}
