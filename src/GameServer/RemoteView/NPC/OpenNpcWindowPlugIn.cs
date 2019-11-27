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
    using MUnique.OpenMU.Network.Packets.ServerToClient;
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
            using var writer = this.player.Connection.StartSafeWrite(NpcWindowResponse.HeaderType, NpcWindowResponse.Length);
            _ = new NpcWindowResponse(writer.Span)
            {
                Window = Convert(window),
            };
            writer.Commit();
        }

        private static NpcWindowResponse.NpcWindow Convert(NpcWindow window)
        {
            return window switch
            {
                NpcWindow.Merchant => NpcWindowResponse.NpcWindow.Merchant,
                NpcWindow.Merchant1 => NpcWindowResponse.NpcWindow.Merchant1,
                NpcWindow.VaultStorage => NpcWindowResponse.NpcWindow.VaultStorage,
                NpcWindow.ChaosMachine => NpcWindowResponse.NpcWindow.ChaosMachine,
                NpcWindow.DevilSquare => NpcWindowResponse.NpcWindow.DevilSquare,
                NpcWindow.BloodCastle => NpcWindowResponse.NpcWindow.BloodCastle,
                NpcWindow.PetTrainer => NpcWindowResponse.NpcWindow.PetTrainer,
                NpcWindow.Lahap => NpcWindowResponse.NpcWindow.Lahap,
                NpcWindow.CastleSeniorNPC => NpcWindowResponse.NpcWindow.CastleSeniorNPC,
                NpcWindow.ElphisRefinery => NpcWindowResponse.NpcWindow.ElphisRefinery,
                NpcWindow.RefineStoneMaking => NpcWindowResponse.NpcWindow.RefineStoneMaking,
                NpcWindow.RemoveJohOption => NpcWindowResponse.NpcWindow.RemoveJohOption,
                NpcWindow.IllusionTemple => NpcWindowResponse.NpcWindow.IllusionTemple,
                NpcWindow.ChaosCardCombination => NpcWindowResponse.NpcWindow.ChaosCardCombination,
                NpcWindow.CherryBlossomBranchesAssembly => NpcWindowResponse.NpcWindow.CherryBlossomBranchesAssembly,
                NpcWindow.SeedMaster => NpcWindowResponse.NpcWindow.SeedMaster,
                NpcWindow.SeedResearcher => NpcWindowResponse.NpcWindow.SeedResearcher,
                NpcWindow.StatReInitializer => NpcWindowResponse.NpcWindow.StatReInitializer,
                NpcWindow.DelgadoLuckyCoinRegistration => NpcWindowResponse.NpcWindow.DelgadoLuckyCoinRegistration,
                NpcWindow.DoorkeeperTitusDuelWatch => NpcWindowResponse.NpcWindow.DoorkeeperTitusDuelWatch,
                NpcWindow.LugardDoppelgangerEntry => NpcWindowResponse.NpcWindow.LugardDoppelgangerEntry,
                NpcWindow.JerintGaionEvententry => NpcWindowResponse.NpcWindow.JerintGaionEvententry,
                NpcWindow.JuliaWarpMarketServer => NpcWindowResponse.NpcWindow.JuliaWarpMarketServer,
                NpcWindow.CombineLuckyItem => NpcWindowResponse.NpcWindow.CombineLuckyItem,
                NpcWindow.GuildMaster => throw new ArgumentException("guild master dialog is opened by another action."),
                _ => throw new ArgumentException($"Unhandled case {window}."),
            };
        }
    }
}