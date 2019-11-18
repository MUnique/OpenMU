// <copyright file="AppearanceChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IAppearanceChangedPlugIn"/> which is forwarding appearance changes of other players to the game client with specific data packets.
    /// </summary>
    [PlugIn("Appearance changed", "The default implementation of the IAppearanceChangedPlugIn which is forwarding appearance changes of other players to the game client with specific data packets.")]
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
        public void AppearanceChanged(Player changedPlayer, Item item)
        {
            var itemSerializer = this.player.ItemSerializer;
            using var writer = this.player.Connection.StartSafeWrite(
                Network.Packets.ServerToClient.AppearanceChanged.HeaderType,
                Network.Packets.ServerToClient.AppearanceChanged.GetRequiredSize(itemSerializer.NeededSpace));
            var packet = new AppearanceChanged(writer.Span)
            {
                ChangedPlayerId = changedPlayer.GetId(this.player),
            };

            if (changedPlayer.Inventory.EquippedItems.Contains(item))
            {
                itemSerializer.SerializeItem(packet.ItemData, item);
            }
            else
            {
                packet.ItemData.Fill(0xFF);
            }

            // The byte with index 1 usually now holds the item level and one part of the item option level.
            // This full information is irrelevant. For this message, we just need the "glow" level, which means the one of the appearance serializer.
            // In the available space, the item position is serialized.
            // To summarize: The 4 higher bits hold the item position, the 4 lower bits hold the "glow" level
            packet.ItemData[1] = (byte)(item.ItemSlot << 4);
            packet.ItemData[1] |= item.GetGlowLevel();

            // We could also continue to dumb down information here as this packet reveals all of the options of an item to
            // other players - something which is probably not in interest of the players.
            // However, for now we keep this logic close to the original server, which doesn't do a thing about it.

            // Additionally, we could think of ignoring changes of rings and pendants, as they are usually not visible in the game client, except
            // maybe transformation rings. So we'll leave it as it is, too.
            writer.Commit();
        }
    }
}