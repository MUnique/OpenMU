// <copyright file="ShowSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowSkillAnimationPlugIn", "The default implementation of the IShowSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("a25cc420-c848-4a87-81e5-b86c4241af35")]
    public class ShowSkillAnimationPlugIn : IShowSkillAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowSkillAnimationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            var playerId = attackingPlayer.GetId(this.player);
            var targetId = target.GetId(this.player);
            var skillId = NumberConversionExtensions.ToUnsigned(skill.Number);
            using var writer = this.player.Connection.StartSafeWrite(
                SkillAnimation.HeaderType,
                SkillAnimation.Length);
            _ = new SkillAnimation(writer.Span)
            {
                SkillId = skillId,
                PlayerId = playerId,
                TargetId = targetId,
            };
            writer.Commit();
        }
    }
}