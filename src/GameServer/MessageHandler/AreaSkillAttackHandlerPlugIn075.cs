// <copyright file="AreaSkillAttackHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for area skill attack packets.
    /// </summary>
    [PlugIn(nameof(AreaSkillAttackHandlerPlugIn075), "Handler for area skill attack packets for version 0.75")]
    [Guid("A03821B8-2A8E-4A1D-A45D-C92C7621DC8A")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    internal class AreaSkillAttackHandlerPlugIn075 : IPacketHandlerPlugIn
    {
        private readonly AreaSkillAttackAction attackAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => AreaSkill075.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            AreaSkill075 message = packet;
            var skill = player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillIndex);
            if (skill is null)
            {
                return;
            }

            this.attackAction.Attack(player, 0, (ushort)skill.Number, new Point(message.TargetX, message.TargetY), message.Rotation);
        }
    }
}