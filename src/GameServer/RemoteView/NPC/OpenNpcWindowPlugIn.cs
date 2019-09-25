// <copyright file="OpenNpcWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IOpenNpcWindowPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("OpenNpcWindowPlugIn", "The default implementation of the IOpenNpcWindowPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("3d77d6ef-479c-45f6-8ec9-a7bb046d306a")]
    public class OpenNpcWindowPlugIn : IOpenNpcWindowPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenNpcWindowPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public OpenNpcWindowPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void OpenNpcWindow(NpcWindow window)
        {
            if (window == NpcWindow.NpcDialog)
            {
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0C))
                {
                    var packet = writer.Span;
                    packet[2] = 0xF9;
                    packet[3] = 0x01; // success
                    packet.Slice(4).SetShortBigEndian(this.player.OpenedNpc.Definition.Number.ToUnsigned());
                    writer.Commit();
                }
            }
            else
            {
                using (var writer = this.player.Connection.StartSafeWrite(0xC3, 0x0B))
                {
                    var packet = writer.Span;
                    packet[2] = 0x30;
                    packet[3] = this.GetWindowIdOf(window);
                    writer.Commit();
                }
            }
        }

        private byte GetWindowIdOf(NpcWindow window)
        {
            switch (window)
            {
                case NpcWindow.Merchant: return 0;
                case NpcWindow.Merchant1: return 1;
                case NpcWindow.VaultStorage: return 2;
                case NpcWindow.ChaosMachine: return 3;
                case NpcWindow.DevilSquare: return 4;
                case NpcWindow.BloodCastle: return 6;
                case NpcWindow.PetTrainer: return 7;
                case NpcWindow.Lahap: return 9;
                case NpcWindow.CastleSeniorNPC: return 0x0C;
                case NpcWindow.ElphisRefinery: return 0x11;
                case NpcWindow.RefineStoneMaking: return 0x12;
                case NpcWindow.RemoveJohOption: return 0x13;
                case NpcWindow.IllusionTemple: return 0x14;
                case NpcWindow.ChaosCardCombination: return 0x15;
                case NpcWindow.CherryBlossomBranchesAssembly: return 0x16;
                case NpcWindow.SeedMaster: return 0x17;
                case NpcWindow.SeedResearcher: return 0x18;
                case NpcWindow.StatReInitializer: return 0x19;
                case NpcWindow.DelgadoLuckyCoinRegistration: return 0x20;
                case NpcWindow.DoorkeeperTitusDuelWatch: return 0x21;
                case NpcWindow.LugardDoppelgangerEntry: return 0x23;
                case NpcWindow.JerintGaionEvententry: return 0x24;
                case NpcWindow.JuliaWarpMarketServer: return 0x25;
                case NpcWindow.CombineLuckyItem: return 0x26;
                case NpcWindow.GuildMaster: throw new ArgumentException("guild master dialog is opened by another action.");
                case NpcWindow.NpcDialog: throw new ArgumentException("The quest dialog is opened by another action");
                default: return (byte)window;
            }
        }
    }
}