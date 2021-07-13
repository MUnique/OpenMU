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
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ShowSkillAnimationPlugIn), "The default implementation of the IShowSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("a25cc420-c848-4a87-81e5-b86c4241af35")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
    public class ShowSkillAnimationPlugIn : IShowSkillAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowSkillAnimationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowSkillAnimation(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
        {
            this.ShowSkillAnimation(attacker, target, skill.Number, effectApplied);
        }

        /// <inheritdoc/>
        public void ShowSkillAnimation(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
        {
            var playerId = attacker.GetId(this.player);
            var targetId = target.GetId(this.player);
            var skillId = NumberConversionExtensions.ToUnsigned(skillNumber);
            this.player.Connection?.SendSkillAnimation(skillId, playerId, targetId);
        }
    }
}