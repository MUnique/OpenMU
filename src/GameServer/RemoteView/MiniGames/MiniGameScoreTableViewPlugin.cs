// <copyright file="MiniGameScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IMiniGameScoreTableViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(MiniGameScoreTableViewPlugin), "The default implementation of the IMiniGameScoreTableViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("A4AC1264-7410-4578-A318-E57F3B450DC5")]
public class MiniGameScoreTableViewPlugin : IMiniGameScoreTableViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameScoreTableViewPlugin"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MiniGameScoreTableViewPlugin(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowScoreTableAsync(byte playerRank, IReadOnlyCollection<(string Player, int Score, int BonusExp, int BonusMoney)> scores)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        const int maxScores = 10;
        int Write()
        {
            var size = MiniGameScoreTableRef.GetRequiredSize(Math.Min(maxScores, scores.Count));
            var span = connection.Output.GetSpan(size)[..size];
            var message = new MiniGameScoreTableRef(span)
            {
                PlayerRank = playerRank,
                ResultCount = (byte)scores.Count,
            };
            var i = 0;
            foreach (var (playerName, totalScore, bonusExp, bonusMoney) in scores.Take(maxScores))
            {
                var item = message[i];
                item.PlayerName = playerName;
                item.TotalScore = (uint)totalScore;
                item.BonusExperience = (uint)bonusExp;
                item.BonusMoney = (uint)bonusMoney;
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}