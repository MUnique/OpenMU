// <copyright file="ShowMoneyDrop097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Money drop packet format for 0.97 clients (packed group/number byte).
/// </summary>
[PlugIn("ShowMoneyDrop 0.97", "Uses the 0.75-style money drop packet for 0.97 clients.")]
[Guid("5C6B0D46-4A42-4D24-8A44-1F64C532E7F5")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ShowMoneyDrop097 : IShowMoneyDropPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMoneyDrop097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMoneyDrop097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMoneyAsync(ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        return Version097CompatibilityProfile.SendMoneyDropAsync(this._player, itemId, isFreshDrop, amount, point);
    }
}
