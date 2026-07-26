// <copyright file="GoldenArcherWindowExitHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for Golden Archer exit dialog packets (0x97).
/// </summary>
[PlugIn]
[Guid("ED982BA5-39A0-449D-BB11-4BD31C77926A")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
internal class GoldenArcherWindowExitHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly CloseNpcDialogAction _closeNpcDialogAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EventChipExitDialog.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._closeNpcDialogAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
    }
}
