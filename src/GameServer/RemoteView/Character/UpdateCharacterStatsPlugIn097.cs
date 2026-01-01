// <copyright file="UpdateCharacterStatsPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateCharacterStatsPlugIn097), "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("8ACD9D6B-6FA7-42C3-8C07-E137655CB92F")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateCharacterStatsPlugIn097 : IUpdateCharacterStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterStatsAsync()
    {
        var connection = this._player.Connection;
        var selectedCharacter = this._player.SelectedCharacter;
        var attributes = this._player.Attributes;
        if (connection is null || this._player.Account is null || selectedCharacter?.CurrentMap is null || attributes is null)
        {
            return;
        }

        await connection.SendCharacterInformation097Async(
            this._player.Position.X,
            this._player.Position.Y,
            (byte)selectedCharacter.CurrentMap.Number,
            this._player.Rotation.ToPacketByte(),
            (uint)selectedCharacter.Experience,
            (uint)this._player.GameServerContext.ExperienceTable[(int)attributes[Stats.Level] + 1],
            (ushort)Math.Max(selectedCharacter.LevelUpPoints, 0),
            (ushort)attributes[Stats.BaseStrength],
            (ushort)attributes[Stats.BaseAgility],
            (ushort)attributes[Stats.BaseVitality],
            (ushort)attributes[Stats.BaseEnergy],
            (ushort)attributes[Stats.CurrentHealth],
            (ushort)attributes[Stats.MaximumHealth],
            (ushort)attributes[Stats.CurrentMana],
            (ushort)attributes[Stats.MaximumMana],
            (ushort)attributes[Stats.CurrentAbility],
            (ushort)attributes[Stats.MaximumAbility],
            (uint)this._player.Money,
            selectedCharacter.State.Convert(),
            selectedCharacter.CharacterStatus.Convert(),
            (ushort)selectedCharacter.UsedFruitPoints,
            selectedCharacter.GetMaximumFruitPoints(),
            (ushort)attributes[Stats.BaseLeadership])
            .ConfigureAwait(false);

        await connection.SendAsync(WriteNewCharacterInfoPacket).ConfigureAwait(false);
        await connection.SendAsync(WriteNewCharacterCalcPacket).ConfigureAwait(false);

        await this._player.InvokeViewPlugInAsync<IApplyKeyConfigurationPlugIn>(p => p.ApplyKeyConfigurationAsync()).ConfigureAwait(false);

        int WriteNewCharacterInfoPacket()
        {
            const int packetLength = 76;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE0;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.Level]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.LevelUpPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.Experience));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.GameServerContext.ExperienceTable[(int)attributes[Stats.Level] + 1]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseStrength]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseAgility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseVitality]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseEnergy]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.UsedFruitPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.GetMaximumFruitPoints()));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.Resets]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);

            return packetLength;
        }

        int WriteNewCharacterCalcPacket()
        {
            const int packetLength = 72;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE1;

            var wizardryIncrease = attributes[Stats.WizardryAttackDamageIncrease];
            var magicDamageRate = wizardryIncrease > 1f ? ClampToUInt32((wizardryIncrease - 1f) * 100f) : 0;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.AttackSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MagicSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MinimumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MinimumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.MaximumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), magicDamageRate);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.AttackRatePvm]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.DefenseFinal]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.DefenseRatePvm]));

            return packetLength;
        }
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
