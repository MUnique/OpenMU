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
/// The extended implementation of the <see cref="IUpdateStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateStatsExtendedPlugIn), "The extended implementation of the IUpdateStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("E9A1CCBE-416F-41BA-8E74-74CBEB7042DD")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateStatsExtendedPlugIn : IUpdateStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateStatsExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateMaximumStatsAsync(IUpdateStatsPlugIn.UpdatedStats updatedStats = IUpdateStatsPlugIn.UpdatedStats.Undefined)
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        await this._player.Connection.SendMaximumStatsExtendedAsync(
            (uint)this._player.Attributes[Stats.MaximumHealth],
            (uint)this._player.Attributes[Stats.MaximumShield],
            (uint)this._player.Attributes[Stats.MaximumMana],
            (uint)this._player.Attributes[Stats.MaximumAbility]).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask UpdateCurrentStatsAsync(IUpdateStatsPlugIn.UpdatedStats updatedStats = IUpdateStatsPlugIn.UpdatedStats.Undefined)
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        await this._player.Connection.SendCurrentStatsExtendedAsync(
            (uint)this._player.Attributes[Stats.CurrentHealth],
            (uint)this._player.Attributes[Stats.CurrentShield],
            (uint)this._player.Attributes[Stats.CurrentMana],
            (uint)this._player.Attributes[Stats.CurrentAbility],
            (ushort)this._player.Attributes[Stats.AttackSpeed],
            (ushort)this._player.Attributes[Stats.MagicSpeed]).ConfigureAwait(false);
    }
}