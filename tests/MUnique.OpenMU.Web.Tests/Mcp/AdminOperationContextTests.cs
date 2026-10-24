// <copyright file="AdminOperationContextTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
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
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>(), CreateAvailability(new InMemoryAdminUserRepository()));
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
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>(), CreateAvailability(new InMemoryAdminUserRepository()));
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
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, authentication.Object, availability);
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
    public void BootstrapUserPreventsAnonymousSetup()
    {
        var options = new AdminPanelAuthOptions { BootstrapUser = new BootstrapAdminUserOptions { LoginName = "bootstrap", Password = "long-bootstrap-password" } };
        var authentication = new Mock<AuthenticationStateProvider>();
        authentication.Setup(a => a.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        var context = new AdminOperationContext(new HttpContextAccessor(), authentication.Object, CreateAvailability(new InMemoryAdminUserRepository(), options));
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    private static AdminUserAvailabilityService CreateAvailability(InMemoryAdminUserRepository repository, AdminPanelAuthOptions? options = null)
        => new(repository, new BootstrapAdminUserProvider(Options.Create(options ?? new AdminPanelAuthOptions()), new BCryptPasswordHasher(), new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance), NullLogger<BootstrapAdminUserProvider>.Instance));

}
