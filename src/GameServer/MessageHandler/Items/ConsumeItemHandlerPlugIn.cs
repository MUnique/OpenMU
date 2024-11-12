// <copyright file="ConsumeItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for item consume packets.
/// </summary>
[PlugIn(nameof(ConsumeItemHandlerPlugIn), "Handler for item consume packets.")]
[Guid("53992288-0d11-49df-98a3-2912b7616558")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
internal class ConsumeItemHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ItemConsumeAction _consumeAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ConsumeItemRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ConsumeItemRequest message = packet;
        await this._consumeAction.HandleConsumeRequestAsync(player, message.ItemSlot, message.TargetSlot, Convert(message.FruitConsumption)).ConfigureAwait(false);
    }

    private static FruitUsage Convert(ConsumeItemRequest.FruitUsage fruitConsumption)
    {
        return fruitConsumption switch
        {
            ConsumeItemRequest.FruitUsage.AddPoints => FruitUsage.AddPoints,
            ConsumeItemRequest.FruitUsage.RemovePoints => FruitUsage.RemovePoints,
            _ => FruitUsage.Undefined,
        };
    }
}