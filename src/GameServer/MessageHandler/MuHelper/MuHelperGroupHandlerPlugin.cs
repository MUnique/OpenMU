namespace MUnique.OpenMU.GameServer.MessageHandler.MuHelper
{
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for mu helper packets (0xF6 identifier).
    /// </summary>
    [PlugIn("Mu Helper Group", "Packet handler for mu helper packets (0xBF identifier).")]
    [Guid("2641F83A-42D3-42B0-BD2F-63AD0E3A380A")]
    internal class MuHelperGroupHandlerPlugin : GroupPacketHandlerPlugIn
    {
        /// <summary>
        /// The group key.
        /// </summary>
        internal const byte GroupKey = 0xBF;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuHelperGroupHandlerPlugin" /> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="loggerFactory">The logger.</param>
        public MuHelperGroupHandlerPlugin(
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