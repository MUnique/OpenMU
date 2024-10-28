// <copyright file="ShowHitExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IShowHitPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowHitExtendedPlugIn), "The extended implementation of the IShowHitPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("E79C8065-21A8-4774-B84F-5B8658F6A820")]
[MinimumClient(106, 3, ClientLanguage.English)]
public class ShowHitExtendedPlugIn : IShowHitPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowHitExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowHitExtendedPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <remarks>
    /// This Packet is sent to the Client when a Player or Monster got hit and damaged.
    /// It includes which Player/Monster got hit by who, and the Damage Type.
    /// It is obvious that the mu online protocol only supports 16 bits for each damage value. To prevent bugs (own player health)
    /// and to make it somehow visible that the damage exceeds 65k, we send more than one packet, if the 16bits are not enough.
    /// </remarks>
    /// <inheritdoc/>
    public async ValueTask ShowHitAsync(IAttackable target, HitInfo hitInfo)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var healthStatus = this.CalcStatStatus(target, Stats.CurrentHealth, Stats.MaximumHealth);
        var shieldStatus = this.CalcStatStatus(target, Stats.CurrentShield, Stats.MaximumShield);
        var targetId = target.GetId(this._player);
        await connection.SendObjectHitExtendedAsync(
            this.GetDamageKind(hitInfo.Attributes),
            hitInfo.Attributes.HasFlag(DamageAttributes.Double),
            hitInfo.Attributes.HasFlag(DamageAttributes.Triple),
            targetId,
            healthStatus,
            shieldStatus,
            hitInfo.HealthDamage,
            hitInfo.ShieldDamage).ConfigureAwait(false);
    }

    private byte CalcStatStatus(IAttackable target, AttributeDefinition currentStat, AttributeDefinition maximumStat)
    {
        var current = target.Attributes[currentStat];
        var maximum = target.Attributes[maximumStat];

        if (maximum == 0 || float.IsNaN(maximum))
        {
            return 0xFF;
        }

        if (current <= 0)
        {
            return 0;
        }

        return (byte)Math.Round(current / maximum * 250, MidpointRounding.AwayFromZero);
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