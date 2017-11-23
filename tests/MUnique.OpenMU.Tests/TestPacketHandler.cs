// <copyright file="TestPacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.MessageHandler;

    /// <summary>
    /// A packet handler, for testing.
    /// </summary>
    public class TestPacketHandler : IPacketHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPacketHandler"/> class.
        /// </summary>
        public TestPacketHandler()
        {
            this.ConstructorCalled = true;
        }

        /// <summary>
        /// Gets a value indicating whether the constructor has been called.
        /// </summary>
        public bool ConstructorCalled { get; }

        /// <summary>
        /// Gets a value indicating whether the method <see cref="HandlePacket(Player, byte[])"/> has been called.
        /// </summary>
        public bool HandlePacketCalled { get; private set; }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            this.HandlePacketCalled = true;
        }
    }
}
