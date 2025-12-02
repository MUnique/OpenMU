// <copyright file="UpdateCharacterBaseStatsExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IUpdateCharacterBaseStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateCharacterBaseStatsExtendedPlugIn), "The extended implementation of the IUpdateCharacterBaseStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("851C4579-FB3D-454C-A238-217542E8E6B9")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateCharacterBaseStatsExtendedPlugIn : IUpdateCharacterBaseStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterBaseStatsExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterBaseStatsExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterBaseStatsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        await connection.SendBaseStatsExtendedAsync(
                (uint)this._player.Attributes![Stats.BaseStrength],
                (uint)this._player.Attributes[Stats.BaseAgility],
                (uint)this._player.Attributes[Stats.BaseVitality],
                (uint)this._player.Attributes[Stats.BaseEnergy],
                (uint)this._player.Attributes[Stats.BaseLeadership])
            .ConfigureAwait(false);
    }
}