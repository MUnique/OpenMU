// <copyright file="UpdateCharacterStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

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
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void UpdateCharacterStats()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        using (var writer = connection.StartSafeWrite(CharacterInformation.HeaderType, CharacterInformation.Length))
        {
            _ = new CharacterInformation(writer.Span)
            {
                X = this._player.Position.X,
                Y = this._player.Position.Y,
                MapId = ShortExtensions.ToUnsigned(this._player.SelectedCharacter!.CurrentMap!.Number),
                CurrentExperience = (ulong)this._player.SelectedCharacter.Experience,
                ExperienceForNextLevel = (ulong)this._player.GameServerContext.Configuration.ExperienceTable![(int)this._player.Attributes![Stats.Level] + 1],
                LevelUpPoints = (ushort)this._player.SelectedCharacter.LevelUpPoints,
                Strength = (ushort)this._player.Attributes[Stats.BaseStrength],
                Agility = (ushort)this._player.Attributes[Stats.BaseAgility],
                Vitality = (ushort)this._player.Attributes[Stats.BaseVitality],
                Energy = (ushort)this._player.Attributes[Stats.BaseEnergy],
                CurrentHealth = (ushort)this._player.Attributes[Stats.CurrentHealth],
                MaximumHealth = (ushort)this._player.Attributes[Stats.MaximumHealth],
                CurrentMana = (ushort)this._player.Attributes[Stats.CurrentMana],
                MaximumMana = (ushort)this._player.Attributes[Stats.MaximumMana],
                CurrentShield = (ushort)this._player.Attributes[Stats.CurrentShield],
                MaximumShield = (ushort)this._player.Attributes[Stats.MaximumShield],
                CurrentAbility = (ushort)this._player.Attributes[Stats.CurrentAbility],
                MaximumAbility = (ushort)this._player.Attributes[Stats.MaximumAbility],
                Money = (uint)this._player.Money,
                HeroState = this._player.SelectedCharacter.State.Convert(),
                Status = this._player.SelectedCharacter.CharacterStatus.Convert(),
                UsedFruitPoints = (ushort)this._player.SelectedCharacter.UsedFruitPoints,
                MaxFruitPoints = this._player.SelectedCharacter.GetMaximumFruitPoints(),
                Leadership = (ushort)this._player.Attributes[Stats.BaseLeadership],
                UsedNegativeFruitPoints = (ushort)this._player.SelectedCharacter.UsedNegFruitPoints,
                MaxNegativeFruitPoints = this._player.SelectedCharacter.GetMaximumFruitPoints(),
                IsVaultExtended = this._player.Account.IsVaultExtended,
            };

            writer.Commit();
        }

        if (this._player.SelectedCharacter.CharacterClass!.IsMasterClass)
        {
            this._player.ViewPlugIns.GetPlugIn<IUpdateMasterStatsPlugIn>()?.SendMasterStats();
        }

        this._player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
    }
}