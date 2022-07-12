// <copyright file="ShowCharacterListPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

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
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCharacterListPlugIn075(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCharacterListAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;

        // 0.75 doesn't support dark lord (number 16) and newer classes yet
        var supportedCharacters = this._player.Account.Characters.Where(c => c.CharacterClass?.Number < 16).OrderBy(c => c.CharacterSlot).ToList();
        int Write()
        {
            var size = CharacterList075Ref.GetRequiredSize(supportedCharacters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterList075Ref(span)
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

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}