// <copyright file="PlayerPortalApiTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Text.Encodings.Web;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>Regression tests for isolated player-only account operations over OpenMU's in-memory persistence.</summary>
[TestFixture]
public sealed class PlayerPortalApiTests
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromMinutes(30);
    private InMemoryPersistenceContextProvider _persistence = null!;
    private TestTimeProvider _time = null!;
    private IDataSource<GameConfiguration> _configuration = null!;
    private AccountRegistrationService _registrations = null!;
    private PlayerSessionStore _sessions = null!;
    private PlayerLoginAttemptLimiter _attempts = null!;
    private PlayerPortalService _service = null!;

    /// <summary>Creates an empty, isolated persistence provider for each test.</summary>
    [SetUp]
    public void SetUp()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._time = new TestTimeProvider(DateTimeOffset.Parse("2026-01-01T00:00:00Z", System.Globalization.CultureInfo.InvariantCulture));
        var configuration = new Mock<IDataSource<GameConfiguration>>();
        configuration.Setup(source => source.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GameConfiguration());
        this._configuration = configuration.Object;
        this._registrations = new AccountRegistrationService(this._persistence, this._configuration);
        this._sessions = new PlayerSessionStore(this._time);
        this._attempts = new PlayerLoginAttemptLimiter(this._time);
        this._service = new PlayerPortalService(this._persistence, this._configuration, this._registrations, this._sessions, this._attempts);
    }

    /// <summary>A valid account uses BCrypt persistence accepted by the same native player authentication method.</summary>
    [Test]
    public async Task RegistrationUsesNativeOpenMuAccountAndBCryptAsync()
    {
        var status = await this._service.RegisterAsync(Registration("portaluser", "GamePass1")).ConfigureAwait(false);

        Assert.That(status, Is.EqualTo(AccountRegistrationStatus.Created));
        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        var account = await context.GetAccountByLoginNameAsync("portaluser").ConfigureAwait(false);
        Assert.That(account, Is.Not.Null);
        Assert.That(BCrypt.Net.BCrypt.Verify("GamePass1", account!.PasswordHash), Is.True);
        Assert.That(await context.AuthenticateAsync("portaluser", "GamePass1").ConfigureAwait(false), Is.EqualTo(AccountState.Normal));
    }

    /// <summary>Invalid and duplicate account registrations do not replace existing credentials.</summary>
    [Test]
    public async Task InvalidAndDuplicateRegistrationAreHandledAsync()
    {
        Assert.That(await this._service.RegisterAsync(Registration("too-long-user", "GamePass1")).ConfigureAwait(false), Is.EqualTo(AccountRegistrationStatus.Invalid));
        Assert.That(await this._service.RegisterAsync(Registration("portaluser", "GamePass1")).ConfigureAwait(false), Is.EqualTo(AccountRegistrationStatus.Created));
        Assert.That(await this._service.RegisterAsync(Registration("portaluser", "Changed22")).ConfigureAwait(false), Is.EqualTo(AccountRegistrationStatus.AlreadyExists));

        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        Assert.That(await context.AuthenticateAsync("portaluser", "GamePass1").ConfigureAwait(false), Is.EqualTo(AccountState.Normal));
        Assert.That(await context.AuthenticateAsync("portaluser", "Changed22").ConfigureAwait(false), Is.Null);
    }

    /// <summary>Correct and incorrect passwords produce native authentication outcomes with no account disclosure.</summary>
    [Test]
    public async Task LoginUsesNativeCredentialsAndGenericFailuresAsync()
    {
        await this._service.RegisterAsync(Registration("portaluser", "GamePass1")).ConfigureAwait(false);

        var success = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "portaluser", Password = "GamePass1" }).ConfigureAwait(false);
        var wrong = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "portaluser", Password = "WrongPass1" }).ConfigureAwait(false);
        var missing = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "missing", Password = "WrongPass1" }).ConfigureAwait(false);

        Assert.That(success.Status, Is.EqualTo(PlayerLoginStatus.Authenticated));
        Assert.That(success.Session!.Token, Has.Length.EqualTo(43));
        Assert.That(success.Session.ExpiresAt, Is.EqualTo(this._time.GetUtcNow().Add(SessionLifetime)));
        Assert.That(wrong.Status, Is.EqualTo(PlayerLoginStatus.InvalidCredentials));
        Assert.That(missing.Status, Is.EqualTo(wrong.Status));
    }

    /// <summary>Banned, temporarily banned, spectator, and privileged accounts cannot create portal sessions.</summary>
    [TestCase(AccountState.Banned)]
    [TestCase(AccountState.TemporarilyBanned)]
    [TestCase(AccountState.Spectator)]
    [TestCase(AccountState.GameMaster)]
    [TestCase(AccountState.GameMasterInvisible)]
    public async Task NonNormalAccountCannotLoginAsync(AccountState state)
    {
        await this._service.RegisterAsync(Registration("portaluser", "GamePass1")).ConfigureAwait(false);
        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = await context.GetAccountByLoginNameAsync("portaluser").ConfigureAwait(false);
            account!.State = state;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var result = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "portaluser", Password = "GamePass1" }).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(PlayerLoginStatus.InvalidCredentials));
        Assert.That(result.Session, Is.Null);
    }

    /// <summary>Account data returned to a session is derived only from its stored owner ID.</summary>
    [Test]
    public async Task EachSessionReadsOnlyItsOwnAccountAndCharactersAsync()
    {
        await this._service.RegisterAsync(Registration("ownerone", "GamePass1")).ConfigureAwait(false);
        await this._service.RegisterAsync(Registration("ownertwo", "GamePass2")).ConfigureAwait(false);
        await this.AddCharacterAsync("ownerone", "OwnerHero", 75, 12).ConfigureAwait(false);
        await this.AddCharacterAsync("ownertwo", "OtherHero", 250, 40).ConfigureAwait(false);
        var owner = await this.GetAccountAsync("ownerone").ConfigureAwait(false);
        var other = await this.GetAccountAsync("ownertwo").ConfigureAwait(false);
        var session = this._sessions.Create(owner!.GetId(), SessionLifetime);
        var controller = new PlayerPortalController(this._service, this._sessions)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
        controller.Request.Headers[PlayerPortalApiDefaults.SessionHeaderName] = session.Token;
        controller.Request.QueryString = new QueryString($"?accountId={other!.GetId()}");

        var result = await controller.GetOwnAccountAsync(CancellationToken.None).ConfigureAwait(false);

        var response = (PlayerAccountSnapshot)((OkObjectResult)result).Value!;
        Assert.That(response.LoginName, Is.EqualTo("ownerone"));
        Assert.That(response.Characters.Select(character => character.Name), Is.EqualTo(new[] { "OwnerHero" }));
        Assert.That(response.Characters.Single().Level, Is.EqualTo(75));
        Assert.That(response.Characters.Single().MasterLevel, Is.EqualTo(12));

        var otherSession = this._sessions.Create(other!.GetId(), SessionLifetime);
        var otherController = new PlayerPortalController(this._service, this._sessions)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
        otherController.Request.Headers[PlayerPortalApiDefaults.SessionHeaderName] = otherSession.Token;
        otherController.Request.QueryString = new QueryString($"?accountId={owner!.GetId()}");

        var otherResult = await otherController.GetOwnAccountAsync(CancellationToken.None).ConfigureAwait(false);

        var otherResponse = (PlayerAccountSnapshot)((OkObjectResult)otherResult).Value!;
        Assert.That(otherResponse.LoginName, Is.EqualTo("ownertwo"));
        Assert.That(otherResponse.Characters.Select(character => character.Name), Is.EqualTo(new[] { "OtherHero" }));
        Assert.That(otherResponse.Characters.Single().Level, Is.EqualTo(250));
        Assert.That(otherResponse.Characters.Single().MasterLevel, Is.EqualTo(40));
        Assert.That(typeof(PlayerPortalController).GetMethod(nameof(PlayerPortalController.GetOwnAccountAsync))!.GetParameters(), Has.Length.EqualTo(1));
    }

    /// <summary>The login limiter blocks the eleventh request in a one-minute account window.</summary>
    [Test]
    public async Task RepeatedLoginAttemptsAreLimitedAsync()
    {
        await this._service.RegisterAsync(Registration("portaluser", "GamePass1")).ConfigureAwait(false);
        for (var index = 0; index < PlayerLoginAttemptLimiter.MaximumAttempts; index++)
        {
            var failed = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "portaluser", Password = "WrongPass1" }).ConfigureAwait(false);
            Assert.That(failed.Status, Is.EqualTo(PlayerLoginStatus.InvalidCredentials));
        }

        var limited = await this._service.LoginAsync(new PlayerLoginRequest { LoginName = "portaluser", Password = "WrongPass1" }).ConfigureAwait(false);
        Assert.That(limited.Status, Is.EqualTo(PlayerLoginStatus.RateLimited));
        Assert.That(limited.RetryAfter, Is.EqualTo(PlayerLoginAttemptLimiter.Window));
    }

    /// <summary>Expired and explicitly revoked session tokens no longer resolve to an account.</summary>
    [Test]
    public void SessionExpiryAndRevocationAreEnforced()
    {
        var expiring = this._sessions.Create(Guid.NewGuid(), TimeSpan.FromMinutes(30));
        var expiringForRevoke = this._sessions.Create(Guid.NewGuid(), TimeSpan.FromMinutes(30));
        Assert.That(this._sessions.TryGetAccountId(expiring.Token, out _), Is.True);
        this._time.Advance(TimeSpan.FromMinutes(30));
        Assert.That(this._sessions.TryGetAccountId(expiring.Token, out _), Is.False);
        Assert.That(this._sessions.Revoke(expiringForRevoke.Token), Is.False);

        var revocable = this._sessions.Create(Guid.NewGuid(), TimeSpan.FromMinutes(30));
        Assert.That(this._sessions.Revoke(revocable.Token), Is.True);
        Assert.That(this._sessions.TryGetAccountId(revocable.Token, out _), Is.False);
        Assert.That(this._sessions.Revoke(revocable.Token), Is.False);
    }

    /// <summary>Duplicate registration responses do not reveal account existence.</summary>
    [Test]
    public async Task RegistrationReturnsSamePublicResponseForCreatedAndDuplicateAsync()
    {
        var controller = new PlayerPortalController(this._service, this._sessions)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        var created = await controller.RegisterAsync(Registration("portaluser", "GamePass1"), CancellationToken.None).ConfigureAwait(false);
        var duplicate = await controller.RegisterAsync(Registration("portaluser", "Changed22"), CancellationToken.None).ConfigureAwait(false);

        Assert.That(created, Is.InstanceOf<AcceptedResult>());
        Assert.That(duplicate, Is.InstanceOf<AcceptedResult>());
        Assert.That(((AcceptedResult)created).Value, Is.EqualTo(((AcceptedResult)duplicate).Value));
    }

    /// <summary>Every route is policy-protected, API startup fails closed without a Worker secret, and logout revokes its token.</summary>
    [Test]
    public void ControllerRequiresWorkerAndLogoutRevokesSession()
    {
        var authorization = typeof(PlayerPortalController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();
        Assert.That(authorization.Policy, Is.EqualTo(PlayerPortalApiDefaults.WorkerAuthorizationPolicy));
        Assert.That(() => new ServiceCollection().AddPlayerPortalApi(new ConfigurationBuilder().Build()), Throws.InvalidOperationException);

        var session = this._sessions.Create(Guid.NewGuid(), SessionLifetime);
        var controller = new PlayerPortalController(this._service, this._sessions)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
        controller.Request.Headers[PlayerPortalApiDefaults.SessionHeaderName] = session.Token;

        Assert.That(controller.Logout(), Is.InstanceOf<NoContentResult>());
        Assert.That(this._sessions.TryGetAccountId(session.Token, out _), Is.False);
    }

    /// <summary>The private API registers the native account registration service it consumes.</summary>
    [Test]
    public void PlayerPortalApiRegistersNativeAccountRegistrationService()
    {
        var services = new ServiceCollection();
        services.AddPlayerPortalApi(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            [PlayerPortalApiDefaults.WorkerTokenConfigurationKey] = "synthetic-worker-secret-with-32-bytes-minimum",
        }).Build());

        Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(AccountRegistrationService)), Is.True);
    }

    /// <summary>Worker authentication rejects missing, weakly-configured, and incorrect service credentials.</summary>
    [Test]
    public async Task WorkerAuthenticationRequiresDistinctBearerCredentialAsync()
    {
        const string workerSecret = "synthetic-worker-secret-with-32-bytes-minimum";
        var loggerProvider = new CapturingLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider).SetMinimumLevel(LogLevel.Trace));
        var options = new FixedOptionsMonitor<AuthenticationSchemeOptions>(new AuthenticationSchemeOptions());

        var unauthenticated = await AuthenticateWorkerAsync(workerSecret, null, options, loggerFactory).ConfigureAwait(false);
        var rejected = await AuthenticateWorkerAsync(workerSecret, "Bearer wrong-secret-value", options, loggerFactory).ConfigureAwait(false);
        var accepted = await AuthenticateWorkerAsync(workerSecret, $"Bearer {workerSecret}", options, loggerFactory).ConfigureAwait(false);

        Assert.That(unauthenticated.Succeeded, Is.False);
        Assert.That(rejected.Succeeded, Is.False);
        Assert.That(accepted.Succeeded, Is.True);
        Assert.That(loggerProvider.Messages.Any(message => message.Contains(workerSecret, StringComparison.Ordinal)), Is.False);
    }

    private static AccountRegistration Registration(string loginName, string password) => new() { LoginName = loginName, Password = password };

    private static async Task<AuthenticateResult> AuthenticateWorkerAsync(string configuredSecret, string? presentedCredential, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory loggerFactory)
    {
        var context = new DefaultHttpContext();
        if (presentedCredential is not null)
        {
            context.Request.Headers.Authorization = presentedCredential;
        }

        var handler = new PlayerPortalWorkerAuthenticationHandler(options, loggerFactory, UrlEncoder.Default, new PlayerPortalWorkerCredential(configuredSecret));
        var authenticationHandler = (IAuthenticationHandler)handler;
        await authenticationHandler.InitializeAsync(new AuthenticationScheme(PlayerPortalApiDefaults.WorkerAuthenticationScheme, "", typeof(PlayerPortalWorkerAuthenticationHandler)), context).ConfigureAwait(false);
        return await authenticationHandler.AuthenticateAsync().ConfigureAwait(false);
    }

    private async Task AddCharacterAsync(string loginName, string characterName, int level, int masterLevel)
    {
        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        var account = await context.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
        var character = context.CreateNew<Character>();
        character.Name = characterName;
        character.CharacterSlot = 0;
        character.CharacterClass = new CharacterClass { Name = "Dark Wizard", Number = 1 };
        character.Attributes.Add(context.CreateNew<StatAttribute>(Stats.Level, (float)level));
        character.Attributes.Add(context.CreateNew<StatAttribute>(Stats.MasterLevel, (float)masterLevel));
        account!.Characters.Add(character);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task<Account?> GetAccountAsync(string loginName)
    {
        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        return await context.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
    }

    private sealed class TestTimeProvider(DateTimeOffset now) : TimeProvider
    {
        private DateTimeOffset _now = now;

        public override DateTimeOffset GetUtcNow() => this._now;

        public void Advance(TimeSpan amount) => this._now = this._now.Add(amount);
    }

    private sealed class FixedOptionsMonitor<TOptions>(TOptions currentValue) : IOptionsMonitor<TOptions>
        where TOptions : class
    {
        public TOptions CurrentValue => currentValue;

        public TOptions Get(string? name) => currentValue;

        public IDisposable? OnChange(Action<TOptions, string?> listener) => null;
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public List<string> Messages { get; } = [];

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(this.Messages);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(List<string> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
                => messages.Add(formatter(state, exception));
        }
    }
}
