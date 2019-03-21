// <copyright file="UpdateCurrentHealthPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCurrentHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCurrentHealthPlugIn", "The default implementation of the IUpdateCurrentHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("0c832ed3-fea7-4239-8208-b46897b44c84")]
    public class UpdateCurrentHealthPlugIn : IUpdateCurrentHealthPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCurrentHealthPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCurrentHealthPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCurrentHealth()
        {
            // C1 09 26 FE 00 C3 00 00 85
            var hp = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f);
            var sd = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f);
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Current;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }
    }
}