// <copyright file="DiscordGameMasterCommandsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="DiscordGameMasterCommands"/>.
/// </summary>
[TestFixture]
public class DiscordGameMasterCommandsTest
{
    private const ulong GameMasterUser = 42;
    private const ulong PlayerUser = 43;

    private InMemoryPersistenceContextProvider _persistence = null!;
    private Mock<IGameServer> _server1 = null!;
    private Mock<IGameServer> _server2 = null!;
    private DiscordGameMasterCommands _commands = null!;

    /// <summary>
    /// Sets up a linked game master and a linked player, and two game servers.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        var linkService = new AccountLinkService(this.CreateContext);
        await this.CreateLinkedAccountAsync(linkService, "admin", "Admin", CharacterStatus.GameMaster, GameMasterUser).ConfigureAwait(false);
        await this.CreateLinkedAccountAsync(linkService, "hero", "Hero", CharacterStatus.Normal, PlayerUser).ConfigureAwait(false);

        this._server1 = new Mock<IGameServer>();
        this._server2 = new Mock<IGameServer>();
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { this._server1.Object, this._server2.Object });
        this._commands = new DiscordGameMasterCommands(serverProvider.Object, linkService, this.CreateContext, CultureInfo.GetCultureInfo("en"), NullLogger<DiscordGameMasterCommands>.Instance);
    }

    /// <summary>
    /// Tests that a command needs the GM role and a game master character.
    /// </summary>
    [Test]
    public async Task OnlyGameMastersCanUseCommandsAsync()
    {
        var withoutRole = await this._commands.ExecuteAsync("announce", "Hello", GameMasterUser, "admin", false).ConfigureAwait(false);
        var withoutGameMaster = await this._commands.ExecuteAsync("announce", "Hello", PlayerUser, "hero", true).ConfigureAwait(false);

        Assert.That(withoutRole.StaffAlert, Is.Null);
        Assert.That(withoutGameMaster.StaffAlert, Is.Null);
        Assert.That(withoutGameMaster.Embed.Description, Does.StartWith("Only game masters can use this command."));
        this._server1.Verify(s => s.SendGlobalMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never);
    }

    /// <summary>
    /// Tests that a notice is sent to all game servers, and that the staff is alerted.
    /// </summary>
    [Test]
    public async Task AnnounceSendsToAllGameServersAsync()
    {
        var answer = await this._commands.ExecuteAsync("announce", "Hello", GameMasterUser, "admin", true).ConfigureAwait(false);

        this._server1.Verify(s => s.SendGlobalMessageAsync("Hello", MessageType.GoldenCenter), Times.Once);
        this._server2.Verify(s => s.SendGlobalMessageAsync("Hello", MessageType.GoldenCenter), Times.Once);
        Assert.That(answer.StaffAlert?.Description, Is.EqualTo("admin (Admin) used /announce Hello"));
    }

    /// <summary>
    /// Tests that a character is disconnected from the game server on which it's online.
    /// </summary>
    [Test]
    public async Task KickDisconnectsFromTheGameServerOfTheCharacterAsync()
    {
        this._server2.Setup(s => s.DisconnectPlayerAsync("Hero")).ReturnsAsync(true);

        var answer = await this._commands.ExecuteAsync("kick", "Hero", GameMasterUser, "admin", true).ConfigureAwait(false);
        var offlineAnswer = await this._commands.ExecuteAsync("ban", "Nobody", GameMasterUser, "admin", true).ConfigureAwait(false);

        Assert.That(answer.Embed.Description, Is.EqualTo("Hero was disconnected."));
        Assert.That(offlineAnswer.Embed.Description, Is.EqualTo("Nobody isn't online."));
        Assert.That(offlineAnswer.StaffAlert, Is.Not.Null);
    }

    private IPlayerContext CreateContext() => this._persistence.CreateNewPlayerContext(new GameConfiguration());

    private async Task CreateLinkedAccountAsync(AccountLinkService linkService, string loginName, string characterName, CharacterStatus status, ulong userId)
    {
        Guid accountId;
        using (var context = this.CreateContext())
        {
            var account = context.CreateNew<Account>();
            account.LoginName = loginName;
            var character = context.CreateNew<Character>();
            character.Name = characterName;
            character.CharacterStatus = status;
            account.Characters.Add(character);
            await context.SaveChangesAsync().ConfigureAwait(false);
            accountId = account.GetId();
        }

        var code = await linkService.CreateCodeAsync(accountId, AccountLinkService.DiscordProvider, characterName).ConfigureAwait(false);
        await linkService.LinkAsync(AccountLinkService.DiscordProvider, code, userId.ToString(CultureInfo.InvariantCulture), loginName).ConfigureAwait(false);
    }
}
