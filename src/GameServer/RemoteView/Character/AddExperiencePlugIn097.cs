// <copyright file="AddExperiencePlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Experience update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(AddExperiencePlugIn097), "Experience update plugin for 0.97 clients.")]
[Guid("742A2D34-3B1C-4A8C-88AB-9F4F9D5F6B58")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class AddExperiencePlugIn097 : IAddExperiencePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddExperiencePlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AddExperiencePlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AddExperienceAsync(int exp, IAttackable? obj, ExperienceType experienceType)
    {
        var connection = this._player.Connection;
        var attributes = this._player.Attributes;
        var selectedCharacter = this._player.SelectedCharacter;
        if (connection is null || attributes is null || selectedCharacter is null)
        {
            return;
        }

        var remainingExperience = exp;
        ushort damage = 0;
        if (obj is not null && obj.Id != obj.LastDeath?.KillerId)
        {
            damage = (ushort)Math.Min(obj.LastDeath?.FinalHit.HealthDamage ?? 0, ushort.MaxValue);
        }

        var id = (ushort)(obj?.GetId(this._player) ?? 0);
        if (id != 0)
        {
            id |= 0x8000;
        }

        while (remainingExperience > 0)
        {
            ushort sendExp = remainingExperience > ushort.MaxValue ? ushort.MaxValue : (ushort)remainingExperience;
            var viewExperience = ClampToUInt32(selectedCharacter.Experience);
            var viewNextExperience = ClampToUInt32(this._player.GameServerContext.ExperienceTable[(int)attributes[Stats.Level] + 1]);
            var viewDamage = (uint)damage;

            int WritePacket()
            {
                const int packetLength = 23;
                var span = connection.Output.GetSpan(packetLength)[..packetLength];
                span[0] = 0xC1;
                span[1] = (byte)packetLength;
                span[2] = 0x9C;
                BinaryPrimitives.WriteUInt16BigEndian(span.Slice(3, 2), id);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(5, 4), sendExp);
                BinaryPrimitives.WriteUInt16BigEndian(span.Slice(9, 2), damage);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(11, 4), viewDamage);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(15, 4), viewExperience);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(19, 4), viewNextExperience);
                return packetLength;
            }

            await connection.SendAsync(WritePacket).ConfigureAwait(false);
            damage = 0;
            remainingExperience -= sendExp;
        }
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
