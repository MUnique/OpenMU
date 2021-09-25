// <copyright file="ShowMiniGameEnterResultViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.MiniGames;
    using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;
    using MiniGameType = MUnique.OpenMU.DataModel.Configuration.MiniGameType;

    /// <summary>
    /// The default implementation of the <see cref="IShowMiniGameEnterResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ShowMiniGameEnterResultViewPlugIn), "The default implementation of the IShowMiniGameEnterResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("2BB953FC-7BF7-4B13-B602-E3CD56A5EED2")]
    public class ShowMiniGameEnterResultViewPlugIn : IShowMiniGameEnterResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMiniGameEnterResultViewPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowMiniGameEnterResultViewPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ShowResult(MiniGameType miniGameType, byte gameTicketInventoryIndex, EnterResult enterResult)
        {
            switch (miniGameType)
            {
                case MiniGameType.DevilSquare:
                    this.player.Connection?.SendDevilSquareEnterResult(enterResult.ToDevilSquareEnterResult(), gameTicketInventoryIndex);
                    break;
                case MiniGameType.BloodCastle:
                case MiniGameType.ChaosCastle:
                    throw new NotImplementedException();
                case MiniGameType.Undefined:
                    throw new ArgumentException("undefined game type", nameof(miniGameType));
                default:
                    throw new ArgumentOutOfRangeException(nameof(miniGameType), miniGameType, null);
            }
        }
    }
}
