// <copyright file="BloodCastleScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IBloodCastleScoreTableViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(BloodCastleScoreTableViewPlugin), "The default implementation of the IBloodCastleScoreTableViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("C7F02F66-987A-42EC-A994-E5F1E8606900")]
public class BloodCastleScoreTableViewPlugin : IBloodCastleScoreTableViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleScoreTableViewPlugin"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public BloodCastleScoreTableViewPlugin(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowScoreTableAsync(bool success, string playerName, int totalScore, int bonusExp, int bonusMoney)
    {
        await this._player.Connection.SendBloodCastleScoreAsync(success, playerName, (uint)totalScore, (uint)bonusExp, (uint)bonusMoney).ConfigureAwait(false);
    }
}