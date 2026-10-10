// <copyright file="ShowDuelScoreUpdatePlugInSeason3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IShowDuelScoreUpdatePlugIn"/> for the clients before Season 4,
/// which get the score of the duel with its own packet code.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowDuelScoreUpdatePlugInSeason3_Name), Description = nameof(PlugInResources.ShowDuelScoreUpdatePlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("CC502CFF-E6DB-4E0B-89FA-D4FB863AA517")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
public class ShowDuelScoreUpdatePlugInSeason3 : IShowDuelScoreUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelScoreUpdatePlugInSeason3"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelScoreUpdatePlugInSeason3(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateScoreAsync(DuelRoom duelRoom)
    {
        if (this._player == duelRoom.Opponent)
        {
            await this._player.Connection.SendDuelScoreSeason3Async(duelRoom.Opponent.GetId(this._player), duelRoom.Requester.GetId(this._player), duelRoom.ScoreOpponent, duelRoom.ScoreRequester).ConfigureAwait(false);
        }
        else
        {
            await this._player.Connection.SendDuelScoreSeason3Async(duelRoom.Requester.GetId(this._player), duelRoom.Opponent.GetId(this._player), duelRoom.ScoreRequester, duelRoom.ScoreOpponent).ConfigureAwait(false);
        }
    }
}
