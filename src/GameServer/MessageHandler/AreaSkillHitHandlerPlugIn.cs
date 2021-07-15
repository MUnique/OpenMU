// <copyright file="AreaSkillHitHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for area skill hit packets.
    /// </summary>
    /// <remarks>
    /// TODO: It's usually required to perform a <see cref="AreaSkillAttackAction"/> before, so this check has to be implemented.
    ///       Each animation and hit is usually referenced due a counter value in the packets.
    /// </remarks>
    [PlugIn("AreaSkillHitHandlerPlugIn", "Handler for area skill hit packets.")]
    [Guid("2f5848fd-a1bd-488b-84b3-fd88bdef5ac8")]
    internal class AreaSkillHitHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly AreaSkillHitAction skillHitAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => AreaSkillHit.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 11)
            {
                return;
            }

            AreaSkillHit message = packet;
            if (player.SkillList is null || !player.SkillList.ContainsSkill(message.SkillId))
            {
                return;
            }

            if (player.GetObject(message.TargetId) is IAttackable target)
            {
                if (target is IObservable observable && observable.Observers.Contains(player))
                {
                    if (player.SkillList.GetSkill(message.SkillId) is { } skillEntry)
                    {
                        this.skillHitAction.AttackTarget(player, target, skillEntry);
                    }
                }
                else
                {
                    // Client may be out of sync (or it's an hacker attempt),
                    // so we tell him the object is out of scope - this should prevent further attempts to attack it.
                    player.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()?.ObjectsOutOfScope(target.GetAsEnumerable());
                }
            }
        }
    }
}
