// <copyright file="GensGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Gens;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the packets of the gens system (0xF8 identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.GensGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("4D8B2E61-A3F7-4C09-B5D1-7E2A9C6F3B80")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
internal class GensGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key for the packets of the gens system.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.GensGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="GensGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public GensGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
