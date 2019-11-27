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
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="T:MUnique.OpenMU.GameLogic.Views.World.INewNpcsInScopePlugIn" /> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("NPCs in scope PlugIn 0.75", "The default implementation of the INewNpcsInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("7E9CE800-E59F-4E90-A6F1-28214483213C")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
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
            if (newObjects == null || !newObjects.Any())
            {
                return;
            }

            var newObjectList = newObjects.ToList();
            using var writer = this.player.Connection.StartSafeWrite(AddNpcsToScope075.HeaderType, AddNpcsToScope075.GetRequiredSize(newObjectList.Count));

            var packet = new AddNpcsToScope075(writer.Span)
            {
                NpcCount = (byte)newObjectList.Count,
            };

            int i = 0;
            foreach (var npc in newObjectList)
            {
                var npcBlock = packet[i];
                npcBlock.Id = npc.Id;
                npcBlock.TypeNumber = (byte)npc.Definition.Number;
                npcBlock.CurrentPositionX = npc.Position.X;
                npcBlock.CurrentPositionY = npc.Position.Y;
                if (npc is Monster monster)
                {
                    npcBlock.IsPoisoned = monster.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Poisoned);
                    npcBlock.IsIced = monster.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Iced);
                    npcBlock.IsDamageBuffed = monster.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DamageBuff);
                    npcBlock.IsDefenseBuffed = monster.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DefenseBuff);
                }

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