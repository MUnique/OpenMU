// <copyright file="CastleSiegeNpcGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for Castle Siege NPC-list packets with the 0xB3 identifier.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeNpcGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeNpcGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F40FD671-43DE-46B4-9527-F17B9A8A5A0D")]
internal sealed class CastleSiegeNpcGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = 0xB3;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeNpcGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client-version provider.</param>
    /// <param name="manager">The plugin manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CastleSiegeNpcGroupHandlerPlugIn(
        IClientVersionProvider clientVersionProvider,
        PlugInManager manager,
        ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public override byte Key => GroupKey;
}
