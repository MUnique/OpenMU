// <copyright file="AreaSkillAttackHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for area skill attack packets.
    /// </summary>
    [PlugIn("AreaSkillAttackHandlerPlugIn", "Handler for area skill attack packets.")]
    [Guid("9681bed4-70e8-4017-b27c-5eee164dd7b0")]
    internal class AreaSkillAttackHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly AreaSkillAttackAction attackAction = new AreaSkillAttackAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => AreaSkill.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            AreaSkill message = packet;
            if (!player.SkillList.ContainsSkill(message.SkillId))
            {
                return;
            }

            this.attackAction.Attack(player, message.ExtraTargetId, message.SkillId, new Point(message.TargetX, message.TargetY), message.Rotation);
        }
    }
}
