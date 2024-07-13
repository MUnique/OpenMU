// <copyright file="DuelGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Duel;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for duel start packets (0xAA identifier).
/// </summary>
[PlugIn(nameof(DuelGroupHandlerPlugIn), "Packet handler for duel packets (new system).")]
[Guid("88A8E1DB-C3EC-45A0-98F9-E9DA6F17373C")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
internal class DuelGroupHandlerPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.RequestDuelStart;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelGroupHandlerPlugIn" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public DuelGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}