// <copyright file="DiscordAccountCommandsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="DiscordAccountCommands"/>.
/// </summary>
[TestFixture]
public class DiscordAccountCommandsTest
{
    private AccountLinkService _service = null!;
    private DiscordAccountCommands _commands = null!;
    private Guid _accountId;

    /// <summary>
    /// Sets up an account.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        this._service = new AccountLinkService(() => persistence.CreateNewPlayerContext(new GameConfiguration()));
        this._commands = new DiscordAccountCommands(this._service, CultureInfo.GetCultureInfo("en"), NullLogger<DiscordAccountCommands>.Instance);
        using var context = persistence.CreateNewPlayerContext(new GameConfiguration());
        var account = context.CreateNew<Account>();
        account.LoginName = "hero";
        await context.SaveChangesAsync().ConfigureAwait(false);
        this._accountId = account.GetId();
    }

    /// <summary>
    /// Tests that linking with a code links the user and gives it the role, and that unlinking removes it again.
    /// </summary>
    [Test]
    public async Task LinkAndUnlinkAsync()
    {
        var code = await this._service.CreateCodeAsync(this._accountId, AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);

        var linkAnswer = await this._commands.ExecuteAsync("link", code, 42, "hero").ConfigureAwait(false);
        var unlinkAnswer = await this._commands.ExecuteAsync("unlink", null, 42, "hero").ConfigureAwait(false);

        Assert.That(linkAnswer.Embed.Description, Does.StartWith("Your Discord user is linked to your game account now. You appear as **Hero**"));
        Assert.That(linkAnswer.RoleChanges, Is.EqualTo(new[] { new DiscordRoleChange(42, true) }));
        Assert.That(unlinkAnswer.Embed.Description, Is.EqualTo("Your Discord user isn't linked to a game account anymore."));
        Assert.That(unlinkAnswer.RoleChanges, Is.EqualTo(new[] { new DiscordRoleChange(42, false) }));
    }

    /// <summary>
    /// Tests the answer to an invalid code.
    /// </summary>
    [Test]
    public async Task InvalidCodeIsRejectedAsync()
    {
        var answer = await this._commands.ExecuteAsync("link", "WRONG-CODE", 42, "hero").ConfigureAwait(false);

        Assert.That(answer.Embed.Description, Does.StartWith("The code is invalid or expired."));
        Assert.That(answer.RoleChanges, Is.Empty);
    }

    /// <summary>
    /// Tests that the previous user of the account loses the role, when another user links the account.
    /// </summary>
    [Test]
    public async Task PreviousUserLosesRoleAsync()
    {
        var firstCode = await this._service.CreateCodeAsync(this._accountId, AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);
        await this._commands.ExecuteAsync("link", firstCode, 42, "hero").ConfigureAwait(false);
        var secondCode = await this._service.CreateCodeAsync(this._accountId, AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);

        var answer = await this._commands.ExecuteAsync("link", secondCode, 43, "new").ConfigureAwait(false);

        Assert.That(answer.RoleChanges, Is.EqualTo(new[] { new DiscordRoleChange(43, true), new DiscordRoleChange(42, false) }));
    }

    /// <summary>
    /// Tests the answers for a user who isn't linked.
    /// </summary>
    [Test]
    public async Task NotLinkedUserAsync()
    {
        var unlinkAnswer = await this._commands.ExecuteAsync("unlink", null, 42, "hero").ConfigureAwait(false);
        var characterAnswer = await this._commands.ExecuteAsync("character", "Hero", 42, "hero").ConfigureAwait(false);

        Assert.That(unlinkAnswer.Embed.Description, Does.StartWith("Your Discord user isn't linked to a game account."));
        Assert.That(characterAnswer.Embed.Description, Is.EqualTo(unlinkAnswer.Embed.Description));
    }
}
