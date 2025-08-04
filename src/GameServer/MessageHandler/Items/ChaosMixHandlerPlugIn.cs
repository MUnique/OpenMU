// <copyright file="ChaosMixHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for chaos mix packets.
/// </summary>
[PlugIn(nameof(ChaosMixHandlerPlugIn), "Handler for chaos mix packets.")]
[Guid("0693e102-0adc-41e4-b0d4-ce22687b6dbb")]
internal class ChaosMixHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ItemCraftAction _mixAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ChaosMachineMixRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ChaosMachineMixRequest message = packet;

        byte mixType;
        if (packet.Length == 3)
        {
            // Older versions (e.g. 0.75, 0.95d) don't provide a mix type identifier, so we have to infer the item crafting
            var crafting = this._mixAction.FindAppropriateCraftingByItems(player);
            if (crafting is null)
            {
                await player.InvokeViewPlugInAsync<IShowItemCraftingResultPlugIn>(p => p.ShowResultAsync(CraftingResult.IncorrectMixItems, null)).ConfigureAwait(false);
                return;
            }

            mixType = crafting.Number;
        }
        else
        {
            mixType = (byte)message.MixType;
        }

        var socketSlot = packet.Length > 4 ? message.SocketSlot : (byte)0;
        await this._mixAction.MixItemsAsync(player, mixType, socketSlot).ConfigureAwait(false);
    }
}