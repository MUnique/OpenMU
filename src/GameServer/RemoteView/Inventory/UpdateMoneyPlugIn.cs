// <copyright file="UpdateMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateMoneyPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateMoneyPlugIn", "The default implementation of the IUpdateMoneyPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("7a13a613-7098-4407-8ef5-39bae08ce12d")]
public class UpdateMoneyPlugIn : IUpdateMoneyPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMoneyPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMoneyPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateMoneyAsync()
    {
        await this._player.Connection.SendInventoryMoneyUpdateAsync((uint)this._player.Money).ConfigureAwait(false);
    }
}