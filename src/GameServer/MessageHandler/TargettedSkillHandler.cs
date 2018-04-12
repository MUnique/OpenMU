// <copyright file="TargettedSkillHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for targetted skill packets.
    /// </summary>
    internal class TargettedSkillHandler : BasePacketHandler
    {
        private readonly TargettedSkillAction attackAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargettedSkillHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public TargettedSkillHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.attackAction = new TargettedSkillAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, byte[] packet)
        {
            ////          skill targt
            ////C3 len 19 XX XX TT TT
            ushort skillId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);

            if (!player.SkillList.ContainsSkill(skillId))
            {
                return;
            }

            // The target can be the own player too, for example when using buff skills.
            ushort targetId = NumberConversionExtensions.MakeWord(packet[6], packet[5]);
            if (player.GetObject(targetId) is IAttackable target)
            {
                this.attackAction.PerformSkill(player, target, skillId);
            }
        }
    }
}
