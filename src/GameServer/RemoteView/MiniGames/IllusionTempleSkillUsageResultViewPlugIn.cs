// <copyright file="IllusionTempleSkillUsageResultViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleSkillUsageResultViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("2D6A6B0E-3F8B-4B1A-9A0D-8E7C6B5A4F31")]
public class IllusionTempleSkillUsageResultViewPlugIn : IIllusionTempleSkillUsageResultViewPlugin
{
    private const string PlugInName = "Illusion Temple Skill Usage Result";

    private const string PlugInDescription = "View plugin which shows the result of a requested illusion temple special skill.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleSkillUsageResultViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleSkillUsageResultViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowSkillUsageResultAsync(bool success, ushort skillNumber, ushort sourceId, ushort targetId)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = IllusionTempleSkillUsageResultRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var message = new IllusionTempleSkillUsageResultRef(span)
            {
                Result = (byte)(success ? 1 : 0),
                SkillNumber = skillNumber,
                SourceObjectId = sourceId,
                TargetObjectId = targetId,
            };

            return message.Header.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
