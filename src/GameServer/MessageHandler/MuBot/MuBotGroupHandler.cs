﻿// <copyright file="MuBotGroupHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MuBot
{
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for mu helper packets (0xF6 identifier).
    /// </summary>
    [PlugIn("MuBotGroupHandler", "Group packet handler for mu helper packets (0xBF identifier).")]
    [Guid("2641F83A-42D3-42B0-BD2F-63AD0E3A380A")]
    internal class MuBotGroupHandler : GroupPacketHandlerPlugIn
    {
        /// <summary>
        /// The group key.
        /// </summary>
        internal const byte GroupKey = 0xBF;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBotGroupHandler" /> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="loggerFactory">The logger.</param>
        public MuBotGroupHandler(
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
}