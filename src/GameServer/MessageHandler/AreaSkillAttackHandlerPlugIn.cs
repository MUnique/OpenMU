// <copyright file="AreaSkillAttackHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill attack packets.
/// </summary>
[PlugIn(nameof(AreaSkillAttackHandlerPlugIn), "Handler for area skill attack packets.")]
[Guid("9681bed4-70e8-4017-b27c-5eee164dd7b0")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
internal class AreaSkillAttackHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AreaSkillAttackAction _attackAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => AreaSkill.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        AreaSkill message = packet;
        if (player.SkillList is null || !player.SkillList.ContainsSkill(message.SkillId))
        {
            return;
        }

        if (player.SkillList.GetSkill(message.SkillId) is { Skill.SkillType: SkillType.AreaSkillExplicitHits })
        {
            // we don't need to return if it fails - it doesn't cause any damage, and the player
            // still "pays" the mana and ag.
            player.SkillHitValidator.TryRegisterAnimation(message.SkillId, message.AnimationCounter);
        }

        await this._attackAction.AttackAsync(player, message.ExtraTargetId, message.SkillId, new Point(message.TargetX, message.TargetY), message.Rotation).ConfigureAwait(false);
    }
}