// <copyright file="ItemRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;
using MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IItemRegistrationResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Guid("4A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D")]
public class ItemRegistrationResultPlugIn : IItemRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RegistrationResultAsync(short npcNumber, ItemRegistrationOperation operation)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var feature = this._player.GameContext.FeaturePlugIns.GetPlugIn<ItemRegistrationFeaturePlugIn>();
        if (feature?.Configuration is not { } config)
        {
            return;
        }

        var rule = config.Rules.FirstOrDefault(r => r.NpcNumber == npcNumber);
        if (rule is null)
        {
            return;
        }

        var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, IItemRegistrationStrategy>(npcNumber);
        if (strategy is null)
        {
            return;
        }

        // Calculate the number of matching items in the inventory
        int invItem = this._player.Inventory?.Items.Count(i =>
            i.Definition?.Group == rule.AcceptedItemGroup && i.Definition?.Number == rule.AcceptedItemNumber) ?? 0;

        // Stateless NPCs (e.g. TraderBob) have no TargetStat - report 0 instead of skipping the packet entirely.
        int registered = 0;
        if (strategy.TargetStat is { } targetStat)
        {
            var registeredStat = this._player.SelectedCharacter?.Attributes.FirstOrDefault(a => a.Definition == targetStat);
            registered = (int)(registeredStat?.Value ?? this._player.Attributes?[targetStat] ?? 0);
        }

        // Only MissingItem gets a distinct result byte; OpenRegistrationDialog and RegistrationCompleted
        // must both send 0 - the client doesn't recognize other values and falls back to the wrong window.
        byte result = operation == ItemRegistrationOperation.MissingItem ? (byte)1 : (byte)0;

        await connection.SendEventChipRegistrationResultAsync(result, (uint)registered, (ushort)invItem).ConfigureAwait(false);
    }
}
