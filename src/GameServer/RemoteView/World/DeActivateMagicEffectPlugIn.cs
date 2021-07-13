// <copyright file="DeActivateMagicEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IActivateMagicEffectPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("De & ActivateMagicEffectPlugIn", "The default implementation of the IActivateMagicEffectPlugIn and IDeactivateMagicEffectPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("67642604-8abb-44b9-a668-989cb3b28e89")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
    public class DeActivateMagicEffectPlugIn : IActivateMagicEffectPlugIn, IDeactivateMagicEffectPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeActivateMagicEffectPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public DeActivateMagicEffectPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ActivateMagicEffect(MagicEffect effect, IAttackable affectedObject)
        {
            this.SendMagicEffectStatus(effect, affectedObject, true, effect.Definition.SendDuration ? (uint)effect.Duration.TotalMilliseconds : 0);
        }

        /// <inheritdoc/>
        public void DeactivateMagicEffect(MagicEffect effect, IAttackable affectedObject)
        {
            this.SendMagicEffectStatus(effect, affectedObject, false, 0);
        }

        private void SendMagicEffectStatus(MagicEffect effect, IAttackable affectedPlayer, bool isActive, uint duration)
        {
            if (effect.Definition.Number <= 0)
            {
                return;
            }

            // TODO: Duration
            var playerId = affectedPlayer.GetId(this.player);
            this.player.Connection?.SendMagicEffectStatus(isActive, playerId, (byte)effect.Id);
        }
    }
}