// <copyright file="IllusionTempleScoreTableViewPlugIn.cs" company="MUnique">
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
/// The default implementation of the <see cref="IIllusionTempleScoreTableViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("F47F9EC1-0030-49AB-BCF2-986AA6AFA8C6")]
public class IllusionTempleScoreTableViewPlugIn : IIllusionTempleScoreTableViewPlugIn
{
    private const string PlugInName = "Illusion Temple Score Table";

    private const string PlugInDescription = "View plugin which sends the result of a finished illusion temple event to the client, so that it can show the score board.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleScoreTableViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleScoreTableViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowScoreTableAsync(byte alliedForcesPoints, byte illusionForcesPoints, IReadOnlyCollection<(string Name, byte MapNumber, IllusionTempleTeam Team, byte CharacterClass, int AddedExperience)> results)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = IllusionTempleResultRef.GetRequiredSize(results.Count);
            var span = connection.Output.GetSpan(size)[..size];

            // The pipe buffer is reused and isn't zeroed, while each player entry contains three
            // alignment bytes which are never written. Without clearing, leftovers of previous packets
            // would go out on the wire and the client would read them as part of the entry.
            span.Clear();

            var message = new IllusionTempleResultRef(span)
            {
                Team1Points = alliedForcesPoints,
                Team2Points = illusionForcesPoints,
                PlayerCount = (byte)results.Count,
            };

            var i = 0;
            foreach (var (name, mapNumber, team, characterClass, addedExperience) in results)
            {
                var entry = message[i];
                entry.Name = name;
                entry.MapNumber = mapNumber;
                entry.Team = (byte)team;
                entry.Class = characterClass.ToIllusionTempleCharacterClass();
                entry.AddedExperience = (uint)Math.Max(0, addedExperience);
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
