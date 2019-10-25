// <copyright file="UpdateMaximumManaPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateMaximumManaPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateMaximumManaPlugIn", "The default implementation of the IUpdateMaximumManaPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("dc84be82-7ab0-4348-aa34-4a3dc8c1ee7a")]
    public class UpdateMaximumManaPlugIn : IUpdateMaximumManaPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMaximumManaPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateMaximumManaPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateMaximumMana()
        {
            using var writer = this.player.Connection.StartSafeWrite(MaximumManaAndAbility.HeaderType, MaximumManaAndAbility.Length);
            _ = new MaximumManaAndAbility(writer.Span)
            {
                Mana = (ushort)this.player.Attributes[Stats.MaximumMana],
                Ability = (ushort)this.player.Attributes[Stats.MaximumAbility],
            };

            writer.Commit();
        }
    }
}