// <copyright file="CharacterFocusedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ICharacterFocusedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("CharacterFocusedPlugIn", "The default implementation of the ICharacterFocusedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f6fa0149-3a2c-41c2-aeb1-94b647641ffb")]
    public class CharacterFocusedPlugIn : ICharacterFocusedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterFocusedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public CharacterFocusedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void CharacterFocused(Character character)
        {
            using var writer = this.player.Connection.StartSafeWrite(Network.Packets.ServerToClient.CharacterFocused.HeaderType, Network.Packets.ServerToClient.CharacterFocused.Length);
            _ = new CharacterFocused(writer.Span)
            {
                CharacterName = character.Name,
            };

            writer.Commit();
        }
    }
}