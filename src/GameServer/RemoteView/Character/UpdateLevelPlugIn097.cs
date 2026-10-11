// <copyright file="UpdateLevelPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using PlugInResources = MUnique.OpenMU.GameServer.Properties.PlugInResources;

/// <summary>
/// The implementation of the <see cref="IUpdateLevelPlugIn"/> for the clients of version 0.97,
/// which don't know the shield, so the packet has no field for its maximum value.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateLevelPlugIn097_Name), Description = nameof(PlugInResources.UpdateLevelPlugIn097_Description), ResourceType = typeof(PlugInResources))]
[Guid("EE957A73-E155-4877-BAB7-5A49BE2EEF74")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 255, ClientLanguage.Invariant)]
public class UpdateLevelPlugIn097 : IUpdateLevelPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLevelPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateLevelPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateLevelAsync()
    {
        var selectedCharacter = this._player.SelectedCharacter;
        var charStats = this._player.Attributes;
        var connection = this._player.Connection;
        if (selectedCharacter is null || charStats is null || connection is null)
        {
            return;
        }

        await connection.SendCharacterLevelUpdate097Async(
            (ushort)charStats[Stats.Level],
            (ushort)Math.Max(selectedCharacter.LevelUpPoints, 0),
            charStats[Stats.MaximumHealth].ToUInt16Clamped(),
            charStats[Stats.MaximumMana].ToUInt16Clamped(),
            charStats[Stats.MaximumAbility].ToUInt16Clamped(),
            (ushort)selectedCharacter.UsedFruitPoints,
            selectedCharacter.GetMaximumFruitPoints(),
            (ushort)selectedCharacter.UsedNegFruitPoints,
            selectedCharacter.GetMaximumFruitPoints()).ConfigureAwait(false);

        await this._player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LevelUpCongrats), charStats[Stats.Level]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <remarks>The client of version 0.97 doesn't know master levels.</remarks>
    public ValueTask UpdateMasterLevelAsync() => ValueTask.CompletedTask;
}
