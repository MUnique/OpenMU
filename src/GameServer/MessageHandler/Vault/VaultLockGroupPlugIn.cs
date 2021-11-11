// <copyright file="VaultLockGroupPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Vault;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for vault lock packets (0x83 identifier).
/// </summary>
[PlugIn("Vault Lock Packet Handler", "Packet handler for vault lock packets (0x83 identifier).")]
[Guid("751B3608-D9D9-45F1-BEF1-7B42AE851ABB")]
internal class VaultLockGroupPlugIn : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = (byte)PacketType.VaultPassword;

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultLockGroupPlugIn" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public VaultLockGroupPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}