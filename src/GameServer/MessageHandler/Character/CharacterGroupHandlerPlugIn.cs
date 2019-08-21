// <copyright file="CharacterGroupHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for character packets (0xF3 identifier).
    /// </summary>
    [PlugIn("Character Packet Handler", "Packet handler for character packets (0xF3 identifier).")]
    [Guid("dce31462-c8a6-4d9a-a8b8-54a50cf16aff")]
    internal class CharacterGroupHandlerPlugIn : GroupPacketHandlerPlugIn
    {
        /// <summary>
        /// The group key.
        /// </summary>
        internal const byte GroupKey = (byte)PacketType.CharacterGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterGroupHandlerPlugIn"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        public CharacterGroupHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(clientVersionProvider, manager)
        {
        }

        /// <inheritdoc/>
        public override bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public override byte Key => GroupKey;
    }
}
