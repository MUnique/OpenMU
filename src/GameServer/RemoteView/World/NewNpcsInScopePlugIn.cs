// <copyright file="NewNpcsInScopePlugIn.cs" company="MUnique">
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
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="INewNpcsInScopePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("NPCs in scope PlugIn", "The default implementation of the INewNpcsInScopePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("35449477-0fba-48cb-9371-f337433b0f9d")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class NewNpcsInScopePlugIn : INewNpcsInScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NewNpcsInScopePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            if (newObjects == null || !newObjects.Any())
            {
                return;
            }

            var newObjectList = newObjects.ToList();
            using var writer = this.player.Connection.StartSafeWrite(AddNpcsToScope.HeaderType, AddNpcsToScope.GetRequiredSize(newObjectList.Count));
            {
                var packet = new AddNpcsToScope(writer.Span)
                {
                    NpcCount = (byte)newObjectList.Count,
                };

                int i = 0;
                foreach (var npc in newObjectList)
                {
                    var npcBlock = packet[i];
                    npcBlock.Id = npc.Id;
                    npcBlock.TypeNumber = (ushort)(npc.Definition?.Number ?? 0);
                    npcBlock.CurrentPositionX = npc.Position.X;
                    npcBlock.CurrentPositionY = npc.Position.Y;

                    var supportWalk = npc as ISupportWalk;
                    if (supportWalk?.IsWalking ?? false)
                    {
                        npcBlock.TargetPositionX = supportWalk.WalkTarget.X;
                        npcBlock.TargetPositionY = supportWalk.WalkTarget.Y;
                    }
                    else
                    {
                        npcBlock.TargetPositionX = npc.Position.X;
                        npcBlock.TargetPositionY = npc.Position.Y;
                    }

                    npcBlock.Rotation = npc.Rotation.ToPacketByte();

                    i++;
                }

                writer.Commit();
            }
        }
    }
}