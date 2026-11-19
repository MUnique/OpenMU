// <copyright file="LoginTicketServiceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.LoginTickets;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="LoginTicketService"/>.
/// </summary>
[TestFixture]
public class LoginTicketServiceTest
{
    private InMemoryPersistenceContextProvider _persistence = null!;
    private ManualTimeProvider _time = null!;
    private LoginTicketService _service = null!;
    private Guid _accountId;

    /// <summary>
    /// Sets up an account.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._time = new ManualTimeProvider();
        this._service = new LoginTicketService(this.CreateContext, null, this._time);
        using var context = this.CreateContext();
        var account = context.CreateNew<Account>();
        account.LoginName = "hero";
        await context.SaveChangesAsync().ConfigureAwait(false);
        this._accountId = account.GetId();
    }

    /// <summary>
    /// Tests that a ticket is redeemed once, for the account it was issued for.
    /// </summary>
    [Test]
    public async Task TicketIsRedeemedOnceAsync()
    {
        var ticket = await this.IssueAsync().ConfigureAwait(false);

        var first = await this._service.RedeemAsync(ticket.Token).ConfigureAwait(false);
        var replay = await this._service.RedeemAsync(ticket.Token).ConfigureAwait(false);

        Assert.That(first, Is.EqualTo(new LoginTicketRedemption(LoginTicketRedeemStatus.Redeemed, this._accountId)));
        Assert.That(replay.Status, Is.EqualTo(LoginTicketRedeemStatus.AlreadyUsed));
        Assert.That(replay.AccountId, Is.EqualTo(Guid.Empty));
    }

    /// <summary>
    /// Tests that of concurrent redeemers of the same ticket, only one succeeds.
    /// </summary>
    [Test]
    public async Task ConcurrentRedeemersSucceedOnceAsync()
    {
        var ticket = await this.IssueAsync().ConfigureAwait(false);

        var results = await Task.WhenAll(Enumerable.Range(0, 16)
            .Select(_ => Task.Run(() => this._service.RedeemAsync(ticket.Token).AsTask()))).ConfigureAwait(false);

        Assert.That(results.Count(r => r.Status == LoginTicketRedeemStatus.Redeemed), Is.EqualTo(1));
        Assert.That(results.Where(r => r.Status != LoginTicketRedeemStatus.Redeemed).Select(r => r.Status), Is.All.EqualTo(LoginTicketRedeemStatus.AlreadyUsed));
    }

    /// <summary>
    /// Tests that an expired ticket can't be redeemed.
    /// </summary>
    [Test]
    public async Task ExpiredTicketIsRejectedAsync()
    {
        var ticket = await this.IssueAsync().ConfigureAwait(false);
        Assert.That(ticket.ExpiresAt, Is.EqualTo(this._time.GetUtcNow().UtcDateTime + LoginTicketService.DefaultLifetime));

        this._time.Advance(LoginTicketService.DefaultLifetime);

        Assert.That((await this._service.RedeemAsync(ticket.Token).ConfigureAwait(false)).Status, Is.EqualTo(LoginTicketRedeemStatus.Expired));
    }

    /// <summary>
    /// Tests that unknown and malformed tokens are rejected.
    /// </summary>
    /// <param name="token">The token.</param>
    [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
    [TestCase("")]
    [TestCase("short")]
    [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
    [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA+")]
    public async Task UnknownTokenIsRejectedAsync(string token)
    {
        await this.IssueAsync().ConfigureAwait(false);

        Assert.That((await this._service.RedeemAsync(token).ConfigureAwait(false)).Status, Is.EqualTo(LoginTicketRedeemStatus.Unknown));
    }

    /// <summary>
    /// Tests that a token is 256 random bits as base64url, and only its hash is stored.
    /// </summary>
    [Test]
    public async Task OnlyTheHashOfTheTokenIsStoredAsync()
    {
        var ticket = await this.IssueAsync().ConfigureAwait(false);
        var other = await this.IssueAsync().ConfigureAwait(false);

        Assert.That(ticket.Token, Does.Match("^[A-Za-z0-9_-]{43}$"));
        Assert.That(other.Token, Is.Not.EqualTo(ticket.Token));
        Assert.That(ticket.ToString(), Does.Not.Contain(ticket.Token));
        var stored = await this.GetTicketsAsync().ConfigureAwait(false);
        Assert.That(stored.Select(t => t.TokenHash), Has.All.Match("^[0-9A-F]{64}$"));
        Assert.That(stored.Any(t => t.TokenHash.Contains(ticket.Token, StringComparison.Ordinal) || t.TokenHash.Contains(other.Token, StringComparison.Ordinal)), Is.False);
    }

    /// <summary>
    /// Tests that an account gets a limited number of tickets within one lifetime, and that expired tickets are deleted.
    /// </summary>
    [Test]
    public async Task TicketsPerAccountAreLimitedAsync()
    {
        for (var i = 0; i < LoginTicketService.MaximumTicketsPerLifetime; i++)
        {
            var ticket = await this.IssueAsync().ConfigureAwait(false);
            await this._service.RedeemAsync(ticket.Token).ConfigureAwait(false);
        }

        Assert.That(await this._service.IssueAsync(this._accountId).ConfigureAwait(false), Is.Null, "Used tickets count, too.");

        this._time.Advance(LoginTicketService.DefaultLifetime);
        await this.IssueAsync().ConfigureAwait(false);
        Assert.That(await this.GetTicketsAsync().ConfigureAwait(false), Has.Count.EqualTo(1), "The expired tickets were deleted.");
    }

    /// <summary>
    /// Tests that concurrent issuers for the same account get no more tickets than the maximum together.
    /// </summary>
    [Test]
    public async Task ConcurrentIssuersStayWithinTheLimitAsync()
    {
        var tickets = await Task.WhenAll(Enumerable.Range(0, 16)
            .Select(_ => Task.Run(() => this._service.IssueAsync(this._accountId).AsTask()))).ConfigureAwait(false);

        Assert.That(tickets.Count(ticket => ticket is not null), Is.EqualTo(LoginTicketService.MaximumTicketsPerLifetime));
        Assert.That(await this.GetTicketsAsync().ConfigureAwait(false), Has.Count.EqualTo(LoginTicketService.MaximumTicketsPerLifetime));
    }

    /// <summary>
    /// Tests that the lifetime of the tickets can be configured.
    /// </summary>
    [Test]
    public async Task LifetimeIsConfigurableAsync()
    {
        var service = new LoginTicketService(this.CreateContext, TimeSpan.FromSeconds(10), this._time);
        var ticket = await service.IssueAsync(this._accountId).ConfigureAwait(false);

        this._time.Advance(TimeSpan.FromSeconds(10));

        Assert.That(ticket!.ExpiresAt, Is.EqualTo(this._time.GetUtcNow().UtcDateTime));
        Assert.That((await service.RedeemAsync(ticket.Token).ConfigureAwait(false)).Status, Is.EqualTo(LoginTicketRedeemStatus.Expired));
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new LoginTicketService(this.CreateContext, TimeSpan.Zero));
    }

    private IPlayerContext CreateContext() => this._persistence.CreateNewPlayerContext(new GameConfiguration());

    private async Task<IssuedLoginTicket> IssueAsync()
    {
        var ticket = await this._service.IssueAsync(this._accountId).ConfigureAwait(false);
        Assert.That(ticket, Is.Not.Null);
        return ticket!;
    }

    private async Task<List<LoginTicket>> GetTicketsAsync()
    {
        using var context = this.CreateContext();
        return (await context.GetAsync<LoginTicket>().ConfigureAwait(false)).ToList();
    }

    /// <summary>
    /// A time provider whose time is advanced by the test.
    /// </summary>
    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this._now;

        public void Advance(TimeSpan timeSpan) => this._now += timeSpan;
    }
}
