// <copyright file="StatIncreaseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IStatIncreaseResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("StatIncreaseResultPlugIn", "The default implementation of the IStatIncreaseResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ce603b3c-cf25-426f-9cb9-5cc367843de8")]
public class StatIncreaseResultPlugIn : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, ushort addedPoints)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        if (addedPoints <= 1)
        {
#pragma warning disable SA1118 // Parameter should not span multiple lines
            await connection.SendCharacterStatIncreaseResponseAsync(
                addedPoints > 0,
                attribute.GetStatType(),
                attribute == Stats.BaseEnergy
                    ? (ushort)this._player.Attributes![Stats.MaximumMana]
                    : attribute == Stats.BaseVitality
                        ? (ushort)this._player.Attributes![Stats.MaximumHealth]
                        : default,
                (ushort)this._player.Attributes![Stats.MaximumShield],
                (ushort)this._player.Attributes[Stats.MaximumAbility]).ConfigureAwait(false);
#pragma warning restore SA1118 // Parameter should not span multiple lines
            return;
        }

        // Workaround with multiple points for older clients
        var player = this._player;
        var map = player.CurrentMap!;

        await player.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(player.GetAsEnumerable())).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IUpdateCharacterStatsPlugIn>(p => p.UpdateCharacterStatsAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
        var currentGate = new Persistence.BasicModel.ExitGate
        {
            Map = map.Definition,
            X1 = player.Position.X,
            X2 = player.Position.X,
            Y1 = player.Position.Y,
            Y2 = player.Position.Y,
        };

        await player.WarpToAsync(currentGate).ConfigureAwait(false);
    }
}