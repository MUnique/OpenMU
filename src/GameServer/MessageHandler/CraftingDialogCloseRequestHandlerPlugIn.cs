// <copyright file="CraftingDialogCloseRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for chaos mix packets.
/// </summary>
[PlugIn(nameof(CraftingDialogCloseRequestHandlerPlugIn), "Handler for packets which should close the crafting dialog (Chaos Machine).")]
[Guid("1857513c-d09c-4e03-8bf4-f4ead19ea60f")]
internal class CraftingDialogCloseRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly CloseNpcDialogAction _closeNpcDialogAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CraftingDialogCloseRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._closeNpcDialogAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
    }
}