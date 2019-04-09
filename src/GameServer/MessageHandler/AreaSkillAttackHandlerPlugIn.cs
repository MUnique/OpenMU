// <copyright file="AreaSkillAttackHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;
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
        public byte Key => (byte)PacketType.AreaSkill;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ushort skillId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            if (!player.SkillList.ContainsSkill(skillId))
            {
                return;
            }

            ushort targetId = NumberConversionExtensions.MakeWord(packet[10], packet[9]);
            byte tX = packet[5];
            byte tY = packet[6];
            byte rotation = packet[7];
            this.attackAction.Attack(player, targetId, skillId, new Point(tX, tY), rotation);
        }
    }
}
