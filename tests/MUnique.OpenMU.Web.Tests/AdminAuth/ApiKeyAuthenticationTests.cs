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
    private const string ConfiguredKey = "0123456789abcdef0123456789abcdef";
    private const string OtherConfiguredKey = "fedcba9876543210fedcba9876543210";

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
    /// Tests that a configured key resolves to its client.
    /// </summary>
    [Test]
    public async Task ConfiguredKeyIsFoundAsync()
    {
        var registry = this.CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ConfiguredKey });

        var client = await registry.FindAsync(ConfiguredKey).ConfigureAwait(false);

        Assert.That(client, Is.Not.Null);
        Assert.That(client!.Name, Is.EqualTo("launcher"));
    }

    /// <summary>
    /// Tests that an unknown key is not accepted.
    /// </summary>
    [Test]
    public async Task UnknownKeyIsNotFoundAsync()
    {
        var registry = this.CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ConfiguredKey });

        Assert.That(await registry.FindAsync(OtherConfiguredKey).ConfigureAwait(false), Is.Null);
        Assert.That(await registry.FindAsync(ConfiguredKey + "x").ConfigureAwait(false), Is.Null);
        Assert.That(await registry.FindAsync(string.Empty).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that a configured key which is too short to be safe is not usable at all.
    /// </summary>
    [Test]
    public async Task TooShortConfiguredKeyIsIgnoredAsync()
    {
        var shortKey = new string('a', ApiKeyAuthenticationDefaults.MinimumKeyLength - 1);

        var registry = this.CreateRegistry(new ApiKeyEntry { Name = "sloppy", Key = shortKey });

        Assert.That(await registry.FindAsync(shortKey).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that a client without configured roles gets the least privileged role, and that the
    /// roles of a client build up on each other like they do for a user.
    /// </summary>
    [Test]
    public async Task RolesBuildUpOnEachOtherAsync()
    {
        var registry = this.CreateRegistry(
            new ApiKeyEntry { Name = "reader", Key = ConfiguredKey },
            new ApiKeyEntry { Name = "writer", Key = OtherConfiguredKey, Roles = AdminRoles.Operator });

        var reader = await registry.FindAsync(ConfiguredKey).ConfigureAwait(false);
        var writer = await registry.FindAsync(OtherConfiguredKey).ConfigureAwait(false);

        Assert.That(reader!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer }));
        Assert.That(writer!.Roles, Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator }));
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
    /// Tests that the last usage of a stored key is not written on every single request.
    /// </summary>
    [Test]
    public async Task LastUsageIsUpdatedAtMostOncePerIntervalAsync()
    {
        var generatedKey = await this.AddStoredKeyAsync("website", AdminRoles.Viewer).ConfigureAwait(false);
        var registry = this.CreateRegistry();

        for (var i = 0; i < 5; i++)
        {
            await registry.FindAsync(generatedKey).ConfigureAwait(false);
        }

        Assert.That(this._repository.TouchCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that two generated keys are not the same.
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
        var context = new DefaultHttpContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = ConfiguredKey;

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
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.Authorization] = $"Bearer {ConfiguredKey}";

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
        var context = new DefaultHttpContext();
        context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName] = OtherConfiguredKey;

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

    private ApiKeyRegistry CreateRegistry(params ApiKeyEntry[] entries)
    {
        var options = Options.Create(new ApiKeyOptions { Keys = entries.ToList() });
        return new ApiKeyRegistry(options, this._repository, NullLogger<ApiKeyRegistry>.Instance);
    }

    private async Task<AuthenticateResult> AuthenticateAsync(HttpContext context)
    {
        var registry = this.CreateRegistry(new ApiKeyEntry { Name = "launcher", Key = ConfiguredKey });
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
