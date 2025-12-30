// <copyright file="RespawnAfterDeathPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Respawn plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(RespawnAfterDeathPlugIn097), "Respawn plugin for 0.97 clients.")]
[Guid("780DF9D2-4E5B-4B3F-B4C2-31B36F60C2F4")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class RespawnAfterDeathPlugIn097 : IRespawnAfterDeathPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RespawnAfterDeathPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RespawnAfterDeathPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RespawnAsync()
    {
        await Version097CompatibilityProfile.SendRespawnAsync(this._player).ConfigureAwait(false);
    }
}
