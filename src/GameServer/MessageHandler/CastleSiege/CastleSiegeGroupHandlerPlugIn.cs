// <copyright file="CastleSiegeGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for Castle Siege packets with the 0xB2 identifier.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2438FBA6-4962-4BC7-8784-69F89AB53D8F")]
internal class CastleSiegeGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = 0xB2;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The plugin manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CastleSiegeGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
