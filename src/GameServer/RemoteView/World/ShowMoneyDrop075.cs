// <copyright file="ShowMoneyDrop075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDroppedItemsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowMoneyDrop 0.75", "The default implementation of the IShowMoneyDropPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("2C00F283-3229-48A8-A974-3DE0C543DC17")]
[MaximumClient(0, 89, ClientLanguage.Invariant)]
public class ShowMoneyDrop075 : IShowMoneyDropPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMoneyDrop075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMoneyDrop075(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMoneyAsync(ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        return this._player.Connection.SendMoneyDropped075Async(itemId, isFreshDrop, point.X, point.Y, amount);
    }
}