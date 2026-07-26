// <copyright file="GoldenArcherWindowHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the Golden Archer registration window.
/// </summary>
[PlugIn]
[Guid("F68AFB2B-CEE0-420E-89F2-30694045ED45")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
internal class GoldenArcherWindowHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EventChipRegistrationRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var action = new ItemRegistrationAction();
        var success = await action.RegisterAsync(player).ConfigureAwait(false);
        if (!success && player.OpenedNpc is { } npc)
        {
            await player.InvokeViewPlugInAsync<IItemRegistrationResultPlugIn>(
                p => p.RegistrationResultAsync(npc.Definition.Number, ItemRegistrationOperation.MissingItem)).ConfigureAwait(false);
        }
    }
}
