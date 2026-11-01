// <copyright file="DiscordConfigurationServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.IO;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="DiscordConfigurationService"/>.
/// </summary>
[TestFixture]
public class DiscordConfigurationServiceTests
{
    private const string ConfigurationJson = """
        {
          // A comment, which is allowed in the configuration files of .NET.
          "AllowedHosts": "*",
          "Discord": {
            "Language": "de",
            "Webhooks": { "Events": "https://discord.example/api/webhooks/1/secret" },
            "Bot": {
              "Token": "token",
              "GuildId": 123456789012345678,
              "Channels": { "WorldNews": "42" },
              "StatusUpdateInterval": "00:02:00",
              "ChatBridge": { "BindingMode": "HostedOnly", "AllowedDiscordServerIds": [ 1, 2 ] }
            }
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
    /// Tests that the settings are loaded like the server binds them.
    /// </summary>
    [Test]
    public async Task LoadsSettingsAsync()
    {
        var settings = await this.CreateService().LoadAsync().ConfigureAwait(false);

        Assert.That(settings.Language, Is.EqualTo("de"));
        Assert.That(settings.Webhooks[DiscordChannelCategory.Events], Is.EqualTo("https://discord.example/api/webhooks/1/secret"));
        Assert.That(settings.Bot.Token, Is.EqualTo("token"));
        Assert.That(settings.Bot.GuildId, Is.EqualTo(123456789012345678UL));
        Assert.That(settings.Bot.Channels[DiscordChannelCategory.WorldNews], Is.EqualTo(42UL));
        Assert.That(settings.Bot.StatusUpdateInterval, Is.EqualTo(TimeSpan.FromMinutes(2)));
        Assert.That(settings.Bot.ChatBridge.BindingMode, Is.EqualTo(MUnique.OpenMU.Discord.ChatBridge.DiscordGuildChatBindingMode.HostedOnly));
        Assert.That(settings.Bot.ChatBridge.AllowedDiscordServerIds, Is.EqualTo(new ulong[] { 1, 2 }));
    }

    /// <summary>
    /// Tests that saved settings are loaded again in the same way, that the rest of the file is kept,
    /// and that empty webhooks and the computed properties are not written.
    /// </summary>
    [Test]
    public async Task SavedSettingsAreLoadedAgainAsync()
    {
        var service = this.CreateService();
        var settings = await service.LoadAsync().ConfigureAwait(false);
        settings.Webhooks[DiscordChannelCategory.Notices] = " ";
        settings.Bot.Channels[DiscordChannelCategory.Events] = 43;
        settings.Bot.ChatBridge.ReadMessages = true;

        await service.SaveAsync(settings).ConfigureAwait(false);
        var loaded = await service.LoadAsync().ConfigureAwait(false);

        var root = JsonNode.Parse(File.ReadAllText(this._filePath))!;
        Assert.That(root["AllowedHosts"]?.ToString(), Is.EqualTo("*"));
        Assert.That(root["Discord"]!["Bot"]!["IsEnabled"], Is.Null);
        Assert.That(root["Discord"]!["Webhooks"]!["Notices"], Is.Null);
        Assert.That(root["Discord"]!["Bot"]!["ChatBridge"]!["BindingMode"]?.ToString(), Is.EqualTo("HostedOnly"));
        Assert.That(loaded.Bot.Channels, Has.Count.EqualTo(2));
        Assert.That(loaded.Bot.Channels[DiscordChannelCategory.Events], Is.EqualTo(43UL));
        Assert.That(loaded.Bot.ChatBridge.ReadMessages, Is.True);
        Assert.That(loaded.Bot.Token, Is.EqualTo("token"));
        Assert.That(loaded.Webhooks.Keys, Is.EquivalentTo(new[] { DiscordChannelCategory.Events }));
    }

    /// <summary>
    /// Tests that a setting which another configuration source defines is recognized.
    /// </summary>
    [Test]
    public void RecognizesSettingsWhichAreDefinedElsewhere()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(this._filePath)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Discord:Bot:Token"] = "from the environment",
                ["Discord:Webhooks:Notices"] = "https://discord.example/api/webhooks/2/secret",
                ["Discord:EventAnnouncingServerIds:0"] = "1",
            })
            .Build();
        var service = new DiscordConfigurationService(this._filePath, configuration);

        Assert.That(service.IsDefinedElsewhere("Bot:Token"), Is.True);
        Assert.That(service.IsDefinedElsewhere("Webhooks:Notices"), Is.True);
        Assert.That(service.IsDefinedElsewhere("EventAnnouncingServerIds"), Is.True);
        Assert.That(service.IsDefinedElsewhere("Webhooks:Events"), Is.False);
        Assert.That(service.IsDefinedElsewhere("Language"), Is.False);
        Assert.That(service.IsDefinedElsewhere("Bot:LayoutFile"), Is.False);
    }

    /// <summary>
    /// Managed values are displayed correctly but never copied from the environment into the file.
    /// </summary>
    [Test]
    public async Task ManagedValuesAreDisplayedWithoutPersistingSecretsAsync()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile(this._filePath)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Discord:Language"] = "en",
                ["Discord:Bot:Token"] = "environment-secret",
                ["Discord:Webhooks:Notices"] = "https://discord.example/environment-secret",
                ["Discord:Bot:ChatBridge:AllowedDiscordServerIds:0"] = "99",
            }).Build();
        var service = new DiscordConfigurationService(this._filePath, configuration);
        var editable = await service.LoadForEditingAsync().ConfigureAwait(false);
        Assert.That(editable.Language, Is.EqualTo("en"));
        Assert.That(editable.Bot.Token, Is.EqualTo("environment-secret"));
        Assert.That(editable.Bot.ChatBridge.AllowedDiscordServerIds[0], Is.EqualTo(99));
        editable.MaximumQueuedMessages = 200;
        await service.SaveAsync(editable).ConfigureAwait(false);
        var saved = await service.LoadAsync().ConfigureAwait(false);
        Assert.That(saved.Language, Is.EqualTo("de"));
        Assert.That(saved.Bot.Token, Is.EqualTo("token"));
        Assert.That(saved.Webhooks.ContainsKey(DiscordChannelCategory.Notices), Is.False);
        Assert.That(saved.Bot.ChatBridge.AllowedDiscordServerIds, Is.EqualTo(new ulong[] { 1, 2 }));
        Assert.That(saved.MaximumQueuedMessages, Is.EqualTo(200));
        Assert.That(File.ReadAllText(this._filePath), Does.Not.Contain("environment-secret"));
    }

    private DiscordConfigurationService CreateService()
    {
        return new DiscordConfigurationService(this._filePath, new ConfigurationBuilder().AddJsonFile(this._filePath).Build());
    }
}
