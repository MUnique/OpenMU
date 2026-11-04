// <copyright file="DiscordDirectMessagesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="DiscordDirectMessages"/>.
/// </summary>
[TestFixture]
public class DiscordDirectMessagesTest
{
    private const ulong HeroUser = 42;

    private InMemoryPersistenceContextProvider _persistence = null!;
    private AccountLinkService _linkService = null!;
    private Mock<IFriendServer> _friendServer = null!;
    private DiscordDirectMessages _directMessages = null!;
    private Guid _elfId;

    /// <summary>
    /// Sets up the linked account of Hero, who is a friend of Elf.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._linkService = new AccountLinkService(this.CreateContext);
        var heroAccountId = await this.CreateAccountAsync("hero", "Hero").ConfigureAwait(false);
        await this.CreateAccountAsync("elf", "Elf").ConfigureAwait(false);
        using (var context = this.CreateContext())
        {
            this._elfId = (await context.GetAccountByCharacterNameAsync("Elf").ConfigureAwait(false))!.Characters.Single().GetId();
        }

        using (var friendContext = this._persistence.CreateNewFriendServerContext())
        {
            await friendContext.CreateNewFriendAsync("Elf", "Hero").ConfigureAwait(false);
            await friendContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var code = await this._linkService.CreateCodeAsync(heroAccountId, AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);
        await this._linkService.LinkAsync(AccountLinkService.DiscordProvider, code, HeroUser.ToString(CultureInfo.InvariantCulture), "hero").ConfigureAwait(false);

        this._friendServer = new Mock<IFriendServer>();
        this._friendServer.Setup(s => s.GetOnlineServerIdAsync("Elf")).ReturnsAsync((byte)0);
        this._directMessages = new DiscordDirectMessages(
            this._linkService,
            this._friendServer.Object,
            this.CreateContext,
            this._persistence.CreateNewFriendServerContext,
            _ => "Server 1",
            CultureInfo.GetCultureInfo("en"));
    }

    /// <summary>
    /// Tests that there are no direct messages, until the user turns them on.
    /// </summary>
    [Test]
    public async Task DirectMessagesAreOptInAsync()
    {
        var letter = new LetterReceivedEvent(0, DateTime.UtcNow, "Hero", "Elf", "Hi");
        Assert.That(await this._directMessages.GetMessagesAsync(letter).ConfigureAwait(false), Is.Empty);

        await this.TurnOnAsync(AccountNotificationTypes.LetterReceived).ConfigureAwait(false);
        var messages = await this._directMessages.GetMessagesAsync(letter).ConfigureAwait(false);

        Assert.That(messages, Has.Count.EqualTo(1));
        Assert.That(messages[0].UserId, Is.EqualTo(HeroUser));
        Assert.That(messages[0].Embed.Description, Is.EqualTo("Hero received a letter from Elf: Hi"));
    }

    /// <summary>
    /// Tests the direct message about a blocked login.
    /// </summary>
    [Test]
    public async Task LoginAttemptIsReportedAsync()
    {
        await this.TurnOnAsync(AccountNotificationTypes.LoginAttempt).ConfigureAwait(false);

        var messages = await this._directMessages.GetMessagesAsync(new AccountLoginBlockedEvent(0, DateTime.UtcNow, "hero")).ConfigureAwait(false);

        Assert.That(messages.Single().Embed.Description, Does.StartWith("Somebody tried to log into your account on Server 1"));
    }

    /// <summary>
    /// Tests that the friends of a character get a direct message when it enters the game, unless it's invisible.
    /// </summary>
    [Test]
    public async Task FriendsAreNotifiedUnlessInvisibleAsync()
    {
        await this.TurnOnAsync(AccountNotificationTypes.FriendOnline).ConfigureAwait(false);
        var entered = new PlayerEnteredGameEvent(0, DateTime.UtcNow, this._elfId, "Elf");

        var messages = await this._directMessages.GetMessagesAsync(entered).ConfigureAwait(false);
        this._friendServer.Setup(s => s.GetOnlineServerIdAsync("Elf")).ReturnsAsync((byte?)null);
        var invisibleMessages = await this._directMessages.GetMessagesAsync(entered).ConfigureAwait(false);

        Assert.That(messages.Single().Embed.Description, Is.EqualTo("Your friend Elf is online."));
        Assert.That(invisibleMessages, Is.Empty);
    }

    private Task TurnOnAsync(AccountNotificationTypes type)
    {
        return this._linkService.SetNotificationByUserAsync(AccountLinkService.DiscordProvider, HeroUser.ToString(CultureInfo.InvariantCulture), type, true).AsTask();
    }

    private IPlayerContext CreateContext() => this._persistence.CreateNewPlayerContext(new GameConfiguration());

    private async Task<Guid> CreateAccountAsync(string loginName, string characterName)
    {
        using var context = this.CreateContext();
        var account = context.CreateNew<Account>();
        account.LoginName = loginName;
        var character = context.CreateNew<Character>();
        character.Name = characterName;
        account.Characters.Add(character);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return account.GetId();
    }
}
