// <copyright file="AreaSkillAttackHandlerPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for area skill attack packets.
    /// </summary>
    [PlugIn(nameof(AreaSkillAttackHandlerPlugIn095), "Handler for area skill attack packets for version 0.95")]
    [Guid("2C293304-713C-4D43-9C24-6A308DD9686C")]
    [MinimumClient(0, 95, ClientLanguage.Invariant)]
    internal class AreaSkillAttackHandlerPlugIn095 : IPacketHandlerPlugIn
    {
        private readonly AreaSkillAttackAction attackAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => AreaSkill095.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            AreaSkill095 message = packet;
            var skill = player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillIndex);
            if (skill is null)
            {
                return;
            }

            this.attackAction.Attack(player, 0, (ushort)skill.Number, new Point(message.TargetX, message.TargetY), message.Rotation);
        }
    }
}