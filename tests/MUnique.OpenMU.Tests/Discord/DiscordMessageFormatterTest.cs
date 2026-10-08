// <copyright file="DiscordMessageFormatterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests for the <see cref="DiscordMessageFormatter"/>.
/// </summary>
[TestFixture]
public class DiscordMessageFormatterTest
{
    private static readonly Guid OwnerGuildId = Guid.NewGuid();

    /// <summary>
    /// Tests that names are translated into the configured language and the server is named in the footer.
    /// </summary>
    [Test]
    public async Task NamesAreTranslatedAsync()
    {
        var formatter = CreateFormatter("de");

        var message = await formatter.FormatAsync(new BossKilledEvent(1, DateTime.UtcNow, "Hero", "Kundun||de=Kundun DE", "Kalima 7||de=Kalima Sieben")).ConfigureAwait(false);

        Assert.That(message?.Category, Is.EqualTo(DiscordChannelCategory.WorldNews));
        Assert.That(message?.Embed.Title, Is.EqualTo("Boss besiegt"));
        Assert.That(message?.Embed.Description, Is.EqualTo("**Hero** hat **Kundun DE** in Kalima Sieben besiegt!"));
        Assert.That(message?.Embed.Footer, Is.EqualTo("Server: Server 1"));
    }

    /// <summary>
    /// Tests that markdown in texts of players is escaped.
    /// </summary>
    [Test]
    public async Task MarkdownIsEscapedAsync()
    {
        var formatter = CreateFormatter("en");

        var message = await formatter.FormatAsync(new GlobalNoticeEvent(1, DateTime.UtcNow, "GM", "**Event** at _8_")).ConfigureAwait(false);

        Assert.That(message?.Category, Is.EqualTo(DiscordChannelCategory.Notices));
        Assert.That(message?.Embed.Description, Is.EqualTo(@"\*\*Event\*\* at \_8\_"));
    }

    /// <summary>
    /// Tests that the name of an item drop contains its qualities and level.
    /// </summary>
    [Test]
    public async Task ItemDropContainsQualityAndLevelAsync()
    {
        var formatter = CreateFormatter("en");

        var message = await formatter.FormatAsync(new MonsterItemDroppedEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7", "Sword of Destruction", 9, true, true)).ConfigureAwait(false);

        Assert.That(message?.Embed.Description, Is.EqualTo("**Hero** found **Excellent Ancient Sword of Destruction +9** from Kundun in Kalima 7."));
    }

    /// <summary>
    /// Tests that the end of the castle siege names the owner of the castle.
    /// </summary>
    [Test]
    public async Task CastleSiegeEndNamesTheOwnerAsync()
    {
        var formatter = CreateFormatter("en");

        var message = await formatter.FormatAsync(new CastleSiegeStateChangedEvent(1, DateTime.UtcNow, "Start", "End", DateTime.UtcNow, OwnerGuildId)).ConfigureAwait(false);

        Assert.That(message?.Category, Is.EqualTo(DiscordChannelCategory.CastleSiege));
        Assert.That(message?.Embed.Description, Is.EqualTo("The battle is over. **Winners** owns the castle."));
    }

    /// <summary>
    /// Tests that the opening of a mini game contains the time when the entrance closes, as Discord time markup.
    /// </summary>
    [Test]
    public async Task MiniGameOpeningContainsClosingTimeAsync()
    {
        var formatter = CreateFormatter("en");
        var closesAt = new DateTime(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc);

        var message = await formatter.FormatAsync(new MiniGameEntranceOpenedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, closesAt)).ConfigureAwait(false);

        Assert.That(message?.Embed.Title, Is.EqualTo("Blood Castle opens"));
        Assert.That(message?.Embed.Description, Is.EqualTo($"The entrance closes <t:{new DateTimeOffset(closesAt).ToUnixTimeSeconds()}:R>."));
    }

    /// <summary>
    /// Tests that events which aren't interesting for the players aren't posted.
    /// </summary>
    /// <param name="index">The index of the event in <see cref="GetIgnoredEvents"/>.</param>
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task UninterestingEventIsNotPostedAsync(int index)
    {
        var formatter = CreateFormatter("en");

        var message = await formatter.FormatAsync(GetIgnoredEvents()[index]).ConfigureAwait(false);

        Assert.That(message, Is.Null);
    }

    private static GameEvent[] GetIgnoredEvents() => new GameEvent[]
    {
        new MiniGameStartedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, 5),
        new MiniGameEndedEvent(1, DateTime.UtcNow, "DevilSquare", "Devil Square", 1, null, Array.Empty<string>()),
        new CastleSiegeStateChangedEvent(1, DateTime.UtcNow, "Idle1", "Idle2", DateTime.UtcNow, null),
    };

    private static DiscordMessageFormatter CreateFormatter(string language)
    {
        return new DiscordMessageFormatter(
            CultureInfo.GetCultureInfo(language),
            serverId => $"Server {serverId}",
            guildId => ValueTask.FromResult<string?>(guildId == OwnerGuildId ? "Winners" : null));
    }
}
