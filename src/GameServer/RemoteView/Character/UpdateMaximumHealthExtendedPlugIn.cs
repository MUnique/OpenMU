// <copyright file="UpdateMaximumHealthExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="IUpdateMaximumHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateMaximumHealthExtendedPlugIn), "The extended implementation of the IUpdateMaximumHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("7A287D6D-5C32-4FA9-9A0D-A4E0DEB053D1")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateMaximumHealthExtendedPlugIn : IUpdateMaximumHealthPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMaximumHealthPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMaximumHealthExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateMaximumHealthAsync()
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        await this._player.Connection.SendMaximumHealthAndShieldExtendedAsync(
            (uint)this._player.Attributes[Stats.MaximumHealth],
            (uint)this._player.Attributes[Stats.MaximumShield]).ConfigureAwait(false);
    }
}