// <copyright file="AppearanceChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IAppearanceChangedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("AppearanceChangedPlugIn", "The default implementation of the IAppearanceChangedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("1d097399-d5af-40de-a97d-a812f13c2f20")]
    public class AppearanceChangedPlugIn : IAppearanceChangedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppearanceChangedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public AppearanceChangedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void AppearanceChanged(Player changedPlayer)
        {
            var appearanceSerializer = this.player.AppearanceSerializer;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5 + appearanceSerializer.NeededSpace))
            {
                var packet = writer.Span;
                packet[2] = 0x25;
                packet.Slice(3).SetShortSmallEndian(this.player.GetId(changedPlayer));
                appearanceSerializer.WriteAppearanceData(packet.Slice(5), changedPlayer.AppearanceData, true);
                writer.Commit();
            }

            // PMSG_USEREQUIPMENTCHANGED, 0x25
            /*
             * struct PMSG_USEREQUIPMENTCHANGED
                {
                    PBMSG_HEAD h;
                    BYTE NumberH;   // 3
                    BYTE NumberL;   // 4
                    BYTE ItemInfo[MAX_ITEM_INFO];   // 5
                };
             */
        }
    }
}