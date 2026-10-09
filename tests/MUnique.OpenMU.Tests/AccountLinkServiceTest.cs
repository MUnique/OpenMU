// <copyright file="AccountLinkServiceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="AccountLinkService"/>.
/// </summary>
[TestFixture]
public class AccountLinkServiceTest
{
    private const string Provider = AccountLinkService.DiscordProvider;

    private InMemoryPersistenceContextProvider _persistence = null!;
    private ManualTimeProvider _time = null!;
    private AccountLinkService _service = null!;
    private Guid _accountId;
    private Guid _otherAccountId;

    /// <summary>
    /// Sets up two accounts.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._time = new ManualTimeProvider();
        this._service = new AccountLinkService(this.CreateContext, this._time);
        this._accountId = await this.CreateAccountAsync("hero", "Hero").ConfigureAwait(false);
        this._otherAccountId = await this.CreateAccountAsync("other", "Other").ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a user is linked to the account with the code, and appears as the character which requested it.
    /// </summary>
    [Test]
    public async Task UserIsLinkedWithCodeAsync()
    {
        var code = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);

        var result = await this._service.LinkAsync(Provider, code.ToLowerInvariant(), "42", "hero#1").ConfigureAwait(false);

        Assert.That(code, Does.Match("^[A-Z2-9]{4}-[A-Z2-9]{4}$"));
        Assert.That(result, Is.EqualTo(new AccountLinkResult(this._accountId, "Hero", null)));
        var link = await this._service.GetLinkAsync(this._accountId, Provider).ConfigureAwait(false);
        Assert.That(link?.ExternalUserId, Is.EqualTo("42"));
        Assert.That(link?.ExternalUserName, Is.EqualTo("hero#1"));
        Assert.That(link?.CodeHash, Is.Null);
    }

    /// <summary>
    /// Tests that a code can only be used once.
    /// </summary>
    [Test]
    public async Task CodeCanBeUsedOnceAsync()
    {
        var code = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);
        await this._service.LinkAsync(Provider, code, "42", "hero").ConfigureAwait(false);

        var secondResult = await this._service.LinkAsync(Provider, code, "43", "thief").ConfigureAwait(false);

        Assert.That(secondResult, Is.Null);
        Assert.That((await this._service.GetLinkAsync(this._accountId, Provider).ConfigureAwait(false))?.ExternalUserId, Is.EqualTo("42"));
    }

    /// <summary>
    /// Tests that an expired code can't be used, and that a new code replaces the previous one.
    /// </summary>
    [Test]
    public async Task ExpiredAndReplacedCodesAreRejectedAsync()
    {
        var expiredCode = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);
        this._time.Advance(AccountLinkService.CodeValidity + TimeSpan.FromSeconds(1));
        Assert.That(await this._service.LinkAsync(Provider, expiredCode, "42", "hero").ConfigureAwait(false), Is.Null);

        var replacedCode = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);
        var newCode = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);
        Assert.That(await this._service.LinkAsync(Provider, replacedCode, "42", "hero").ConfigureAwait(false), Is.Null);
        Assert.That(await this._service.LinkAsync(Provider, newCode, "42", "hero").ConfigureAwait(false), Is.Not.Null);
    }

    /// <summary>
    /// Tests that a user who links another account is unlinked from the previous one,
    /// and that the previous user of the account is reported.
    /// </summary>
    [Test]
    public async Task RelinkingReplacesPreviousLinksAsync()
    {
        await this.LinkAsync(this._accountId, "42").ConfigureAwait(false);
        await this.LinkAsync(this._otherAccountId, "43").ConfigureAwait(false);

        // User 43 links the first account, which was linked to user 42.
        var code = await this._service.CreateCodeAsync(this._accountId, Provider, "Hero").ConfigureAwait(false);
        var result = await this._service.LinkAsync(Provider, code, "43", "other").ConfigureAwait(false);

        Assert.That(result?.ReplacedExternalUserId, Is.EqualTo("42"));
        Assert.That((await this._service.GetLinkByUserAsync(Provider, "43").ConfigureAwait(false))?.AccountId, Is.EqualTo(this._accountId));
        Assert.That(await this._service.GetLinkByUserAsync(Provider, "42").ConfigureAwait(false), Is.Null);
        Assert.That(await this._service.GetLinkAsync(this._otherAccountId, Provider).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that the link can be removed from both sides.
    /// </summary>
    [Test]
    public async Task UnlinkingWorksFromBothSidesAsync()
    {
        await this.LinkAsync(this._accountId, "42").ConfigureAwait(false);
        await this.LinkAsync(this._otherAccountId, "43").ConfigureAwait(false);

        Assert.That(await this._service.UnlinkAccountAsync(this._accountId, Provider).ConfigureAwait(false), Is.EqualTo("42"));
        Assert.That(await this._service.UnlinkUserAsync(Provider, "43").ConfigureAwait(false), Is.True);

        Assert.That(await this._service.GetLinkAsync(this._accountId, Provider).ConfigureAwait(false), Is.Null);
        Assert.That(await this._service.GetLinkAsync(this._otherAccountId, Provider).ConfigureAwait(false), Is.Null);
        Assert.That(await this._service.UnlinkUserAsync(Provider, "43").ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that only a character of the linked account can be selected.
    /// </summary>
    [Test]
    public async Task OnlyCharacterOfAccountCanBeSelectedAsync()
    {
        await this.LinkAsync(this._accountId, "42").ConfigureAwait(false);

        Assert.That(await this._service.SelectCharacterAsync(Provider, "42", "Other").ConfigureAwait(false), Is.EqualTo(CharacterSelectionResult.CharacterNotFound));
        Assert.That(await this._service.SelectCharacterAsync(Provider, "43", "Hero").ConfigureAwait(false), Is.EqualTo(CharacterSelectionResult.NotLinked));
        Assert.That(await this._service.SelectCharacterAsync(Provider, "42", "Hero2").ConfigureAwait(false), Is.EqualTo(CharacterSelectionResult.Selected));
        Assert.That((await this._service.GetLinkAsync(this._accountId, Provider).ConfigureAwait(false))?.CharacterName, Is.EqualTo("Hero2"));
    }

    private IPlayerContext CreateContext() => this._persistence.CreateNewPlayerContext(new GameConfiguration());

    private async Task LinkAsync(Guid accountId, string userId)
    {
        var code = await this._service.CreateCodeAsync(accountId, Provider, null).ConfigureAwait(false);
        Assert.That(await this._service.LinkAsync(Provider, code, userId, userId).ConfigureAwait(false), Is.Not.Null);
    }

    private async Task<Guid> CreateAccountAsync(string loginName, string characterName)
    {
        using var context = this.CreateContext();
        var account = context.CreateNew<Account>();
        account.LoginName = loginName;
        foreach (var name in new[] { characterName, characterName + "2" })
        {
            var character = context.CreateNew<Character>();
            character.Name = name;
            account.Characters.Add(character);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        return account.GetId();
    }

    /// <summary>
    /// A time provider whose time is advanced by the test.
    /// </summary>
    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this._now;

        public void Advance(TimeSpan timeSpan) => this._now += timeSpan;
    }
}
