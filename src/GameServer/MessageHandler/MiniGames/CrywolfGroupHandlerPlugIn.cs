// <copyright file="CrywolfGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameServer.Properties;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Group packet handler for the 0xBD (Crywolf) packet group.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.CrywolfGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5D2C8E14-6A9B-4F37-8C1E-9B4A7D2F6E03")]
internal class CrywolfGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key for the crywolf packet group.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.CrywolfGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfGroupHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CrywolfGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
