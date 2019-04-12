// <copyright file="StoreHandlerGroupPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for player store related packets.
    /// </summary>
    [PlugIn("StoreHandlerGroupPlugIn", "Handler for player store related packets.")]
    [Guid("8C6DBAB0-FED6-4F4C-9924-6243FEB4E1F2")]
    internal class StoreHandlerGroupPlugIn : GroupPacketHandlerPlugIn
    {
        /// <summary>
        /// The group key.
        /// </summary>
        internal const byte GroupKey = (byte)PacketType.PersonalShopGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreHandlerGroupPlugIn"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        public StoreHandlerGroupPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(clientVersionProvider, manager)
        {
        }

        /// <inheritdoc/>
        public override bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public override byte Key => GroupKey;
    }
}
