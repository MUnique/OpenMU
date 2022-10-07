// <copyright file="ShowRageAttackPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowRageAttackPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowRageAttackPlugIn), "The default implementation of the IShowRageAttackPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("F3E2FA03-AA87-4D61-855C-8ECAF990E108")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class ShowRageAttackPlugIn : IShowRageAttackPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowRageAttackPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowRageAttackPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowAttackAsync(IIdentifiable attacker, IIdentifiable? target, ushort skillId)
    {
        await this._player.Connection.SendRageAttackAsync(skillId, attacker.GetId(this._player), (ushort)(target.GetId(this._player) | 0x8000)).ConfigureAwait(false);
    }
}