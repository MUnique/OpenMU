// <copyright file="CrywolfRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests the packets of the <see cref="CrywolfEventViewPlugIn"/>, which have to match the (not packed) structures of the client.
/// </summary>
[TestFixture]
public class CrywolfRemoteViewTests
{
    /// <summary>
    /// Tests the packets of the state, the altars, the contract, the time and the boss monsters.
    /// </summary>
    [Test]
    public async Task EventPacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CrywolfEventViewPlugIn(player);

        await view.ShowStateAsync(CrywolfOccupationState.War, CrywolfState.Ready).ConfigureAwait(false);
        await view.ShowStatueAndAltarsAsync(75, [0x12, 0x21, 0x02, 0x30, 0x01]).ConfigureAwait(false);
        await view.ShowContractResultAsync(true, 2, 0x21).ConfigureAwait(false);
        await view.ShowRemainingTimeAsync(TimeSpan.FromSeconds(880)).ConfigureAwait(false);
        await view.ShowBossMonsterInfoAsync(-1, 12).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(6 + 16 + 8 + 6 + 12));

        Assert.That(data[..6], Is.EqualTo(new byte[] { 0xC1, 6, 0xBD, 0x00, 2, 3 }));
        // The padding at the end isn't checked, because the client ignores it.
        Assert.That(data[6..19], Is.EqualTo(new byte[] { 0xC1, 16, 0xBD, 0x02, 75, 0, 0, 0, 0x12, 0x21, 0x02, 0x30, 0x01 }));

        // The client uses the key minus 317 as index of the altar.
        Assert.That(data[22..30], Is.EqualTo(new byte[] { 0xC1, 8, 0xBD, 0x03, 1, 0x21, 0x01, 0x3F }));

        // 880 seconds are 14 started minutes; the client counts down the seconds by itself.
        Assert.That(data[30..36], Is.EqualTo(new byte[] { 0xC1, 6, 0xBD, 0x04, 0, 14 }));
        Assert.That(data[36..45], Is.EqualTo(new byte[] { 0xC1, 12, 0xBD, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 12 }));
    }

    /// <summary>
    /// Tests the packets of the result: the personal rank and the heroes.
    /// </summary>
    [Test]
    public async Task ResultPacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CrywolfEventViewPlugIn(player);

        await view.ShowPersonalRankAsync(4, 1_800_000).ConfigureAwait(false);
        await view.ShowHeroListAsync([new CrywolfHero("Hero", 10500, 8), new CrywolfHero("Second", 3000, 4)]).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(12 + 5 + (2 * 20)));

        var rank = (CrywolfPersonalRank)data.AsMemory(0, 12);
        Assert.That(data[..4], Is.EqualTo(new byte[] { 0xC1, 12, 0xBD, 0x07 }));
        Assert.That(rank.Rank, Is.EqualTo(CrywolfPersonalRank.CrywolfRank.S));
        Assert.That(rank.Experience, Is.EqualTo(1_800_000));

        var heroes = data.AsSpan(12);
        Assert.That(heroes[..5].ToArray(), Is.EqualTo(new byte[] { 0xC1, 45, 0xBD, 0x08, 2 }));
        var first = heroes[5..25];
        Assert.That(first[0], Is.EqualTo(0), "The rank is the index of the hero.");
        Assert.That(System.Text.Encoding.UTF8.GetString(first[1..5]), Is.EqualTo("Hero"));
        Assert.That(BitConverter.ToInt32(first[12..16]), Is.EqualTo(10500));
        Assert.That(first[16], Is.EqualTo(8));
        Assert.That(heroes[25], Is.EqualTo(1), "The second hero has the rank 1.");
    }
}
