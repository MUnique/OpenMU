// <copyright file="ShowHitPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowHitPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowHitPlugIn", "The default implementation of the IShowHitPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("bb59de05-d3a1-4b52-a1c6-975decf0f1a3")]
public class ShowHitPlugIn : IShowHitPlugIn
{
    private readonly RemotePlayer _player;

    private readonly byte _operation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowHitPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowHitPlugIn(RemotePlayer player)
    {
        this._player = player;
        this._operation = this.DetermineOperation();
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
        var targetId = target.GetId(this._player);
        var remainingHealthDamage = hitInfo.HealthDamage;
        var remainingShieldDamage = hitInfo.ShieldDamage;

        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // do/while, so that a 'miss' with 0 damage sends a message, too.
        do
        {
            var healthDamage = (ushort)System.Math.Min(0xFFFF, remainingHealthDamage);
            var shieldDamage = (ushort)System.Math.Min(0xFFFF, remainingShieldDamage);

            await connection.SendObjectHitAsync(
                this._operation,
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

    private byte DetermineOperation()
    {
        if (this._player.ClientVersion.Season < 1)
        {
            return 0x15;
        }

        switch (this._player.ClientVersion.Language)
        {
            case ClientLanguage.English:
                return 0x11;
            case ClientLanguage.Japanese:
                return 0xD6;
            case ClientLanguage.Vietnamese:
                return 0xDC;
            case ClientLanguage.Filipino:
            case ClientLanguage.Korean:
                return 0xDF;
            case ClientLanguage.Chinese:
                return 0xD0;
            case ClientLanguage.Thai:
                return 0xD2;
            default:
                return (byte)MUnique.OpenMU.GameServer.PacketType.Hit;
        }
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