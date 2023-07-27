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
            _ => DevilSquareEnterResult.EnterResult.Failed,
        };
    }

    /// <summary>
    /// Converts the <see cref="EnterResult"/> to the corresponding <see cref="BloodCastleEnterResult.EnterResult"/>.
    /// </summary>
    /// <param name="enterResult">The enter result.</param>
    /// <returns>The converted result.</returns>
    public static BloodCastleEnterResult.EnterResult ToBloodCastleEnterResult(this EnterResult enterResult)
    {
        return enterResult switch
        {
            EnterResult.Success => BloodCastleEnterResult.EnterResult.Success,
            EnterResult.Failed => BloodCastleEnterResult.EnterResult.Failed,
            EnterResult.NotOpen => BloodCastleEnterResult.EnterResult.NotOpen,
            EnterResult.Full => BloodCastleEnterResult.EnterResult.Full,
            EnterResult.CharacterLevelTooLow => BloodCastleEnterResult.EnterResult.CharacterLevelTooLow,
            EnterResult.CharacterLevelTooHigh => BloodCastleEnterResult.EnterResult.CharacterLevelTooHigh,
            _ => BloodCastleEnterResult.EnterResult.Failed,
        };
    }

    /// <summary>
    /// Converts the <see cref="EnterResult"/> to the corresponding <see cref="BloodCastleEnterResult.EnterResult"/>.
    /// </summary>
    /// <param name="enterResult">The enter result.</param>
    /// <returns>The converted result.</returns>
    public static ChaosCastleEnterResult.EnterResult ToChaosCastleEnterResult(this EnterResult enterResult)
    {
        return enterResult switch
        {
            EnterResult.Success => ChaosCastleEnterResult.EnterResult.Success,
            EnterResult.Failed => ChaosCastleEnterResult.EnterResult.Failed,
            EnterResult.NotOpen => ChaosCastleEnterResult.EnterResult.NotOpen,
            EnterResult.Full => ChaosCastleEnterResult.EnterResult.Full,
            EnterResult.CharacterLevelTooLow => ChaosCastleEnterResult.EnterResult.Failed,
            EnterResult.CharacterLevelTooHigh => ChaosCastleEnterResult.EnterResult.Failed,
            EnterResult.NotEnoughMoney => ChaosCastleEnterResult.EnterResult.NotEnoughMoney,
            EnterResult.PlayerKillerCantEnter => ChaosCastleEnterResult.EnterResult.PlayerKillerCantEnter,
            _ => ChaosCastleEnterResult.EnterResult.Failed,
        };
    }
}