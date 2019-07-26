// <copyright file="TargetedSkillHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for targeted skill packets until season 2.
    /// In these versions, the skill identifiers were only 1 byte big. After that, the master skill tree required more skills to be added than fit into it.
    /// </summary>
    [PlugIn("TargetedSkillHandlerPlugIn until Season 2", "Handler for targeted skill packets for version 0.75 until season 2.")]
    [Guid("83D2ABC0-6491-409A-8525-941794C54660")]
    [MaximumClient(2, 255, ClientLanguage.Invariant)]
    internal class TargetedSkillHandlerPlugIn075 : TargetedSkillHandlerPlugIn
    {
        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            byte skillId = packet[3];
            if (!player.SkillList.ContainsSkill(skillId))
            {
                return;
            }

            ushort targetId = packet.MakeWordSmallEndian(4);
            this.Handle(player, skillId, targetId);
        }
    }
}