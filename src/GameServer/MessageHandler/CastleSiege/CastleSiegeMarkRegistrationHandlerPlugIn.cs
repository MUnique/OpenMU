// <copyright file="CastleSiegeMarkRegistrationHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles Sign of Lord registration requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMarkRegistrationHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMarkRegistrationHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("676913C3-502D-409D-84FA-6BA283ACF349")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal class CastleSiegeMarkRegistrationHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeRegisterMarkAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeMarkRegistration.SubCode;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CastleSiegeMarkRegistration.Length)
        {
            return ValueTask.CompletedTask;
        }

        CastleSiegeMarkRegistration request = packet;
        if (!TryGetInventorySlot(request.ItemIndex, out var inventorySlot))
        {
            return ValueTask.CompletedTask;
        }

        return this._action.RegisterMarkAsync(player, CastleSiegeHandlerContext.Get(player), inventorySlot);
    }

    /// <summary>
    /// Translates the client-side backpack-grid index to OpenMU's inventory slot, which includes equipment slots.
    /// </summary>
    /// <param name="clientItemIndex">The zero-based backpack-grid index sent by the client.</param>
    /// <param name="inventorySlot">The translated OpenMU inventory slot.</param>
    /// <returns><see langword="true"/> if the translated slot fits into the packet slot range; otherwise, <see langword="false"/>.</returns>
    internal static bool TryGetInventorySlot(byte clientItemIndex, out byte inventorySlot)
    {
        var translatedSlot = clientItemIndex + InventoryConstants.EquippableSlotsCount;
        if (translatedSlot > byte.MaxValue)
        {
            inventorySlot = default;
            return false;
        }

        inventorySlot = (byte)translatedSlot;
        return true;
    }
}
