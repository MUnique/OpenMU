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
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        await connection.SendCharacterInformation097Async(
            this._player.Position.X,
            this._player.Position.Y,
            (byte)this._player.SelectedCharacter!.CurrentMap!.Number,
            this._player.Rotation.ToPacketByte(),
            (uint)this._player.SelectedCharacter.Experience,
            (uint)this._player.GameServerContext.ExperienceTable[(int)this._player.Attributes![Stats.Level] + 1],
            (ushort)Math.Max(this._player.SelectedCharacter.LevelUpPoints, 0),
            (ushort)this._player.Attributes[Stats.BaseStrength],
            (ushort)this._player.Attributes[Stats.BaseAgility],
            (ushort)this._player.Attributes[Stats.BaseVitality],
            (ushort)this._player.Attributes[Stats.BaseEnergy],
            (ushort)this._player.Attributes[Stats.CurrentHealth],
            (ushort)this._player.Attributes[Stats.MaximumHealth],
            (ushort)this._player.Attributes[Stats.CurrentMana],
            (ushort)this._player.Attributes[Stats.MaximumMana],
            (ushort)this._player.Attributes[Stats.CurrentAbility],
            (ushort)this._player.Attributes[Stats.MaximumAbility],
            (uint)this._player.Money,
            this._player.SelectedCharacter.State.Convert(),
            this._player.SelectedCharacter.CharacterStatus.Convert(),
            (ushort)this._player.SelectedCharacter.UsedFruitPoints,
            this._player.SelectedCharacter.GetMaximumFruitPoints(),
            (ushort)this._player.Attributes[Stats.BaseLeadership])
            .ConfigureAwait(false);

        await this.SendNewCharacterInfoAsync(connection).ConfigureAwait(false);
        await this.SendNewCharacterCalcAsync(connection).ConfigureAwait(false);

        await this._player.InvokeViewPlugInAsync<IApplyKeyConfigurationPlugIn>(p => p.ApplyKeyConfigurationAsync()).ConfigureAwait(false);
    }

    private static uint ClampToUInt32(float value)
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

    private async ValueTask SendNewCharacterInfoAsync(IConnection connection)
    {
        const int packetLength = 76;
        var level = ClampToUInt32(this._player.Attributes![Stats.Level]);
        var nextExperience = (uint)this._player.GameServerContext.ExperienceTable[(int)this._player.Attributes[Stats.Level] + 1];

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE0;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), level);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)Math.Max(0, this._player.SelectedCharacter!.LevelUpPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)this._player.SelectedCharacter.Experience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), nextExperience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.BaseStrength]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.BaseAgility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.BaseVitality]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.BaseEnergy]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)this._player.SelectedCharacter.UsedFruitPoints);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), this._player.SelectedCharacter.GetMaximumFruitPoints());
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.Resets]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    private async ValueTask SendNewCharacterCalcAsync(IConnection connection)
    {
        const int packetLength = 72;
        var wizardryIncrease = this._player.Attributes![Stats.WizardryAttackDamageIncrease];
        var magicDamageRate = wizardryIncrease > 1f ? ClampToUInt32((wizardryIncrease - 1f) * 100f) : 0;

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE1;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.AttackSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MagicSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MinimumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MinimumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.MaximumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), magicDamageRate);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.AttackRatePvm]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.DefenseFinal]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(this._player.Attributes[Stats.DefenseRatePvm]));

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
