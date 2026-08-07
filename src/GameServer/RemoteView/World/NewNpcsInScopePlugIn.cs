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
[PlugIn]
[Display(Name = nameof(PlugInResources.NewNpcsInScopePlugIn_Name), Description = nameof(PlugInResources.NewNpcsInScopePlugIn_Description), ResourceType = typeof(PlugInResources))]
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
            if (npcs.Any(npc => npc.Position.X > byte.MaxValue || npc.Position.Y > byte.MaxValue))
            {
                await NpcsInGlobalScopeAsync(isSpawned, connection, npcs).ConfigureAwait(false);
            }
            else
            {
                await NpcsInScopeAsync(isSpawned, connection, npcs).ConfigureAwait(false);
            }
        }

        if (summons.Any())
        {
            await SummonedMonstersInScopeAsync(isSpawned, connection, summons).ConfigureAwait(false);
        }
    }

    private static async ValueTask NpcsInGlobalScopeAsync(bool isSpawned, IConnection connection, ICollection<NonPlayerCharacter> npcs)
    {
        int Write()
        {
            var size = AddNpcsToScopeGlobalRef.GetRequiredSize(npcs.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddNpcsToScopeGlobalRef(span)
            {
                NpcCount = (byte)npcs.Count,
            };

            int i = 0;
            foreach (var npc in npcs)
            {
                var block = packet[i];
                block.Id = isSpawned ? (ushort)(npc.Id | 0x8000) : npc.Id;
                block.TypeNumber = (ushort)(npc.Definition?.Number ?? 0);
                block.CurrentPositionX = npc.Position.X;
                block.CurrentPositionY = npc.Position.Y;
                var walker = npc as ISupportWalk;
                var target = walker?.IsWalking == true ? walker.WalkTarget : npc.Position;
                block.TargetPositionX = target.X;
                block.TargetPositionY = target.Y;
                block.Rotation = npc.Rotation.ToPacketByte();
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
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
                npcBlock.CurrentPositionX = checked((byte)(npc.Position.X));
                npcBlock.CurrentPositionY = checked((byte)(npc.Position.Y));

                var supportWalk = npc as ISupportWalk;
                if (supportWalk?.IsWalking ?? false)
                {
                    npcBlock.TargetPositionX = checked((byte)(supportWalk.WalkTarget.X));
                    npcBlock.TargetPositionY = checked((byte)(supportWalk.WalkTarget.Y));
                }
                else
                {
                    npcBlock.TargetPositionX = checked((byte)(npc.Position.X));
                    npcBlock.TargetPositionY = checked((byte)(npc.Position.Y));
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
                block.CurrentPositionX = checked((byte)(summon.Position.X));
                block.CurrentPositionY = checked((byte)(summon.Position.Y));

                if (summon is ISupportWalk walker && walker.IsWalking)
                {
                    block.TargetPositionX = checked((byte)(walker.WalkTarget.X));
                    block.TargetPositionY = checked((byte)(walker.WalkTarget.Y));
                }
                else
                {
                    block.TargetPositionX = checked((byte)(summon.Position.X));
                    block.TargetPositionY = checked((byte)(summon.Position.Y));
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
