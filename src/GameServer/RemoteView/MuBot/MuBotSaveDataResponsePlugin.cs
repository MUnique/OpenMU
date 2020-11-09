// <copyright file="MuBotSaveDataResponsePlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuBot
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.MuBot;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="MuBotSaveDataResponsePlugin"/> which response with new mu bot status.
    /// </summary>
    [PlugIn("MuBotSaveDataResponsePlugin", "Response with new mu bot data")]
    [Guid("d30bea99-9d77-4182-99be-e08095c1968a")]
    public class MuBotSaveDataResponsePlugin : IMuBotSaveDataResponse
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBotSaveDataResponsePlugin"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MuBotSaveDataResponsePlugin(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void SendMuBotSavedDataResponse(Span<byte> data)
        {
            using var writer =
                this.player.Connection.StartSafeWrite(MuBotSaveDataResponse.HeaderType, MuBotSaveDataResponse.Length);
            var packet = new MuBotSaveDataResponse(writer.Span);
            data.CopyTo(packet.BotData);
            writer.Commit();
        }
    }
}
