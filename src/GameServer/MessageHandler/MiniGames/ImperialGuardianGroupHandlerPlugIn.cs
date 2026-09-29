// <copyright file="ImperialGuardianGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameServer.Properties;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the packets of the imperial guardian event (0xF7), which are forwarded to the sub handlers.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ImperialGuardianGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.ImperialGuardianGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5A91D3E7-4C28-4B6F-8E03-B7D2F19C6A54")]
internal class ImperialGuardianGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key for the packets of the imperial guardian event.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.ImperialGuardianGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ImperialGuardianGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
