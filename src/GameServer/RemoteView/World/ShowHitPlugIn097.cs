// <copyright file="ShowHitPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Hit visualization for 0.97 clients.
/// </summary>
[PlugIn(nameof(ShowHitPlugIn097), "Hit visualization for 0.97 clients.")]
[Guid("9D1E1D6D-1C0D-4C74-9C27-8C1E54AC7D72")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ShowHitPlugIn097 : IShowHitPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowHitPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowHitPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowHitAsync(IAttackable target, HitInfo hitInfo)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        const int packetLength = 15;
        var targetId = target.GetId(this._player);
        var damage = (ushort)Math.Min(ushort.MaxValue, hitInfo.HealthDamage);
        var viewCurHp = ClampToUInt32(target.Attributes[Stats.CurrentHealth]);
        var viewDamageHp = ClampToUInt32(hitInfo.HealthDamage);

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x15;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(3, 2), targetId);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(5, 2), damage);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(7, 4), viewCurHp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(11, 4), viewDamageHp);
            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
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
}
