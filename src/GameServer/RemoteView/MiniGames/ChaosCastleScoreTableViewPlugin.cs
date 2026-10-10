// <copyright file="ChaosCastleScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IChaosCastleScoreTableViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ChaosCastleScoreTableViewPlugin_Name), Description = nameof(PlugInResources.ChaosCastleScoreTableViewPlugin_Description), ResourceType = typeof(PlugInResources))]
[Guid("7F45B9E3-F0C0-40DD-B6A9-7B8D7C9A48C0")]
public class ChaosCastleScoreTableViewPlugin : IChaosCastleScoreTableViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleScoreTableViewPlugin"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChaosCastleScoreTableViewPlugin(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowScoreTableAsync(bool success, string playerName, int monsterKillCount, int playerKillCount, int bonusExp)
    {
        await this._player.Connection.SendChaosCastleScoreAsync(success, playerName, (uint)monsterKillCount, (uint)bonusExp, (uint)playerKillCount).ConfigureAwait(false);
    }
}
