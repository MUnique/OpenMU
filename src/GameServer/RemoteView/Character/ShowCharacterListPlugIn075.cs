// <copyright file="ShowCharacterListPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCharacterListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCharacterListPlugIn v0.75", "The implementation of the IShowCharacterListPlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("650A3478-729E-4995-ADAF-BDEB829A92E5")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    public class ShowCharacterListPlugIn075 : IShowCharacterListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterListPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterList()
        {
            var appearanceSerializer = this.player.AppearanceSerializer;

            // 0.75 doesn't support dark lord (number 16) and newer classes yet
            var supportedCharacters = this.player.Account.Characters.Where(c => c.CharacterClass.Number < 16).OrderBy(c => c.CharacterSlot).ToList();

            using var writer = this.player.Connection.StartSafeWrite(CharacterList075.HeaderType, CharacterList075.GetRequiredSize(supportedCharacters.Count));
            var packet = new CharacterList075(writer.Span)
            {
                CharacterCount = (byte)supportedCharacters.Count,
            };

            int i = 0;
            foreach (var character in supportedCharacters.OrderBy(c => c.CharacterSlot))
            {
                var characterBlock = packet[i];
                characterBlock.SlotIndex = character.CharacterSlot;
                characterBlock.Name = character.Name;
                characterBlock.Level = (ushort)(character.Attributes.FirstOrDefault(s => s.Definition == Stats.Level)?.Value ?? 1);
                characterBlock.Status = character.CharacterStatus.Convert();
                appearanceSerializer.WriteAppearanceData(characterBlock.Appearance, new CharacterAppearanceDataAdapter(character), false);
                i++;
            }

            writer.Commit();
        }
    }
}
