// <copyright file="UpdateCharacterStatsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
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
    public async ValueTask UpdateCharacterStatsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        await connection.SendCharacterInformation075Async(
            this._player.Position.X,
            this._player.Position.Y,
            (byte)this._player.SelectedCharacter!.CurrentMap!.Number,
            (uint)this._player.SelectedCharacter.Experience,
            (uint)this._player.GameServerContext.ExperienceTable[(int)this._player.Attributes![Stats.Level] + 1],
            (ushort)Math.Max(this._player.SelectedCharacter.LevelUpPoints, 0),
            (ushort)this._player.Attributes[Stats.BaseStrength],
            (ushort)this._player.Attributes[Stats.BaseAgility],
            (ushort)this._player.Attributes[Stats.BaseVitality],
            (ushort)this._player.Attributes[Stats.BaseEnergy],
            (ushort)this._player.Attributes[Stats.CurrentHealth],
            (ushort)this._player.Attributes[Stats.MaximumHealth],
            (ushort)this._player.Attributes[Stats.CurrentMana],
            (ushort)this._player.Attributes[Stats.MaximumMana],
            (uint)this._player.Money,
            this._player.SelectedCharacter.State.Convert(),
            this._player.SelectedCharacter.CharacterStatus.Convert())
            .ConfigureAwait(false);

        await this._player.InvokeViewPlugInAsync<IApplyKeyConfigurationPlugIn>(p => p.ApplyKeyConfigurationAsync()).ConfigureAwait(false);
    }
}