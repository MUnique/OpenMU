// <copyright file="ApplyKeyConfigurationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IApplyKeyConfigurationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ApplyKeyConfigurationPlugIn", "The default implementation of the IUpdateVaultMoneyPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("6DFF0BF8-2E35-4C1D-9778-3406FCFB4716")]
    public class ApplyKeyConfigurationPlugIn : IApplyKeyConfigurationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyKeyConfigurationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ApplyKeyConfigurationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ApplyKeyConfiguration()
        {
            var keyConfiguration = this.player.SelectedCharacter.KeyConfiguration;
            if (keyConfiguration == null || keyConfiguration.Length == 0)
            {
                return;
            }

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 4 + keyConfiguration.Length))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x30;
                keyConfiguration.AsSpan().CopyTo(packet.Slice(4));
                writer.Commit();
            }
        }
    }
}