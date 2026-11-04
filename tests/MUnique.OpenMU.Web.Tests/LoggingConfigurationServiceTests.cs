// <copyright file="LoggingConfigurationServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="LoggingConfigurationService"/>.
/// </summary>
[TestFixture]
public class LoggingConfigurationServiceTests
{
    private const string ConfigurationJson = """
        {
          // A comment, which is allowed in the configuration files of .NET.
          "AllowedHosts": "*",
          "Serilog": {
            "Using": [ "Serilog.Sinks.Console" ],
            "MinimumLevel": {
              "Default": "information",
              "Override": {
                "Microsoft": "Fatal",
                "MUnique.OpenMU.Network.Connection": "Error"
              }
            },
            "WriteTo": [ { "Name": "Console", "Args": { "outputTemplate": "{Timestamp:HH:mm:ss zzz} [{Level}] {Message}{NewLine}" } } ]
          }
        }
        """;

    private string _filePath = string.Empty;

    /// <summary>
    /// Creates the configuration file.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._filePath = Path.GetTempFileName();
        File.WriteAllText(this._filePath, ConfigurationJson);
    }

    /// <summary>
    /// Deletes the configuration file.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        File.Delete(this._filePath);
    }

    /// <summary>
    /// Tests that the levels are loaded, and their spelling normalized.
    /// </summary>
    [Test]
    public async Task LoadsLevelsAsync()
    {
        var settings = await new LoggingConfigurationService(this._filePath).LoadAsync().ConfigureAwait(false);

        Assert.That(settings.DefaultLevel, Is.EqualTo("Information"));
        Assert.That(settings.Overrides.Select(o => (o.Source, o.Level)), Is.EqualTo(new[] { ("Microsoft", "Fatal"), ("MUnique.OpenMU.Network.Connection", "Error") }));
        Assert.That(JsonNode.Parse(settings.SectionJson)?["Using"]?[0]?.ToString(), Is.EqualTo("Serilog.Sinks.Console"));
    }

    /// <summary>
    /// Tests that saving the levels replaces them, and keeps everything else of the file.
    /// </summary>
    [Test]
    public async Task SavesLevelsAndKeepsTheRestAsync()
    {
        var service = new LoggingConfigurationService(this._filePath);
        var overrides = new[] { new LogLevelOverride { Source = " MUnique.OpenMU.GameLogic ", Level = "Debug" } };

        await service.SaveMinimumLevelsAsync("Warning", overrides).ConfigureAwait(false);

        var root = JsonNode.Parse(await File.ReadAllTextAsync(this._filePath).ConfigureAwait(false))!;
        Assert.That(root["AllowedHosts"]?.ToString(), Is.EqualTo("*"));
        Assert.That(root["Serilog"]?["MinimumLevel"]?["Default"]?.ToString(), Is.EqualTo("Warning"));
        Assert.That(root["Serilog"]?["MinimumLevel"]?["Override"]?.AsObject().Select(o => (o.Key, o.Value?.ToString())), Is.EqualTo(new[] { ("MUnique.OpenMU.GameLogic", "Debug") }));
        Assert.That(root["Serilog"]?["WriteTo"]?[0]?["Args"]?["outputTemplate"]?.ToString(), Is.EqualTo("{Timestamp:HH:mm:ss zzz} [{Level}] {Message}{NewLine}"));

        var settings = await service.LoadAsync().ConfigureAwait(false);
        Assert.That(settings.DefaultLevel, Is.EqualTo("Warning"));
    }

    /// <summary>
    /// Tests that the short form of the minimum level is kept, so that Serilog can still reload it.
    /// </summary>
    [Test]
    public async Task KeepsShortFormOfMinimumLevelAsync()
    {
        await File.WriteAllTextAsync(this._filePath, """{ "Serilog": { "MinimumLevel": "Information" } }""").ConfigureAwait(false);
        var service = new LoggingConfigurationService(this._filePath);

        await service.SaveMinimumLevelsAsync("Debug", []).ConfigureAwait(false);

        var root = JsonNode.Parse(await File.ReadAllTextAsync(this._filePath).ConfigureAwait(false))!;
        Assert.That(root["Serilog"]?["MinimumLevel"]?.ToString(), Is.EqualTo("Debug"));
    }

    /// <summary>
    /// Tests that invalid levels and duplicated sources are rejected without changing the file.
    /// </summary>
    [Test]
    public void RejectsInvalidLevels()
    {
        var service = new LoggingConfigurationService(this._filePath);

        Assert.ThrowsAsync<ArgumentException>(async () => await service.SaveMinimumLevelsAsync("Loud", []).ConfigureAwait(false));
        Assert.ThrowsAsync<ArgumentException>(async () => await service.SaveMinimumLevelsAsync("Debug", [new LogLevelOverride { Source = string.Empty }]).ConfigureAwait(false));
        Assert.ThrowsAsync<ArgumentException>(async () => await service.SaveMinimumLevelsAsync("Debug", [new LogLevelOverride { Source = "A" }, new LogLevelOverride { Source = "A " }]).ConfigureAwait(false));
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(ConfigurationJson));
    }

    /// <summary>
    /// Tests that the whole section can be replaced, and the rest of the file is kept.
    /// </summary>
    [Test]
    public async Task SavesSectionAsync()
    {
        var service = new LoggingConfigurationService(this._filePath);

        await service.SaveSectionAsync("""{ "MinimumLevel": "Verbose" }""").ConfigureAwait(false);

        var root = JsonNode.Parse(await File.ReadAllTextAsync(this._filePath).ConfigureAwait(false))!;
        Assert.That(root["AllowedHosts"]?.ToString(), Is.EqualTo("*"));
        Assert.That(root["Serilog"]?.ToJsonString(), Is.EqualTo("""{"MinimumLevel":"Verbose"}"""));
    }

    /// <summary>
    /// Tests that an invalid section is rejected without changing the file.
    /// </summary>
    /// <param name="sectionJson">The invalid JSON of the section.</param>
    [TestCase("""{ "MinimumLevel": """)]
    [TestCase("[]")]
    public void RejectsInvalidSection(string sectionJson)
    {
        var service = new LoggingConfigurationService(this._filePath);

        Assert.That(async () => await service.SaveSectionAsync(sectionJson).ConfigureAwait(false), Throws.InstanceOf<JsonException>());
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(ConfigurationJson));
    }
}
