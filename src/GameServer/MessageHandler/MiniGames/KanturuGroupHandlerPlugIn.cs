// <copyright file="KanturuGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Group packet handler for the 0xD1 (Kanturu) packet group.
/// Routes sub-codes to the appropriate sub-handlers:
///   0x00 — KanturuInfoRequest (client requests event state info → opens dialog).
///   0x01 — KanturuEnterRequest (client requests to enter the event map).
/// </summary>
[PlugIn]
[Display(Name = "Kanturu Group Handler", Description = "Routes 0xD1 Kanturu client packets to sub-handlers.")]
[Guid("C4E8A1F2-7B93-4D06-9E45-2A1F8B3C7D02")]
internal class KanturuGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key for the Kanturu packet group.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.KanturuGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuGroupHandlerPlugIn"/> class.
    /// </summary>
    public KanturuGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
