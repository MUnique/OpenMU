// <copyright file="UpdateCurrentHealthExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IUpdateCurrentHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateCurrentHealthExtendedPlugIn), "The extended implementation of the IUpdateCurrentHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("C609BC7E-170B-4C79-94E5-D97AB9A3CB4B")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateCurrentHealthExtendedPlugIn : IUpdateCurrentHealthPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCurrentHealthPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCurrentHealthExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCurrentHealthAsync()
    {
        if (this._player.Attributes is null)
        {
            return;
        }

        await this._player.Connection.SendCurrentHealthAndShieldExtendedAsync(
            (uint)Math.Max(this._player.Attributes[Stats.CurrentHealth], 0f),
            (uint)Math.Max(this._player.Attributes[Stats.CurrentShield], 0f)).ConfigureAwait(false);
    }
}