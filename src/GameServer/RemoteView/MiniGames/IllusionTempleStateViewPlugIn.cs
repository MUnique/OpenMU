// <copyright file="IllusionTempleStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleStateViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("0FD01C6D-AEAB-4F06-8CFC-AC0E89C7B526")]
public class IllusionTempleStateViewPlugIn : IIllusionTempleStateViewPlugin
{
    private const string PlugInName = "Illusion Temple State";

    private const string PlugInDescription = "View plugin which sends the cyclic state update of a running illusion temple event to the client - the remaining time, the points of both teams and the positions of the own team.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleStateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync(
        TimeSpan remainingTime,
        byte alliedForcesPoints,
        byte illusionForcesPoints,
        IllusionTempleTeam ownTeam,
        IReadOnlyCollection<(ushort PlayerId, byte MapNumber, byte PositionX, byte PositionY)> teamMembers,
        (ushort PlayerId, byte PositionX, byte PositionY)? relicCarrier)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var seconds = (ushort)Math.Clamp(remainingTime.TotalSeconds, 0, ushort.MaxValue);

        int Write()
        {
            // Unlike other list packets, this one has no fixed-length array with unused slots - only
            // PartyCount entries are sent.
            var size = IllusionTempleStateRef.GetRequiredSize(teamMembers.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var message = new IllusionTempleStateRef(span)
            {
                RemainingSeconds = seconds,
                AlliedForcesPoints = alliedForcesPoints,
                IllusionForcesPoints = illusionForcesPoints,
                MyTeam = (byte)ownTeam,
                PartyCount = (byte)teamMembers.Count,
            };

            // The holy relic's carrier is identified by his id and current position. As long as nobody
            // carries it, both have to be filled with -1 resp. 0xFF: a real value makes the client
            // announce a carrier, which it does with the score animation.
            if (relicCarrier is { } carrier)
            {
                message.RelicCarrierId = carrier.PlayerId;
                message.PositionX = carrier.PositionX;
                message.PositionY = carrier.PositionY;
            }
            else
            {
                message.RelicCarrierId = 0xFFFF;
                message.PositionX = 0xFF;
                message.PositionY = 0xFF;
            }

            var i = 0;
            foreach (var (playerId, mapNumber, positionX, positionY) in teamMembers)
            {
                var entry = message[i];
                entry.PlayerId = playerId;
                entry.MapNumber = mapNumber;
                entry.PositionX = positionX;
                entry.PositionY = positionY;
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
