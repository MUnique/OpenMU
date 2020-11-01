namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.MuHelper;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ToggleMuHelperStatus"/> which response with new mu helper status.
    /// </summary>
    [PlugIn("ToggleMuHelperStatusPlugin", "Response with new mu helper status")]
    [Guid("d30bea99-9d77-4182-99be-e08095c1968f")]
    public class ToggleMuHelperStatusPlugin : IToggleMuHelperStatus
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleMuHelperStatusPlugin"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ToggleMuHelperStatusPlugin(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ToggleMuHelperStatus(byte status)
        {
            using var writer =
                this.player.Connection.StartSafeWrite(
                    MuHelperStatusToggleResponse.HeaderType,
                    MuHelperStatusToggleResponse.Length);
            _ = new MuHelperStatusToggleResponse(writer.Span)
            {
                Status = status,
                Money = 0,
                Time = 0,
                TimeMultiplier = 0,
            };
            writer.Commit();
        }
    }
}