// <copyright file="TargetedSkillHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for targeted skill packets.
    /// </summary>
    [PlugIn("TargetedSkillHandlerPlugIn", "Handler for targeted skill packets.")]
    [Guid("5b07d03c-509c-4aec-972c-a99db77561f2")]
    internal class TargetedSkillHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TargetedSkillAction attackAction = new TargetedSkillAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.SkillAttack;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ////          skill target
            ////C3 len 19 XX XX TT TT
            ushort skillId = packet.MakeWordSmallEndian(3);

            if (!player.SkillList.ContainsSkill(skillId))
            {
                return;
            }

            // The target can be the own player too, for example when using buff skills.
            ushort targetId = packet.MakeWordSmallEndian(5);
            if (player.GetObject(targetId) is IAttackable target)
            {
                this.attackAction.PerformSkill(player, target, skillId);
            }
        }
    }
}
