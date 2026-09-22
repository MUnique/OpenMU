// <copyright file="CastleSiegeHuntZoneGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for Castle Siege hunting-zone packets with the 0xB9 identifier.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeHuntZoneGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeHuntZoneGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("87916020-23B1-494A-AC41-58AF2BB19DE1")]
internal sealed class CastleSiegeHuntZoneGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = 0xB9;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeHuntZoneGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The plug-in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CastleSiegeHuntZoneGroupHandlerPlugIn(
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
