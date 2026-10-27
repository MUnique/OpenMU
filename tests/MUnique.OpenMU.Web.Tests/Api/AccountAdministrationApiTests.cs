// <copyright file="AccountAdministrationApiTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.Tests.Mcp;

/// <summary>Exercises account REST routes with real authentication, persistence and auditing.</summary>
[TestFixture]
public sealed class AccountAdministrationApiTests : System.IDisposable
{
    private McpIntegrationTests _host = null!;
    private HttpClient _http = null!;
    private InMemoryAdminAuditRepository _audit = null!;

    /// <summary>Starts an isolated HTTP host with an offline test account.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._host = new McpIntegrationTests();
        await this._host.SetUpAsync().ConfigureAwait(false);
        this._http = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(this._host.App.Urls.Single()) };
        this._audit = (InMemoryAdminAuditRepository)this._host.App.Services.GetRequiredService<IAdminAuditRepository>();
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte>());
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_mcp-admin-test");
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

    /// <summary>Viewer reads use the existing safe snapshots and hide credentials.</summary>
    [TestCase("/api/accounts/tester")]
    [TestCase("/api/characters/Hero")]
    public async Task ViewerReadsSafeSnapshotsAsync(string path)
    {
        this.UseViewerKey();
        using var response = await this._http.GetAsync(path).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.That(body, Does.Contain("Hero").And.Not.Contain("secret-password-hash").And.Not.Contain("secret-security-code").And.Not.Contain("secret-vault-password"));
    }

    /// <summary>All account routes require authentication, including during initial setup.</summary>
    [TestCase("/api/accounts/tester")]
    [TestCase("/api/characters/Hero")]
    public async Task AnonymousReadsAreRefusedAsync(string path)
    {
        this._http.DefaultRequestHeaders.Clear();
        using var response = await this._http.GetAsync(path).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    /// <summary>Only missing resources map to 404.</summary>
    [TestCase("/api/accounts/missing")]
    [TestCase("/api/characters/missing")]
    public async Task MissingResourcesReturnNotFoundAsync(string path)
    {
        using var response = await this._http.GetAsync(path).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    /// <summary>Unexpected argument failures must not be disguised as missing resources.</summary>
    [TestCase("/api/accounts/tester")]
    [TestCase("/api/characters/Hero")]
    public async Task UnexpectedArgumentFailuresDoNotReturnNotFoundAsync(string path)
    {
        Mock.Get(this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>())
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Unexpected configuration failure."));
        using var response = await this._http.GetAsync(path).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    /// <summary>New mutations reject an authenticated operator cookie without a token but accept an API key.</summary>
    [TestCase("disconnect", "POST")]
    [TestCase("state", "PUT")]
    [TestCase("password", "PUT")]
    public async Task MutationsRequireApiKeyAsync(string operation, string method)
    {
        this._http.DefaultRequestHeaders.Clear();
        this._http.DefaultRequestHeaders.Host = "admin.example.test";
        var options = this._host.App.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>().Get(CookieAuthenticationDefaults.AuthenticationScheme);
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "test-operator"), new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, AdminRoles.Viewer), new Claim(ClaimTypes.Role, AdminRoles.Operator)], CookieAuthenticationDefaults.AuthenticationScheme);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), CookieAuthenticationDefaults.AuthenticationScheme);
        this._http.DefaultRequestHeaders.Add("Cookie", $"{options.Cookie.Name}={options.TicketDataFormat.Protect(ticket)}");
        using var read = await this._http.GetAsync("/api/accounts/tester").ConfigureAwait(false);
        Assert.That(read.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        if (operation == "disconnect")
        {
            var server = new Mock<IGameServer>();
            server.SetupGet(s => s.Id).Returns(4);
            server.Setup(s => s.DisconnectAccountAsync("tester")).ReturnsAsync(true);
            Mock.Get(this._host.App.Services.GetRequiredService<IServerProvider>()).SetupGet(s => s.Servers).Returns(new List<IManageableServer> { server.Object });
            Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["tester"] = 4 });
        }

        var path = operation == "disconnect" ? "/api/players/tester/disconnect" : $"/api/accounts/tester/{operation}";
        using var request = new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = operation == "disconnect" ? new FormUrlEncodedContent([]) : JsonContent.Create(new { password = "secret2", state = "Banned", reason = "Test" }),
        };
        request.Headers.Add("Origin", "https://community.example.test");
        using var refused = await this._http.SendAsync(request).ConfigureAwait(false);
        Assert.That(refused.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
        this._http.DefaultRequestHeaders.Clear();
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_mcp-admin-test");
        using var acceptedRequest = new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = operation == "disconnect" ? new FormUrlEncodedContent([]) : JsonContent.Create(new { password = "secret2", state = "Banned", reason = "Test" }),
        };
        using var accepted = await this._http.SendAsync(acceptedRequest).ConfigureAwait(false);
        Assert.That(accepted.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Succeeded"));
    }

    /// <summary>Viewer keys cannot invoke writes or register an account.</summary>
    [TestCase("password", "PUT")]
    [TestCase("state", "PUT")]
    [TestCase("disconnect", "POST")]
    [TestCase("register", "POST")]
    public async Task ViewerCannotWriteAsync(string operation, string method)
    {
        this.UseViewerKey();
        var path = operation == "disconnect" ? "/api/players/tester/disconnect" : operation == "register" ? "/api/accounts" : $"/api/accounts/tester/{operation}";
        using var request = new HttpRequestMessage(new HttpMethod(method), path) { Content = JsonContent.Create(new { password = "secret2", state = "Banned", reason = "Test" }) };
        using var response = await this._http.SendAsync(request).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>State and password changes use shared validation, persistence and secret-free audit records.</summary>
    [Test]
    public async Task UpdatesAndAuditsOfflineAccountAsync()
    {
        using var state = await this._http.PutAsJsonAsync("/api/accounts/tester/state", new { state = "Banned", reason = "Test restriction" }).ConfigureAwait(false);
        Assert.That(state.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var password = await this._http.PutAsJsonAsync("/api/accounts/tester/password", new { password = "secret2", reason = "Test recovery" }).ConfigureAwait(false);
        Assert.That(password.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false));
        var account = await context.GetAccountByLoginNameAsync("tester").ConfigureAwait(false);
        Assert.That(account!.State, Is.EqualTo(AccountState.Banned));
        Assert.That(BCrypt.Net.BCrypt.Verify("secret2", account.PasswordHash), Is.True);
        var audit = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items;
        Assert.That(audit.Select(e => e.Operation), Is.EquivalentTo(new[] { "accounts_set_state", "accounts_reset_password" }));
        Assert.That(audit.All(e => e.Frontend == "api" && e.Outcome == "Succeeded"), Is.True);
        Assert.That(JsonSerializer.Serialize(audit), Does.Not.Contain("secret2"));
        Assert.That(await password.Content.ReadAsStringAsync().ConfigureAwait(false), Does.Not.Contain("secret2"));
    }

    /// <summary>Online accounts and unavailable audit storage fail before the account changes.</summary>
    [TestCase(true, HttpStatusCode.Conflict)]
    [TestCase(false, HttpStatusCode.ServiceUnavailable)]
    public async Task RefusesUnsafeWritesAsync(bool online, HttpStatusCode expected)
    {
        if (online)
        {
            Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["tester"] = 4 });
        }
        else
        {
            this._audit.RejectWrites = true;
        }

        using var response = await this._http.PutAsJsonAsync("/api/accounts/tester/state", new { state = "Banned", reason = "Test restriction" }).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(expected));
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false));
        Assert.That((await context.GetAccountByLoginNameAsync("tester").ConfigureAwait(false))!.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>Missing and invalid states cannot accidentally restore an account to the default state.</summary>
    [TestCase("{}")]
    [TestCase("{\"state\":null,\"reason\":\"Test\"}")]
    [TestCase("{\"state\":\"unknown\",\"reason\":\"Test\"}")]
    [TestCase("{\"state\":999,\"reason\":\"Test\"}")]
    [TestCase("{\"state\":\"Banned\",\"reason\":\"\"}")]
    public async Task InvalidStateRequestsAreRefusedAsync(string json)
    {
        using var request = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        using var response = await this._http.PutAsync("/api/accounts/tester/state", request).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Disconnect routes to the owning server and records an API action.</summary>
    [Test]
    public async Task DisconnectsThroughOwningServerAsync()
    {
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        server.Setup(s => s.DisconnectAccountAsync("tester")).ReturnsAsync(true);
        Mock.Get(this._host.App.Services.GetRequiredService<IServerProvider>()).SetupGet(s => s.Servers).Returns(new List<IManageableServer> { server.Object });
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["tester"] = 4 });
        using var response = await this._http.PostAsync("/api/players/tester/disconnect", null).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await response.Content.ReadFromJsonAsync<JsonElement>().ConfigureAwait(false)).GetProperty("disconnected").GetBoolean(), Is.True);
        server.Verify(s => s.DisconnectAccountAsync("tester"), Times.Once);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Frontend, Is.EqualTo("api"));
    }

    private void UseViewerKey()
    {
        this._http.DefaultRequestHeaders.Clear();
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_viewer-key-for-mcp-integration-tests");
    }
}
