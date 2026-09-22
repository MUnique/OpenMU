// <copyright file="CastleSiegeMachineGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for Castle Siege warfare-machine packets with the 0xB7 identifier.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C517DA3B-C4EB-4807-9D7C-AED3CB396A38")]
internal sealed class CastleSiegeMachineGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = 0xB7;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client-version provider.</param>
    /// <param name="manager">The plug-in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CastleSiegeMachineGroupHandlerPlugIn(
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
