// <copyright file="WeeklyQuestListViewPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests for the <see cref="WeeklyQuestListViewPlugIn"/>.
/// </summary>
[TestFixture]
public class WeeklyQuestListViewPlugInTest
{
    /// <summary>
    /// Tests that one message is sent per quest, with the progress and the texts of the quest.
    /// </summary>
    [Test]
    public async Task SendsOneMessagePerQuestAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new WeeklyQuestListViewPlugIn(player);
        var overview = new WeeklyQuestOverview(
            new List<WeeklyQuestOverviewEntry>
            {
                new(CreateQuest("a", "Cazador"), 3, false, false),
                new(CreateQuest("b", "Castillo"), 5, true, true),
            },
            DateTime.UtcNow.AddHours(1));

        await view.ShowWeeklyQuestsAsync(overview).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(2 * WeeklyQuestEntry.Length));

        var first = new WeeklyQuestEntry(data.AsMemory(0, WeeklyQuestEntry.Length));
        Assert.That(first.Index, Is.EqualTo(0));
        Assert.That(first.Count, Is.EqualTo(2));
        Assert.That(first.IsUpdate, Is.False);
        Assert.That(first.Id, Is.EqualTo("a"));
        Assert.That(first.Name, Is.EqualTo("Cazador"));
        Assert.That(first.CurrentCount, Is.EqualTo(3));
        Assert.That(first.RequiredCount, Is.EqualTo(5));
        Assert.That(first.SecondsUntilReset, Is.InRange(3500u, 3600u));

        var second = new WeeklyQuestEntry(data.AsMemory(WeeklyQuestEntry.Length, WeeklyQuestEntry.Length));
        Assert.That(second.Index, Is.EqualTo(1));
        Assert.That(second.IsCompleted, Is.True);
        Assert.That(second.IsRewarded, Is.True);
    }

    /// <summary>
    /// Tests that an empty list is sent as one message with a count of 0, so that the client forgets the previous quests.
    /// </summary>
    [Test]
    public async Task SendsEmptyListAsOneMessageAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new WeeklyQuestListViewPlugIn(player);

        await view.ShowWeeklyQuestsAsync(new WeeklyQuestOverview(new List<WeeklyQuestOverviewEntry>(), DateTime.UtcNow)).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(WeeklyQuestEntry.Length));
        Assert.That(new WeeklyQuestEntry(data).Count, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that an update is marked as such.
    /// </summary>
    [Test]
    public async Task SendsUpdateAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new WeeklyQuestListViewPlugIn(player);

        await view.UpdateWeeklyQuestAsync(new(CreateQuest("a", "Cazador"), 4, false, false), DateTime.UtcNow).ConfigureAwait(false);

        var packet = new WeeklyQuestEntry(output.ToArray());
        Assert.That(packet.IsUpdate, Is.True);
        Assert.That(packet.Id, Is.EqualTo("a"));
        Assert.That(packet.CurrentCount, Is.EqualTo(4));
    }

    /// <summary>
    /// Tests the text of the rewards.
    /// </summary>
    [Test]
    public void RewardsText()
    {
        var quest = CreateQuest("a", "Cazador");
        quest.Rewards.Add(new WeeklyQuestReward { RewardType = WeeklyQuestRewardType.Experience, Amount = 2500 });

        Assert.That(quest.GetRewardsText(CultureInfo.GetCultureInfo("es-AR")), Is.EqualTo("1.000.000 Zen, 2.500 EXP"));
    }

    private static WeeklyQuestDefinition CreateQuest(string id, string name) => new()
    {
        Id = id,
        Name = name,
        Description = "Descripción",
        RequiredCount = 5,
        Rewards = new List<WeeklyQuestReward>
        {
            new() { RewardType = WeeklyQuestRewardType.Money, Amount = 1_000_000 },
        },
    };
}
