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
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true)
        {
            var connection = this.player.Connection;
            if (connection is null || newObjects is null || !newObjects.Any())
            {
                return;
            }

            var summons = newObjects.OfType<Monster>().Where(m => m.SummonedBy is { }).ToList();
            var npcs = newObjects.Except(summons).ToList();

            if (npcs.Any())
            {
                NpcsInScope(isSpawned, connection, npcs);
            }

            if (summons.Any())
            {
                SummonedMonstersInScope(isSpawned, connection, summons);
            }
        }

        private static void NpcsInScope(bool isSpawned, IConnection connection, ICollection<NonPlayerCharacter> npcs)
        {
            using var writer = connection.StartSafeWrite(AddNpcsToScope.HeaderType, AddNpcsToScope.GetRequiredSize(npcs.Count));

            var packet = new AddNpcsToScope(writer.Span)
            {
                NpcCount = (byte) npcs.Count,
            };

            int i = 0;
            foreach (var npc in npcs)
            {
                var npcBlock = packet[i];
                npcBlock.Id = npc.Id;
                if (isSpawned)
                {
                    npcBlock.Id |= 0x8000;
                }

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

        private static void SummonedMonstersInScope(bool isSpawned, IConnection connection, ICollection<Monster> summons)
        {
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSizePerCharacter = AddSummonedMonstersToScope.SummonedMonsterData.GetRequiredSize(estimatedEffectsPerPlayer);
            var estimatedSize = AddSummonedMonstersToScope.GetRequiredSize(summons.Count, estimatedSizePerCharacter);
            using var writer = connection.StartSafeWrite(AddSummonedMonstersToScope.HeaderType, estimatedSize);

            var packet = new AddSummonedMonstersToScope(writer.Span)
            {
                MonsterCount = (byte)summons.Count,
            };

            int i = 0;
            foreach (var summon in summons)
            {
                var block = packet[i];
                block.Id = summon.Id;
                if (isSpawned)
                {
                    block.Id |= 0x8000;
                }

                block.TypeNumber = (ushort)(summon.Definition?.Number ?? 0);
                block.CurrentPositionX = summon.Position.X;
                block.CurrentPositionY = summon.Position.Y;

                if (summon.IsWalking)
                {
                    block.TargetPositionX = summon.WalkTarget.X;
                    block.TargetPositionY = summon.WalkTarget.Y;
                }
                else
                {
                    block.TargetPositionX = summon.Position.X;
                    block.TargetPositionY = summon.Position.Y;
                }

                block.Rotation = summon.Rotation.ToPacketByte();
                block.OwnerCharacterName = summon.SummonedBy?.Name ?? string.Empty;

                var activeEffects = summon.MagicEffectList.VisibleEffects;
                block.EffectCount = (byte)activeEffects.Count;
                for (int e = block.EffectCount - 1; e >= 0; e--)
                {
                    var effectBlock = block[e];
                    effectBlock.Id = (byte)activeEffects[e].Id;
                }

                i++;
            }

            writer.Commit();
        }
    }
}