// <copyright file="AssignPlayersToGensPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Gens;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using GensType = MUnique.OpenMU.DataModel.Entities.GensType;

/// <summary>
/// The default implementation of the <see cref="IAssignPlayersToGensPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AssignPlayersToGensPlugIn_Name), Description = nameof(PlugInResources.AssignPlayersToGensPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E1A74C28-5B9D-4F36-8C02-D6F3B9A1E785")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class AssignPlayersToGensPlugIn : IAssignPlayersToGensPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignPlayersToGensPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AssignPlayersToGensPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask AssignPlayersToGensAsync(ICollection<Player> players)
    {
        var connection = this._player.Connection;
        if (connection is null || players.Count == 0)
        {
            return;
        }

        int Write()
        {
            var size = AssignCharactersToGensRef.GetRequiredSize(players.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AssignCharactersToGensRef(span)
            {
                PlayerCount = (byte)players.Count,
            };

            var i = 0;
            foreach (var player in players)
            {
                this.SetGensPlayerBlock(packet[i], player);
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private void SetGensPlayerBlock(AssignCharactersToGensRef.GensMemberRelationRef playerBlock, Player player)
    {
        playerBlock.PlayerId = player.GetId(this._player);
        if (player.GensMember is { Gens: not GensType.None } member)
        {
            playerBlock.GensType = member.Gens.ToPacketGensType();
            playerBlock.RankingPosition = (uint)member.RankingPosition;
            playerBlock.Rank = member.Rank;
            playerBlock.ContributionPoints = (uint)Math.Max(member.Contribution, 0);
        }
        else
        {
            playerBlock.GensType = Network.Packets.ServerToClient.GensType.Undefined;
            playerBlock.RankingPosition = 0;
            playerBlock.Rank = 0;
            playerBlock.ContributionPoints = 0;
        }
    }
}
