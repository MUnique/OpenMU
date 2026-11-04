// <copyright file="GensChatTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Tests for the chat messages to the gens (prefix <c>$</c>).
/// </summary>
[TestFixture]
public class GensChatTest
{
    private const string Message = "$Hello";

    /// <summary>
    /// Tests that a message to the gens reaches the members of the own gens, but not the ones of the other gens.
    /// </summary>
    [Test]
    public async Task MessageReachesTheOwnGensAsync()
    {
        var gameContext = await CreateGameContextAsync().ConfigureAwait(false);
        var sender = await CreateMemberAsync(gameContext, GensType.Duprian).ConfigureAwait(false);
        var friend = await CreateMemberAsync(gameContext, GensType.Duprian).ConfigureAwait(false);
        var enemy = await CreateMemberAsync(gameContext, GensType.Vanert).ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, Message, false).ConfigureAwait(false);

        VerifyMessages(sender, Times.Once());
        VerifyMessages(friend, Times.Once());
        VerifyMessages(enemy, Times.Never());
    }

    /// <summary>
    /// Tests that a player which isn't a gens member can't send a message to a gens.
    /// </summary>
    [Test]
    public async Task NonMemberCantSendAsync()
    {
        var gameContext = await CreateGameContextAsync().ConfigureAwait(false);
        var sender = await CreateMemberAsync(gameContext, GensType.None).ConfigureAwait(false);
        var member = await CreateMemberAsync(gameContext, GensType.Duprian).ConfigureAwait(false);

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, Message, false).ConfigureAwait(false);

        VerifyMessages(member, Times.Never());
    }

    /// <summary>
    /// Tests that the message to the gens gets a second prefix for the game client, which removes two characters of it.
    /// </summary>
    [Test]
    public async Task ClientGetsTwoPrefixCharactersAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new ChatViewPlugIn(player).ChatMessageAsync(Message, "Sender", ChatMessageType.Gens).ConfigureAwait(false);

        var text = System.Text.Encoding.UTF8.GetString(output.ToArray());
        Assert.That(text, Does.Contain("$" + Message));
    }

    private static void VerifyMessages(Player player, Times times)
    {
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()!);
        view.Verify(v => v.ChatMessageAsync(Message, It.IsAny<string>(), ChatMessageType.Gens), times);
    }

    private static async ValueTask<GameContext> CreateGameContextAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = (GameContext)player.GameContext;
        gameContext.FeaturePlugIns.AddPlugIn(new GensFeaturePlugIn { Configuration = new GensConfiguration() }, true);
        return gameContext;
    }

    private static async ValueTask<Player> CreateMemberAsync(GameContext gameContext, GensType gens)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.GensMember = new GensMember { Gens = gens };
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }
}
