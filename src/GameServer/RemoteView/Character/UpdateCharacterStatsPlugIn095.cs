// <copyright file="UpdateCharacterStatsPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(UpdateCharacterStatsPlugIn095), "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("8ACD9D6B-6FA7-42C3-8C07-E137655CB92F")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
    public class UpdateCharacterStatsPlugIn095 : IUpdateCharacterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn095"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterStatsPlugIn095(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            var connection = this.player.Connection;
            if (connection is null || this.player.Account is null)
            {
                return;
            }

            using (var writer = connection.StartSafeWrite(CharacterInformation095.HeaderType, CharacterInformation095.Length))
            {
                _ = new CharacterInformation095(writer.Span)
                {
                    X = this.player.Position.X,
                    Y = this.player.Position.Y,
                    MapId = (byte)this.player.SelectedCharacter!.CurrentMap!.Number,
                    CurrentExperience = (uint)this.player.SelectedCharacter.Experience,
                    ExperienceForNextLevel = (uint)this.player.GameServerContext.Configuration.ExperienceTable![(int)this.player.Attributes![Stats.Level] + 1],
                    LevelUpPoints = (ushort)this.player.SelectedCharacter.LevelUpPoints,
                    Strength = (ushort)this.player.Attributes[Stats.BaseStrength],
                    Agility = (ushort)this.player.Attributes[Stats.BaseAgility],
                    Vitality = (ushort)this.player.Attributes[Stats.BaseVitality],
                    Energy = (ushort)this.player.Attributes[Stats.BaseEnergy],
                    CurrentHealth = (ushort)this.player.Attributes[Stats.CurrentHealth],
                    MaximumHealth = (ushort)this.player.Attributes[Stats.MaximumHealth],
                    CurrentMana = (ushort)this.player.Attributes[Stats.CurrentMana],
                    MaximumMana = (ushort)this.player.Attributes[Stats.MaximumMana],
                    CurrentAbility = (ushort)this.player.Attributes[Stats.CurrentAbility],
                    MaximumAbility = (ushort)this.player.Attributes[Stats.MaximumAbility],
                    Money = (uint)this.player.Money,
                    HeroState = this.player.SelectedCharacter.State.Convert(),
                    Status = this.player.SelectedCharacter.CharacterStatus.Convert(),
                    UsedFruitPoints = (ushort)this.player.SelectedCharacter.UsedFruitPoints,
                    MaxFruitPoints = this.player.SelectedCharacter.GetMaximumFruitPoints(),
                    Leadership = (ushort)this.player.Attributes[Stats.BaseLeadership],
                };

                writer.Commit();
            }

            if (this.player.SelectedCharacter.CharacterClass!.IsMasterClass)
            {
                this.player.ViewPlugIns.GetPlugIn<IUpdateMasterStatsPlugIn>()?.SendMasterStats();
            }

            this.player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
        }
    }
}