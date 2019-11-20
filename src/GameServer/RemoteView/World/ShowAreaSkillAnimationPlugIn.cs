// <copyright file="ShowAreaSkillAnimationPlugIn.cs" company="MUnique">
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
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowAreaSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowAreaSkillAnimationPlugIn", "The default implementation of the IShowAreaSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("4cc09cdd-55a3-4191-94fc-b8e684b87cac")]
    public class ShowAreaSkillAnimationPlugIn : IShowAreaSkillAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAreaSkillAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowAreaSkillAnimationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player playerWhichPerformsSkill, Skill skill, Point point, byte rotation)
        {
            var skillId = NumberConversionExtensions.ToUnsigned(skill.Number);
            var playerId = playerWhichPerformsSkill.GetId(this.player);
            using var writer = this.player.Connection.StartSafeWrite(
                AreaSkillAnimation.HeaderType,
                AreaSkillAnimation.Length);
            _ = new AreaSkillAnimation(writer.Span)
            {
                // Example: C3 0A 1E 00 09 23 47 3D 62 3A
                SkillId = skillId,
                PlayerId = playerId,
                PointX = point.X,
                PointY = point.Y,
                Rotation = rotation,
            };
            writer.Commit();
        }
    }
}