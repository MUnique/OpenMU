// <copyright file="BloodCastleStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using System.Runtime.InteropServices;

/// <summary>
/// The default implementation of the <see cref="IBloodCastleStateViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(BloodCastleStateViewPlugIn), "The default implementation of the IBloodCastleStateViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("75A44740-FEE8-447A-BBA4-081A2410E408")]
public class BloodCastleStateViewPlugIn : IBloodCastleStateViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public BloodCastleStateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void UpdateState(byte state, int remainSecond, int maxMonster, int curMonster, int itemOwner, byte itemLevel)
    {
        this._player.Connection?.SendBloodCastleState(state, (ushort)remainSecond, (ushort)maxMonster, (ushort)curMonster, (ushort)itemOwner, itemLevel);
    }
}