// <copyright file="LogInHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.Xor;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for login packets.
    /// </summary>
    [PlugIn("Login", "Packet handler for login packets.")]
    [Guid("4A816FE5-809B-4D42-AF9F-1929FABD3295")]
    [BelongsToGroup(LogInOutGroup.GroupKey)]
    public class LogInHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly ISpanDecryptor decryptor = new Xor3Decryptor(0);

        private readonly LoginAction loginAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => LoginLongPassword.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 42)
            {
                if (packet.Length > 28 + 3)
                {
                    LoginShortPassword message = packet;
                    this.HandleLogin(player, message.Username, message.Password, message.TickCount, message.ClientVersion);
                }
                else
                {
                    // we have some version like 0.75 which just uses three bytes as version identifier
                    Login075 message = packet;
                    this.HandleLogin(player, message.Username, message.Password, message.TickCount, message.ClientVersion);
                }
            }
            else
            {
                LoginLongPassword message = packet;
                this.HandleLogin(player, message.Username, message.Password, message.TickCount, message.ClientVersion);
            }
        }

        private void HandleLogin(Player player, Span<byte> userNameSpan, Span<byte> passwordSpan, uint tickCount, Span<byte> version)
        {
            this.decryptor.Decrypt(userNameSpan);
            this.decryptor.Decrypt(passwordSpan);
            var username = userNameSpan.ExtractString(0, 10, Encoding.UTF8);
            var password = passwordSpan.ExtractString(0, 20, Encoding.UTF8);
            if (player.Logger.IsEnabled(LogLevel.Debug))
            {
                player.Logger.LogDebug($"User tries to log in. username:{username}, version:{version.AsString()}, tickCount:{tickCount} ");
            }

            this.loginAction.Login(player, username, password);
            if (player is RemotePlayer remotePlayer)
            {
                // Set Version in RemotePlayer so that the right plugins will be selected
                var clientVersion = ClientVersionResolver.Resolve(version);
                remotePlayer.ClientVersion = clientVersion;
            }
        }
    }
}
