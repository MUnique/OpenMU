// <copyright file="IllusionTempleSkillEndedViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleSkillEndedViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("7A1B2C3D-4E5F-4A6B-8C9D-0E1F2A3B4C5D")]
public class IllusionTempleSkillEndedViewPlugIn : IIllusionTempleSkillEndedViewPlugin
{
    private const string PlugInName = "Illusion Temple Skill Ended";

    private const string PlugInDescription = "View plugin which announces that an illusion temple special skill's effect ended on an object.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleSkillEndedViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleSkillEndedViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowSkillEndedAsync(ushort skillNumber, ushort objectId)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = IllusionTempleSkillEndedRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var message = new IllusionTempleSkillEndedRef(span)
            {
                SkillNumber = skillNumber,
                ObjectIndex = objectId,
            };

            return message.Header.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
