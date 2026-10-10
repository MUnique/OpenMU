// <copyright file="DiscordIntegrationViewPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.IO;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using DiscordIntegrationInfo = MUnique.OpenMU.GameLogic.Discord.DiscordIntegrationInfo;
using DiscordIntegrationInfoPacket = MUnique.OpenMU.Network.Packets.ServerToClient.DiscordIntegrationInfoRef;

/// <summary>
/// Tests for the <see cref="DiscordIntegrationViewPlugIn"/> and the way the chat reaches the clients which
/// support the Discord integration.
/// </summary>
[TestFixture]
public class DiscordIntegrationViewPlugInTest
{
    private static readonly ClientVersion MuMainVersion = new(106, 3, ClientLanguage.Invariant);

    /// <summary>
    /// Tests that the integration info carries the configured values and the link state.
    /// </summary>
    [Test]
    public async Task IntegrationInfoCarriesTheValuesAsync()
    {
        var (player, output) = CreatePlayer();
        var configuration = new DiscordIntegrationConfiguration
        {
            InviteUrl = "https://discord.gg/abc123",
            RichPresenceApplicationId = "123456789012345678",
            RichPresenceLargeImageKey = "logo",
        };
        var plugIn = new DiscordIntegrationViewPlugIn(player);

        await plugIn.ShowDiscordIntegrationInfoAsync(new DiscordIntegrationInfo("sven", true, false, true, configuration)).ConfigureAwait(false);

        var packet = new DiscordIntegrationInfoPacket(ReadPackets(output)[0]);
        Assert.That(packet.Header.Code, Is.EqualTo(0xF5));
        Assert.That(packet.Header.SubCode, Is.EqualTo(0x03));
        Assert.That(packet.IsAccountLinked, Is.True);
        Assert.That(packet.IsGuildChatBridged, Is.True);
        Assert.That(packet.IsAllianceChatBridged, Is.False);
        Assert.That(packet.IsWorldChatBridged, Is.True);
        Assert.That(packet.InviteUrl, Is.EqualTo("https://discord.gg/abc123"));
        Assert.That(packet.RichPresenceApplicationId, Is.EqualTo("123456789012345678"));
        Assert.That(packet.RichPresenceLargeImageKey, Is.EqualTo("logo"));
        Assert.That(packet.RichPresenceSmallImageKey, Is.Empty);
        Assert.That(packet.LinkedUserName, Is.EqualTo("sven"));
    }

    /// <summary>
    /// Tests that the results of a link code request reach the client as such.
    /// </summary>
    [Test]
    public async Task LinkCodeResultsAreSentAsync()
    {
        var (player, output) = CreatePlayer();
        var plugIn = new DiscordIntegrationViewPlugIn(player);

        await plugIn.ShowDiscordLinkCodeAsync(DiscordLinkCodeResult.NotAvailable, null, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
        await plugIn.ShowDiscordLinkCodeAsync(DiscordLinkCodeResult.Created, "ABCD-EFGH", TimeSpan.FromMinutes(10)).ConfigureAwait(false);
        await plugIn.ShowDiscordLinkCodeAsync(DiscordLinkCodeResult.TooSoon, null, TimeSpan.FromMinutes(10)).ConfigureAwait(false);

        var packets = ReadPackets(output);
        var missing = new DiscordLinkCodeRef(packets[0]);
        var created = new DiscordLinkCodeRef(packets[1]);
        Assert.That(new DiscordLinkCodeRef(packets[2]).Result, Is.EqualTo(DiscordLinkCode.DiscordLinkCodeResult.TooSoon));
        Assert.That(missing.Result, Is.EqualTo(DiscordLinkCode.DiscordLinkCodeResult.NotAvailable));
        Assert.That(created.Result, Is.EqualTo(DiscordLinkCode.DiscordLinkCodeResult.Success));
        Assert.That(created.LinkCode, Is.EqualTo("ABCD-EFGH"));
        Assert.That(created.ValidMinutes, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that an external chat message is only sent after the client asked for the integration info,
    /// because only then it's known to understand it.
    /// </summary>
    [Test]
    public async Task ExternalChatMessageNeedsTheClientToAskFirstAsync()
    {
        var (player, output) = CreatePlayer();
        var plugIn = new DiscordIntegrationViewPlugIn(player);

        var shownBefore = await plugIn.TryShowExternalChatMessageAsync(ExternalChatScope.Guild, "Hero", "hi").ConfigureAwait(false);
        await plugIn.ShowDiscordIntegrationInfoAsync(DiscordIntegrationInfo.None).ConfigureAwait(false);
        var shownAfter = await plugIn.TryShowExternalChatMessageAsync(ExternalChatScope.Guild, "Hero", "hi").ConfigureAwait(false);

        var packets = ReadPackets(output);
        Assert.That(shownBefore, Is.False);
        Assert.That(shownAfter, Is.True);
        Assert.That(packets, Has.Count.EqualTo(2));
        var message = new ExternalChatMessageRef(packets[1]);
        Assert.That(message.Source, Is.EqualTo(ExternalChatMessage.ExternalChatSource.Discord));
        Assert.That(message.Scope, Is.EqualTo(ExternalChatMessage.ExternalChatScope.Guild));
        Assert.That(message.SenderName, Is.EqualTo("Hero"));
        Assert.That(message.Message, Is.EqualTo("hi"));
    }

    /// <summary>
    /// Tests that a guild message of a sender bridged from Discord reaches a supporting client as external chat
    /// message, without the prefixes of the sender and the guild chat.
    /// </summary>
    [Test]
    public async Task BridgedGuildMessageBecomesExternalChatMessageAsync()
    {
        var (player, output) = CreatePlayer();
        await player.InvokeViewPlugInAsync<IDiscordIntegrationViewPlugIn>(p => p.ShowDiscordIntegrationInfoAsync(DiscordIntegrationInfo.None)).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync("@hello guild", "@Hero", ChatMessageType.Guild)).ConfigureAwait(false);

        var message = new ExternalChatMessageRef(ReadPackets(output)[1]);
        Assert.That(message.Scope, Is.EqualTo(ExternalChatMessage.ExternalChatScope.Guild));
        Assert.That(message.SenderName, Is.EqualTo("Hero"));
        Assert.That(message.Message, Is.EqualTo("hello guild"));
    }

    /// <summary>
    /// Tests that a client which didn't ask for the integration info keeps getting the bridged message
    /// as normal chat message, with the prefix in front of the sender.
    /// </summary>
    [Test]
    public async Task BridgedGuildMessageStaysNormalChatForOtherClientsAsync()
    {
        var (player, output) = CreatePlayer();

        await player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync("@hello guild", "@Hero", ChatMessageType.Guild)).ConfigureAwait(false);

        var message = new ChatMessageRef(ReadPackets(output)[0]);
        Assert.That(message.Sender, Is.EqualTo("@Hero"));
        Assert.That(message.Message, Is.EqualTo("@hello guild"));
    }

    /// <summary>
    /// Tests that a guild message of a character stays a normal chat message, even for a supporting client.
    /// </summary>
    [Test]
    public async Task CharacterGuildMessageStaysNormalChatAsync()
    {
        var (player, output) = CreatePlayer();
        await player.InvokeViewPlugInAsync<IDiscordIntegrationViewPlugIn>(p => p.ShowDiscordIntegrationInfoAsync(DiscordIntegrationInfo.None)).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync("@hello guild", "Hero", ChatMessageType.Guild)).ConfigureAwait(false);

        var message = new ChatMessageRef(ReadPackets(output)[1]);
        Assert.That(message.Sender, Is.EqualTo("Hero"));
    }

    private static (RemotePlayer Player, MemoryStream Output) CreatePlayer()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IViewPlugIn, DiscordIntegrationViewPlugIn>();
        manager.RegisterPlugIn<IViewPlugIn, ChatViewPlugIn>();
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(new Mock<IPersistenceContextProvider>().Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(manager);
        gameContext.Setup(c => c.FeaturePlugIns).Returns(new FeaturePlugInContainer(manager));
        gameContext.Setup(c => c.LoggerFactory).Returns(new NullLoggerFactory());

        var output = new MemoryStream();
        var writer = PipeWriter.Create(output, new StreamPipeWriterOptions(leaveOpen: true));
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(writer);
        connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
        return (new RemotePlayer(gameContext.Object, connection.Object, MuMainVersion), output);
    }

    /// <summary>
    /// Splits what was written to the connection into its packets, by the length in their headers.
    /// </summary>
    private static List<byte[]> ReadPackets(MemoryStream output)
    {
        var data = output.ToArray();
        var packets = new List<byte[]>();
        var offset = 0;
        while (offset < data.Length)
        {
            var length = data[offset] switch
            {
                0xC1 or 0xC3 => data[offset + 1],
                _ => (data[offset + 1] << 8) | data[offset + 2],
            };
            packets.Add(data[offset..(offset + length)]);
            offset += length;
        }

        return packets;
    }
}
