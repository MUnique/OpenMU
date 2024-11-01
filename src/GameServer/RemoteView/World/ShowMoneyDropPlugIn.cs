// <copyright file="ShowMoneyDropPlugIn.cs" company="MUnique">
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
[PlugIn(nameof(ShowMoneyDropPlugIn), "The default implementation of the IShowMoneyDropPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("f89308c3-5fe7-46e2-adfc-85a56ba23232")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class ShowMoneyDropPlugIn : IShowMoneyDropPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMoneyDropPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMoneyDropPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMoneyAsync(ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        return this._player.Connection.SendMoneyDroppedAsync(itemId, isFreshDrop, point.X, point.Y, amount);
    }
}