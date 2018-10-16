// <copyright file="AreaSkillHitHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for area skill hit packets.
    /// </summary>
    internal class AreaSkillHitHandler : BasePacketHandler
    {
        private readonly AreaSkillHitAction skillHitAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSkillHitHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AreaSkillHitHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.skillHitAction = new AreaSkillHitAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 11)
            {
                return;
            }

            ushort skillId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            if (!player.SkillList.ContainsSkill(skillId))
            {
                return;
            }

            SkillEntry skillEntry = player.SkillList.GetSkill(skillId);
            ushort targetId = NumberConversionExtensions.MakeWord(packet[10], packet[9]);
            if (player.GetObject(targetId) is IAttackable target)
            {
                this.skillHitAction.AttackTarget(player, target, skillEntry);
            }
        }
    }
}
