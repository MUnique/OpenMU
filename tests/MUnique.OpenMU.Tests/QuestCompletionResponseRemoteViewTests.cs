// <copyright file="QuestCompletionResponseRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Quest;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the packets which the remote view sends for a refused quest completion.
/// </summary>
[TestFixture]
public class QuestCompletionResponseRemoteViewTests
{
    /// <summary>
    /// Tests that a refusal of a newer quest is sent as the quest completion response with the result which the
    /// client shows: 3 for a full inventory, 2 for the Zen limit.
    /// </summary>
    /// <param name="refusal">The refusal.</param>
    /// <param name="expectedResult">The expected result of the response.</param>
    [TestCase(QuestCompletionRefusal.InventoryFull, QuestCompletionResponse.QuestCompletionResult.InventoryFull)]
    [TestCase(QuestCompletionRefusal.ZenLimitReached, QuestCompletionResponse.QuestCompletionResult.ZenLimitReached)]
    public async ValueTask RefusalIsSentWithItsResultAsync(QuestCompletionRefusal refusal, QuestCompletionResponse.QuestCompletionResult expectedResult)
    {
        var (player, output) = PlayerTestHelper.CreateRemotePlayer();
        var quest = new BasicModel.QuestDefinition { Group = 5, Number = 7 };

        await new QuestCompletionResponsePlugIn(player).QuestCompletionRefusedAsync(quest, refusal).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(QuestCompletionResponse.Length));
        QuestCompletionResponse packet = data.AsMemory();
        Assert.That(packet.Header.Code, Is.EqualTo(QuestCompletionResponse.Code));
        Assert.That(packet.Header.SubCode, Is.EqualTo(QuestCompletionResponse.SubCode));
        Assert.That(packet.QuestGroup, Is.EqualTo(5));
        Assert.That(packet.QuestNumber, Is.EqualTo(7));
        Assert.That(packet.Result, Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// Tests that a refusal of a legacy quest sends only a blue server message with the reason, and no quest state
    /// response, which has no result for it.
    /// </summary>
    /// <param name="refusal">The refusal.</param>
    /// <param name="expectedMessageKey">The key of the expected message.</param>
    [TestCase(QuestCompletionRefusal.InventoryFull, nameof(PlayerMessage.InventoryFull))]
    [TestCase(QuestCompletionRefusal.ZenLimitReached, nameof(PlayerMessage.QuestZenLimitReached))]
    public async ValueTask LegacyRefusalSendsOnlyTheMessageAsync(QuestCompletionRefusal refusal, string expectedMessageKey)
    {
        var (player, output) = PlayerTestHelper.CreateRemotePlayer();
        player.GameContext.PlugInManager.RegisterPlugIn<IShowMessagePlugIn, ShowMessagePlugIn>();
        var quest = new BasicModel.QuestDefinition { Group = 0, Number = 3 };

        await new QuestCompletionResponsePlugIn(player).QuestCompletionRefusedAsync(quest, refusal).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Is.Not.Empty);
        Assert.That(data[0], Is.EqualTo(ServerMessage.HeaderType));
        Assert.That(data[1], Is.EqualTo(data.Length), "only one packet is sent");
        Assert.That(data[2], Is.EqualTo(ServerMessage.Code));
        ServerMessage packet = data.AsMemory();
        Assert.That(packet.Type, Is.EqualTo(ServerMessage.MessageType.BlueNormal));
        Assert.That(packet.Message, Is.EqualTo(player.GetLocalizedMessage(expectedMessageKey)));
        Assert.That(packet.Message, Is.Not.Empty);
    }
}
