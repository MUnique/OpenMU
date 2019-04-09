﻿// <copyright file="WarpHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for warp request packets.
    /// This one is called when a player uses the warp list.
    /// </summary>
    [PlugIn("WarpHandlerPlugIn", "Handler for warp request packets.")]
    [Guid("3d261a26-4357-4367-b999-703ea936f4e9")]
    internal class WarpHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly WarpAction warpAction = new WarpAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.WarpCommand;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet[3] != 2) ////is always 2 i guess?
            {
                return;
            }

            ushort warpInfoIndex = NumberConversionExtensions.MakeWord(packet[8], packet[9]);
            var warpInfo = player.GameContext.Configuration.WarpList?.FirstOrDefault(info => info.Index == warpInfoIndex);
            if (warpInfo != null)
            {
                this.warpAction.WarpTo(player, warpInfo);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Unknown warp index {warpInfoIndex}", MessageType.BlueNormal);
            }
        }
    }
}
