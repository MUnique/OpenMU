// <copyright file="ShowCharacterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameServer.RemoteView.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCharacterListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowCharacterListPlugIn", "The default implementation of the IShowCharacterListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("9563dd2c-85cc-4b23-aa95-9d1a18582032")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class ShowCharacterListPlugIn : IShowCharacterListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCharacterListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCharacterListAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is not { } account)
        {
            return;
        }

        var unlockFlags = CreateUnlockFlags(account);
        await this.SendCharacterListAsync(connection, account, unlockFlags).ConfigureAwait(false);
        if (unlockFlags > CharacterCreationUnlockFlags.None)
        {
            await connection.SendCharacterClassCreationUnlockAsync(unlockFlags).ConfigureAwait(false);
        }
    }

    private static CharacterCreationUnlockFlags CreateUnlockFlags(Account account)
    {
        byte aggregatedFlags = 0;
        var result = account.UnlockedCharacterClasses?
            .Select(c => c.CreationAllowedFlag)
            .Aggregate(aggregatedFlags, (current, flag) => (byte)(current | flag)) ?? 0;
        return (CharacterCreationUnlockFlags)result;
    }

    private async ValueTask SendCharacterListAsync(IConnection connection, Account account, CharacterCreationUnlockFlags unlockFlags)
    {
        var guildPositions = new GuildPosition?[account.Characters.Count];
        int i = 0;
        foreach (var character in account.Characters)
        {
            guildPositions[i] = await this._player.GameServerContext.GuildServer.GetGuildPositionAsync(character.Id).ConfigureAwait(false);
            i++;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        int Write()
        {
            var size = CharacterListRef.GetRequiredSize(account.Characters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterListRef(span)
            {
                UnlockFlags = unlockFlags,
                CharacterCount = (byte)account.Characters.Count,
                IsVaultExtended = account.IsVaultExtended,
            };

            var j = 0;
            foreach (var character in account.Characters.OrderBy(c => c.CharacterSlot))
            {
                var characterData = packet[j];
                characterData.SlotIndex = character.CharacterSlot;
                characterData.Name = character.Name;
                characterData.Level = (ushort)(character.Attributes.FirstOrDefault(s => s.Definition == Stats.Level)?.Value ?? 1);
                characterData.Status = character.CharacterStatus.Convert();
                characterData.GuildPosition = guildPositions[j].Convert();
                appearanceSerializer.WriteAppearanceData(characterData.Appearance, new CharacterAppearanceDataAdapter(character), false);
                j++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}