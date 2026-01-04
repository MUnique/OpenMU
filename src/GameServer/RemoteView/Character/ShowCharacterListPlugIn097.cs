// <copyright file="ShowCharacterListPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Text;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IShowCharacterListPlugIn"/> for version 0.97,
/// including the move list packet for the M-menu.
/// </summary>
[PlugIn(nameof(ShowCharacterListPlugIn097), "Shows character list and move list for version 0.97.")]
[Guid("9F31B977-3E7B-4C73-9E17-0A7F6D6A9E3D")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
public class ShowCharacterListPlugIn097 : IShowCharacterListPlugIn
{
    private static readonly Encoding MoveListEncoding = Encoding.Latin1;
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCharacterListPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCharacterListPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCharacterListAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        var unlockFlags = CreateUnlockFlags(this._player.Account);

        // 0.97 doesn't support dark lord (number 16) and newer classes yet
        var supportedCharacters = this._player.Account.Characters
            .Where(c => c.CharacterClass?.Number < 16)
            .OrderBy(c => c.CharacterSlot)
            .ToList();

        int WriteCharacterList()
        {
            var size = CharacterList095Ref.GetRequiredSize(supportedCharacters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterList095Ref(span)
            {
                CharacterCount = (byte)supportedCharacters.Count,
            };

            var i = 0;
            foreach (var character in supportedCharacters)
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

        await connection.SendAsync(WriteCharacterList).ConfigureAwait(false);
        if ((unlockFlags & CharacterCreationUnlockFlags.MagicGladiator) != 0)
        {
            await connection.SendAsync(WriteCharacterCreationEnable).ConfigureAwait(false);
        }
        await connection.SendAsync(WriteMoveList).ConfigureAwait(false);

        int WriteMoveList()
        {
            var warpList = this._player.GameContext.Configuration.WarpList
                .Where(warp => warp.Gate?.Map is not null)
                .OrderBy(warp => warp.Index)
                .ToList();

            const int entrySize = 48;
            var length = 7 + (warpList.Count * entrySize);
            var span = connection.Output.GetSpan(length)[..length];

            span[0] = 0xC2;
            span[1] = (byte)(length >> 8);
            span[2] = (byte)(length & 0xFF);
            span[3] = 0xF3;
            span[4] = 0xE5;
            span[5] = 1; // PKLimitFree
            span[6] = (byte)warpList.Count;

            var offset = 7;
            foreach (var warp in warpList)
            {
                span[offset] = (byte)warp.Gate!.Map!.Number;
                offset += 1;

                span.Slice(offset, 32).Clear();
                var nameBytes = MoveListEncoding.GetBytes(warp.Name ?? string.Empty);
                var nameLength = Math.Min(nameBytes.Length, 32);
                nameBytes.AsSpan(0, nameLength).CopyTo(span.Slice(offset, nameLength));
                offset += 32;

                span[offset] = 1; // CanMove
                offset += 1;

                var minLevel = (short)Math.Min(short.MaxValue, warp.LevelRequirement);
                BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, 2), minLevel);
                offset += 2;
                BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, 2), -1);
                offset += 2;
                BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, 2), -1);
                offset += 2;
                BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, 2), -1);
                offset += 2;
                BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, 2), -1);
                offset += 2;

                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)Math.Max(0, warp.Costs));
                offset += 4;
            }

            return length;
        }

        int WriteCharacterCreationEnable()
        {
            const int packetLength = 4;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = packetLength;
            span[2] = 0xDE;
            span[3] = 1; // allow Magic Gladiator creation for 0.97 clients
            return packetLength;
        }
    }

    private static CharacterCreationUnlockFlags CreateUnlockFlags(MUnique.OpenMU.DataModel.Entities.Account account)
    {
        byte aggregatedFlags = 0;
        var result = account.UnlockedCharacterClasses?
            .Select(c => c.CreationAllowedFlag)
            .Aggregate(aggregatedFlags, (current, flag) => (byte)(current | flag)) ?? 0;
        return (CharacterCreationUnlockFlags)result;
    }
}
