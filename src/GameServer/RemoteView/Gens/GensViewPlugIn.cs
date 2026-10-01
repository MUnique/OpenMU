// <copyright file="GensViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Gens;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using GensJoinResult = MUnique.OpenMU.GameLogic.Views.Gens.GensJoinResult;
using GensLeaveResult = MUnique.OpenMU.GameLogic.Views.Gens.GensLeaveResult;
using GensRewardResult = MUnique.OpenMU.GameLogic.Views.Gens.GensRewardResult;
using GensType = MUnique.OpenMU.DataModel.Entities.GensType;

/// <summary>
/// The default implementation of the <see cref="IGensViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensViewPlugIn_Name), Description = nameof(PlugInResources.GensViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B6E3F1A9-2D47-4C85-9E06-5A8C1D7B4F23")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class GensViewPlugIn : IGensViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GensViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GensViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowJoinResultAsync(GensJoinResult result, GensType gens)
    {
        await this._player.Connection.SendGensJoinResponseAsync(Convert(result), gens.ToPacketGensType()).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowLeaveResultAsync(GensLeaveResult result)
    {
        await this._player.Connection.SendGensLeaveResponseAsync(Convert(result)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowRewardResultAsync(GensRewardResult result)
    {
        await this._player.Connection.SendGensRewardResponseAsync(Convert(result)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowGensInfoAsync()
    {
        if (this._player.GensMember is { Gens: not GensType.None } member)
        {
            await this._player.Connection.SendGensInfoAsync(
                member.Gens.ToPacketGensType(),
                (uint)member.RankingPosition,
                member.Rank,
                (uint)Math.Max(member.Contribution, 0),
                0).ConfigureAwait(false);
        }
        else
        {
            await this._player.Connection.SendGensInfoAsync(Network.Packets.ServerToClient.GensType.Undefined, 0, 0, 0, 0).ConfigureAwait(false);
        }
    }

    private static GensJoinResponse.GensJoinResult Convert(GensJoinResult result)
    {
        return result switch
        {
            GensJoinResult.Success => GensJoinResponse.GensJoinResult.Success,
            GensJoinResult.AlreadyJoined => GensJoinResponse.GensJoinResult.AlreadyJoined,
            GensJoinResult.LeftRecently => GensJoinResponse.GensJoinResult.LeftRecently,
            GensJoinResult.LevelTooLow => GensJoinResponse.GensJoinResult.LevelTooLow,
            GensJoinResult.GuildMember => GensJoinResponse.GensJoinResult.GuildInDifferentGens,
            GensJoinResult.GuildMaster => GensJoinResponse.GensJoinResult.GuildMasterNotInGens,
            GensJoinResult.InParty => GensJoinResponse.GensJoinResult.InParty,
            _ => throw new ArgumentException($"Unhandled case {result}.", nameof(result)),
        };
    }

    private static GensLeaveResponse.GensLeaveResult Convert(GensLeaveResult result)
    {
        return result switch
        {
            GensLeaveResult.Success => GensLeaveResponse.GensLeaveResult.Success,
            GensLeaveResult.NotJoined => GensLeaveResponse.GensLeaveResult.NotJoined,
            GensLeaveResult.GuildMaster => GensLeaveResponse.GensLeaveResult.GuildMasterCannotLeave,
            GensLeaveResult.DifferentGensNpc => GensLeaveResponse.GensLeaveResult.DifferentGensNpc,
            _ => throw new ArgumentException($"Unhandled case {result}.", nameof(result)),
        };
    }

    private static GensRewardResponse.GensRewardResult Convert(GensRewardResult result)
    {
        return result switch
        {
            GensRewardResult.NotEligible => GensRewardResponse.GensRewardResult.NotEligible,
            GensRewardResult.DifferentGensNpc => GensRewardResponse.GensRewardResult.DifferentGensNpc,
            GensRewardResult.NotJoined => GensRewardResponse.GensRewardResult.NotJoined,
            _ => throw new ArgumentException($"Unhandled case {result}.", nameof(result)),
        };
    }
}
