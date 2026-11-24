// <copyright file="QuestRewardCountTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameServer.RemoteView.Quest;

/// <summary>
/// Tests the count of a quest reward which is shown in the quest window of the client,
/// see <see cref="QuestStructExtensions.GetRewardCount"/>.
/// </summary>
[TestFixture]
public class QuestRewardCountTest
{
    /// <summary>
    /// An item reward gives one item, so the client shows a count of 1 instead of the unused value 0.
    /// </summary>
    [Test]
    public void ItemRewardHasCountOne()
    {
        var reward = new QuestReward { RewardType = QuestRewardType.Item, Value = 0 };

        Assert.That(reward.GetRewardCount(), Is.EqualTo(1));
    }

    /// <summary>
    /// Other rewards show their value, e.g. the amount of experience or money.
    /// </summary>
    [Test]
    public void OtherRewardHasItsValue()
    {
        var reward = new QuestReward { RewardType = QuestRewardType.Money, Value = 50000 };

        Assert.That(reward.GetRewardCount(), Is.EqualTo(50000));
    }
}
