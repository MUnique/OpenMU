// <copyright file="Extensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Extension methods for various classes regarding views of mini games.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Converts the <see cref="EnterResult"/> to the corresponding <see cref="DevilSquareEnterResult.EnterResult"/>.
    /// </summary>
    /// <param name="enterResult">The enter result.</param>
    /// <returns>The converted result.</returns>
    public static DevilSquareEnterResult.EnterResult ToDevilSquareEnterResult(this EnterResult enterResult)
    {
        return enterResult switch
        {
            EnterResult.Success => DevilSquareEnterResult.EnterResult.Success,
            EnterResult.Failed => DevilSquareEnterResult.EnterResult.Failed,
            EnterResult.NotOpen => DevilSquareEnterResult.EnterResult.NotOpen,
            EnterResult.Full => DevilSquareEnterResult.EnterResult.Full,
            EnterResult.CharacterLevelTooLow => DevilSquareEnterResult.EnterResult.CharacterLevelTooLow,
            EnterResult.CharacterLevelTooHigh => DevilSquareEnterResult.EnterResult.CharacterLevelTooHigh,
            _ => DevilSquareEnterResult.EnterResult.Failed
        };
    }
}