// <copyright file="UpdateMaximumManaExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="IUpdateMaximumManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateMaximumManaExtendedPlugIn), "The extended implementation of the IUpdateMaximumManaPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("25AF11D5-FA10-4634-AF5A-CBF6F5E8BDFE")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateMaximumManaExtendedPlugIn : IUpdateMaximumManaPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMaximumManaPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMaximumManaExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateMaximumManaAsync()
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        await this._player.Connection.SendMaximumManaAndAbilityExtendedAsync(
            (uint)this._player.Attributes[Stats.MaximumMana],
            (uint)this._player.Attributes[Stats.MaximumAbility]).ConfigureAwait(false);
    }
}