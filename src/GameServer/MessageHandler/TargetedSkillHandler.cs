// <copyright file="TargetedSkillHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for targeted skill packets.
    /// </summary>
    internal class TargetedSkillHandler : BasePacketHandler
    {
        private readonly TargetedSkillAction attackAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedSkillHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public TargetedSkillHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.attackAction = new TargetedSkillAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
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
