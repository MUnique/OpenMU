// <copyright file="UpdateMiniGameStateViewViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using MiniGameType = MUnique.OpenMU.DataModel.Configuration.MiniGameType;

/// <summary>
/// The default implementation of the <see cref="IUpdateMiniGameStateViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateMiniGameStateViewViewPlugIn), "The default implementation of the IUpdateMiniGameStateViewPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("FB4622A0-65CE-4D86-9A02-848A874F0AC1")]
public class UpdateMiniGameStateViewViewPlugIn : IUpdateMiniGameStateViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMiniGameStateViewViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMiniGameStateViewViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync(MiniGameType type, MiniGameState state)
    {
        if (TryGetState(type, state, out var miniGameState))
        {
            await this._player.Connection.SendUpdateMiniGameStateAsync(miniGameState).ConfigureAwait(false);
        }
    }

    private static bool TryGetState(MiniGameType type, MiniGameState state, out UpdateMiniGameState.MiniGameTypeState miniGameState)
    {
        switch (type, state)
        {
            case (MiniGameType.DevilSquare, MiniGameState.Closed):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.DevilSquareClosed;
                break;
            case (MiniGameType.DevilSquare, MiniGameState.Open):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.DevilSquareOpened;
                break;
            case (MiniGameType.DevilSquare, MiniGameState.Playing):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.DevilSquareRunning;
                break;
            case (MiniGameType.BloodCastle, MiniGameState.Closed):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.BloodCastleClosed;
                break;
            case (MiniGameType.BloodCastle, MiniGameState.Open):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.BloodCastleOpened;
                break;
            case (MiniGameType.BloodCastle, MiniGameState.Playing):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.BloodCastleEnding;
                break;
            case (MiniGameType.BloodCastle, MiniGameState.Ended):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.BloodCastleFinished;
                break;
            case (MiniGameType.ChaosCastle, MiniGameState.Closed):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.ChaosCastleClosed;
                break;
            case (MiniGameType.ChaosCastle, MiniGameState.Open):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.ChaosCastleOpened;
                break;
            case (MiniGameType.ChaosCastle, MiniGameState.Playing):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.ChaosCastleEnding;
                break;
            case (MiniGameType.ChaosCastle, MiniGameState.Ended):
                miniGameState = UpdateMiniGameState.MiniGameTypeState.ChaosCastleFinished;
                break;
            default:
                miniGameState = default;
                return false;
        }

        return true;
    }
}