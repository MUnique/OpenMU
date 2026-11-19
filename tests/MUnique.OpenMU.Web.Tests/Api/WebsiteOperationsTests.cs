// <copyright file="WebsiteOperationsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;
using MUnique.OpenMU.Web.Tests.Mcp;

/// <summary>
/// Tests for the <see cref="WebsiteOperations"/>, which a website uses with a key of the
/// <see cref="AdminRoles.WebManager"/> role.
/// </summary>
[TestFixture]
public class WebsiteOperationsTests
{
    /// <summary>
    /// The website role stands alone: an administrator doesn't have it, so an administration key can't use the website API.
    /// </summary>
    [Test]
    public void WebManagerIsNotImpliedByOtherRoles()
    {
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.WebManager), Is.EquivalentTo(new[] { AdminRoles.WebManager }));
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.Administrator), Does.Not.Contain(AdminRoles.WebManager));
        Assert.That(AdminRoles.All, Does.Not.Contain(AdminRoles.WebManager));
    }

    /// <summary>
    /// A website key without a selected function can't do anything, unlike an MCP key with an empty allowlist.
    /// </summary>
    [Test]
    public void KeyWithoutSelectedFunctionIsRefused()
    {
        var operations = CreateOperations(WebManager(string.Empty));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// A website key may only use the functions which were selected for it.
    /// </summary>
    [Test]
    public void KeyWithOtherFunctionIsRefused()
    {
        var operations = CreateOperations(WebManager("website_register"));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// An administration key can't use the website API.
    /// </summary>
    [Test]
    public void AdministratorIsRefused()
    {
        var operations = CreateOperations(new AdminActor("admin-key", "admin", "api", [AdminRoles.Administrator]));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// The status lists the game servers with their players, but no other servers.
    /// </summary>
    [Test]
    public async Task StatusListsGameServersAsync()
    {
        var operations = CreateOperations(WebManager(WebsitePermissions.Status), GameServer(1, "Server 1", ServerState.Started, 12), GameServer(0, "Server 0", ServerState.Stopped, 0), Mock.Of<IManageableServer>());

        var status = await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.That(status.Players, Is.EqualTo(12));
        Assert.That(status.Servers.Select(s => (s.Id, s.Name, s.IsOnline, s.Players)), Is.EqualTo(new[] { (0, "Server 0", false, 0), (1, "Server 1", true, 12) }));
    }

    /// <summary>
    /// The ranking can be limited to the line of a class, which includes its next generations, but no other class.
    /// Each entry names the guild of the character.
    /// </summary>
    [Test]
    public async Task RankingIsLimitedToClassLineAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        // The basic model initializes its collections, unlike the abstract data model.
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var wizard = AddClass(context, gameConfiguration, 0, "Dark Wizard");
            var soulMaster = AddClass(context, gameConfiguration, 1, "Soul Master");
            wizard.NextGenerationClass = soulMaster;
            var knight = AddClass(context, gameConfiguration, 16, "Dark Knight");
            var account = context.CreateNew<Account>();
            account.LoginName = "ranked";
            foreach (var (name, characterClass, level) in new[] { ("Wizard", wizard, 200), ("Master", soulMaster, 400), ("Knight", knight, 380) })
            {
                var character = context.CreateNew<Character>();
                character.Name = name;
                character.CharacterClass = characterClass;
                character.Attributes.Add(new MUnique.OpenMU.Persistence.BasicModel.StatAttribute(Stats.Level, level));
                account.Characters.Add(character);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        await AddToGuildAsync(persistence, gameConfiguration, "Heroes", "Master", "Knight").ConfigureAwait(false);
        var operations = CreateOperations(WebManager(WebsitePermissions.Ranking), persistence, gameConfiguration);

        var all = await operations.RankingAsync(10, null, CancellationToken.None).ConfigureAwait(false);
        var wizards = await operations.RankingAsync(10, 1, CancellationToken.None).ConfigureAwait(false);

        Assert.That(all.Select(e => (e.Rank, e.Name, e.GuildName)), Is.EqualTo(new (int, string, string?)[] { (1, "Master", "Heroes"), (2, "Knight", "Heroes"), (3, "Wizard", null) }));
        Assert.That(wizards.Select(e => e.Name), Is.EqualTo(new[] { "Master", "Wizard" }));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.RankingAsync(101, null, CancellationToken.None).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.RankingAsync(10, 99, CancellationToken.None).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
    }

    /// <summary>
    /// The ranking is cached like the one of the Discord bot: further requests within the cache duration, also of other
    /// requests and for fewer entries, don't query the database again. The ranking of a class line is cached separately.
    /// </summary>
    [Test]
    public async Task RankingIsCachedPerClassLineAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var wizard = AddClass(context, gameConfiguration, 0, "Dark Wizard");
            var knight = AddClass(context, gameConfiguration, 16, "Dark Knight");
            var account = context.CreateNew<Account>();
            account.LoginName = "ranked";
            foreach (var (name, characterClass, level) in new[] { ("Wizard", wizard, 200), ("Knight", knight, 380) })
            {
                var character = context.CreateNew<Character>();
                character.Name = name;
                character.CharacterClass = characterClass;
                character.Attributes.Add(new MUnique.OpenMU.Persistence.BasicModel.StatAttribute(Stats.Level, level));
                account.Characters.Add(character);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var countingPersistence = new Mock<IPersistenceContextProvider>();
        countingPersistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns((GameConfiguration configuration) => persistence.CreateNewPlayerContext(configuration));
        countingPersistence.Setup(p => p.CreateNewGuildContext()).Returns(() => persistence.CreateNewGuildContext());
        var cache = new WebsiteRankingCache();
        var operations = CreateOperations(WebManager(WebsitePermissions.Ranking), countingPersistence.Object, gameConfiguration, cache);
        var nextRequest = CreateOperations(WebManager(WebsitePermissions.Ranking), countingPersistence.Object, gameConfiguration, cache);

        var all = await operations.RankingAsync(10, null, CancellationToken.None).ConfigureAwait(false);
        var best = await operations.RankingAsync(1, null, CancellationToken.None).ConfigureAwait(false);
        var allAgain = await nextRequest.RankingAsync(10, null, CancellationToken.None).ConfigureAwait(false);
        var knights = await nextRequest.RankingAsync(10, 16, CancellationToken.None).ConfigureAwait(false);

        Assert.That(all.Select(e => (e.Rank, e.Name)), Is.EqualTo(new[] { (1, "Knight"), (2, "Wizard") }));
        Assert.That(best, Is.EqualTo(all.Take(1)));
        Assert.That(allAgain, Is.EqualTo(all));
        Assert.That(knights.Select(e => (e.Rank, e.Name)), Is.EqualTo(new[] { (1, "Knight") }));
        countingPersistence.Verify(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>()), Times.Exactly(2));
    }

    /// <summary>
    /// A cached ranking is loaded again after the cache duration; the rankings of all classes and of a class line don't mix.
    /// </summary>
    [Test]
    public async Task RankingCacheExpiresAsync()
    {
        var cache = new WebsiteRankingCache();
        var now = new DateTime(2026, 11, 19, 12, 0, 0, DateTimeKind.Utc);
        var loads = 0;
        ValueTask<IReadOnlyList<WebsiteRankingEntry>> LoadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            loads++;
            return ValueTask.FromResult<IReadOnlyList<WebsiteRankingEntry>>([new WebsiteRankingEntry(1, $"Load {loads}", "Dark Knight", 400, 0, 0, null)]);
        }

        var first = await cache.GetOrLoadAsync(null, LoadAsync, now, CancellationToken.None).ConfigureAwait(false);
        var cached = await cache.GetOrLoadAsync(null, LoadAsync, now + WebsiteRankingCache.Duration - TimeSpan.FromSeconds(1), CancellationToken.None).ConfigureAwait(false);
        var classLine = await cache.GetOrLoadAsync(0, LoadAsync, now, CancellationToken.None).ConfigureAwait(false);
        var expired = await cache.GetOrLoadAsync(null, LoadAsync, now + WebsiteRankingCache.Duration, CancellationToken.None).ConfigureAwait(false);

        Assert.That(first[0].Name, Is.EqualTo("Load 1"));
        Assert.That(cached[0].Name, Is.EqualTo("Load 1"));
        Assert.That(classLine[0].Name, Is.EqualTo("Load 2"));
        Assert.That(expired[0].Name, Is.EqualTo("Load 3"));
    }

    /// <summary>
    /// Parallel requests of a ranking which isn't cached load it only once.
    /// </summary>
    [Test]
    public async Task ParallelRankingRequestsLoadOnceAsync()
    {
        var cache = new WebsiteRankingCache();
        var now = new DateTime(2026, 11, 19, 12, 0, 0, DateTimeKind.Utc);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var loads = 0;
        async ValueTask<IReadOnlyList<WebsiteRankingEntry>> LoadAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref loads);
            await release.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            return [];
        }

        var requests = Enumerable.Range(0, 10).Select(_ => cache.GetOrLoadAsync(null, LoadAsync, now, CancellationToken.None).AsTask()).ToList();
        release.SetResult();
        await Task.WhenAll(requests).ConfigureAwait(false);

        Assert.That(loads, Is.EqualTo(1));
    }

    /// <summary>
    /// The mini game ranking is the leaderboard of the admin panel; invalid arguments are the caller's fault.
    /// </summary>
    [Test]
    public async Task MiniGameRankingUsesLeaderboardAsync()
    {
        var operations = CreateOperations(WebManager(WebsitePermissions.MiniGameRanking), new InMemoryPersistenceContextProvider(), new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration());

        var ranking = await operations.MiniGameRankingAsync(null, 7, CancellationToken.None).ConfigureAwait(false);

        Assert.That(ranking.MiniGameType, Is.Null);
        Assert.That(ranking.AvailableTypes, Is.Empty);
        Assert.That(ranking.Days, Is.EqualTo(7));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.MiniGameRankingAsync(null, 0, CancellationToken.None).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
    }

    /// <summary>
    /// The events are read from each started game server, which knows which events run on it.
    /// A game server which doesn't answer is left out.
    /// </summary>
    [Test]
    public async Task EventsAreReadPerStartedGameServerAsync()
    {
        var started = new Mock<IGameServer>();
        started.Setup(s => s.Id).Returns(1);
        started.Setup(s => s.Description).Returns("Server 1");
        started.Setup(s => s.ServerState).Returns(ServerState.Started);
        var golden = new ScheduledEventInfo(Guid.NewGuid(), "Golden Invasion", new DateTime(2026, 11, 18, 20, 0, 0, DateTimeKind.Utc));
        started.As<IGameServerAdministration>().Setup(a => a.GetScheduledEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([golden]);
        var failing = new Mock<IGameServer>();
        failing.Setup(s => s.Id).Returns(2);
        failing.Setup(s => s.ServerState).Returns(ServerState.Started);
        failing.As<IGameServerAdministration>().Setup(a => a.GetScheduledEventsAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new TimeoutException());
        var stopped = new Mock<IGameServer>();
        stopped.Setup(s => s.ServerState).Returns(ServerState.Stopped);
        var stoppedAdministration = stopped.As<IGameServerAdministration>();
        var operations = CreateOperations(WebManager(WebsitePermissions.Events), started.Object, failing.Object, stopped.Object);

        var events = await operations.EventsAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0].ServerName, Is.EqualTo("Server 1"));
        Assert.That(events[0].Events, Is.EqualTo(new[] { new WebsiteEvent(golden.PlugInId, golden.Name, golden.NextStartUtc) }));
        stoppedAdministration.Verify(a => a.GetScheduledEventsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// A login returns the account with its characters; a wrong password and an unknown account look the same.
    /// After too many failures, even the right password is refused for a while.
    /// </summary>
    [Test]
    public async Task LoginChecksPasswordAndLocksOutAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword("right1");
            var character = context.CreateNew<Character>();
            character.Name = "Hero";
            character.Attributes.Add(new MUnique.OpenMU.Persistence.BasicModel.StatAttribute(Stats.Level, 123));
            character.Inventory = context.CreateNew<ItemStorage>();
            character.Inventory.Money = 5000;
            character.PositionX = 130;
            character.PositionY = 120;
            account.Characters.Add(character);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var operations = CreateOperations(WebManager(WebsitePermissions.Login), persistence, gameConfiguration);

        var success = await operations.LoginAsync("player", "right1", CancellationToken.None).ConfigureAwait(false);
        Assert.That(success.Status, Is.EqualTo(WebsiteLoginStatus.Succeeded));
        Assert.That(success.Account!.Characters.Select(c => (c.Name, c.Level, c.Money, c.PositionX, c.PositionY)), Is.EqualTo(new[] { ("Hero", 123, 5000, 130, 120) }));
        Assert.That(success.Account.IsOnline, Is.False);
        Assert.That((await operations.LoginAsync("nobody", "right1", CancellationToken.None).ConfigureAwait(false)).Status, Is.EqualTo(WebsiteLoginStatus.Failed));

        for (var i = 0; i < WebsiteLoginThrottle.MaximumFailures; i++)
        {
            Assert.That((await operations.LoginAsync("player", "wrong1", CancellationToken.None).ConfigureAwait(false)).Status, Is.EqualTo(WebsiteLoginStatus.Failed));
        }

        var locked = await operations.LoginAsync("player", "right1", CancellationToken.None).ConfigureAwait(false);
        Assert.That(locked.Status, Is.EqualTo(WebsiteLoginStatus.LockedOut));
        Assert.That(locked.Account, Is.Null);
    }

    /// <summary>
    /// The login tells the player whether a character of the account is offline leveling, which only the game server knows.
    /// </summary>
    [Test]
    public async Task LoginTellsOfflineLevelingAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword("right1");
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var server = new Mock<IGameServer>();
        server.Setup(s => s.ServerState).Returns(ServerState.Started);
        server.As<IGameServerAdministration>().Setup(a => a.IsOfflineLevelingAsync("player", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var operations = CreateOperations(WebManager(WebsitePermissions.Login), persistence, gameConfiguration, server.Object);

        var result = await operations.LoginAsync("player", "right1", CancellationToken.None).ConfigureAwait(false);

        Assert.That(result.Account!.IsOfflineLeveling, Is.True);
        Assert.That(result.Account.IsOnline, Is.False);
    }

    /// <summary>
    /// The password of an account with an offline leveling character isn't changed: the account is logged off,
    /// but the game server saves it again when the offline leveling ends.
    /// </summary>
    [Test]
    public async Task PasswordIsNotChangedWhileOfflineLevelingAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword("right1");
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var server = new Mock<IGameServer>();
        server.Setup(s => s.ServerState).Returns(ServerState.Started);
        server.As<IGameServerAdministration>().Setup(a => a.IsOfflineLevelingAsync("player", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var operations = CreateOperations(WebManager(WebsitePermissions.ChangePassword), persistence, gameConfiguration, server.Object);

        var status = await operations.ChangePasswordAsync("player", "right1", "new123", CancellationToken.None).ConfigureAwait(false);

        Assert.That(status, Is.EqualTo(WebsitePasswordChangeStatus.OfflineLeveling));
        using var checkContext = persistence.CreateNewPlayerContext(gameConfiguration);
        Assert.That(await checkContext.AuthenticateAsync("player", "right1", CancellationToken.None).ConfigureAwait(false), Is.Not.Null, "The old password still works.");
    }

    /// <summary>
    /// The password isn't changed when a game server can't tell whether a character of the account is offline leveling,
    /// because that game server would overwrite the new password later.
    /// </summary>
    [Test]
    public async Task PasswordIsNotChangedWhenGameServerFailsAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword("right1");
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var server = new Mock<IGameServer>();
        server.Setup(s => s.ServerState).Returns(ServerState.Started);
        server.As<IGameServerAdministration>().Setup(a => a.IsOfflineLevelingAsync("player", It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("The game server doesn't answer."));
        var operations = CreateOperations(WebManager(WebsitePermissions.ChangePassword + "," + WebsitePermissions.Login), persistence, gameConfiguration, server.Object);

        var status = await operations.ChangePasswordAsync("player", "right1", "new123", CancellationToken.None).ConfigureAwait(false);

        Assert.That(status, Is.EqualTo(WebsitePasswordChangeStatus.GameServerUnavailable));
        using var checkContext = persistence.CreateNewPlayerContext(gameConfiguration);
        Assert.That(await checkContext.AuthenticateAsync("player", "right1", CancellationToken.None).ConfigureAwait(false), Is.Not.Null, "The old password still works.");
        Assert.That((await operations.LoginAsync("player", "right1", CancellationToken.None).ConfigureAwait(false)).Status, Is.EqualTo(WebsiteLoginStatus.Succeeded), "The login isn't prevented by the failing game server.");
    }

    /// <summary>
    /// The lockout lasts for the window from the failed login which started it, not from the first failed login,
    /// and a successful login forgets the failures.
    /// </summary>
    [Test]
    public void LoginThrottleLocksOutForWindowFromLockout()
    {
        var throttle = new WebsiteLoginThrottle();
        var start = new DateTime(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);
        var lockoutStart = start.AddMinutes(14);
        for (var i = 0; i < WebsiteLoginThrottle.MaximumFailures; i++)
        {
            var time = i == WebsiteLoginThrottle.MaximumFailures - 1 ? lockoutStart : start.AddMinutes(i);
            Assert.That(throttle.TryBeginAttempt("Player", time), Is.True);
            Assert.That(throttle.CompleteAttempt("Player", false, time), Is.EqualTo(i == WebsiteLoginThrottle.MaximumFailures - 1));
        }

        Assert.That(throttle.TryBeginAttempt("player", start.AddMinutes(20)), Is.False, "Login names are compared case insensitive.");
        Assert.That(throttle.GetRetryAfter("player", start.AddMinutes(20)), Is.EqualTo(TimeSpan.FromMinutes(9)));
        Assert.That(throttle.TryBeginAttempt("player", lockoutStart + WebsiteLoginThrottle.Window), Is.True);

        Assert.That(throttle.TryBeginAttempt("other", start), Is.True);
        throttle.CompleteAttempt("other", false, start);
        Assert.That(throttle.TryBeginAttempt("other", start), Is.True);
        throttle.CompleteAttempt("other", true, start);
        Assert.That(throttle.GetRetryAfter("other", start), Is.EqualTo(TimeSpan.Zero));
    }

    /// <summary>
    /// The password checks which are still running count against the limit, and a cancelled check doesn't count as a failure.
    /// </summary>
    [Test]
    public void LoginThrottleCountsRunningAttempts()
    {
        var throttle = new WebsiteLoginThrottle();
        var now = new DateTime(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);
        for (var i = 0; i < WebsiteLoginThrottle.MaximumFailures; i++)
        {
            Assert.That(throttle.TryBeginAttempt("player", now), Is.True);
        }

        Assert.That(throttle.TryBeginAttempt("player", now), Is.False);
        Assert.That(throttle.GetRetryAfter("player", now), Is.EqualTo(WebsiteLoginThrottle.Window));
        throttle.CancelAttempt("player");
        Assert.That(throttle.TryBeginAttempt("player", now), Is.True);
    }

    /// <summary>
    /// Parallel wrong logins can't check more passwords than the limit allows; the others are locked out right away.
    /// </summary>
    [Test]
    public async Task ParallelLoginsCantBypassThrottleAsync()
    {
        const int requests = 20;
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword("right1");
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var operations = CreateOperations(WebManager(WebsitePermissions.Login), persistence, gameConfiguration);

        var results = await Task.WhenAll(Enumerable.Range(0, requests).Select(_ => Task.Run(async () => (await operations.LoginAsync("player", "wrong1", CancellationToken.None).ConfigureAwait(false)).Status))).ConfigureAwait(false);

        Assert.That(results.Count(status => status == WebsiteLoginStatus.Failed), Is.LessThanOrEqualTo(WebsiteLoginThrottle.MaximumFailures), "Each failed login is a checked password.");
        Assert.That(results.Count(status => status == WebsiteLoginStatus.LockedOut), Is.GreaterThanOrEqualTo(requests - WebsiteLoginThrottle.MaximumFailures));
        Assert.That((await operations.LoginAsync("player", "right1", CancellationToken.None).ConfigureAwait(false)).Status, Is.EqualTo(WebsiteLoginStatus.LockedOut));
    }

    private static CharacterClass AddClass(IContext context, GameConfiguration gameConfiguration, byte number, string name)
    {
        var characterClass = context.CreateNew<CharacterClass>();
        characterClass.Number = number;
        characterClass.Name = name;
        gameConfiguration.CharacterClasses.Add(characterClass);
        return characterClass;
    }

    private static async Task AddToGuildAsync(IPersistenceContextProvider persistence, GameConfiguration gameConfiguration, string guildName, params string[] characterNames)
    {
        using var playerContext = persistence.CreateNewPlayerContext(gameConfiguration);
        using var guildContext = persistence.CreateNewGuildContext();
        var guild = guildContext.CreateNew<DataModel.Entities.Guild>();
        guild.Name = guildName;
        foreach (var characterName in characterNames)
        {
            var account = await playerContext.GetAccountByCharacterNameAsync(characterName).ConfigureAwait(false);
            var member = guildContext.CreateNew<GuildMember>(account!.Characters.Single(c => c.Name == characterName).GetId());
            member.GuildId = guild.GetId();
            guild.Members.Add(member);
        }

        await guildContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private static AdminActor WebManager(string allowedFunctions)
        => new("website-key", "website", "api", [AdminRoles.WebManager]) { AllowedToolGroups = allowedFunctions };

    private static IManageableServer GameServer(int id, string name, ServerState state, int players)
    {
        var server = new Mock<IGameServer>();
        server.Setup(s => s.Id).Returns(id);
        server.Setup(s => s.Description).Returns(name);
        server.Setup(s => s.ServerState).Returns(state);
        server.Setup(s => s.CurrentConnections).Returns(players);
        server.Setup(s => s.MaximumConnections).Returns(100);
        return server.Object;
    }

    private static WebsiteOperations CreateOperations(AdminActor actor, params IManageableServer[] servers)
        => CreateOperations(actor, new InMemoryPersistenceContextProvider(), new GameConfiguration(), servers);

    private static WebsiteOperations CreateOperations(AdminActor actor, IPersistenceContextProvider persistence, GameConfiguration gameConfiguration, params IManageableServer[] servers)
        => CreateOperations(actor, persistence, gameConfiguration, new WebsiteRankingCache(), servers);

    private static WebsiteOperations CreateOperations(AdminActor actor, IPersistenceContextProvider persistence, GameConfiguration gameConfiguration, WebsiteRankingCache rankingCache, params IManageableServer[] servers)
    {
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.Setup(p => p.Servers).Returns(servers.ToList());
        var configurations = new Mock<IDataSource<GameConfiguration>>();
        configurations.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameConfiguration);
        var executor = AdminOperationTestServices.CreateExecutor("api", actor: () => actor);
        var accounts = new AccountWriteOperations(persistence, configurations.Object, Mock.Of<ILoginServer>(), executor);
        var miniGames = new MiniGameReadOperations(persistence, configurations.Object);
        var loginServer = new Mock<ILoginServer>();
        loginServer.Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte>());
        return new WebsiteOperations(serverProvider.Object, persistence, configurations.Object, accounts, rankingCache, miniGames, loginServer.Object, new WebsiteLoginThrottle(), NullLogger<WebsiteOperations>.Instance, executor);
    }
}
