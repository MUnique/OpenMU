// <copyright file="UpdateLevelPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Level update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(UpdateLevelPlugIn097), "Level update plugin for 0.97 clients.")]
[Guid("D04B2700-3F1D-4A9E-A7F0-84D7F9D76A0F")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateLevelPlugIn097 : IUpdateLevelPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLevelPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateLevelPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateLevelAsync()
    {
        var selectedCharacter = this._player.SelectedCharacter;
        var charStats = this._player.Attributes;
        var connection = this._player.Connection;
        if (selectedCharacter is null || charStats is null || connection is null)
        {
            return;
        }

        await connection.SendAsync(() =>
        {
            const int packetLength = 46;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0x05;

            var (viewExperience, viewNextExperience) = Version097ExperienceViewHelper.GetViewExperience(this._player);
            var offset = 4;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.Level]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(selectedCharacter.LevelUpPoints));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumHealth]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumMana]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumAbility]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)selectedCharacter.UsedFruitPoints);
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), selectedCharacter.GetMaximumFruitPoints());
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)selectedCharacter.UsedNegFruitPoints);
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), selectedCharacter.GetMaximumFruitPoints());
            offset += 2;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.LevelUpPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), viewExperience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), viewNextExperience);

            return packetLength;
        }).ConfigureAwait(false);

        var message = this._player.GameServerContext.Localization.GetString("Server_Message_LevelUp", (int)charStats[Stats.Level]);
        await this._player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask UpdateMasterLevelAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static ushort GetUShort(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }

    private static ushort GetUShort(int value)
    {
        if (value <= 0)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }

    private static uint ClampToUInt32(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= uint.MaxValue)
        {
            return uint.MaxValue;
        }

        return (uint)value;
    }

    private static uint ClampToUInt32(long value)
    {
        if (value <= 0)
        {
            return 0;
        }

        if (value >= uint.MaxValue)
        {
            return uint.MaxValue;
        }

        return (uint)value;
    }

}
