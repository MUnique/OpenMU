// <copyright file="CashShopGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for cash shop packets (0xD2 identifier).
/// </summary>
[PlugIn("Cash Shop Packet Handler", "Packet handler for cash shop packets (0xD2 identifier).")]
[Guid("8A3F4C7E-1D2B-4E9F-B5A3-6C8D9E0F1A2B")]
internal class CashShopGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.CashShopGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopGroupHandlerPlugIn" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CashShopGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
