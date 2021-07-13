// <copyright file="ShowSkillAnimationPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ShowSkillAnimationPlugIn075), "The default implementation of the IShowSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("8DED7CDF-AB3E-4CCB-A817-604560120320")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class ShowSkillAnimationPlugIn075 : IShowSkillAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowSkillAnimationPlugIn075(RemotePlayer player) => this.player = player;

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
            var skillId = (byte)skillNumber;

            this.player.Connection?.SendSkillAnimation075(skillId, playerId, targetId, effectApplied);
        }
    }
}