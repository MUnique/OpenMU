// <copyright file="UpdateCurrentManaPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IUpdateCurrentManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCurrentManaPlugIn", "The default implementation of the IUpdateCurrentManaPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("814fcc24-022a-47c8-b7d2-b1d1ca0208cb")]
    public class UpdateCurrentManaPlugIn : IUpdateCurrentManaPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCurrentManaPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCurrentManaPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCurrentMana()
        {
            // C1 08 27 FE 00 16 00 21
            var mana = (ushort)this.player.Attributes[Stats.CurrentMana];
            var ag = (ushort)this.player.Attributes[Stats.CurrentAbility];
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x08))
            {
                var packet = writer.Span;
                packet[2] = 0x27;
                packet[3] = (byte)UpdateType.Current;
                packet[4] = mana.GetHighByte();
                packet[5] = mana.GetLowByte();
                packet[6] = ag.GetHighByte();
                packet[7] = ag.GetLowByte();
                writer.Commit();
            }
        }
    }
}