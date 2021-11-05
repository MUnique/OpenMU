// <copyright file="ShowCharacterListPlugIn095.cs" company="MUnique">
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
    [PlugIn(nameof(ShowCharacterListPlugIn095), "The implementation of the IShowCharacterListPlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("004E97F5-5817-45F5-BB0E-D4F78007768C")]
    [MinimumClient(0, 95, ClientLanguage.Invariant)]
    public class ShowCharacterListPlugIn095 : IShowCharacterListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn095"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterListPlugIn095(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterList()
        {
            var connection = this.player.Connection;
            if (connection is null || this.player.Account is null)
            {
                return;
            }

            var appearanceSerializer = this.player.AppearanceSerializer;

            // 0.95 doesn't support dark lord (number 16) and newer classes yet
            var supportedCharacters = this.player.Account.Characters.Where(c => c.CharacterClass?.Number < 16).OrderBy(c => c.CharacterSlot).ToList();

            using var writer = connection.StartSafeWrite(CharacterList095.HeaderType, CharacterList095.GetRequiredSize(supportedCharacters.Count));
            var packet = new CharacterList095(writer.Span)
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