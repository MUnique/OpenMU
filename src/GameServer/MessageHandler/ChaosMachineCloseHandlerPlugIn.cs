﻿// <copyright file="ChaosMachineCloseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for chaos mix packets.
    /// </summary>
    [PlugIn("ChaosMachineCloseHandlerPlugIn", "Handler for chaos mix packets.")]
    [Guid("1857513c-d09c-4e03-8bf4-f4ead19ea60f")]
    internal class ChaosMachineCloseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly CloseNpcDialogAction closeNpcDialogAction = new CloseNpcDialogAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.ChaosMachineClose;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.closeNpcDialogAction.CloseNpcDialog(player);
        }
    }
}
