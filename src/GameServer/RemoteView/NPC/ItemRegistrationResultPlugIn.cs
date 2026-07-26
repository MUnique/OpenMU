// <copyright file="ItemRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;



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
    public async ValueTask RegistrationResultAsync(int npcNumber, IItemRegistrationResultPlugIn.ItemRegistrationOperation operation)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var feature = this._player.GameContext.FeaturePlugIns.GetPlugIn<ItemRegistrationFeaturePlugIn>();
        if (feature is null)
        {
            return;
        }
        var config = feature?.Configuration ?? new ItemRegistrationConfiguration();
        var rule = config.Rules.FirstOrDefault(r => r.NpcNumber == npcNumber);
        if (rule is null)
        {
            return;
        }

        var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, IItemRegistrationStrategy>((short)npcNumber);
        if (strategy?.TargetStat is not { } targetStat)
        {
            return;
        }

        // Calculate the number of matching items in the inventory
        int invItem = this._player.Inventory?.Items.Count(i =>
            i.Definition?.Group == rule.AcceptedItemGroup && i.Definition?.Number == rule.AcceptedItemNumber) ?? 0;

        var registeredStat = this._player.SelectedCharacter?.Attributes.FirstOrDefault(a => a.Definition == targetStat);
        int registered = (int)(registeredStat?.Value ?? this._player.Attributes?[targetStat] ?? 0);

        byte result = operation switch
        {
            IItemRegistrationResultPlugIn.ItemRegistrationOperation.MissingItem => 1,
            _ => 0, // OpenRegistrationDialog and RegistrationCompleted - matches the known-working value from commit 91efa714b
        };

        await connection.SendEventChipRegistrationResultAsync(result, (uint)registered, (ushort)invItem).ConfigureAwait(false);
    }
}
