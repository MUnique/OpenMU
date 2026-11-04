// <copyright file="AdminOperationContextTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Checks actor attribution and safe API error mapping at the front-end boundary.</summary>
[TestFixture]
public sealed class AdminOperationContextTests
{
    /// <summary>A cash-shop key cannot borrow Administrator claims from a simultaneous browser cookie.</summary>
    [Test]
    public async Task ApiKeyDoesNotInheritCookiePrivilegesAsync()
    {
        var http = new DefaultHttpContext();
        http.Request.Path = "/api/accounts/test/cash-shop";
        http.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "cookie-id"), new Claim(ClaimTypes.Name, "administrator"), new Claim(ClaimTypes.Role, AdminRoles.Administrator)], "Cookies"),
            new ClaimsIdentity([new Claim(ApiKeyAuthenticationDefaults.ClientIdClaimType, "key-id"), new Claim(ClaimTypes.Name, "payment-key"), new Claim(ClaimTypes.Role, AdminRoles.CashShop)], ApiKeyAuthenticationDefaults.AuthenticationScheme),
        });
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>(), CreateAvailability(new InMemoryAdminUserRepository()), new AdminOperationCircuitState());
        var actor = await context.GetActorAsync(default).ConfigureAwait(false);
        Assert.That(actor.Id, Is.EqualTo("key-id"));
        Assert.That(actor.Name, Is.EqualTo("payment-key"));
        Assert.That(actor.Roles, Is.EquivalentTo(new[] { AdminRoles.CashShop }));
        Assert.That(actor.Frontend, Is.EqualTo("api"));
    }

    /// <summary>The operation layer also rejects a browser-cookie identity on MCP.</summary>
    [Test]
    public void McpRequiresAKeyIdentity()
    {
        var http = new DefaultHttpContext();
        http.Request.Path = "/mcp";
        http.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "cookie-id"), new Claim(ClaimTypes.Role, AdminRoles.Administrator)], "Cookies"));
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>(), CreateAvailability(new InMemoryAdminUserRepository()), new AdminOperationCircuitState());
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    /// <summary>Known safe operational failures map to useful HTTP statuses.</summary>
    [TestCase(AdminOperationErrorKind.Forbidden, 403)]
    [TestCase(AdminOperationErrorKind.InvalidInput, 400)]
    [TestCase(AdminOperationErrorKind.Conflict, 409)]
    [TestCase(AdminOperationErrorKind.Unavailable, 503)]
    [TestCase(AdminOperationErrorKind.Uncertain, 409)]
    public void MapsOnlyExplicitlySafeErrors(AdminOperationErrorKind kind, int expected)
    {
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>())
        {
            Exception = new AdminOperationException("Safe operational error", kind),
        };
        new AdminOperationExceptionFilter().OnException(context);
        Assert.That(context.ExceptionHandled, Is.True);
        Assert.That(((ObjectResult)context.Result!).StatusCode, Is.EqualTo(expected));
        context.Exception = new InvalidOperationException("private-database-details");
        context.ExceptionHandled = false;
        context.Result = null;
        new AdminOperationExceptionFilter().OnException(context);
        Assert.That(context.ExceptionHandled, Is.False);
        Assert.That(context.Result, Is.Null);
    }

    /// <summary>Setup attribution is limited to panel circuits and ends when a real user exists.</summary>
    [Test]
    public async Task AnonymousSetupIsPanelOnlyAndEndsAfterFirstUserAsync()
    {
        var repository = new InMemoryAdminUserRepository();
        var availability = CreateAvailability(repository);
        var authentication = new Mock<AuthenticationStateProvider>();
        authentication.Setup(a => a.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        var http = new DefaultHttpContext();
        http.Request.Path = "/_blazor";
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, authentication.Object, availability, await OpenCircuitAsync().ConfigureAwait(false));
        var actor = await context.GetActorAsync(default).ConfigureAwait(false);
        Assert.That(actor.Id, Is.EqualTo("setup"));
        Assert.That(actor.Frontend, Is.EqualTo("panel"));
        Assert.That(actor.IsInitialSetup, Is.True);
        foreach (var path in new[] { "/api/send/0", "/mcp" })
        {
            http.Request.Path = path;
            Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
        }

        await repository.AddAsync(new AdminUser { Id = Guid.NewGuid(), LoginName = "admin" }).ConfigureAwait(false);
        availability.Invalidate();
        http.Request.Path = "/_blazor";
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    /// <summary>A configured bootstrap identity closes anonymous setup even without a database user.</summary>
    [Test]
    public async Task BootstrapUserPreventsAnonymousSetupAsync()
    {
        var options = new AdminPanelAuthOptions { BootstrapUser = new BootstrapAdminUserOptions { LoginName = "bootstrap", Password = "long-bootstrap-password" } };
        var authentication = new Mock<AuthenticationStateProvider>();
        authentication.Setup(a => a.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        var context = new AdminOperationContext(new HttpContextAccessor(), authentication.Object, CreateAvailability(new InMemoryAdminUserRepository(), options), await OpenCircuitAsync().ConfigureAwait(false));
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    /// <summary>Unknown HTTP routes cannot borrow a circuit principal or gain setup privileges.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public void UnknownHttpEndpointsNeverBecomePanelCallers(bool authenticated)
    {
        var http = new DefaultHttpContext();
        http.Request.Path = "/future-admin-action";
        var authentication = new Mock<AuthenticationStateProvider>();
        var principal = authenticated
            ? new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "admin"), new Claim(ClaimTypes.Role, AdminRoles.Administrator)], "Cookies"))
            : new ClaimsPrincipal(new ClaimsIdentity());
        authentication.Setup(a => a.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(principal));
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, authentication.Object,
            CreateAvailability(new InMemoryAdminUserRepository()), new AdminOperationCircuitState());
        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
        authentication.Verify(a => a.GetAuthenticationStateAsync(), Times.Never);
    }

    /// <summary>A closed circuit no longer grants anonymous setup access.</summary>
    [Test]
    public async Task ClosingCircuitRevokesPanelAttributionAsync()
    {
        var circuit = await OpenCircuitAsync().ConfigureAwait(false);
        var authentication = new Mock<AuthenticationStateProvider>();
        authentication.Setup(a => a.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        var context = new AdminOperationContext(new HttpContextAccessor(), authentication.Object,
            CreateAvailability(new InMemoryAdminUserRepository()), circuit);
        Assert.That((await context.GetActorAsync(default).ConfigureAwait(false)).IsInitialSetup, Is.True);
        await circuit.OnCircuitClosedAsync(null!, default).ConfigureAwait(false);
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    /// <summary>Only the circuit scope receives its lifecycle marker; ordinary HTTP scopes remain closed.</summary>
    [Test]
    public async Task CircuitMarkerRegistrationIsSharedAndScopedAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdminPanelAuth(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();
        using var circuitScope = provider.CreateScope();
        using var httpScope = provider.CreateScope();
        var marker = circuitScope.ServiceProvider.GetRequiredService<AdminOperationCircuitState>();
        var handler = circuitScope.ServiceProvider.GetServices<CircuitHandler>().OfType<AdminOperationCircuitState>().Single();
        Assert.That(handler, Is.SameAs(marker));
        await handler.OnCircuitOpenedAsync(null!, default).ConfigureAwait(false);
        Assert.That(marker.IsOpen, Is.True);
        Assert.That(httpScope.ServiceProvider.GetRequiredService<AdminOperationCircuitState>().IsOpen, Is.False);
    }

    private static async ValueTask<AdminOperationCircuitState> OpenCircuitAsync()
    {
        var circuit = new AdminOperationCircuitState();
        // The lifecycle token is unused; framework Circuit instances have no public constructor.
        await circuit.OnCircuitOpenedAsync(null!, default).ConfigureAwait(false);
        return circuit;
    }

    private static AdminUserAvailabilityService CreateAvailability(InMemoryAdminUserRepository repository, AdminPanelAuthOptions? options = null)
        => new(repository, new BootstrapAdminUserProvider(Options.Create(options ?? new AdminPanelAuthOptions()), new BCryptPasswordHasher(), new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance), NullLogger<BootstrapAdminUserProvider>.Instance));

}
