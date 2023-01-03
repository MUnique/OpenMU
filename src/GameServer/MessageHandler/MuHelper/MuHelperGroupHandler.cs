// <copyright file="MuHelperGroupHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MuHelper;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for mu helper packets (0xBF identifier).
/// </summary>
[PlugIn(nameof(MuHelperGroupHandler), "Group packet handler for mu helper packets (0xBF identifier).")]
[Guid("F92D5B35-02C9-4C95-9A29-B4E4323B63A2")]
internal class MuHelperGroupHandler : GroupPacketHandlerPlugIn
{
    /// <summary>
    /// The group key.
    /// </summary>
    internal const byte GroupKey = 0xBF;

    /// <summary>
    /// Initializes a new instance of the <see cref="MuHelperGroupHandler" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger.</param>
    public MuHelperGroupHandler(
        IClientVersionProvider clientVersionProvider,
        PlugInManager manager,
        ILoggerFactory loggerFactory)
        : base(clientVersionProvider, manager, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public override byte Key => GroupKey;
}