// <copyright file="MuHelperSaveDataRequestHandlerPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MuHelper;

using System.Buffers;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for mu helper data save packets (0xAE identifier).
/// </summary>
[PlugIn(nameof(MuHelperSaveDataRequestHandlerPlugin), "Handler for mu bot data save request.")]
[Guid("493B12F2-5115-4587-B0CF-B1E4F9B77249")]
public class MuHelperSaveDataRequestHandlerPlugin : IPacketHandlerPlugIn
{
    private readonly UpdateMuHelperConfigurationAction _updateMuBotConfigurationAction = new();

    /// <inheritdoc />
    public byte Key => MuHelperSaveDataRequest.Code;

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        MuHelperSaveDataRequest message = packet;
        var dataSize = message.HelperData.Length;
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(dataSize);
        var memory = memoryOwner.Memory[..dataSize];
        message.HelperData.CopyTo(memory.Span);
        await this._updateMuBotConfigurationAction.SaveDataAsync(player, memory).ConfigureAwait(false);
    }
}