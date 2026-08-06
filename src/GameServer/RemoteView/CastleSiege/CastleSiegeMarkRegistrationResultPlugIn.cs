// <copyright file="CastleSiegeMarkRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeMarkRegistrationResultPlugIn"/>
/// which forwards mark registration results to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMarkRegistrationResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMarkRegistrationResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("12ED701D-B865-4D2B-B044-445E662BFE0E")]
public class CastleSiegeMarkRegistrationResultPlugIn : ICastleSiegeMarkRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMarkRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeMarkRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowMarkRegistrationResultAsync(CastleSiegeMarkRegistrationResult result, string guildName, int marks)
        => this._player.Connection.SendCastleSiegeMarkRegistrationResponseAsync(
            (byte)result,
            guildName,
            checked((uint)marks));
}
