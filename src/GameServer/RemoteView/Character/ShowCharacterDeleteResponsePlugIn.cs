// <copyright file="ShowCharacterDeleteResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCharacterDeleteResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCharacterDeleteResponsePlugIn", "The default implementation of the IShowCharacterDeleteResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("303af9ef-4f7d-4e07-9976-a0241311e50d")]
    public class ShowCharacterDeleteResponsePlugIn : IShowCharacterDeleteResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterDeleteResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterDeleteResponsePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterDeleteResponse(CharacterDeleteResult result)
        {
            this.player.Connection.SendCharacterDeleteResponse(ConvertResult(result));
        }

        private static CharacterDeleteResponse.CharacterDeleteResult ConvertResult(CharacterDeleteResult result)
        {
            return result switch
            {
                CharacterDeleteResult.Successful => CharacterDeleteResponse.CharacterDeleteResult.Successful,
                CharacterDeleteResult.Unsuccessful => CharacterDeleteResponse.CharacterDeleteResult.Unsuccessful,
                CharacterDeleteResult.WrongSecurityCode => CharacterDeleteResponse.CharacterDeleteResult.WrongSecurityCode,
                _ => throw new ArgumentException($"Case {result} is not handled."),
            };
        }
    }
}