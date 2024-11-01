// <copyright file="PickupItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for item pickup packets.
/// </summary>
[PlugIn("PickupItemHandlerPlugIn", "Handler for item pickup packets.")]
[Guid("8bcb9d85-95ae-4611-ae64-e9cc801ec647")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
internal class PickupItemHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly PickupItemAction _pickupAction = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PickupItemRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PickupItemRequest message = packet;
        await this._pickupAction.PickupItemAsync(player, message.ItemId).ConfigureAwait(false);
    }
}