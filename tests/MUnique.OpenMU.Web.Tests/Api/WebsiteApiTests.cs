// <copyright file="WebsiteApiTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;
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

    /// <summary>The profile of a character is public, but doesn't reveal its account, its secrets or its whereabouts.</summary>
    [Test]
    public async Task CharacterProfileHidesAccountAsync()
    {
        await this.UseWebsiteKeyAsync(WebsitePermissions.Character).ConfigureAwait(false);
        await this.EquipItemAsync("tester", "Hero").ConfigureAwait(false);

        using var found = await this._http.GetAsync("/api/website/characters/Hero").ConfigureAwait(false);
        using var missing = await this._http.GetAsync("/api/website/characters/missing").ConfigureAwait(false);

        Assert.That(found.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(missing.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var body = await found.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.That(body, Does.Contain("Hero").And.Not.Contain("tester").And.Not.Contain("secret"));
        using var json = JsonDocument.Parse(body);
        Assert.That(json.RootElement.GetProperty("equippedItems").GetArrayLength(), Is.GreaterThan(0));
        var hidden = new[] { "money", "position", "positionX", "positionY", "map", "isOnline", "loginName", "inventory" };
        Assert.That(GetPropertyNames(json.RootElement).Where(name => hidden.Contains(name, StringComparer.OrdinalIgnoreCase)), Is.Empty);
    }

    /// <summary>Like in the ranking, the characters of game masters, bots and templates have no public profile.</summary>
    [Test]
    public async Task CharacterProfileHidesGameMastersBotsAndTemplatesAsync()
    {
        await this.UseWebsiteKeyAsync(WebsitePermissions.Character).ConfigureAwait(false);

        await this.ChangeAccountAsync("tester", account => account.Characters.Single(c => c.Name == "Hero").CharacterStatus = CharacterStatus.GameMaster).ConfigureAwait(false);
        using var gameMaster = await this._http.GetAsync("/api/website/characters/Hero").ConfigureAwait(false);
        await this.ChangeAccountAsync("tester", account =>
        {
            account.Characters.Single(c => c.Name == "Hero").CharacterStatus = CharacterStatus.Normal;
            account.IsBot = true;
        }).ConfigureAwait(false);
        using var bot = await this._http.GetAsync("/api/website/characters/Hero").ConfigureAwait(false);
        await this.ChangeAccountAsync("tester", account =>
        {
            account.IsBot = false;
            account.IsTemplate = true;
        }).ConfigureAwait(false);
        using var template = await this._http.GetAsync("/api/website/characters/Hero").ConfigureAwait(false);

        Assert.That(gameMaster.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(bot.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(template.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    /// <summary>A locked out login answers <c>429</c> and tells the remaining time of the lockout.</summary>
    [Test]
    public async Task LockedOutLoginTellsRemainingTimeAsync()
    {
        await this.UseWebsiteKeyAsync(WebsitePermissions.Login + "," + WebsitePermissions.Register).ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);
        for (var i = 0; i < WebsiteLoginThrottle.MaximumFailures; i++)
        {
            using var failed = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "wrong1" }).ConfigureAwait(false);
            Assert.That(failed.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        using var locked = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);

        Assert.That(locked.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
        Assert.That(locked.Headers.RetryAfter?.Delta, Is.GreaterThan(TimeSpan.Zero).And.LessThanOrEqualTo(WebsiteLoginThrottle.Window));
    }

    private static IEnumerable<string> GetPropertyNames(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject().SelectMany(property => GetPropertyNames(property.Value).Prepend(property.Name)),
            JsonValueKind.Array => element.EnumerateArray().SelectMany(GetPropertyNames),
            _ => [],
        };
    }

    private async Task ChangeAccountAsync(string loginName, Action<Account> change)
    {
        var gameConfiguration = await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false);
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(gameConfiguration);
        var account = await context.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
        change(account!);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task EquipItemAsync(string loginName, string characterName)
    {
        var gameConfiguration = await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false);
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(gameConfiguration);
        var account = await context.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
        var character = account!.Characters.Single(c => c.Name == characterName);
        character.Inventory ??= context.CreateNew<ItemStorage>();
        var item = context.CreateNew<Item>();
        item.Definition = gameConfiguration.Items.First();
        item.ItemSlot = 0;
        character.Inventory.Items.Add(item);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>The login answers with the account, or the same status for a wrong password and an unknown account.</summary>
    [Test]
    public async Task LoginAnswersAsync()
    {
        await this.UseWebsiteKeyAsync(WebsitePermissions.Login + "," + WebsitePermissions.Register).ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);

        using var success = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);
        using var wrongPassword = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "wrong1" }).ConfigureAwait(false);
        using var unknown = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "nobody", password = "secret1" }).ConfigureAwait(false);

        Assert.That(success.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await success.Content.ReadAsStringAsync().ConfigureAwait(false), Does.Contain("visitor").And.Not.Contain("secret1"));
        Assert.That(wrongPassword.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(unknown.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["visitor"] = 0 });
        using var online = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);
        Assert.That(await online.Content.ReadAsStringAsync().ConfigureAwait(false), Does.Contain("\"isOnline\":true"));
        Assert.That(await unknown.Content.ReadAsStringAsync().ConfigureAwait(false), Is.EqualTo(await wrongPassword.Content.ReadAsStringAsync().ConfigureAwait(false)));
    }

    /// <summary>A player changes the password with the current one; a wrong current password changes nothing.</summary>
    [Test]
    public async Task PasswordChangeNeedsCurrentPasswordAsync()
    {
        await this.UseWebsiteKeyAsync(string.Join(',', WebsitePermissions.Register, WebsitePermissions.Login, WebsitePermissions.ChangePassword)).ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);

        using var wrong = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "wrong1", newPassword = "changed1" }).ConfigureAwait(false);
        using var invalid = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "secret1", newPassword = "x" }).ConfigureAwait(false);
        using var changed = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "secret1", newPassword = "changed1" }).ConfigureAwait(false);
        using var login = await this._http.PostAsJsonAsync("/api/website/login", new { loginName = "visitor", password = "changed1" }).ConfigureAwait(false);

        Assert.That(wrong.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(invalid.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(changed.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(login.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var audit = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Where(e => e.Operation == WebsitePermissions.ChangePassword).ToList();
        Assert.That(audit.Select(e => e.ArgumentsJson + e.ResultJson), Has.None.Contains("secret1").And.None.Contains("changed1"));
    }

    /// <summary>
    /// Each checked current password of a password change is audited, but a locked out account answers <c>429</c>
    /// before anything is checked or audited.
    /// </summary>
    [Test]
    public async Task LockedOutPasswordChangeIsNotAuditedAsync()
    {
        await this.UseWebsiteKeyAsync(string.Join(',', WebsitePermissions.Register, WebsitePermissions.ChangePassword)).ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);
        for (var i = 0; i < WebsiteLoginThrottle.MaximumFailures; i++)
        {
            using var wrong = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "wrong1", newPassword = "changed1" }).ConfigureAwait(false);
            Assert.That(wrong.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        using var locked = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "secret1", newPassword = "changed1" }).ConfigureAwait(false);

        Assert.That(locked.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
        Assert.That(locked.Headers.RetryAfter?.Delta, Is.GreaterThan(TimeSpan.Zero).And.LessThanOrEqualTo(WebsiteLoginThrottle.Window));
        var audit = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Where(e => e.Operation == WebsitePermissions.ChangePassword).ToList();
        Assert.That(audit, Has.Count.EqualTo(WebsiteLoginThrottle.MaximumFailures));
    }

    /// <summary>The password of an online account can't be changed, because the game server would save the old one.</summary>
    [Test]
    public async Task PasswordOfOnlineAccountIsNotChangedAsync()
    {
        await this.UseWebsiteKeyAsync(string.Join(',', WebsitePermissions.Register, WebsitePermissions.ChangePassword)).ConfigureAwait(false);
        using var registration = await this._http.PostAsJsonAsync("/api/website/accounts", new { loginName = "visitor", password = "secret1" }).ConfigureAwait(false);
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["visitor"] = 0 });

        using var response = await this._http.PutAsJsonAsync("/api/website/accounts/visitor/password", new { currentPassword = "secret1", newPassword = "changed1" }).ConfigureAwait(false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
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
