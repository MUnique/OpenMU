﻿// <copyright file="ItemDropResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IItemDropResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ItemDropResultPlugIn", "The default implementation of the IItemDropResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("377cd4cb-7334-4c74-a165-058e6bb46baf")]
public class ItemDropResultPlugIn : IItemDropResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemDropResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemDropResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void ItemDropResult(byte slot, bool success)
    {
        this._player.Connection?.SendItemDropResponse(success, slot);
    }
}