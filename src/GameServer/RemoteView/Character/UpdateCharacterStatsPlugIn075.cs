// <copyright file="UpdateCharacterStatsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

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
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsPlugIn075(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void UpdateCharacterStats()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        using (var writer = connection.StartSafeWrite(CharacterInformation075.HeaderType, CharacterInformation075.Length))
        {
            _ = new CharacterInformation075(writer.Span)
            {
                X = this._player.Position.X,
                Y = this._player.Position.Y,
                MapId = (byte)this._player.SelectedCharacter!.CurrentMap!.Number,
                CurrentExperience = (uint)this._player.SelectedCharacter.Experience,
                ExperienceForNextLevel = (uint)this._player.GameServerContext.Configuration.ExperienceTable![(int)this._player.Attributes![Stats.Level] + 1],
                LevelUpPoints = (ushort)this._player.SelectedCharacter.LevelUpPoints,
                Strength = (ushort)this._player.Attributes[Stats.BaseStrength],
                Agility = (ushort)this._player.Attributes[Stats.BaseAgility],
                Vitality = (ushort)this._player.Attributes[Stats.BaseVitality],
                Energy = (ushort)this._player.Attributes[Stats.BaseEnergy],
                CurrentHealth = (ushort)this._player.Attributes[Stats.CurrentHealth],
                MaximumHealth = (ushort)this._player.Attributes[Stats.MaximumHealth],
                CurrentMana = (ushort)this._player.Attributes[Stats.CurrentMana],
                MaximumMana = (ushort)this._player.Attributes[Stats.MaximumMana],
                Money = (uint)this._player.Money,
                HeroState = this._player.SelectedCharacter.State.Convert(),
                Status = this._player.SelectedCharacter.CharacterStatus.Convert(),
            };
            writer.Commit();
        }

        this._player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
    }
}