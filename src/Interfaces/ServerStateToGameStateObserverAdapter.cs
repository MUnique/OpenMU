// <copyright file="ServerStateToGameStateObserverAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// An adapter which implements <see cref="IGameStateObserver"/> for an instance of <see cref="IServerStateObserver"/>.
    /// </summary>
    public class ServerStateToGameStateObserverAdapter : IGameStateObserver
    {
        private readonly IServerStateObserver serverStateObserver;
        private readonly int serverId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerStateToGameStateObserverAdapter"/> class.
        /// </summary>
        /// <param name="serverStateObserver">The server state observer.</param>
        /// <param name="serverId">The server identifier.</param>
        public ServerStateToGameStateObserverAdapter(IServerStateObserver serverStateObserver, int serverId)
        {
            this.serverStateObserver = serverStateObserver;
            this.serverId = serverId;
        }

        /// <inheritdoc />
        public void PlayerCountChanged(int playerCount)
        {
            this.serverStateObserver.PlayerCountChanged(this.serverId, playerCount);
        }
    }
}
