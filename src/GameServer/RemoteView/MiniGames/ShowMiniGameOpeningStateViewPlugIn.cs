// <copyright file="ShowMiniGameOpeningStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowMiniGameOpeningStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowMiniGameOpeningStateViewPlugIn), "The default implementation of the IShowMiniGameOpeningStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B4D89EFA-0593-4CC7-B720-CD1FC2C0513A")]
public class ShowMiniGameOpeningStateViewPlugIn : IShowMiniGameOpeningStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMiniGameOpeningStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMiniGameOpeningStateViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowOpeningStateAsync(MiniGameType miniGameType, TimeSpan? timeUntilOpening, int playerCount)
    {
        var minutesUntilNextStart = 0xFFFF;
        if (timeUntilOpening.HasValue)
        {
            minutesUntilNextStart = (int)double.Round(timeUntilOpening.Value.TotalMinutes);
            if (timeUntilOpening.Value.Seconds > 0)
            {
                // Rounding up...
                minutesUntilNextStart++;
            }
        }

        if (miniGameType == MiniGameType.ChaosCastle)
        {
            await this._player.Connection.SendMiniGameOpeningStateAsync(
               Convert(miniGameType),
               (byte)(minutesUntilNextStart / 0x100),
               (byte)playerCount,
               (byte)(minutesUntilNextStart % 0x100))
           .ConfigureAwait(false);
        }
        else
        {
            await this._player.Connection.SendMiniGameOpeningStateAsync(
               Convert(miniGameType),
               (byte)(minutesUntilNextStart % 0x100),
               (byte)playerCount,
               0)
           .ConfigureAwait(false);
        }
    }

    private static Network.Packets.MiniGameType Convert(MiniGameType miniGameType)
    {
        return miniGameType switch
        {
            MiniGameType.ChaosCastle => Network.Packets.MiniGameType.ChaosCastle,
            MiniGameType.BloodCastle => Network.Packets.MiniGameType.BloodCastle,
            MiniGameType.DevilSquare => Network.Packets.MiniGameType.DevilSquare,
            MiniGameType.IllusionTemple => Network.Packets.MiniGameType.IllusionTemple,
            MiniGameType.Doppelganger => Network.Packets.MiniGameType.Doppelganger,
            _ => throw new ArgumentOutOfRangeException($"Unknown mini game value {miniGameType}", nameof(miniGameType)),
        };
    }
}