// <copyright file="AreaSkillHitHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for area skill hit packets.
    /// </summary>
    /// <remarks>
    /// TODO: It's usually required to perform a <see cref="AreaSkillAttackAction"/> before, so this check has to be implemented.
    /// </remarks>
    [PlugIn(nameof(AreaSkillHitHandlerPlugIn075), "Handler for area skill hit packets.")]
    [Guid("D08CA02F-C413-4527-B79C-87F3C4641B60")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    internal class AreaSkillHitHandlerPlugIn075 : IPacketHandlerPlugIn
    {
        private readonly AreaSkillHitAction skillHitAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => AreaSkillHit075.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 7)
            {
                return;
            }

            AreaSkillHit075 message = packet;
            if (packet.Length < AreaSkillHit075.GetRequiredSize(message.TargetCount)
                || player.SkillList is null
                || player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillIndex) is not { } skill
                || player.SkillList.GetSkill((ushort)skill.Number) is not { } skillEntry)
            {
                return;
            }

            void AttackTarget(ushort targetId)
            {
                if (player.GetObject(targetId) is IAttackable target)
                {
                    if (target is IObservable observable
                        && observable.Observers.Contains(player))
                    {
                        this.skillHitAction.AttackTarget(player, target, skillEntry);
                    }
                    else
                    {
                        // Client may be out of sync (or it's an hacker attempt),
                        // so we tell him the object is out of scope - this should prevent further attempts to attack it.
                        player.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()?.ObjectsOutOfScope(target.GetAsEnumerable());
                    }
                }
            }

            for (var i = 0; i < message.TargetCount; i++)
            {
                AttackTarget(message[i].TargetId);
            }
        }
    }
}