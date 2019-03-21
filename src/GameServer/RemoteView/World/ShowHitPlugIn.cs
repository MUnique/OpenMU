// <copyright file="ShowHitPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowHitPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowHitPlugIn", "The default implementation of the IShowHitPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("bb59de05-d3a1-4b52-a1c6-975decf0f1a3")]
    public class ShowHitPlugIn : IShowHitPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowHitPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowHitPlugIn(RemotePlayer player) => this.player = player;

        /// <summary>
        /// The color of the damage.
        /// </summary>
        public enum DamageColor : byte
        {
            /// <summary>
            /// The normal, red damage color.
            /// </summary>
            NormalRed = 0,

            /// <summary>
            /// The ignore defense, cyan damage color.
            /// </summary>
            IgnoreDefenseCyan = 1,

            /// <summary>
            /// The excellent, light green damage color.
            /// </summary>
            ExcellentLightGreen = 2,

            /// <summary>
            /// The critical, blue damage color.
            /// </summary>
            CriticalBlue = 3,

            /// <summary>
            /// The light pink damage color.
            /// </summary>
            LightPink = 4,

            /// <summary>
            /// The poison, dark green damage color.
            /// </summary>
            PoisonDarkGreen = 5,

            /// <summary>
            /// The dark pink damage color.
            /// </summary>
            DarkPink = 6,

            /// <summary>
            /// The white damage color.
            /// </summary>
            White = 7,
        }

        /// <summary>
        /// The special damage.
        /// </summary>
        public enum SpecialDamage : byte
        {
            /// <summary>
            /// The double damage.
            /// </summary>
            Double = 0x40,

            /// <summary>
            /// The triple damage.
            /// </summary>
            Triple = 0x80
        }

        /// <remarks>
        /// This Packet is sent to the Client when a Player or Monster got Hit and damaged.
        /// It includes which Player/Monster got hit by who, and the Damage Type.
        /// It is obvious that the mu online protocol only supports 16 bits for each damage value. To prevent bugs (own player health)
        /// and to make it somehow visible that the damage exceeds 65k, we send more than one packet, if the 16bits are not enough.
        /// </remarks>
        /// <inheritdoc/>
        public void ShowHit(IAttackable target, HitInfo hitInfo)
        {
            var targetId = target.GetId(this.player);
            var remainingHealthDamage = hitInfo.HealthDamage;
            var remainingShieldDamage = hitInfo.ShieldDamage;
            while (remainingHealthDamage > 0 || remainingShieldDamage > 0)
            {
                var healthDamage = (ushort)(remainingHealthDamage & 0xFFFF);
                var shieldDamage = (ushort)(remainingShieldDamage & 0xFFFF);
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0A))
                {
                    var packet = writer.Span;
                    packet[2] = (byte)PacketType.Hit;
                    packet.Slice(3).SetShortSmallEndian(targetId);
                    packet.Slice(5).SetShortSmallEndian(healthDamage);
                    packet[7] = this.GetDamageColor(hitInfo.Attributes);
                    packet.Slice(8).SetShortSmallEndian(shieldDamage);
                    writer.Commit();
                }

                remainingShieldDamage -= shieldDamage;
                remainingHealthDamage -= healthDamage;
            }
        }

        private byte GetDamageColor(DamageAttributes attributes)
        {
            var colorResult = DamageColor.NormalRed;
            if (attributes.HasFlag(DamageAttributes.IgnoreDefense))
            {
                colorResult = DamageColor.IgnoreDefenseCyan;
            }
            else if (attributes.HasFlag(DamageAttributes.Excellent))
            {
                colorResult = DamageColor.ExcellentLightGreen;
            }
            else if (attributes.HasFlag(DamageAttributes.Critical))
            {
                colorResult = DamageColor.CriticalBlue;
            }
            else if (attributes.HasFlag(DamageAttributes.Reflected))
            {
                colorResult = DamageColor.DarkPink;
            }
            else
            {
                // no special color
            }

            byte result = (byte)colorResult;
            if (attributes.HasFlag(DamageAttributes.Double))
            {
                result |= (byte)SpecialDamage.Double;
            }

            if (attributes.HasFlag(DamageAttributes.Triple))
            {
                result |= (byte)SpecialDamage.Triple;
            }

            return result;
        }
    }
}