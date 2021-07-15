// <copyright file="GuildCreateHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild create packets for version 0.75.
    /// </summary>
    [PlugIn(nameof(GuildCreateHandlerPlugIn075), "Handler for guild create packets.")]
    [Guid("6605E425-F1D5-44AA-864D-EA42B25BB17F")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    internal class GuildCreateHandlerPlugIn075 : IPacketHandlerPlugIn
    {
        private readonly GuildCreateAction createAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => GuildCreateRequest075.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            GuildCreateRequest075 request = packet;
            this.createAction.CreateGuild(player, request.GuildName, request.GuildEmblem.ToArray());
        }
    }
}