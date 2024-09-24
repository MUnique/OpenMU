// <copyright file="UpdateMasterStatsExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="IUpdateMasterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateMasterStatsExtendedPlugIn), "The extended implementation of the IUpdateMasterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("EF19E11B-AE26-44F0-AB3E-5ADD5CDEBC56")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateMasterStatsExtendedPlugIn : IUpdateMasterStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMasterStatsExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMasterStatsExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask SendMasterStatsAsync()
    {
        var character = this._player.SelectedCharacter;
        var connection = this._player.Connection;
        if (character is null || this._player.Attributes is null || connection is null)
        {
            return;
        }

        await connection.SendMasterStatsUpdateExtendedAsync(
            (ushort)this._player.Attributes[Stats.MasterLevel],
            (ulong)character.MasterExperience,
            (ulong)this._player.GameServerContext.MasterExperienceTable[(int)this._player.Attributes[Stats.MasterLevel] + 1],
            (ushort)character.MasterLevelUpPoints,
            (ushort)this._player.Attributes[Stats.MaximumHealth],
            (ushort)this._player.Attributes[Stats.MaximumMana],
            (ushort)this._player.Attributes[Stats.MaximumShield],
            (ushort)this._player.Attributes[Stats.MaximumAbility]).ConfigureAwait(false);

        await this._player.InvokeViewPlugInAsync<IUpdateMasterSkillsPlugIn>(p => p.UpdateMasterSkillsAsync()).ConfigureAwait(false);
    }
}