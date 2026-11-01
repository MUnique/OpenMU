// <copyright file="IllusionTempleSkillPointUpdateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleSkillPointUpdateViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("6F6E7B84-8C3B-4B6F-9C0A-3B7C6E5C6D2A")]
public class IllusionTempleSkillPointUpdateViewPlugIn : IIllusionTempleSkillPointUpdateViewPlugin
{
    private const string PlugInName = "Illusion Temple Skill Point Update";

    private const string PlugInDescription = "View plugin which updates the skill point balance of a player taking part in a running illusion temple event.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleSkillPointUpdateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleSkillPointUpdateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateSkillPointsAsync(byte skillPoints)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = IllusionTempleSkillPointUpdateRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var message = new IllusionTempleSkillPointUpdateRef(span)
            {
                SkillPoints = skillPoints,
            };

            return message.Header.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
