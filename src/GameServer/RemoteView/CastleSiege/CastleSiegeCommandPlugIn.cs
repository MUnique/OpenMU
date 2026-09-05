// <copyright file="CastleSiegeCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using JoinSide = MUnique.OpenMU.DataModel.Configuration.CastleSiegeJoinSide;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeCommandPlugIn"/>
/// which forwards an alliance master's guild command to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeCommandPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("82E94F3B-3957-49D4-A785-82F0549EFC04")]
public class CastleSiegeCommandPlugIn : ICastleSiegeCommandPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeCommandPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeCommandPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowGuildCommandAsync(JoinSide side, byte positionX, byte positionY, CastleSiegeCommandType command)
    {
        var wireCommand = command switch
        {
            CastleSiegeCommandType.Attack => CastleSiegeGuildCommandType.Attack,
            CastleSiegeCommandType.Defend => CastleSiegeGuildCommandType.Defend,
            _ => CastleSiegeGuildCommandType.Wait,
        };

        return this._player.Connection?.SendCastleSiegeGuildCommandAsync((byte)side, positionX, positionY, wireCommand) ?? default;
    }
}
