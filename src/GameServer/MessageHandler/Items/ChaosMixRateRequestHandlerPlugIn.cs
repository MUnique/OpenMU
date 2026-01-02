// <copyright file="ChaosMixRateRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for chaos mix rate request packets (0x88) used by 0.97 clients.
/// </summary>
[PlugIn(nameof(ChaosMixRateRequestHandlerPlugIn), "Handler for chaos mix rate request packets (0x88).")]
[Guid("3E8829C3-B30E-4EE9-A9C4-4A2E8F5D8F44")]
internal class ChaosMixRateRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => 0x88;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player is not RemotePlayer remotePlayer)
        {
            return;
        }

        var connection = remotePlayer.Connection;
        var npc = player.OpenedNpc?.Definition;
        if (connection is null || npc is null)
        {
            return;
        }

        if (packet.Length < 7)
        {
            return;
        }

        var mixType = BinaryPrimitives.ReadInt32LittleEndian(packet.Span.Slice(3, 4));
        var crafting = npc.ItemCraftings.FirstOrDefault(c => c.Number == mixType);
        if (crafting is null)
        {
            await SendMixRateAsync(connection, 0, 0).ConfigureAwait(false);
            return;
        }

        var handler = CreateCraftingHandler(crafting);
        if (handler is null)
        {
            await SendMixRateAsync(connection, 0, 0).ConfigureAwait(false);
            return;
        }

        handler.TryGetRequiredItems(player, out _, out var successRate, out _);

        var money = 0u;
        if (crafting.SimpleCraftingSettings is { } settings)
        {
            money = (uint)Math.Max(0, settings.Money + (settings.MoneyPerFinalSuccessPercentage * successRate));
        }

        await SendMixRateAsync(connection, successRate, money).ConfigureAwait(false);
    }

    private static IItemCraftingHandler? CreateCraftingHandler(ItemCrafting crafting)
    {
        if (!string.IsNullOrWhiteSpace(crafting.ItemCraftingHandlerClassName))
        {
            var type = Type.GetType(crafting.ItemCraftingHandlerClassName);
            if (type != null)
            {
                if (type.BaseType == typeof(SimpleItemCraftingHandler))
                {
                    return (IItemCraftingHandler)Activator.CreateInstance(type, crafting.SimpleCraftingSettings)!;
                }

                return (IItemCraftingHandler)Activator.CreateInstance(type)!;
            }
        }

        if (crafting.SimpleCraftingSettings != null)
        {
            return new SimpleItemCraftingHandler(crafting.SimpleCraftingSettings);
        }

        return null;
    }

    private static ValueTask SendMixRateAsync(IConnection connection, uint rate, uint money)
    {
        const int packetLength = 11;
        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x88;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(3, 4), rate);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(7, 4), money);
            return packetLength;
        }

        return connection.SendAsync(WritePacket);
    }
}
