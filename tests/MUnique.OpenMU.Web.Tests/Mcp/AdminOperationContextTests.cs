// <copyright file="AdminOperationContextTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Services;

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
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>());
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
        var context = new AdminOperationContext(new HttpContextAccessor { HttpContext = http }, Mock.Of<AuthenticationStateProvider>());
        Assert.ThrowsAsync<AdminOperationException>(async () => await context.GetActorAsync(default).ConfigureAwait(false));
    }

    /// <summary>Known safe operational failures map to useful HTTP statuses.</summary>
    [TestCase(true, 403)]
    [TestCase(false, 503)]
    public void MapsOnlyExplicitlySafeErrors(bool forbidden, int expected)
    {
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>())
        {
            Exception = new AdminOperationException("Safe operational error", forbidden),
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
}
