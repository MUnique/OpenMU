// <copyright file="UpdateCurrentManaPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateCurrentManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateCurrentManaPlugIn", "The default implementation of the IUpdateCurrentManaPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("814fcc24-022a-47c8-b7d2-b1d1ca0208cb")]
public class UpdateCurrentManaPlugIn : IUpdateCurrentManaPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCurrentManaPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCurrentManaPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCurrentManaAsync()
    {
        if (this._player.Attributes is null)
        {
            return;
        }

        await this._player.Connection.SendCurrentManaAndAbilityAsync(
            (ushort)this._player.Attributes[Stats.CurrentMana],
            (ushort)this._player.Attributes[Stats.CurrentAbility]).ConfigureAwait(false);
    }
}