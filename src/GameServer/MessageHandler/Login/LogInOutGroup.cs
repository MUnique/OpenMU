// <copyright file="LogInOutGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Login
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet group handler for log in and out packets (0xF1).
    /// </summary>
    [PlugIn("Log In & Out Group", "Packet group handler for log in and out packets (0xF1).")]
    [Guid("8590fe8b-2f75-4189-8972-ed8f63b8bf22")]
    internal class LogInOutGroup : GroupPacketHandlerPlugIn
    {
        /// <summary>
        /// The group key.
        /// </summary>
        internal const byte GroupKey = 0xF1;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOutGroup"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        public LogInOutGroup(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(clientVersionProvider, manager)
        {
        }

        /// <inheritdoc />
        public override byte Key => GroupKey;

        /// <inheritdoc />
        public override bool IsEncryptionExpected => false;
    }
}