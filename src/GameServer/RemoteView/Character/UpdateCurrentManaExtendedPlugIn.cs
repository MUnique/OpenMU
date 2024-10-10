// <copyright file="UpdateCurrentManaExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IUpdateCurrentManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateCurrentManaExtendedPlugIn), "The extended implementation of the IUpdateCurrentManaPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("1F99BFB4-35FC-489B-AB3C-E0738314DF37")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateCurrentManaExtendedPlugIn : IUpdateCurrentManaPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCurrentManaPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCurrentManaExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCurrentManaAsync()
    {
        if (this._player.Attributes is null)
        {
            return;
        }

        await this._player.Connection.SendCurrentManaAndAbilityExtendedAsync(
            (uint)this._player.Attributes[Stats.CurrentMana],
            (uint)this._player.Attributes[Stats.CurrentAbility]).ConfigureAwait(false);
    }
}