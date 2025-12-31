// <copyright file="StatIncreaseResultPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Stat increase result plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(StatIncreaseResultPlugIn097), "Stat increase result plugin for 0.97 clients.")]
[Guid("8B019191-4C4C-4FBD-8A65-1AE0E0490CE3")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class StatIncreaseResultPlugIn097 : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, ushort addedPoints)
    {
        var connection = this._player.Connection;
        var selectedCharacter = this._player.SelectedCharacter;
        var attributes = this._player.Attributes;
        if (connection is null || attributes is null || selectedCharacter is null)
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
                    ? GetUShort(attributes[Stats.MaximumMana])
                    : attribute == Stats.BaseVitality
                        ? GetUShort(attributes[Stats.MaximumHealth])
                        : default,
                0,
                GetUShort(attributes[Stats.MaximumAbility])).ConfigureAwait(false);
#pragma warning restore SA1118 // Parameter should not span multiple lines
            return;
        }

        var map = this._player.CurrentMap!;
        await this._player.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(this._player.GetAsEnumerable())).ConfigureAwait(false);
        await this._player.InvokeViewPlugInAsync<IUpdateCharacterStatsPlugIn>(p => p.UpdateCharacterStatsAsync()).ConfigureAwait(false);
        await this._player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
        var currentGate = new Persistence.BasicModel.ExitGate
        {
            Map = map.Definition,
            X1 = this._player.Position.X,
            X2 = this._player.Position.X,
            Y1 = this._player.Position.Y,
            Y2 = this._player.Position.Y,
        };

        await this._player.WarpToAsync(currentGate).ConfigureAwait(false);
    }

    private static ushort GetUShort(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }
}
