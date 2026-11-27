// <copyright file="DuelHealthUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelHealthUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelHealthUpdatePlugIn_Name), Description = nameof(PlugInResources.DuelHealthUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("40CE7F73-F9DF-4F4E-BBCE-04938604A72C")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelHealthUpdatePlugIn : IDuelHealthUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelHealthUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelHealthUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateHealthAsync(DuelRoom duelRoom)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var player1 = this._player == duelRoom.Opponent ? duelRoom.Opponent : duelRoom.Requester;
        var player2 = this._player == duelRoom.Opponent ? duelRoom.Requester : duelRoom.Opponent;

        if (player1.Attributes is not { } player1Attributes || player2.Attributes is not { } player2Attributes)
        {
            return;
        }

        var player1Health = CalculatePercentage(player1Attributes, Stats.CurrentHealth, Stats.MaximumHealth);
        var player1Shield = CalculatePercentage(player1Attributes, Stats.CurrentShield, Stats.MaximumShield);
        var player2Health = CalculatePercentage(player2Attributes, Stats.CurrentHealth, Stats.MaximumHealth);
        var player2Shield = CalculatePercentage(player2Attributes, Stats.CurrentShield, Stats.MaximumShield);

        await connection.SendDuelHealthUpdateAsync(
                player1.GetId(this._player),
                player2.GetId(this._player),
                player1Health,
                player2Health,
                player1Shield,
                player2Shield)
            .ConfigureAwait(false);
    }

    private static byte CalculatePercentage(IAttributeSystem attributes, AttributeDefinition currentStat, AttributeDefinition maximumStat)
    {
        var maximum = attributes[maximumStat];
        var current = attributes[currentStat];
        if (maximum <= 0 || float.IsNaN(maximum) || float.IsNaN(current))
        {
            return 0;
        }

        return (byte)Math.Clamp((double)current * 100.0 / maximum, 0.0, 100.0);
    }
}