// <copyright file="LogOperationsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Globalization;
using System.IO;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>Verifies bounded log reads, filtering, redaction and deployment limitations.</summary>
[TestFixture]
public sealed class LogOperationsTests
{
    private string _directory = null!;
    private LoggingConfigurationService _settings = null!;

    /// <summary>Creates isolated log and configuration files.</summary>
    [SetUp]
    public void SetUp()
    {
        this._directory = Path.Combine(Path.GetTempPath(), "openmu-log-tests-" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
        Directory.CreateDirectory(this._directory);
        var path = Path.Combine(this._directory, "settings.json");
        File.WriteAllText(path, """{ "Serilog": { "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Error" } }, "WriteTo": [{ "Secret": "private-sink-token" }] } }""");
        this._settings = new LoggingConfigurationService(path);
    }

    /// <summary>Deletes only the files created by this fixture.</summary>
    [TearDown]
    public void TearDown() => Directory.Delete(this._directory, true);

    /// <summary>Search includes the final unterminated line, filters time and level, caps context and removes credentials and addresses.</summary>
    [Test]
    public async Task SearchesAndRedactsBoundedLinesAsync()
    {
        var time = DateTimeOffset.UtcNow;
        var stamp = time.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
        await File.WriteAllTextAsync(Path.Combine(this._directory, "log.txt"), string.Join('\n',
            stamp + " [Information] preceding context", stamp + " [Warning] Hero disconnected from 192.0.2.20",
            stamp + " [Warning] Password=private-password", stamp + " [Warning] Hero final line")).ConfigureAwait(false);
        var operations = this.Create(true);
        var result = await operations.SearchAsync("Hero", time.AddMinutes(-1), time.AddMinutes(1), "Warning", 10, 1, default).ConfigureAwait(false);
        var json = result.ToJsonString();
        Assert.That(json, Does.Contain("Hero final line").And.Contain("preceding context").And.Contain("address redacted"));
        Assert.That(json, Does.Not.Contain("private-password").And.Not.Contain("192.0.2.20"));
        var capped = await operations.SearchAsync("Hero", time.AddMinutes(-1), time.AddMinutes(1), null, 1, 3, default).ConfigureAwait(false);
        Assert.That(capped["lines"]?.AsArray().Count, Is.EqualTo(1));
        Assert.That(capped["lines"]?[0]?["text"]?.GetValue<string>(), Does.Contain("Hero"));
        Assert.That(capped["truncated"]?.GetValue<bool>(), Is.True);
    }

    /// <summary>Valid IP addresses are hidden while times and colon-separated identifiers remain searchable.</summary>
    [Test]
    public async Task RedactsAddressesWithoutCorruptingTimesAsync()
    {
        var time = DateTimeOffset.UtcNow;
        var stamp = time.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
        await File.WriteAllTextAsync(Path.Combine(this._directory, "log.txt"), stamp + " [Information] Hero at 12:30:00 id ab:cd:ef addresses 2001:db8::1234 ::1 192.0.2.20 ::ffff:192.0.2.10").ConfigureAwait(false);
        var result = await this.Create(true).SearchAsync("12:30", time.AddMinutes(-1), time.AddMinutes(1), null, 10, 0, default).ConfigureAwait(false);
        var text = result["lines"]!.AsArray().Single()!["text"]!.GetValue<string>();
        Assert.That(text, Does.StartWith(stamp).And.Contain("12:30:00").And.Contain("ab:cd:ef"));
        Assert.That(text, Does.Not.Contain("2001:db8").And.Not.Contain("::1").And.Not.Contain("192.0.2"));
    }

    /// <summary>Only minimum levels are returned, and changing them preserves secret sink configuration.</summary>
    [Test]
    public async Task LevelsExcludeSinkSecretsAsync()
    {
        var operations = this.Create(true);
        var level = await operations.GetLevelAsync(default).ConfigureAwait(false);
        Assert.That(level.ToJsonString(), Does.Not.Contain("private-sink-token").And.Not.Contain("WriteTo"));
        await operations.SetLevelAsync("Warning", default).ConfigureAwait(false);
        Assert.That((await operations.GetLevelAsync(default).ConfigureAwait(false))["defaultLevel"]?.GetValue<string>(), Is.EqualTo("Warning"));
        Assert.That((await this._settings.LoadAsync().ConfigureAwait(false)).Overrides.Single().Level, Is.EqualTo("Error"));
        Assert.That(await File.ReadAllTextAsync(this._settings.FilePath).ConfigureAwait(false), Does.Contain("private-sink-token"));
    }

    /// <summary>Distributed hosts give a useful hint and cannot silently change unrelated host levels.</summary>
    [Test]
    public async Task DistributedLogsExplainCapabilityLimitAsync()
    {
        var operations = this.Create(false);
        Assert.That((await operations.SearchAsync("", null, null, null, 50, 0, default).ConfigureAwait(false))["available"]?.GetValue<bool>(), Is.False);
        Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetLevelAsync("Warning", default).ConfigureAwait(false));
    }

    private LogOperations Create(bool local) => new(this._settings,
        new AdminLogAccess { LocalFilesAvailable = local, DirectoryPath = this._directory }, AdminOperationTestServices.CreateExecutor());
}
