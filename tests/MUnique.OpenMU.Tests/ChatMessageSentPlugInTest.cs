// <copyright file="ChatMessageSentPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Tests for the <see cref="IChatMessageSentPlugIn"/>.
/// </summary>
[TestFixture]
public class ChatMessageSentPlugInTest
{
    /// <summary>
    /// Tests that a delivered normal chat message is reported with its type.
    /// </summary>
    [Test]
    public async Task NormalMessageIsReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "Hello", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.EqualTo(new[] { ("Hello", ChatMessageType.Normal, (Player?)null) }));
    }

    /// <summary>
    /// Tests that a delivered whisper message is reported with its receiver.
    /// </summary>
    [Test]
    public async Task WhisperMessageIsReportedWithReceiverAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);
        var receiver = await CreatePlayerAsync((GameContext)sender.GameContext, "Receiver").ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, receiver.Name, "Psst", true).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.EqualTo(new[] { ("Psst", ChatMessageType.Whisper, (Player?)receiver) }));
    }

    /// <summary>
    /// Tests that the message of a chat banned player isn't reported, because it isn't delivered.
    /// </summary>
    [Test]
    public async Task MessageOfChatBannedPlayerIsNotReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);
        sender.Account!.ChatBanUntil = DateTime.UtcNow.AddHours(1);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "Hello", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.Empty);
    }

    /// <summary>
    /// Tests that a message which got cancelled by an <see cref="IChatMessageReceivedPlugIn"/> isn't reported.
    /// </summary>
    [Test]
    public async Task CancelledMessageIsNotReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);
        var gameContext = (GameContext)sender.GameContext;
        gameContext.FeaturePlugIns.AddPlugIn(new GensFeaturePlugIn { Configuration = new GensConfiguration() }, true);
        sender.GensMember = new GensMember { Gens = GensType.Duprian };
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IChatMessageReceivedPlugIn>(new CancellingPlugIn());

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "$Hello", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.Empty);
    }

    /// <summary>
    /// Tests that a global notification of a player which isn't a game master isn't reported, because it isn't delivered.
    /// </summary>
    [Test]
    public async Task UndeliveredGlobalNotificationIsNotReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "!Hello", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.Empty);
    }

    /// <summary>
    /// Tests that a global notification of a game master is reported.
    /// </summary>
    [Test]
    public async Task GlobalNotificationOfGameMasterIsReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);
        sender.SelectedCharacter!.CharacterStatus = CharacterStatus.GameMaster;

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "!Hello", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.EqualTo(new[] { ("!Hello", ChatMessageType.GlobalNotification, (Player?)null) }));
    }

    /// <summary>
    /// Tests that chat commands aren't reported.
    /// </summary>
    [Test]
    public async Task CommandIsNotReportedAsync()
    {
        var (sender, plugIn) = await CreateSenderAsync().ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "/unknown", false).ConfigureAwait(false);

        Assert.That(plugIn.Messages, Is.Empty);
    }

    private static async ValueTask<(Player Sender, RecordingPlugIn PlugIn)> CreateSenderAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = (GameContext)player.GameContext;
        var sender = await CreatePlayerAsync(gameContext, "Sender").ConfigureAwait(false);
        var plugIn = new RecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IChatMessageSentPlugIn>(plugIn);
        return (sender, plugIn);
    }

    private static async ValueTask<Player> CreatePlayerAsync(GameContext gameContext, string name)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.Name = name;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);

        // Usually happens when the player enters the world.
        gameContext.PlayersByCharacterName.TryAdd(name, player);
        return player;
    }

    [Guid("75E2DC62-DBE7-476B-AA0A-536A85643FE5")]
    private sealed class RecordingPlugIn : IChatMessageSentPlugIn
    {
        public List<(string Message, ChatMessageType Type, Player? Receiver)> Messages { get; } = new();

        public ValueTask ChatMessageSentAsync(Player sender, string message, ChatMessageType messageType, Player? receiver)
        {
            this.Messages.Add((message, messageType, receiver));
            return ValueTask.CompletedTask;
        }
    }

    [Guid("C83D380E-FAEF-496D-A240-01EB975752E8")]
    private sealed class CancellingPlugIn : IChatMessageReceivedPlugIn
    {
        public void ChatMessageReceived(Player sender, string message, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = true;
        }
    }
}
