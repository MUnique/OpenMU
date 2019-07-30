// <copyright file="NewPlayersInScopePlugIn075.cs" company="MUnique">
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
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The implementation of the <see cref="INewPlayersInScopePlugIn"/> which is forwarding everything to the game client of version 0.75 with specific data packets.
    /// </summary>
    [PlugIn("NewPlayersInScopePlugIn 0.75", "The default implementation of the INewPlayersInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("1B68C660-34DD-4733-834D-DEE8DC1517D3")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    public class NewPlayersInScopePlugIn075 : INewPlayersInScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NewPlayersInScopePlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newPlayers)
        {
            if (newPlayers == null || !newPlayers.Any())
            {
                return;
            }

            var appearanceSerializer = this.player.AppearanceSerializer;
            var playerDataSize = appearanceSerializer.NeededSpace + 18;

            IList<Player> guildPlayers = null;
            var newPlayerList = newPlayers.ToList();
            var size = 5 + (newPlayerList.Count * playerDataSize);
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, size))
            {
                var packet = writer.Span;
                packet[3] = 0x12;
                packet[4] = (byte)newPlayerList.Count;
                var i = 0;

                foreach (var newPlayer in newPlayerList)
                {
                    var playerId = newPlayer.GetId(this.player);
                    var playerBlock = packet.Slice(5 + (i * playerDataSize), playerDataSize);
                    playerBlock[0] = playerId.GetHighByte();
                    playerBlock[1] = playerId.GetLowByte();
                    playerBlock[2] = newPlayer.Position.X;
                    playerBlock[3] = newPlayer.Position.Y;

                    var appearanceBlock = playerBlock.Slice(4, appearanceSerializer.NeededSpace);
                    appearanceSerializer.WriteAppearanceData(appearanceBlock, newPlayer.AppearanceData, true); // 4 ... 12
                    playerBlock[4 + appearanceBlock.Length] = (byte)newPlayer.MagicEffectList.GetVisibleEffects().GetSkillFlags();
                    playerBlock.Slice(5 + appearanceBlock.Length, 10).WriteString(newPlayer.SelectedCharacter.Name, Encoding.UTF8); // 14 ... 23
                    if (newPlayer.IsWalking)
                    {
                        playerBlock[15 + appearanceBlock.Length] = newPlayer.WalkTarget.X;
                        playerBlock[16 + appearanceBlock.Length] = newPlayer.WalkTarget.Y;
                    }
                    else
                    {
                        playerBlock[15 + appearanceBlock.Length] = newPlayer.Position.X;
                        playerBlock[16 + appearanceBlock.Length] = newPlayer.Position.Y;
                    }

                    playerBlock[17 + appearanceBlock.Length] = (byte)((newPlayer.Rotation.ToPacketByte() * 0x10) + newPlayer.SelectedCharacter.State);
                    i++;

                    if (newPlayer.GuildStatus != null)
                    {
                        (guildPlayers ?? (guildPlayers = new List<Player>())).Add(newPlayer);
                    }
                }

                writer.Commit();
            }

            if (guildPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayersToGuild(guildPlayers, true);
            }
        }
    }
}