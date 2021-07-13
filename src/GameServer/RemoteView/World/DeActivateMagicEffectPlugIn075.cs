// <copyright file="DeActivateMagicEffectPlugIn075.cs" company="MUnique">
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
    [PlugIn(nameof(DeActivateMagicEffectPlugIn075), "The default implementation of the IActivateMagicEffectPlugIn and IDeactivateMagicEffectPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("3CF8BFBA-FBDA-431D-9806-86B3DEC3AD54")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class DeActivateMagicEffectPlugIn075 : IDeactivateMagicEffectPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeActivateMagicEffectPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public DeActivateMagicEffectPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void DeactivateMagicEffect(MagicEffect effect, IAttackable affectedObject)
        {
            if (effect.Definition.Number <= 0)
            {
                return;
            }

            // We assume, that the magic effect number is equal to the skill number.
            // In early versions, the magic effect number is not used elsewhere.
            this.player.Connection.SendMagicEffectCancelled075((byte)effect.Definition.Number, affectedObject.GetId(this.player));
        }
    }
}