// <copyright file="NewNpcsInScopePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="T:MUnique.OpenMU.GameLogic.Views.World.INewNpcsInScopePlugIn" /> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("NPCs in scope PlugIn 0.75", "The default implementation of the INewNpcsInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("7E9CE800-E59F-4E90-A6F1-28214483213C")]
    [Client(0, 75, ClientLanguage.Invariant)]
    public class NewNpcsInScopePlugIn075 : INewNpcsInScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NewNpcsInScopePlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            const int NpcDataSize = 9;

            if (newObjects == null || !newObjects.Any())
            {
                return;
            }

            var newObjectList = newObjects.ToList();
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, (newObjectList.Count * NpcDataSize) + 5))
            {
                var packet = writer.Span;
                packet[3] = 0x13; ////Packet Id
                packet[4] = (byte)newObjectList.Count;
                int i = 0;
                foreach (var npc in newObjectList)
                {
                    var npcBlock = packet.Slice(5 + (i * NpcDataSize));
                    npcBlock[0] = npc.Id.GetHighByte();
                    npcBlock[1] = npc.Id.GetLowByte();

                    npcBlock[2] = (byte)(npc.Definition.Number & 0xFF);
                    npcBlock[3] = (byte)((npc as Monster)?.MagicEffectList.GetVisibleEffects().GetSkillFlags() ?? 0);

                    ////Coords:
                    npcBlock[4] = npc.Position.X;
                    npcBlock[5] = npc.Position.Y;
                    var supportWalk = npc as ISupportWalk;
                    if (supportWalk?.IsWalking ?? false)
                    {
                        npcBlock[6] = supportWalk.WalkTarget.X;
                        npcBlock[7] = supportWalk.WalkTarget.Y;
                    }
                    else
                    {
                        npcBlock[6] = npc.Position.X;
                        npcBlock[7] = npc.Position.Y;
                    }

                    npcBlock[8] = (byte)(npc.Rotation.ToPacketByte() << 4);
                    i++;
                }

                writer.Commit();
            }
        }
    }
}