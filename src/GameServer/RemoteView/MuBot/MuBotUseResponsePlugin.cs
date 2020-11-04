namespace MUnique.OpenMU.GameServer.RemoteView.MuBot
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.MuBot;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="MuBotUseResponsePlugin"/> which response with new mu bot status.
    /// </summary>
    [PlugIn("MuBotUseResponsePlugin", "Response with new mu bot status")]
    [Guid("d30bea99-9d77-4182-99be-e08095c1968f")]
    public class MuBotUseResponsePlugin : IMuBotUseResponse
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBotUseResponsePlugin"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MuBotUseResponsePlugin(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void SendMuBotUseResponse(byte status)
        {
            using var writer =
                this.player.Connection.StartSafeWrite(
                    MuBotUseResponse.HeaderType,
                    MuBotUseResponse.Length);
            _ = new MuBotUseResponse(writer.Span)
            {
                Status = status,
                Money = 10000,
                Time = 2,
                TimeMultiplier = 10,
            };
            writer.Commit();
        }
    }
}