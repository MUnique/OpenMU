// <copyright file="AreaSkillAttackHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Handler for area skill attack packets.
    /// </summary>
    internal class AreaSkillAttackHandler : BasePacketHandler, IPacketHandler
    {
        private readonly AreaSkillAttackAction attackAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSkillAttackHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AreaSkillAttackHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.attackAction = new AreaSkillAttackAction(gameContext);
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
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
