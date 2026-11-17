// <copyright file="LoginTicketControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.LoginTickets;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>
/// Tests for the <see cref="LoginTicketController"/>, which issues login tickets through the public API.
/// </summary>
[TestFixture]
public class LoginTicketControllerTests
{
    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    private LoginTicketController _controller = null!;

    /// <summary>
    /// Sets up an in-memory database with an account, and the controller.
    /// </summary>
    /// <returns>The task.</returns>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        this._controller = this.CreateController(new Dictionary<string, string?>());

        using var context = this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration());
        var account = context.CreateNew<Account>();
        account.LoginName = "player";
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// An issued ticket logs into the account once.
    /// </summary>
    [Test]
    public async Task IssuedTicketCanBeRedeemedAsync()
    {
        var result = await this._controller.IssueAsync("player", CancellationToken.None).ConfigureAwait(false);

        Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        var ticket = (IssuedLoginTicket)((ObjectResult)result).Value!;
        Assert.That(ticket.ExpiresAt, Is.EqualTo(DateTime.UtcNow + LoginTicketService.DefaultLifetime).Within(TimeSpan.FromSeconds(10)));
        var service = new LoginTicketService(() => this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration()));
        Assert.That((await service.RedeemAsync(ticket.Token).ConfigureAwait(false)).Status, Is.EqualTo(LoginTicketRedeemStatus.Redeemed));
    }

    /// <summary>
    /// The lifetime of the tickets is taken from the configuration.
    /// </summary>
    [Test]
    public async Task LifetimeIsConfiguredAsync()
    {
        var controller = this.CreateController(new Dictionary<string, string?> { ["AdminPanel:LoginTickets:Lifetime"] = "00:00:30" });

        var ticket = (IssuedLoginTicket)((ObjectResult)await controller.IssueAsync("player", CancellationToken.None).ConfigureAwait(false)).Value!;

        Assert.That(ticket.ExpiresAt, Is.EqualTo(DateTime.UtcNow + TimeSpan.FromSeconds(30)).Within(TimeSpan.FromSeconds(10)));
    }

    /// <summary>
    /// An unknown account is answered with not found.
    /// </summary>
    [Test]
    public async Task UnknownAccountIsNotFoundAsync()
    {
        Assert.That(await this._controller.IssueAsync("unknown", CancellationToken.None).ConfigureAwait(false), Is.InstanceOf<NotFoundResult>());
    }

    /// <summary>
    /// Too many tickets for one account are refused.
    /// </summary>
    [Test]
    public async Task TooManyTicketsAreRefusedAsync()
    {
        for (var i = 0; i < LoginTicketService.MaximumTicketsPerLifetime; i++)
        {
            await this._controller.IssueAsync("player", CancellationToken.None).ConfigureAwait(false);
        }

        var result = await this._controller.IssueAsync("player", CancellationToken.None).ConfigureAwait(false);

        Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(StatusCodes.Status429TooManyRequests));
    }

    /// <summary>
    /// The endpoint requires its dedicated policy.
    /// </summary>
    [Test]
    public void EndpointRequiresLoginTicketPolicy()
    {
        var policies = typeof(LoginTicketController).GetCustomAttributes<AuthorizeAttribute>(true).Select(a => a.Policy);

        Assert.That(policies, Is.EqualTo(new[] { AdminPolicies.AuthenticatedLoginTicket }));
    }

    /// <summary>
    /// Only an API key with the login ticket role fulfills the policy of the endpoint.
    /// Not even an administrator key does, because the role allows to log into every account.
    /// </summary>
    /// <param name="role">The role of the API key.</param>
    /// <returns><c>true</c>, if the key is authorized.</returns>
    [TestCase(nameof(ApiKeyRole.Viewer), ExpectedResult = false)]
    [TestCase(nameof(ApiKeyRole.Operator), ExpectedResult = false)]
    [TestCase(nameof(ApiKeyRole.Administrator), ExpectedResult = false)]
    [TestCase(nameof(ApiKeyRole.CashShop), ExpectedResult = false)]
    [TestCase(nameof(ApiKeyRole.LoginTicket), ExpectedResult = true)]
    public async Task<bool> OnlyLoginTicketKeyIsAuthorizedAsync(string role)
    {
        var keys = new InMemoryApiKeyRepository();
        var key = ApiKeyGenerator.GenerateKey();
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "site", KeyHash = ApiKeyGenerator.Hash(key), Roles = role }).ConfigureAwait(false);
        var client = await new ApiKeyRegistry(keys).FindAsync(key).ConfigureAwait(false);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(client!.CreateClaims(), ApiKeyAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role));

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-login-ticket-test-keys") })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdminPanelAuth(configuration);
        await using var serviceProvider = services.BuildServiceProvider();
        var authorization = serviceProvider.GetRequiredService<IAuthorizationService>();

        return (await authorization.AuthorizeAsync(principal, AdminPolicies.AuthenticatedLoginTicket).ConfigureAwait(false)).Succeeded;
    }

    /// <summary>
    /// The login ticket role doesn't imply any other role, and no other role implies it.
    /// </summary>
    [Test]
    public void LoginTicketRoleStandsAlone()
    {
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.LoginTicket), Is.EqualTo(new[] { AdminRoles.LoginTicket }));
        Assert.That(AdminRoles.All.Append(AdminRoles.CashShop).SelectMany(AdminRoles.GetEffectiveRoles), Does.Not.Contain(AdminRoles.LoginTicket));
    }

    private LoginTicketController CreateController(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new LoginTicketController(this._persistenceContextProvider, configuration, Mock.Of<ILogger<LoginTicketController>>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }
}
