// <copyright file="AreaSkillHitHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill hit packets.
/// </summary>
[PlugIn("AreaSkillHitHandlerPlugIn", "Handler for area skill hit packets.")]
[Guid("2f5848fd-a1bd-488b-84b3-fd88bdef5ac8")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
internal class AreaSkillHitHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AreaSkillHitAction _skillHitAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => AreaSkillHit.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < 11)
        {
            return;
        }

        AreaSkillHit message = packet;
        var skillId = message.SkillId;
        if (skillId == 0)
        {
            skillId = player.SkillHitValidator.LastRegisteredSkillId;
        }

        if (player.SkillList is null || !player.SkillList.ContainsSkill(skillId))
        {
            return;
        }

        var increaseCounterAfterLoop = false;
        var targetCount = message.TargetCount;
        try
        {
            for (int i = 0; i < targetCount; i++)
            {
                var targetInfo = message[i];
                var (isHitValid, increaseCounter) = player.SkillHitValidator.IsHitValid(skillId, targetInfo.AnimationCounter, message.HitCounter);
                increaseCounterAfterLoop |= increaseCounter;
                if (!isHitValid)
                {
                    return;
                }

                if (player.GetObject(targetInfo.TargetId) is IAttackable target)
                {
                    if (target is IObservable observable && observable.Observers.Contains(player))
                    {
                        if (player.SkillList.GetSkill(skillId) is { } skillEntry)
                        {
                            await this._skillHitAction.AttackTargetAsync(player, target, skillEntry).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // Client may be out of sync (or it's an hacker attempt),
                        // so we tell him the object is out of scope - this should prevent further attempts to attack it.
                        await player.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(target.GetAsEnumerable())).ConfigureAwait(false);
                    }
                }
            }
        }
        finally
        {
            if (increaseCounterAfterLoop)
            {
                player.SkillHitValidator.IncreaseCounterAfterHit();
            }
        }
    }
}