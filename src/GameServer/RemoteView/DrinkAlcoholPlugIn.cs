// <copyright file="DrinkAlcoholPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IDrinkAlcoholPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("DrinkAlcoholPlugIn", "The default implementation of the IDrinkAlcoholPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("a31546d8-bf79-43dd-872c-52f24ea9bca9")]
    public class DrinkAlcoholPlugIn : IDrinkAlcoholPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrinkAlcoholPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public DrinkAlcoholPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void DrinkAlcohol()
        {
            this.player.Connection?.SendConsumeItemWithEffect(ConsumeItemWithEffect.ConsumedItemType.Ale, 80);
        }
    }
}