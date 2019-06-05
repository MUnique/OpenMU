// <copyright file="LoginHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Network.Xor;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for login packets of version 0.75.
    /// </summary>
    [PlugIn("Login 0.75", "Packet handler for login packets of version 0.75.")]
    [Guid("D2BA04C4-6730-4CF1-B4B9-EBFEA9443FFB")]
    [BelongsToGroup(LogInOutGroup.GroupKey)]
    [Client(0, 75, ClientLanguage.Invariant)]
    public class LoginHandlerPlugIn075 : ISubPacketHandlerPlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginHandlerPlugIn075));

        private readonly ISpanDecryptor decryptor = new Xor3Decryptor(0);

        private readonly LoginAction loginAction = new LoginAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => 1;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var usernameBytes = packet.Slice(4, 10);
            var passwordBytes = packet.Slice(14, 10);

            this.decryptor.Decrypt(usernameBytes);
            this.decryptor.Decrypt(passwordBytes);
            var username = usernameBytes.ExtractString(0, 10, Encoding.UTF8);
            var password = passwordBytes.ExtractString(0, 10, Encoding.UTF8);
            var version = packet.Slice(28, 3).ToArray();
            if (version[0] != 0 && version[1] != 0x37 && version[2] != 0x35)
            {
                Log.WarnFormat("Unexpected version: {0}, serial: {1}, username: {2}", version.AsString(), packet.Slice(31, 16).ToArray().AsString(), username);
            }

            this.loginAction.Login(player, username, password);
        }
    }
}