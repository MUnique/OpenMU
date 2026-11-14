// <copyright file="WhisperReceiverNotFoundTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests that the sender of a whisper is told when the receiver isn't connected.
/// </summary>
[TestFixture]
public class WhisperReceiverNotFoundTest
{
    private CultureInfo _originalCulture = CultureInfo.CurrentCulture;

    /// <summary>
    /// Makes the player culture, which is taken from the current one, independent of the machine.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Restores the culture.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        CultureInfo.CurrentCulture = this._originalCulture;
    }

    /// <summary>
    /// A whisper to a character which isn't connected to the game server is answered to the sender.
    /// </summary>
    [Test]
    public async Task UnknownReceiverTellsTheSenderAsync()
    {
        var sender = await CreatePlayerAsync(null, "Sender").ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, "Ghost", "Psst", true).ConfigureAwait(false);

        VerifyNotFoundNotice(sender, "Character 'Ghost' not found.", Times.Once());
        Mock.Get(sender.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()!).Verify(v => v!.ChatMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChatMessageType>()), Times.Never());
    }

    /// <summary>
    /// A whisper to a connected character is delivered as before, without any notice to the sender.
    /// </summary>
    [Test]
    public async Task OnlineReceiverGetsTheWhisperAsync()
    {
        var sender = await CreatePlayerAsync(null, "Sender").ConfigureAwait(false);
        var receiver = await CreatePlayerAsync((GameContext)sender.GameContext, "Receiver").ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, "receiver", "Psst", true).ConfigureAwait(false);

        Mock.Get(receiver.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()!).Verify(v => v!.ChatMessageAsync("Psst", "Sender", ChatMessageType.Whisper), Times.Once());
        VerifyNotFoundNotice(sender, "Character 'Receiver' not found.", Times.Never());
        Mock.Get(sender.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(v => v!.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never());
    }

    /// <summary>
    /// A normal chat message to nobody in particular doesn't produce a not-found notice.
    /// </summary>
    [Test]
    public async Task NormalMessageDoesNotProduceNoticeAsync()
    {
        var sender = await CreatePlayerAsync(null, "Sender").ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, "Sender", "Hello", false).ConfigureAwait(false);

        Mock.Get(sender.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(v => v!.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never());
    }

    private static void VerifyNotFoundNotice(Player sender, string message, Times times)
    {
        Mock.Get(sender.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(v => v!.ShowMessageAsync(message, MessageType.BlueNormal), times);
    }

    private static async ValueTask<Player> CreatePlayerAsync(GameContext? gameContext, string name)
    {
        var player = gameContext is null
            ? await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false)
            : await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.Name = name;
        var context = (GameContext)player.GameContext;
        await context.AddPlayerAsync(player).ConfigureAwait(false);

        // Usually happens when the player enters the world.
        context.PlayersByCharacterName.TryAdd(name, player);
        return player;
    }
}
