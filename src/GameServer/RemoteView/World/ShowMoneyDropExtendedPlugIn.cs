// <copyright file="ShowMoneyDropExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="IShowDroppedItemsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowMoneyDropExtendedPlugIn), "The extended implementation of the IShowMoneyDropPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("292D399E-3F48-4AF0-9480-F83267BB8619")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ShowMoneyDropExtendedPlugIn : IShowMoneyDropPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMoneyDropExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMoneyDropExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMoneyAsync(ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        return this._player.Connection.SendMoneyDroppedExtendedAsync(isFreshDrop, itemId, point.X, point.Y, amount);
    }
}