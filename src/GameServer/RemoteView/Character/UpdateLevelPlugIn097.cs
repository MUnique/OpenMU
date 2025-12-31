// <copyright file="UpdateLevelPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Level update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(UpdateLevelPlugIn097), "Level update plugin for 0.97 clients.")]
[Guid("D04B2700-3F1D-4A9E-A7F0-84D7F9D76A0F")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
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

        await connection.SendCharacterLevelUpdateAsync(
            GetUShort(charStats[Stats.Level]),
            GetUShort(Math.Max(selectedCharacter.LevelUpPoints, 0)),
            GetUShort(charStats[Stats.MaximumHealth]),
            GetUShort(charStats[Stats.MaximumMana]),
            0,
            GetUShort(charStats[Stats.MaximumAbility]),
            (ushort)selectedCharacter.UsedFruitPoints,
            selectedCharacter.GetMaximumFruitPoints(),
            (ushort)selectedCharacter.UsedNegFruitPoints,
            selectedCharacter.GetMaximumFruitPoints()).ConfigureAwait(false);

        var message = this._player.GameServerContext.Localization.GetString("Server_Message_LevelUp", (int)charStats[Stats.Level]);
        await this._player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask UpdateMasterLevelAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static ushort GetUShort(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }

}
