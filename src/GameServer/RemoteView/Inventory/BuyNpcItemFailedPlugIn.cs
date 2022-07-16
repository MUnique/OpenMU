// <copyright file="BuyNpcItemFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IBuyNpcItemFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("BuyNpcItemFailedPlugIn", "The default implementation of the IBuyNpcItemFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("915324d5-ccdf-42c0-b7c9-9479969346d8")]
public class BuyNpcItemFailedPlugIn : IBuyNpcItemFailedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyNpcItemFailedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public BuyNpcItemFailedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask BuyNpcItemFailedAsync()
    {
        return this._player.Connection.SendNpcItemBuyFailedAsync();
    }
}