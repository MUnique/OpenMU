// <copyright file="NewNpcsInScopePlugIn095.cs" company="MUnique">
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
/// The default implementation of the <see cref="T:MUnique.OpenMU.GameLogic.Views.World.INewNpcsInScopePlugIn" /> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(NewNpcsInScopePlugIn095), "The default implementation of the INewNpcsInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
[Guid("ECCD99EB-425D-4C9B-8F04-2711BA7A4C1E")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class NewNpcsInScopePlugIn095 : INewNpcsInScopePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NewNpcsInScopePlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask NewNpcsInScopeAsync(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true)
    {
        var connection = this._player.Connection;
        if (connection is null || newObjects is null || !newObjects.Any())
        {
            return;
        }

        var summons = newObjects.OfType<Monster>().Where(m => m.SummonedBy is { }).ToList();
        var npcs = newObjects.Except(summons).ToList();
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
            var size = AddNpcsToScope095Ref.GetRequiredSize(npcs.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddNpcsToScope095Ref(span)
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

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private static async ValueTask SummonedMonstersInScopeAsync(bool isSpawned, IConnection connection, ICollection<Monster> summons)
    {
        int Write()
        {
            var size = AddSummonedMonstersToScope095Ref.GetRequiredSize(summons.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddSummonedMonstersToScope095Ref(span)
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

                block.TypeNumber = (byte)summon.Definition.Number;
                block.CurrentPositionX = summon.Position.X;
                block.CurrentPositionY = summon.Position.Y;
                block.IsPoisoned = summon.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Poisoned);
                block.IsIced = summon.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Iced);
                block.IsDamageBuffed = summon.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DamageBuff);
                block.IsDefenseBuffed = summon.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DefenseBuff);

                var supportWalk = summon as ISupportWalk;
                if (supportWalk?.IsWalking ?? false)
                {
                    block.TargetPositionX = supportWalk.WalkTarget.X;
                    block.TargetPositionY = supportWalk.WalkTarget.Y;
                }
                else
                {
                    block.TargetPositionX = summon.Position.X;
                    block.TargetPositionY = summon.Position.Y;
                }

                block.Rotation = summon.Rotation.ToPacketByte();
                block.OwnerCharacterName = summon.SummonedBy?.Name ?? string.Empty;
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}