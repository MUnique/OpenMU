// <copyright file="InventoryItemEquipRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request to start or stop using an item inside the inventory, like a figurine or a charm.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.InventoryItemEquipRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.InventoryItemEquipRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("0CFF0E0C-E744-4F3C-A857-8106BBE76654")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class InventoryItemEquipRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly UseInventoryItemAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => InventoryItemEquipRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < InventoryItemEquipRequest.Length)
        {
            return;
        }

        InventoryItemEquipRequest request = packet;
        await this._action.SetItemInUseAsync(player, request.InventorySlot, request.Value).ConfigureAwait(false);
    }
}
