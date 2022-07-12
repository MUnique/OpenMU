// <copyright file="ShowCharacterListPlugIn095.cs" company="MUnique">
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
[PlugIn(nameof(ShowCharacterListPlugIn095), "The implementation of the IShowCharacterListPlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
[Guid("004E97F5-5817-45F5-BB0E-D4F78007768C")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class ShowCharacterListPlugIn095 : IShowCharacterListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCharacterListPlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCharacterListAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;

        // 0.95 doesn't support dark lord (number 16) and newer classes yet
        var supportedCharacters = this._player.Account.Characters.Where(c => c.CharacterClass?.Number < 16).OrderBy(c => c.CharacterSlot).ToList();
        int Write()
        {
            var size = CharacterList095Ref.GetRequiredSize(supportedCharacters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterList095Ref(span)
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