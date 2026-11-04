// <copyright file="ChatCommandGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the messages about chat commands (0xF5 identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ChatCommandGroupHandlerPlugIn_Name), Description = nameof(PlugInResources.ChatCommandGroupHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2F1B5B26-2E22-4E7D-9C8B-1C6E5B7A9D40")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
internal class ChatCommandGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key. It's the <see cref="ChatCommandListRequest.Code"/>, which can't be
    /// used here directly because it's not a compile time constant.
    /// </summary>
    internal const byte GroupKey = 0xF5;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandGroupHandlerPlugIn" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ChatCommandGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}
