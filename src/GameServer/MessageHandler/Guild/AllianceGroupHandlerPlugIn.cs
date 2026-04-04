// <copyright file="AllianceGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for alliance group packets (0xEB identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AllianceGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.AllianceGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C4D5E6F7-A8B9-4C0D-1E2F-3A4B5C6D7E8F")]
internal class AllianceGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key (packet code 0xEB).
    /// </summary>
    internal const byte GroupKey = 0xEB;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllianceGroupHandlerPlugIn" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public AllianceGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
