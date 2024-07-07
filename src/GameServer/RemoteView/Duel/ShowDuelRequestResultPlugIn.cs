// <copyright file="ShowDuelRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDuelRequestResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowDuelRequestResultPlugIn), "The default implementation of the IShowDuelRequestResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("3A389377-71BC-4EEB-922E-00B343DF1893")]
public class ShowDuelRequestResultPlugIn : IShowDuelRequestResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelRequestResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelRequestResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowDuelRequestResultAsync(GameLogic.DuelStartResult result, Player? opponent)
    {
        await this._player.Connection.SendDuelStartResultAsync(Convert(result), opponent?.GetId(this._player) ?? 0, opponent?.Name ?? string.Empty).ConfigureAwait(false);
    }

    private static Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType Convert(GameLogic.DuelStartResult result)
    {
        return result switch
        {
            GameLogic.DuelStartResult.Success => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.Success,
            GameLogic.DuelStartResult.Refused => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.Refused,
            GameLogic.DuelStartResult.FailedByError => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.FailedByError,
            GameLogic.DuelStartResult.FailedByNoFreeRoom => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.FailedByNoFreeRoom,
            GameLogic.DuelStartResult.FailedByNotEnoughMoney => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.FailedByNotEnoughMoney,
            GameLogic.DuelStartResult.FailedByTooLowLevel => Network.Packets.ServerToClient.DuelStartResult.DuelStartResultType.FailedByTooLowLevel,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }
}