// <copyright file="DiscordCommandsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord;

/// <summary>
/// Tests for the <see cref="DiscordCommands"/>.
/// </summary>
[TestFixture]
public class DiscordCommandsTest
{
    private FakeGameDataProvider _data = null!;

    /// <summary>
    /// Sets up the data.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._data = new FakeGameDataProvider();
        this._data.Servers.Add(new GameServerStatus(0, "Server 1", true, 12, 100));
        this._data.Servers.Add(new GameServerStatus(1, "Server 2", false, 0, 100));
        this._data.Characters.Add(new CharacterInfo("Hero", "Blade Knight||de=Klingenritter", 400, 120, 2, "Server 1"));
        this._data.Characters.Add(new CharacterInfo("Novice", "Dark Wizard", 50, 0, 0, null));
        this._data.Guilds.Add(new GuildInfo("Legends", "Hero", 12, new[] { "Hero", "Elf" }));
    }

    /// <summary>
    /// Tests the online players of each server and in total.
    /// </summary>
    [Test]
    public async Task OnlineShowsServersAndTotalAsync()
    {
        var answer = await CreateCommands("en", this._data).ExecuteAsync("online", null).ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo("Server 1: **12** / 100\nServer 2: offline\nTotal: **12** players"));
    }

    /// <summary>
    /// Tests the information about an online character, in German.
    /// </summary>
    [Test]
    public async Task WhoShowsCharacterAsync()
    {
        var answer = await CreateCommands("de", this._data).ExecuteAsync("who", "Hero").ConfigureAwait(false);

        Assert.That(answer.Title, Is.EqualTo("Hero"));
        Assert.That(answer.Description, Is.EqualTo("Klingenritter\nLevel 400, Meisterlevel 120, Resets 2\nOnline auf Server 1"));
    }

    /// <summary>
    /// Tests the answer for an unknown character.
    /// </summary>
    [Test]
    public async Task WhoOfUnknownCharacterAsync()
    {
        var answer = await CreateCommands("en", this._data).ExecuteAsync("who", "Nobody_").ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo(@"There is no character with the name **Nobody\_**."));
    }

    /// <summary>
    /// Tests the information about a guild.
    /// </summary>
    [Test]
    public async Task GuildShowsMasterAndOnlineMembersAsync()
    {
        var answer = await CreateCommands("en", this._data).ExecuteAsync("guild", "Legends").ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo("Guild master: Hero\nMembers: 12\nOnline: Hero, Elf"));
    }

    /// <summary>
    /// Tests the upcoming events, with the start as Discord time markup.
    /// </summary>
    [Test]
    public async Task EventsShowsStartAsync()
    {
        var start = new DateTime(2026, 10, 8, 20, 0, 0, DateTimeKind.Utc);
        this._data.Events.Add(new UpcomingEventInfo("Blood Castle event", start));

        var answer = await CreateCommands("en", this._data).ExecuteAsync("events", null).ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo($"<t:{new DateTimeOffset(start).ToUnixTimeSeconds()}:R> Blood Castle event"));
    }

    /// <summary>
    /// Tests the ranking.
    /// </summary>
    [Test]
    public async Task RankShowsCharactersInOrderAsync()
    {
        var answer = await CreateCommands("en", this._data).ExecuteAsync("rank", null).ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo("1. **Hero** (Blade Knight) – Level 400, Master level 120, Resets 2\n2. **Novice** (Dark Wizard) – Level 50"));
    }

    /// <summary>
    /// Tests that an error results in a friendly answer.
    /// </summary>
    [Test]
    public async Task ErrorResultsInFriendlyAnswerAsync()
    {
        this._data.Fails = true;

        var answer = await CreateCommands("en", this._data).ExecuteAsync("who", "Hero").ConfigureAwait(false);

        Assert.That(answer.Description, Is.EqualTo("Sorry, that didn't work. Please try again later."));
    }

    /// <summary>
    /// Tests that all descriptions of the commands are defined in the resources, because Discord requires them.
    /// </summary>
    [Test]
    public void AllCommandDescriptionsExist()
    {
        var commands = CreateCommands("en", this._data);
        foreach (var definition in DiscordCommands.Definitions)
        {
            Assert.That(commands.Text(definition.DescriptionKey), Is.Not.EqualTo(definition.DescriptionKey));
            if (definition.OptionDescriptionKey is { } optionKey)
            {
                Assert.That(commands.Text(optionKey), Is.Not.EqualTo(optionKey));
            }
        }
    }

    private static DiscordCommands CreateCommands(string language, IDiscordGameDataProvider data)
        => new(data, CultureInfo.GetCultureInfo(language), NullLogger<DiscordCommands>.Instance);
}
