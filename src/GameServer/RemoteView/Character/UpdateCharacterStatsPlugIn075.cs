// <copyright file="UpdateCharacterStatsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCharacterStatsPlugIn 0.75", "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("C180561D-E055-41CA-817C-44E7A985937C")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    public class UpdateCharacterStatsPlugIn075 : IUpdateCharacterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterStatsPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            using (var writer = this.player.Connection.StartSafeWrite(CharacterInformation075.HeaderType, CharacterInformation075.Length))
            {
                _ = new CharacterInformation075(writer.Span)
                {
                    X = this.player.Position.X,
                    Y = this.player.Position.Y,
                    MapId = (byte)this.player.SelectedCharacter.CurrentMap.Number,
                    CurrentExperience = (uint)this.player.SelectedCharacter.Experience,
                    ExperienceForNextLevel = (uint)this.player.GameServerContext.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1],
                    LevelUpPoints = (ushort)this.player.SelectedCharacter.LevelUpPoints,
                    Strength = (ushort)this.player.Attributes[Stats.BaseStrength],
                    Agility = (ushort)this.player.Attributes[Stats.BaseAgility],
                    Vitality = (ushort)this.player.Attributes[Stats.BaseVitality],
                    Energy = (ushort)this.player.Attributes[Stats.BaseEnergy],
                    CurrentHealth = (ushort)this.player.Attributes[Stats.CurrentHealth],
                    MaximumHealth = (ushort)this.player.Attributes[Stats.MaximumHealth],
                    CurrentMana = (ushort)this.player.Attributes[Stats.CurrentMana],
                    MaximumMana = (ushort)this.player.Attributes[Stats.MaximumMana],
                    Money = (uint)this.player.Money,
                    HeroState = this.player.SelectedCharacter.State.Convert(),
                    Status = this.player.SelectedCharacter.CharacterStatus.Convert(),
                };
                writer.Commit();
            }

            this.player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
        }
    }
}
