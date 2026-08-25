// <copyright file="ApiKeyAuthenticationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests for the API key authentication of the public API.
/// </summary>
[TestFixture]
public class ApiKeyAuthenticationTests
{
    private const string ValidKey = "0123456789abcdef0123456789abcdef";
    private const string OtherValidKey = "fedcba9876543210fedcba9876543210";

    /// <summary>
    /// Tests that a configured key resolves to its client.
    /// </summary>
    [Test]
    public void ConfiguredKeyIsFound()
    {
        var registry = CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ValidKey });

        var client = registry.Find(ValidKey);

        Assert.That(client, Is.Not.Null);
        Assert.That(client!.Name, Is.EqualTo("launcher"));
        Assert.That(registry.IsConfigured, Is.True);
    }

    /// <summary>
    /// Tests that an unknown key is not accepted.
    /// </summary>
    [Test]
    public void UnknownKeyIsNotFound()
    {
        var registry = CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ValidKey });

        Assert.That(registry.Find(OtherValidKey), Is.Null);
        Assert.That(registry.Find(ValidKey + "x"), Is.Null);
        Assert.That(registry.Find(string.Empty), Is.Null);
    }

    /// <summary>
    /// Tests that a key which is too short to be safe is not usable at all.
    /// </summary>
    [Test]
    public void TooShortKeyIsIgnored()
    {
        var shortKey = new string('a', ApiKeyAuthenticationDefaults.MinimumKeyLength - 1);

        var registry = CreateRegistry(new ApiKeyEntry { Name = "sloppy", Key = shortKey });

        Assert.That(registry.IsConfigured, Is.False);
        Assert.That(registry.Find(shortKey), Is.Null);
    }

    /// <summary>
    /// Tests that a client without configured roles gets the least privileged role, and that the
    /// roles of a client build up on each other like they do for a user.
    /// </summary>
    [Test]
    public void RolesBuildUpOnEachOther()
    {
        var registry = CreateRegistry(
            new ApiKeyEntry { Name = "reader", Key = ValidKey },
            new ApiKeyEntry { Name = "writer", Key = OtherValidKey, Roles = AdminRoles.Operator });

        Assert.That(registry.Find(ValidKey)!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer }));
        Assert.That(registry.Find(OtherValidKey)!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator }));
    }

    /// <summary>
    /// Tests that the handler authenticates a request which carries the key in its own header.
    /// </summary>
    [Test]
    public async Task ApiKeyHeaderAuthenticatesAsync()
    {
        var context = CreateContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = ValidKey;

        var result = await CreateHandlerAsync(context).ConfigureAwait(false);

        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Principal!.Identity!.IsAuthenticated, Is.True);
        Assert.That(result.Principal.FindFirstValue(ClaimTypes.Name), Is.EqualTo("launcher"));
        Assert.That(result.Principal.IsInRole(AdminRoles.Viewer), Is.True);
    }

    /// <summary>
    /// Tests that the handler also accepts the key as a bearer token.
    /// </summary>
    [Test]
    public async Task BearerTokenAuthenticatesAsync()
    {
        var context = CreateContext();
        context.Request.Headers[HeaderNames.Authorization] = $"Bearer {ValidKey}";

        var result = await CreateHandlerAsync(context).ConfigureAwait(false);

        Assert.That(result.Succeeded, Is.True);
    }

    /// <summary>
    /// Tests that a request without a key is not a failure, so that the authentication cookie of
    /// the admin panel still gets its chance on the same request.
    /// </summary>
    [Test]
    public async Task RequestWithoutKeyIsNoResultAsync()
    {
        var result = await CreateHandlerAsync(CreateContext()).ConfigureAwait(false);

        Assert.That(result.None, Is.True);
        Assert.That(result.Succeeded, Is.False);
    }

    /// <summary>
    /// Tests that a request with a wrong key fails instead of staying anonymous.
    /// </summary>
    [Test]
    public async Task RequestWithWrongKeyFailsAsync()
    {
        var context = CreateContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = OtherValidKey;

        var result = await CreateHandlerAsync(context).ConfigureAwait(false);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.None, Is.False);
        Assert.That(result.Failure, Is.Not.Null);
    }

    private static ApiKeyRegistry CreateRegistry(params ApiKeyEntry[] entries)
    {
        var options = Options.Create(new ApiKeyOptions { Keys = entries.ToList() });
        return new ApiKeyRegistry(options, NullLogger<ApiKeyRegistry>.Instance);
    }

    private static DefaultHttpContext CreateContext() => new();

    private static async Task<AuthenticateResult> CreateHandlerAsync(HttpContext context)
    {
        var registry = CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ValidKey });
        var handler = new ApiKeyAuthenticationHandler(
            new StaticOptionsMonitor(),
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            registry);
        var scheme = new AuthenticationScheme(
            ApiKeyAuthenticationDefaults.AuthenticationScheme,
            null,
            typeof(ApiKeyAuthenticationHandler));
        await handler.InitializeAsync(scheme, context).ConfigureAwait(false);
        return await handler.AuthenticateAsync().ConfigureAwait(false);
    }

    private sealed class StaticOptionsMonitor : IOptionsMonitor<AuthenticationSchemeOptions>
    {
        public AuthenticationSchemeOptions CurrentValue { get; } = new();

        public AuthenticationSchemeOptions Get(string? name) => this.CurrentValue;

        public IDisposable? OnChange(Action<AuthenticationSchemeOptions, string?> listener) => null;
    }
}
