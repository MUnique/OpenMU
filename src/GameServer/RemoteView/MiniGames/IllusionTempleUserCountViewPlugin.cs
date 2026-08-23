// <copyright file="IllusionTempleUserCountViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowIllusionTempleUserCountViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("E7E73888-8B8E-4D06-8D95-1C1CDEDDA8CC")]
public class IllusionTempleUserCountViewPlugIn : IShowIllusionTempleUserCountViewPlugIn
{
    private const string PlugInName = "Illusion Temple User Count";

    private const string PlugInDescription = "View plugin which sends the number of players of each illusion temple to the client, so that it can show them in the entrance dialog.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleUserCountViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleUserCountViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowUserCountAsync(IReadOnlyList<int> userCounts)
    {
        // The packet holds one byte per temple, so a missing or oversized count is reported as the
        // closest value the client can display, instead of throwing or wrapping around.
        byte Count(int index) => index < userCounts.Count
            ? (byte)Math.Clamp(userCounts[index], 0, byte.MaxValue)
            : (byte)0;

        await this._player.Connection.SendIllusionTempleUserCountAsync(
            Count(0),
            Count(1),
            Count(2),
            Count(3),
            Count(4),
            Count(5)).ConfigureAwait(false);
    }
}
