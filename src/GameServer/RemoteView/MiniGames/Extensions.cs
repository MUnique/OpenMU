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

    /// <summary>
    /// Converts the <see cref="EnterResult"/> to the result value of the <see cref="IllusionTempleEnterResult"/>.
    /// </summary>
    /// <param name="enterResult">The enter result.</param>
    /// <returns>The converted result.</returns>
    /// <remarks>
    /// Unlike the other mini games, the illusion temple result is an undocumented plain byte, so there is no
    /// generated enum to map to. Only the success value (0) is known for sure, because it's consistent over
    /// all other mini games; every failure is reported as 1 until the client's distinct failure codes are known.
    /// </remarks>
    public static byte ToIllusionTempleEnterResult(this EnterResult enterResult)
    {
        return enterResult == EnterResult.Success ? (byte)0 : (byte)1;
    }

    /// <summary>
    /// Converts the internal character class number into the number the game client expects in the
    /// illusion temple result packet, so the score board shows the right class next to each player.
    /// </summary>
    /// <param name="characterClassNumber">The internal character class number.</param>
    /// <returns>The class number as the client knows it.</returns>
    /// <remarks>
    /// The client reads the class line from the lower nibble (0 Dark Wizard, 1 Dark Knight, 2 Fairy
    /// Elf, 3 Magic Gladiator, 4 Dark Lord, 5 Summoner, 6 Rage Fighter) and the evolution step from
    /// the upper one (0 base class, 2 second class, 3 master class), while the internal numbering
    /// packs both the other way around. This was confirmed on a live client: two players of different
    /// lines were shown as the same class, because the values sent at the time happened to share their
    /// lower nibble.
    /// </remarks>
    public static byte ToIllusionTempleCharacterClass(this byte characterClassNumber)
    {
        // The internal number is line * 4 + evolution step.
        var line = (byte)(characterClassNumber / 4);
        var evolution = (byte)(characterClassNumber % 4);
        return (byte)((evolution << 4) | (line & 0x0F));
    }
}