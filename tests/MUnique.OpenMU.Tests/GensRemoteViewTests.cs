// <copyright file="GensRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameServer.RemoteView.Gens;
using MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// Tests for the packets of the gens view plugins, which have to match the (not packed) structures of the client.
/// </summary>
[TestFixture]
public class GensRemoteViewTests
{
    /// <summary>
    /// Tests the results of the join and leave requests.
    /// </summary>
    [Test]
    public async Task ResultsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new GensViewPlugIn(player);

        await view.ShowJoinResultAsync(GensJoinResult.Success, GensType.Vanert).ConfigureAwait(false);
        await view.ShowJoinResultAsync(GensJoinResult.GuildMaster, GensType.Duprian).ConfigureAwait(false);
        await view.ShowLeaveResultAsync(GensLeaveResult.DifferentGensNpc).ConfigureAwait(false);
        await view.ShowRewardResultAsync(GensRewardResult.NotEligible).ConfigureAwait(false);
        await view.ShowRewardResultAsync(GensRewardResult.DifferentGensNpc).ConfigureAwait(false);
        await view.ShowRewardResultAsync(GensRewardResult.NotJoined).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 6, 0xF8, 0x02, 0, 2,
            0xC1, 6, 0xF8, 0x02, 5, 1,
            0xC1, 5, 0xF8, 0x04, 3,
            0xC1, 5, 0xF8, 0x0A, 2,
            0xC1, 5, 0xF8, 0x0A, 5,
            0xC1, 5, 0xF8, 0x0A, 6,
        }));
    }

    /// <summary>
    /// Tests the gens info of the own player, with the integer fields at the offsets 8, 12, 16 and 20.
    /// </summary>
    [Test]
    public async Task GensInfoAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new GensViewPlugIn(player);

        await view.ShowGensInfoAsync().ConfigureAwait(false);
        player.GensMember = new GensMember { Gens = GensType.Duprian, RankingPosition = 300, Rank = 9, Contribution = 10_000 };
        await view.ShowGensInfoAsync().ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(48));
        Assert.That(data[..5], Is.EqualTo(new byte[] { 0xC1, 24, 0xF8, 0x07, 0 }));
        Assert.That(data[8..24], Is.EqualTo(new byte[16]));

        var info = data[24..];
        Assert.That(info[..5], Is.EqualTo(new byte[] { 0xC1, 24, 0xF8, 0x07, 1 }));
        Assert.That(info[8..], Is.EqualTo(new byte[] { 0x2C, 0x01, 0, 0, 9, 0, 0, 0, 0x10, 0x27, 0, 0, 0, 0, 0, 0 }));
    }

    /// <summary>
    /// Tests the gens assignment of the players in the view, with entries of 16 bytes.
    /// A player which is not member of a gens is sent without gens, so that its gens mark is removed.
    /// </summary>
    [Test]
    public async Task AssignPlayersToGensAsync()
    {
        var (observer, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var member = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        member.Id = 0x1234;
        member.GensMember = new GensMember { Gens = GensType.Vanert, RankingPosition = 5, Rank = 2, Contribution = 300 };
        var formerMember = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        formerMember.Id = 0x0102;
        formerMember.GensMember = new GensMember { Gens = GensType.None };

        await new AssignPlayersToGensPlugIn(observer).AssignPlayersToGensAsync([member, formerMember]).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(6 + (2 * 16)));
        Assert.That(data[..6], Is.EqualTo(new byte[] { 0xC2, 0, 38, 0xF8, 0x05, 2 }));

        var first = data[6..22];
        Assert.That(first[..3], Is.EqualTo(new byte[] { 2, 0x12, 0x34 }));
        Assert.That(first[4..], Is.EqualTo(new byte[] { 5, 0, 0, 0, 2, 0, 0, 0, 0xFF, 0, 0, 0 }), "The client keeps the contribution as a byte.");

        var second = data[22..];
        Assert.That(second[..3], Is.EqualTo(new byte[] { 0, 0x01, 0x02 }));
        Assert.That(second[4..], Is.EqualTo(new byte[12]));
    }
}
