// <copyright file="LogInHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Network.Xor;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for login packets.
    /// </summary>
    [PlugIn("Login", "Packet handler for login packets.")]
    [Guid("4A816FE5-809B-4D42-AF9F-1929FABD3295")]
    [BelongsToGroup(LogInOutGroup.GroupKey)]
    [Client(0, 97, ClientLanguage.Invariant)]
    public class LogInHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly ISpanDecryptor decryptor = new Xor3Decryptor(0);

        private readonly LoginAction loginAction = new LoginAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => 1;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // todo: determine password length by looking at the packet length
            var usernameBytes = packet.Slice(4, 10);
            var passwordBytes = packet.Slice(14, player.GameContext.Configuration.MaximumPasswordLength);

            this.decryptor.Decrypt(usernameBytes);
            this.decryptor.Decrypt(passwordBytes);
            var username = usernameBytes.ExtractString(0, 10, Encoding.UTF8);
            var password = passwordBytes.ExtractString(0, player.GameContext.Configuration.MaximumPasswordLength, Encoding.UTF8);

            int startTickCountIndex = 14 + player.GameContext.Configuration.MaximumPasswordLength;
            ////Player.StartTickCount = Utils.MakeDword(buffer[sti], buffer[sti + 1], buffer[sti + 2], buffer[sti + 3]);
            ////Player.StartTick = DateTime.Now;
            this.loginAction.Login(player, username, password);
            var versionStartIndex = startTickCountIndex + 4;
            var version = packet.Slice(versionStartIndex, 5).ToArray();

            if (player is RemotePlayer remotePlayer)
            {
                // Set Version in RemotePlayer so that the right plugins will be selected
                var clientVersion = ClientVersionResolver.Resolve(version);
                remotePlayer.ClientVersion = clientVersion;
            }
        }
    }
}
