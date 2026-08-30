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
    private const string UnknownKey = "0123456789abcdef0123456789abcdef";

    private InMemoryApiKeyRepository _repository = null!;

    /// <summary>
    /// Sets a fresh repository up for each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._repository = new InMemoryApiKeyRepository();
    }

    /// <summary>
    /// Tests that a key which has been created in the admin panel is accepted, and that only its
    /// hash is stored.
    /// </summary>
    [Test]
    public async Task StoredKeyIsFoundAsync()
    {
        var generatedKey = await this.AddStoredKeyAsync("website", AdminRoles.Operator).ConfigureAwait(false);
        var registry = this.CreateRegistry();

        var client = await registry.FindAsync(generatedKey).ConfigureAwait(false);

        Assert.That(client, Is.Not.Null);
        Assert.That(client!.Name, Is.EqualTo("website"));
        Assert.That(client.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator }));

        var stored = (await this._repository.GetAllAsync().ConfigureAwait(false)).Single();
        Assert.That(stored.KeyHash, Is.Not.EqualTo(generatedKey));
        Assert.That(stored.KeyPrefix, Is.EqualTo(generatedKey[..ApiKeyGenerator.VisiblePrefixLength]));
    }

    /// <summary>
    /// Tests that an unknown key is not accepted.
    /// </summary>
    [Test]
    public async Task UnknownKeyIsNotFoundAsync()
    {
        var generatedKey = await this.AddStoredKeyAsync("website", AdminRoles.Viewer).ConfigureAwait(false);
        var registry = this.CreateRegistry();

        Assert.That(await registry.FindAsync(UnknownKey).ConfigureAwait(false), Is.Null);
        Assert.That(await registry.FindAsync(generatedKey + "x").ConfigureAwait(false), Is.Null);
        Assert.That(await registry.FindAsync(string.Empty).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that a key gets the least privileged role when none is assigned, and that the roles
    /// of a key build up on each other like they do for a user.
    /// </summary>
    [Test]
    public async Task RolesBuildUpOnEachOtherAsync()
    {
        var readerKey = await this.AddStoredKeyAsync("reader", string.Empty).ConfigureAwait(false);
        var writerKey = await this.AddStoredKeyAsync("writer", AdminRoles.Operator).ConfigureAwait(false);
        var registry = this.CreateRegistry();

        var reader = await registry.FindAsync(readerKey).ConfigureAwait(false);
        var writer = await registry.FindAsync(writerKey).ConfigureAwait(false);

        Assert.That(reader!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer }));
        Assert.That(writer!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator }));
    }

    /// <summary>
    /// Tests that a disabled key is rejected without having to delete it.
    /// </summary>
    [Test]
    public async Task DisabledStoredKeyIsRejectedAsync()
    {
        var generatedKey = await this.AddStoredKeyAsync("website", AdminRoles.Viewer).ConfigureAwait(false);
        (await this._repository.GetAllAsync().ConfigureAwait(false)).Single().IsDisabled = true;
        var registry = this.CreateRegistry();

        Assert.That(await registry.FindAsync(generatedKey).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that two generated keys are not the same, and that each of them has enough entropy.
    /// </summary>
    [Test]
    public void GeneratedKeysAreUnique()
    {
        var keys = Enumerable.Range(0, 50).Select(_ => ApiKeyGenerator.GenerateKey()).ToList();

        Assert.That(keys.Distinct(StringComparer.Ordinal).Count(), Is.EqualTo(keys.Count));
        Assert.That(keys, Is.All.StartWith(ApiKeyGenerator.KeyPrefix));
        Assert.That(keys, Is.All.Length.AtLeast(ApiKeyAuthenticationDefaults.MinimumKeyLength));
    }

    /// <summary>
    /// Tests that the handler authenticates a request which carries the key in its own header.
    /// </summary>
    [Test]
    public async Task ApiKeyHeaderAuthenticatesAsync()
    {
        var generatedKey = await this.AddStoredKeyAsync("launcher", AdminRoles.Viewer).ConfigureAwait(false);
        var context = new DefaultHttpContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = generatedKey;

        var result = await this.AuthenticateAsync(context).ConfigureAwait(false);

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
        var generatedKey = await this.AddStoredKeyAsync("launcher", AdminRoles.Viewer).ConfigureAwait(false);
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.Authorization] = $"Bearer {generatedKey}";

        var result = await this.AuthenticateAsync(context).ConfigureAwait(false);

        Assert.That(result.Succeeded, Is.True);
    }

    /// <summary>
    /// Tests that a request without a key is not a failure, so that the authentication cookie of
    /// the admin panel still gets its chance on the same request.
    /// </summary>
    [Test]
    public async Task RequestWithoutKeyIsNoResultAsync()
    {
        var result = await this.AuthenticateAsync(new DefaultHttpContext()).ConfigureAwait(false);

        Assert.That(result.None, Is.True);
        Assert.That(result.Succeeded, Is.False);
    }

    /// <summary>
    /// Tests that a request with a wrong key fails instead of staying anonymous.
    /// </summary>
    [Test]
    public async Task RequestWithWrongKeyFailsAsync()
    {
        await this.AddStoredKeyAsync("launcher", AdminRoles.Viewer).ConfigureAwait(false);
        var context = new DefaultHttpContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = UnknownKey;

        var result = await this.AuthenticateAsync(context).ConfigureAwait(false);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.None, Is.False);
        Assert.That(result.Failure, Is.Not.Null);
    }

    private async Task<string> AddStoredKeyAsync(string name, string roles)
    {
        var generatedKey = ApiKeyGenerator.GenerateKey();
        await this._repository.AddAsync(new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = name,
            KeyHash = ApiKeyGenerator.Hash(generatedKey),
            KeyPrefix = ApiKeyGenerator.GetVisiblePrefix(generatedKey),
            Roles = roles,
        }).ConfigureAwait(false);
        return generatedKey;
    }

    private ApiKeyRegistry CreateRegistry() => new(this._repository);

    private async Task<AuthenticateResult> AuthenticateAsync(HttpContext context)
    {
        var handler = new ApiKeyAuthenticationHandler(
            new StaticOptionsMonitor(),
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            this.CreateRegistry());
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
