// <copyright file="WebsiteApiTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.Tests.Mcp;

/// <summary>Exercises the website API with real API key authentication, persistence and auditing.</summary>
[TestFixture]
public sealed class WebsiteApiTests : System.IDisposable
{
    private const string WebsiteKey = "omu_website-key-for-website-api-tests";

    private McpIntegrationTests _host = null!;
    private HttpClient _http = null!;
    private InMemoryAdminAuditRepository _audit = null!;

    /// <summary>Starts an isolated HTTP host.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._host = new McpIntegrationTests();
        await this._host.SetUpAsync().ConfigureAwait(false);
        this._http = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(this._host.App.Urls.Single()) };
        this._audit = (InMemoryAdminAuditRepository)this._host.App.Services.GetRequiredService<IAdminAuditRepository>();
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte>());
    }

    /// <summary>Stops the isolated host.</summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        this.Dispose();
        await this._host.TearDownAsync().ConfigureAwait(false);
    }

    /// <summary>Disposes the HTTP client owned by this fixture.</summary>
    public void Dispose()
    {
        this._http?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>A website key can use the functions which are selected for it, and nothing else.</summary>
    /// <param name="allowedFunctions">The functions which are selected for the key.</param>
    /// <returns>The status code of the response.</returns>
    [TestCase("website_status", ExpectedResult = HttpStatusCode.OK)]
    [TestCase("website_register", ExpectedResult = HttpStatusCode.Forbidden)]
    [TestCase("", ExpectedResult = HttpStatusCode.Forbidden)]
    public async Task<HttpStatusCode> WebsiteKeyNeedsSelectedFunctionAsync(string allowedFunctions)
    {
        await this.UseWebsiteKeyAsync(allowedFunctions).ConfigureAwait(false);
        using var response = await this._http.GetAsync("/api/website/status").ConfigureAwait(false);
        return response.StatusCode;
    }

    /// <summary>The keys of the administration and anonymous callers can't use the website API.</summary>
    /// <param name="key">The presented key; <c>null</c> for an anonymous request.</param>
    /// <returns>The status code of the response.</returns>
    [TestCase("omu_mcp-admin-test", ExpectedResult = HttpStatusCode.Forbidden)]
    [TestCase("omu_viewer-key-for-mcp-integration-tests", ExpectedResult = HttpStatusCode.Forbidden)]
    [TestCase(null, ExpectedResult = HttpStatusCode.Unauthorized)]
    public async Task<HttpStatusCode> OtherCallersAreRefusedAsync(string? key)
    {
        if (key is not null)
        {
            this._http.DefaultRequestHeaders.Add("X-Api-Key", key);
        }

        using var response = await this._http.GetAsync("/api/website/status").ConfigureAwait(false);
        return response.StatusCode;
    }

    /// <summary>A website key can't use the administration API, not even with all website functions.</summary>
    [Test]
    public async Task WebsiteKeyCantUseAdministrationApiAsync()
    {
        await this.UseWebsiteKeyAsync(string.Join(',', WebsitePermissions.All)).ConfigureAwait(false);
        using var account = await this._http.GetAsync("/api/accounts/tester").ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);

        Assert.That(account.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        Assert.That(registration.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    /// <summary>The registration creates an account once, and is audited with the website key.</summary>
    [Test]
    public async Task RegistrationCreatesAccountOnceAsync()
    {
        await this.UseWebsiteKeyAsync(WebsitePermissions.Register).ConfigureAwait(false);

        using var created = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1", eMail = "visitor@example.com" }).ConfigureAwait(false);
        using var repeated = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "other1" }).ConfigureAwait(false);
        using var invalid = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "x", password = "secret1" }).ConfigureAwait(false);

        Assert.That(created.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(repeated.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        Assert.That(invalid.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false));
        var account = await context.GetAccountByLoginNameAsync("visitor").ConfigureAwait(false);
        Assert.That(account, Is.Not.Null);
        Assert.That(BCrypt.Net.BCrypt.Verify("secret1", account!.PasswordHash), Is.True);
        var audit = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Where(e => e.Operation == WebsitePermissions.Register).ToList();
        Assert.That(audit, Is.Not.Empty);
        Assert.That(audit.Select(e => (e.Frontend, e.PrincipalName)), Has.All.EqualTo(("api", "test-website")));
        Assert.That(audit.Select(e => e.ArgumentsJson), Has.None.Contains("secret1"));
    }

    private async Task UseWebsiteKeyAsync(string allowedFunctions)
    {
        await this._host.App.Services.GetRequiredService<IApiKeyRepository>().AddAsync(new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = "test-website",
            KeyHash = ApiKeyGenerator.Hash(WebsiteKey),
            Roles = AdminRoles.WebManager,
            AllowedToolGroups = allowedFunctions,
        }).ConfigureAwait(false);
        this._http.DefaultRequestHeaders.Clear();
        this._http.DefaultRequestHeaders.Add("X-Api-Key", WebsiteKey);
    }
}
