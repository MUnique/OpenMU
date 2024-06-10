// <copyright file="NewNpcsInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

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
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class NewNpcsInScopePlugIn : INewNpcsInScopePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NewNpcsInScopePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask NewNpcsInScopeAsync(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true)
    {
        var connection = this._player.Connection;
        if (connection is null || newObjects is null || !newObjects.Any())
        {
            return;
        }

        var summons = newObjects.OfType<ISummonable>().Where(m => m.SummonedBy is { }).ToList();
        var npcs = newObjects.Except(summons.OfType<NonPlayerCharacter>()).ToList();

        if (npcs.Any())
        {
            await NpcsInScopeAsync(isSpawned, connection, npcs).ConfigureAwait(false);
        }

        if (summons.Any())
        {
            await SummonedMonstersInScopeAsync(isSpawned, connection, summons).ConfigureAwait(false);
        }
    }

    private static async ValueTask NpcsInScopeAsync(bool isSpawned, IConnection connection, ICollection<NonPlayerCharacter> npcs)
    {
        int Write()
        {
            var size = AddNpcsToScopeRef.GetRequiredSize(npcs.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddNpcsToScopeRef(span)
            {
                NpcCount = (byte)npcs.Count,
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

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private static async ValueTask SummonedMonstersInScopeAsync(bool isSpawned, IConnection connection, ICollection<ISummonable> summons)
    {
        int Write()
        {
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSizePerCharacter = AddSummonedMonstersToScopeRef.SummonedMonsterDataRef.GetRequiredSize(estimatedEffectsPerPlayer);
            var estimatedSize = AddSummonedMonstersToScopeRef.GetRequiredSize(summons.Count, estimatedSizePerCharacter);
            var span = connection.Output.GetSpan(estimatedSize)[..estimatedSize];
            var packet = new AddSummonedMonstersToScopeRef(span)
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

                if (summon is ISupportWalk walker && walker.IsWalking)
                {
                    block.TargetPositionX = walker.WalkTarget.X;
                    block.TargetPositionY = walker.WalkTarget.Y;
                }
                else
                {
                    block.TargetPositionX = summon.Position.X;
                    block.TargetPositionY = summon.Position.Y;
                }

                block.Rotation = summon.Rotation.ToPacketByte();
                block.OwnerCharacterName = summon.SummonedBy?.Name ?? string.Empty;

                if (summon is IAttackable attackable)
                {
                    var activeEffects = attackable.MagicEffectList.VisibleEffects;
                    block.EffectCount = (byte)activeEffects.Count;
                    for (int e = block.EffectCount - 1; e >= 0; e--)
                    {
                        var effectBlock = block[e];
                        effectBlock.Id = (byte)activeEffects[e].Id;
                    }
                }
                else
                {
                    block.EffectCount = 0;
                }

                i++;
            }

            return estimatedSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}