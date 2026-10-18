// <copyright file="ServerAnnouncementsPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="ServerAnnouncementsPlugIn"/>.
/// </summary>
[TestFixture]
public class ServerAnnouncementsPlugInTest
{
    /// <summary>
    /// Tests that the first announcement is shown right away, and that the next one waits for the interval.
    /// </summary>
    [Test]
    public async Task ShowsOneAnnouncementPerIntervalAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(TimeSpan.FromHours(1), ("First", MessageType.GoldenCenter), ("Second", MessageType.BlueNormal));

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        view.Verify(p => p.ShowMessageAsync("First", MessageType.GoldenCenter), Times.Once);
        view.Verify(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Once);
    }

    /// <summary>
    /// Tests that the announcements are shown in their order, and that the rotation starts over after the last one.
    /// </summary>
    [Test]
    public async Task RotatesThroughTheAnnouncementsAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(TimeSpan.FromHours(1), ("First", MessageType.GoldenCenter), ("Second", MessageType.BlueNormal));
        var shown = new List<string>();
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        view.Setup(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()))
            .Callback((string message, MessageType _) => shown.Add(message))
            .Returns(ValueTask.CompletedTask);

        for (var i = 0; i < 3; i++)
        {
            await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
            plugIn.ForceStart();
        }

        Assert.That(shown, Is.EqualTo(new[] { "First", "Second", "First" }));
    }

    /// <summary>
    /// Tests that an interval of zero doesn't show an announcement on every run.
    /// </summary>
    [Test]
    public async Task ZeroIntervalDoesNotShowAnAnnouncementOnEveryRunAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(TimeSpan.Zero, ("First", MessageType.GoldenCenter), ("Second", MessageType.BlueNormal));

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Once);
    }

    /// <summary>
    /// Tests that the text is shown in the language of the player, if it has a translation.
    /// </summary>
    [Test]
    public async Task ShowsTheTranslationOfThePlayerAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        player.Culture = CultureInfo.GetCultureInfo("de");
        var text = new LocalizedString("Hello").WithTranslation(CultureInfo.GetCultureInfo("de"), "Hallo");
        var plugIn = new ServerAnnouncementsPlugIn
        {
            Configuration = new ServerAnnouncementsConfiguration
            {
                Messages = { new() { Text = text, Type = MessageType.BlueNormal } },
            },
        };

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(p => p.ShowMessageAsync("Hallo", MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// Tests that nothing is shown without announcements.
    /// </summary>
    [Test]
    public async Task ShowsNothingWithoutAnnouncementsAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new ServerAnnouncementsPlugIn { Configuration = new ServerAnnouncementsConfiguration() };

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never);
    }

    /// <summary>
    /// Tests that the configuration, including the translations of the texts, survives a plugin configuration round trip
    /// with the JSON converters which the server registers at startup.
    /// </summary>
    [Test]
    public void ConfigurationRoundTripsThroughPlugInConfiguration()
    {
        var registerConverter = !JsonConverterRegistry.Converters.OfType<LocalizedStringJsonConverter>().Any();
        if (registerConverter)
        {
            JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        }

        try
        {
            AssertConfigurationRoundTrip();
        }
        finally
        {
            if (registerConverter)
            {
                JsonConverterRegistry.ClearConverters();
            }
        }
    }

    private static void AssertConfigurationRoundTrip()
    {
        var plugInConfiguration = new PlugInConfiguration();
        var configuration = new ServerAnnouncementsConfiguration { Interval = TimeSpan.FromMinutes(5) };
        configuration.Messages.Add(new() { Text = new LocalizedString("Hello").WithTranslation(CultureInfo.GetCultureInfo("de"), "Hallo"), Type = MessageType.BlueNormal });

        plugInConfiguration.SetConfiguration(configuration, null);
        var restored = plugInConfiguration.GetConfiguration<ServerAnnouncementsConfiguration>(null);

        Assert.That(restored, Is.Not.Null);
        var announcement = restored!.Messages.Single();
        Assert.Multiple(() =>
        {
            Assert.That(restored.Interval, Is.EqualTo(TimeSpan.FromMinutes(5)));
            Assert.That(announcement.Type, Is.EqualTo(MessageType.BlueNormal));
            Assert.That(announcement.Text.GetTranslation(CultureInfo.InvariantCulture), Is.EqualTo("Hello"));
            Assert.That(announcement.Text.GetTranslation(CultureInfo.GetCultureInfo("de")), Is.EqualTo("Hallo"));
        });
    }

    private static ServerAnnouncementsPlugIn CreatePlugIn(TimeSpan interval, params (string Text, MessageType Type)[] announcements)
    {
        var configuration = new ServerAnnouncementsConfiguration { Interval = interval };
        foreach (var (text, type) in announcements)
        {
            configuration.Messages.Add(new() { Text = text, Type = type });
        }

        return new ServerAnnouncementsPlugIn { Configuration = configuration };
    }

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await player.GameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }
}
