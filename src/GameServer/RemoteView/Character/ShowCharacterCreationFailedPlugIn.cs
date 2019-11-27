// <copyright file="ShowCharacterCreationFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCharacterCreationFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCharacterCreationFailedPlugIn", "The default implementation of the IShowCharacterCreationFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("8fe59675-8625-4837-9872-e8d9c73471cd")]
    public class ShowCharacterCreationFailedPlugIn : IShowCharacterCreationFailedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterCreationFailedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterCreationFailedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterCreationFailed()
        {
            using var writer = this.player.Connection.StartSafeWrite(CharacterCreationFailed.HeaderType, CharacterCreationFailed.Length);
            _ = new CharacterCreationFailed(writer.Span);
            writer.Commit();
        }
    }
}