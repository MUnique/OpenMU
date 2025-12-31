// <copyright file="ShowHitPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
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
        var targetId = target.GetId(this._player);
        var remainingHealthDamage = hitInfo.HealthDamage;
        var remainingShieldDamage = hitInfo.ShieldDamage;

        if (this._player.Connection is not { } connection)
        {
            return;
        }

        do
        {
            var healthDamage = (ushort)Math.Min(ushort.MaxValue, remainingHealthDamage);
            var shieldDamage = (ushort)Math.Min(ushort.MaxValue, remainingShieldDamage);

            await connection.SendObjectHitAsync(
                0x15,
                targetId,
                healthDamage,
                this.GetDamageKind(hitInfo.Attributes),
                hitInfo.Attributes.HasFlag(DamageAttributes.Double),
                hitInfo.Attributes.HasFlag(DamageAttributes.Triple),
                shieldDamage).ConfigureAwait(false);

            remainingShieldDamage -= shieldDamage;
            remainingHealthDamage -= healthDamage;
        }
        while (remainingHealthDamage > 0 || remainingShieldDamage > 0);
    }

    private DamageKind GetDamageKind(DamageAttributes attributes)
    {
        if (attributes.HasFlag(DamageAttributes.IgnoreDefense))
        {
            return DamageKind.IgnoreDefenseCyan;
        }

        if (attributes.HasFlag(DamageAttributes.Excellent))
        {
            return DamageKind.ExcellentLightGreen;
        }

        if (attributes.HasFlag(DamageAttributes.Critical))
        {
            return DamageKind.CriticalBlue;
        }

        if (attributes.HasFlag(DamageAttributes.Reflected))
        {
            return DamageKind.ReflectedDarkPink;
        }

        if (attributes.HasFlag(DamageAttributes.Poison))
        {
            return DamageKind.PoisonDarkGreen;
        }

        return DamageKind.NormalRed;
    }
}
