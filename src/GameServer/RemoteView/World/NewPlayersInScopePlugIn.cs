// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="INewPlayersInScopePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("NewPlayersInScopePlugIn", "The default implementation of the INewPlayersInScopePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("4cd64537-ae5f-4030-bca1-7fa30ebff6c6")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class NewPlayersInScopePlugIn : INewPlayersInScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NewPlayersInScopePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newPlayers)
        {
            if (newPlayers == null || !newPlayers.Any())
            {
                return;
            }

            var appearanceSerializer = this.player.AppearanceSerializer;
            var playerDataSize = appearanceSerializer.NeededSpace + 18;

            IList<Player> shopPlayers = null;
            IList<Player> guildPlayers = null;
            var newPlayerList = newPlayers.ToList();
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSize = 5 + (newPlayerList.Count * (playerDataSize + estimatedEffectsPerPlayer)); // this should just be a rough number to optimize the capacity of the list
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, estimatedSize))
            {
                var packet = writer.Span;
                packet[3] = 0x12;
                packet[4] = (byte) newPlayerList.Count;
                var actualSize = 5;

                foreach (var newPlayer in newPlayerList)
                {
                    var playerId = newPlayer.GetId(this.player);
                    var playerBlock = packet.Slice(actualSize);
                    playerBlock[0] = playerId.GetHighByte();
                    playerBlock[1] = playerId.GetLowByte();
                    playerBlock[2] = newPlayer.Position.X;
                    playerBlock[3] = newPlayer.Position.Y;

                    var appearanceBlock = playerBlock.Slice(4, appearanceSerializer.NeededSpace);
                    appearanceSerializer.WriteAppearanceData(appearanceBlock, newPlayer.AppearanceData, true); // 4 ... 21

                    playerBlock.Slice(4 + appearanceBlock.Length, 10).WriteString(newPlayer.SelectedCharacter.Name, Encoding.UTF8); // 22 ... 31
                    if (newPlayer.IsWalking)
                    {
                        playerBlock[14 + appearanceBlock.Length] = newPlayer.WalkTarget.X;
                        playerBlock[15 + appearanceBlock.Length] = newPlayer.WalkTarget.Y;
                    }
                    else
                    {
                        playerBlock[14 + appearanceBlock.Length] = newPlayer.Position.X;
                        playerBlock[15 + appearanceBlock.Length] = newPlayer.Position.Y;
                    }

                    playerBlock[16 + appearanceBlock.Length] = (byte)((newPlayer.Rotation.ToPacketByte() * 0x10) + newPlayer.SelectedCharacter.State);
                    var activeEffects = newPlayer.MagicEffectList.GetVisibleEffects();
                    var effectCount = 0;
                    for (int e = activeEffects.Count - 1; e >= 0; e--)
                    {
                        playerBlock[18 + appearanceBlock.Length + e] = (byte)activeEffects[e].Id;
                        effectCount++;
                    }

                    playerBlock[17 + appearanceBlock.Length] = (byte)effectCount;
                    actualSize += playerDataSize + effectCount;

                    if (newPlayer.ShopStorage.StoreOpen)
                    {
                        (shopPlayers ?? (shopPlayers = new List<Player>())).Add(newPlayer);
                    }

                    if (newPlayer.GuildStatus != null)
                    {
                        (guildPlayers ?? (guildPlayers = new List<Player>())).Add(newPlayer);
                    }
                }

                packet.Slice(0, actualSize).SetPacketSize();
                writer.Commit(actualSize);
            }

            if (shopPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IShowShopsOfPlayersPlugIn>()?.ShowShopsOfPlayers(shopPlayers);
            }

            if (guildPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayersToGuild(guildPlayers, true);
            }
        }
    }
}