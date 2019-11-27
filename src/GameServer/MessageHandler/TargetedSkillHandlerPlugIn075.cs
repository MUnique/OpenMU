// <copyright file="TargetedSkillHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
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
        /// <inheritdoc />
        public override bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        /// <remarks>
        /// In early versions, the index of the skill is used as identifier. Later it was the skill id.
        /// This may have changed earlier than season 2!.
        /// </remarks>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            TargetedSkill075 message = packet;
            var skill = player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillId);
            if (skill == null)
            {
                return;
            }

            this.Handle(player, (ushort)skill.Number, message.TargetId);
        }
    }
}