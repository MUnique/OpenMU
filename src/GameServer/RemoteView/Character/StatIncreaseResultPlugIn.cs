// <copyright file="StatIncreaseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IStatIncreaseResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("StatIncreaseResultPlugIn", "The default implementation of the IStatIncreaseResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ce603b3c-cf25-426f-9cb9-5cc367843de8")]
    public class StatIncreaseResultPlugIn : IStatIncreaseResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public StatIncreaseResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void StatIncreaseResult(AttributeDefinition attribute, bool success)
        {
            using var writer = this.player.Connection.StartSafeWrite(CharacterStatIncreaseResponse.HeaderType, CharacterStatIncreaseResponse.Length);
            var packet = new CharacterStatIncreaseResponse(writer.Span)
            {
                Success = success,
                Attribute = attribute.GetStatType(),
            };
            if (success)
            {
                if (attribute == Stats.BaseEnergy)
                {
                    packet.UpdatedDependentMaximumStat = (ushort)this.player.Attributes[Stats.MaximumMana];
                }
                else if (attribute == Stats.BaseVitality)
                {
                    packet.UpdatedDependentMaximumStat = (ushort)this.player.Attributes[Stats.MaximumHealth];
                }
                else
                {
                    // no updated value required
                }

                // since all stats may affect shield and ability, both are included
                packet.UpdatedMaximumShield = (ushort)this.player.Attributes[Stats.MaximumShield];
                packet.UpdatedMaximumAbility = (ushort)this.player.Attributes[Stats.MaximumAbility];
            }

            writer.Commit();
        }
    }
}