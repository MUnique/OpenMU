// <copyright file="UpdateMaximumManaPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IUpdateMaximumManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateMaximumManaPlugIn", "The default implementation of the IUpdateMaximumManaPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("dc84be82-7ab0-4348-aa34-4a3dc8c1ee7a")]
    public class UpdateMaximumManaPlugIn : IUpdateMaximumManaPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMaximumManaPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateMaximumManaPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateMaximumMana()
        {
            var mana = (ushort)this.player.Attributes[Stats.MaximumMana];
            var ag = (ushort)this.player.Attributes[Stats.MaximumAbility];

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x08))
            {
                var packet = writer.Span;
                packet[2] = 0x27;
                packet[3] = (byte)UpdateType.Maximum;
                packet[4] = mana.GetHighByte();
                packet[5] = mana.GetLowByte();
                packet[6] = ag.GetHighByte();
                packet[7] = ag.GetLowByte();
                writer.Commit();
            }
        }
    }
}