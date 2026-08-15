// <copyright file="CastleSiegeRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeRegistrationResultPlugIn"/>
/// which forwards registration results to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeRegistrationResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeRegistrationResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("1FF48BAE-9F33-4316-B4C0-D6082653C383")]
public class CastleSiegeRegistrationResultPlugIn : ICastleSiegeRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowRegistrationResultAsync(CastleSiegeRegistrationResult result, string guildName)
        => this._player.Connection.SendCastleSiegeRegistrationResponseAsync((byte)result, guildName);

    /// <inheritdoc />
    public ValueTask ShowUnregistrationResultAsync(
        CastleSiegeUnregistrationResult result,
        bool isGivingUp,
        string guildName)
        => this._player.Connection.SendCastleSiegeUnregisterResponseAsync(
            (byte)result,
            isGivingUp,
            guildName);
}
