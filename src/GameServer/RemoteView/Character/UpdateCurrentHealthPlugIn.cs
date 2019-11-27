// <copyright file="UpdateCurrentHealthPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCurrentHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCurrentHealthPlugIn", "The default implementation of the IUpdateCurrentHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("0c832ed3-fea7-4239-8208-b46897b44c84")]
    public class UpdateCurrentHealthPlugIn : IUpdateCurrentHealthPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCurrentHealthPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCurrentHealthPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCurrentHealth()
        {
            using var writer = this.player.Connection.StartSafeWrite(CurrentHealthAndShield.HeaderType, CurrentHealthAndShield.Length);
            _ = new CurrentHealthAndShield(writer.Span)
            {
                Health = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f),
                Shield = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f),
            };

            writer.Commit();
        }
    }
}