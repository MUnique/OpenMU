// <copyright file="ShowCharacterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameServer.RemoteView.Guild;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
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
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCharacterListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void ShowCharacterList()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        using var writer = connection.StartSafeWrite(CharacterList.HeaderType, CharacterList.GetRequiredSize(this._player.Account.Characters.Count));
        var packet = new CharacterList(writer.Span);
        byte maxClass = 0;
        packet.CreationFlags = this._player.Account.UnlockedCharacterClasses?
            .Select(c => c.CreationAllowedFlag)
            .Aggregate(maxClass, (current, flag) => (byte)(current | flag)) ?? 0;
        packet.CharacterCount = (byte)this._player.Account.Characters.Count;
        packet.IsVaultExtended = this._player.Account.IsVaultExtended;
        var i = 0;
        foreach (var character in this._player.Account.Characters.OrderBy(c => c.CharacterSlot))
        {
            var characterData = packet[i];
            characterData.SlotIndex = character.CharacterSlot;
            characterData.Name = character.Name;
            characterData.Level = (ushort)(character.Attributes.FirstOrDefault(s => s.Definition == Stats.Level)?.Value ?? 1);
            characterData.Status = character.CharacterStatus.Convert();
            var guildPosition = this._player.GameServerContext.GuildServer?.GetGuildPosition(character.Id);
            characterData.GuildPosition = guildPosition.Convert();
            appearanceSerializer.WriteAppearanceData(characterData.Appearance, new CharacterAppearanceDataAdapter(character), false);
            i++;
        }

        writer.Commit();
    }
}