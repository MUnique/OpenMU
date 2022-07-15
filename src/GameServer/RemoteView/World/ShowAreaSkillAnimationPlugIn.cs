// <copyright file="ShowAreaSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAreaSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAreaSkillAnimationPlugIn), "The default implementation of the IShowAreaSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("4cc09cdd-55a3-4191-94fc-b8e684b87cac")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
public class ShowAreaSkillAnimationPlugIn : IShowAreaSkillAnimationPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAreaSkillAnimationPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAreaSkillAnimationPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowAreaSkillAnimationAsync(Player playerWhichPerformsSkill, Skill skill, Point point, byte rotation)
    {
        var skillId = NumberConversionExtensions.ToUnsigned(skill.Number);
        var playerId = playerWhichPerformsSkill.GetId(this._player);
        await this._player.Connection.SendAreaSkillAnimationAsync(skillId, playerId, point.X, point.Y, rotation).ConfigureAwait(false);
    }
}