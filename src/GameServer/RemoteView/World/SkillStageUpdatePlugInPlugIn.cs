// <copyright file="SkillStageUpdatePlugInPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowSkillStageUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(SkillStageUpdatePlugInPlugIn), "The default implementation of the IShowSkillStageUpdate which is forwarding everything to the game client with specific data packets.")]
[Guid("9EE927BA-E82B-46DC-872B-F8B5F646A4A5")]
public class SkillStageUpdatePlugInPlugIn : IShowSkillStageUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillStageUpdatePlugInPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public SkillStageUpdatePlugInPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateSkillStageAsync(IAttacker attacker, short skillNumber, byte stageNumber)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var attackerId = attacker.GetId(this._player);
        await connection.SendSkillStageUpdateAsync(attackerId, stageNumber, (byte)skillNumber).ConfigureAwait(false);
    }
}