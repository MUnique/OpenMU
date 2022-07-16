// <copyright file="ShowAreaSkillAnimationPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAreaSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAreaSkillAnimationPlugIn095), "The default implementation of the IShowAreaSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("553D9388-3648-4029-959E-F6D74399D51E")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class ShowAreaSkillAnimationPlugIn095 : IShowAreaSkillAnimationPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAreaSkillAnimationPlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAreaSkillAnimationPlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowAreaSkillAnimationAsync(Player playerWhichPerformsSkill, Skill skill, Point point, byte rotation)
    {
        var skillId = (byte)skill.Number;
        var playerId = playerWhichPerformsSkill.GetId(this._player);
        await this._player.Connection.SendAreaSkillAnimation095Async(skillId, playerId, point.X, point.Y, rotation).ConfigureAwait(false);
    }
}