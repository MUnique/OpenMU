// <copyright file="UpdateCharacterStatsPlugIn.cs" company="MUnique">
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
    [PlugIn("UpdateCharacterStatsPlugIn", "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("6eb967c2-b5a2-4510-9d88-5eccc963a6ea")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class UpdateCharacterStatsPlugIn : IUpdateCharacterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterStatsPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            using var writer = this.player.Connection.StartSafeWrite(CharacterInformation.HeaderType, CharacterInformation.Length);
            _ = new CharacterInformation(writer.Span)
            {
                X = this.player.Position.X,
                Y = this.player.Position.Y,
                MapId = ShortExtensions.ToUnsigned(this.player.SelectedCharacter.CurrentMap.Number),
                CurrentExperience = (ulong)this.player.SelectedCharacter.Experience,
                ExperienceForNextLevel = (ulong)this.player.GameServerContext.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1],
                LevelUpPoints = (ushort)this.player.SelectedCharacter.LevelUpPoints,
                Strength = (ushort)this.player.Attributes[Stats.BaseStrength],
                Agility = (ushort)this.player.Attributes[Stats.BaseAgility],
                Vitality = (ushort)this.player.Attributes[Stats.BaseVitality],
                Energy = (ushort)this.player.Attributes[Stats.BaseEnergy],
                CurrentHealth = (ushort)this.player.Attributes[Stats.CurrentHealth],
                MaximumHealth = (ushort)this.player.Attributes[Stats.MaximumHealth],
                CurrentMana = (ushort)this.player.Attributes[Stats.CurrentMana],
                MaximumMana = (ushort)this.player.Attributes[Stats.MaximumMana],
                CurrentShield = (ushort)this.player.Attributes[Stats.CurrentShield],
                MaximumShield = (ushort)this.player.Attributes[Stats.MaximumShield],
                CurrentAbility = (ushort)this.player.Attributes[Stats.CurrentAbility],
                MaximumAbility = (ushort)this.player.Attributes[Stats.MaximumAbility],
                Money = (uint)this.player.Money,
                HeroState = this.player.SelectedCharacter.State.Convert(),
                Status = this.player.SelectedCharacter.CharacterStatus.Convert(),
                UsedFruitPoints = (ushort)this.player.SelectedCharacter.UsedFruitPoints,
                MaxFruitPoints = this.player.SelectedCharacter.GetMaximumFruitPoints(),
                Leadership = (ushort)this.player.Attributes[Stats.BaseLeadership],
                UsedNegativeFruitPoints = (ushort)this.player.SelectedCharacter.UsedNegFruitPoints,
                MaxNegativeFruitPoints = this.player.SelectedCharacter.GetMaximumFruitPoints(),
                IsVaultExtended = this.player.Account.IsVaultExtended,
            };

            writer.Commit();

            if (this.player.SelectedCharacter.CharacterClass.IsMasterClass)
            {
                this.player.ViewPlugIns.GetPlugIn<IUpdateMasterStatsPlugIn>()?.SendMasterStats();
            }

            this.player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
        }
    }
}
