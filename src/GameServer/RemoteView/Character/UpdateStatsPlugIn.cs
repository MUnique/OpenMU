// <copyright file="UpdateStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateStatsPlugIn), "The default implementation of the IUpdateStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("2A8BFB0C-2AFF-4A52-B390-5A68D5C5F26A")]
public class UpdateStatsPlugIn : IUpdateStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateStatsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateMaximumStatsAsync(IUpdateStatsPlugIn.UpdatedStats updatedStats = IUpdateStatsPlugIn.UpdatedStats.Undefined | IUpdateStatsPlugIn.UpdatedStats.Health | IUpdateStatsPlugIn.UpdatedStats.Mana | IUpdateStatsPlugIn.UpdatedStats.Speed)
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        if (updatedStats.HasFlag(IUpdateStatsPlugIn.UpdatedStats.Health))
        {
            await this._player.Connection.SendMaximumHealthAndShieldAsync(
                (ushort)Math.Max(this._player.Attributes[Stats.MaximumHealth], 0f),
                (ushort)Math.Max(this._player.Attributes[Stats.MaximumShield], 0f)).ConfigureAwait(false);
        }

        if (updatedStats.HasFlag(IUpdateStatsPlugIn.UpdatedStats.Mana))
        {
            await this._player.Connection.SendMaximumManaAndAbilityAsync(
                (ushort)Math.Max(this._player.Attributes[Stats.MaximumMana], 0f),
                (ushort)Math.Max(this._player.Attributes[Stats.MaximumAbility], 0f)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask UpdateCurrentStatsAsync(IUpdateStatsPlugIn.UpdatedStats updatedStats = IUpdateStatsPlugIn.UpdatedStats.Undefined | IUpdateStatsPlugIn.UpdatedStats.Health | IUpdateStatsPlugIn.UpdatedStats.Mana | IUpdateStatsPlugIn.UpdatedStats.Speed)
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        if (updatedStats.HasFlag(IUpdateStatsPlugIn.UpdatedStats.Health))
        {
            await this._player.Connection.SendCurrentHealthAndShieldAsync(
                (ushort)Math.Max(this._player.Attributes[Stats.CurrentHealth], 0f),
                (ushort)Math.Max(this._player.Attributes[Stats.CurrentShield], 0f)).ConfigureAwait(false);
        }

        if (updatedStats.HasFlag(IUpdateStatsPlugIn.UpdatedStats.Mana))
        {
            await this._player.Connection.SendCurrentManaAndAbilityAsync(
                (ushort)Math.Max(this._player.Attributes[Stats.CurrentMana], 0f),
                (ushort)Math.Max(this._player.Attributes[Stats.CurrentAbility], 0f)).ConfigureAwait(false);
        }
    }
}