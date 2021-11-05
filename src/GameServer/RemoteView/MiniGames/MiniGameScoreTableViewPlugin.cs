// <copyright file="MiniGameScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniGameScoreTableViewPlugin"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MiniGameScoreTableViewPlugin(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ShowScoreTable(byte playerRank, IReadOnlyCollection<(string Player, int Score, int BonusExp, int BonusMoney)> scores)
        {
            if (this.player.Connection is null)
            {
                return;
            }

            const int maxScores = 10;
            using var writer = this.player.Connection.StartSafeWrite(0xC1, MiniGameScoreTable.GetRequiredSize(Math.Min(maxScores, scores.Count)));
            var message = new MiniGameScoreTable(writer.Span);
            message.PlayerRank = playerRank;
            message.ResultCount = (byte)scores.Count;
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

            writer.Commit();
        }
    }
}