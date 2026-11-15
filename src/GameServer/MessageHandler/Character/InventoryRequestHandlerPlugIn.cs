// <copyright file="InventoryRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Returns an authoritative inventory snapshot after previously queued actions.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateInventoryListPlugIn_Name), Description = nameof(PlugInResources.UpdateInventoryListPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("db463aa9-e091-42e4-a926-c68ba24cfdf9")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class InventoryRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc />
    public byte Key => InventoryRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length != InventoryRequest.Length || player.Inventory is null
            || (player.PlayerState.CurrentState != PlayerState.EnteredWorld
                && player.PlayerState.CurrentState != PlayerState.NpcDialogOpened))
        {
            return;
        }

        await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
    }
}
