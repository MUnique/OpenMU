// <copyright file="IllusionTempleHolyItemRelicsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleHolyItemRelicsViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9C6C0B0B-6E6B-4B6E-9C3A-6A7EDAF07B1E")]
public class IllusionTempleHolyItemRelicsViewPlugIn : IIllusionTempleHolyItemRelicsViewPlugIn
{
    private const string PlugInName = "Illusion Temple Holy Item Relics";

    private const string PlugInDescription = "View plugin which announces the player who just picked up the holy relic of a running illusion temple event.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleHolyItemRelicsViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleHolyItemRelicsViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowHolyItemRelicsAsync(ushort playerId, string playerName)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = IllusionTempleHolyItemRelicsRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var message = new IllusionTempleHolyItemRelicsRef(span)
            {
                UserIndex = playerId,
                Name = playerName,
            };

            return message.Header.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
