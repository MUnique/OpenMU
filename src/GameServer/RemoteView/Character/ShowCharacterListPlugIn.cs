// <copyright file="ShowCharacterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.GameServer.RemoteView.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCharacterListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCharacterListPlugIn", "The default implementation of the IShowCharacterListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9563dd2c-85cc-4b23-aa95-9d1a18582032")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class ShowCharacterListPlugIn : IShowCharacterListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterList()
        {
            var appearanceSerializer = this.player.AppearanceSerializer;
            int characterSize = appearanceSerializer.NeededSpace + 16;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, (this.player.Account.Characters.Count * characterSize) + 8))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0;
                byte maxClass = 0;
                packet[4] = this.player.Account.UnlockedCharacterClasses?.Select(c => c.CreationAllowedFlag)
                                .Aggregate(maxClass, (current, flag) => (byte)(current | flag)) ?? 0;
                packet[5] = 0; // MoveCnt
                packet[6] = (byte)this.player.Account.Characters.Count;
                packet[7] = this.player.Account.IsVaultExtended ? (byte)1 : (byte)0;
                int i = 0;
                foreach (var character in this.player.Account.Characters.OrderBy(c => c.CharacterSlot))
                {
                    var characterBlock = packet.Slice((i * characterSize) + 8, characterSize);
                    characterBlock[0] = character.CharacterSlot;
                    characterBlock.Slice(1, 10).WriteString(character.Name, Encoding.UTF8);
                    characterBlock[11] = 1; // unknown
                    var level = (ushort)(character.Attributes.FirstOrDefault(s => s.Definition == Stats.Level)?.Value ?? 1);
                    characterBlock[12] = level.GetLowByte();
                    characterBlock[13] = level.GetHighByte();
                    characterBlock[14] = (byte)character.CharacterStatus; // | 0x10 for item block?

                    appearanceSerializer.WriteAppearanceData(characterBlock.Slice(15), new CharacterAppearanceDataAdapter(character), false);

                    var guildPosition = this.player.GameServerContext.GuildServer?.GetGuildPosition(character.Id);
                    characterBlock[15 + appearanceSerializer.NeededSpace] = guildPosition?.GetViewValue() ?? (byte)0xFF;
                    i++;
                }

                writer.Commit();
            }
        }
    }
}