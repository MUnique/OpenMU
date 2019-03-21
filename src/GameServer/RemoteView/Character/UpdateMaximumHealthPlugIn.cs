// <copyright file="UpdateMaximumHealthPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateMaximumHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateMaximumHealthPlugIn", "The default implementation of the IUpdateMaximumHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("6f8e7d9a-7d15-4e76-a650-8bfa70c7298e")]
    public class UpdateMaximumHealthPlugIn : IUpdateMaximumHealthPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMaximumHealthPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateMaximumHealthPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateMaximumHealth()
        {
            var hp = (ushort)this.player.Attributes[Stats.MaximumHealth];
            var sd = (ushort)this.player.Attributes[Stats.MaximumShield];
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Maximum;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }
    }
}