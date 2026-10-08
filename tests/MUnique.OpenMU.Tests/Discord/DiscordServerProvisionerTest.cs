// <copyright file="DiscordServerProvisionerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// Tests for the <see cref="DiscordServerProvisioner"/>.
/// </summary>
[TestFixture]
public class DiscordServerProvisionerTest
{
    private readonly DiscordServerProvisioner _provisioner = new(NullLogger<DiscordServerProvisioner>.Instance);

    /// <summary>
    /// Tests that everything of the layout is created on an empty Discord server.
    /// </summary>
    [Test]
    public async Task EmptyServerIsSetUpAsync()
    {
        var server = new FakeDiscordServer();
        var layout = DiscordServerLayout.LoadDefault();

        var result = await this._provisioner.ProvisionAsync(server, layout).ConfigureAwait(false);

        Assert.That(result.Failed, Is.Empty);
        Assert.That(result.Adopted, Is.Empty);
        Assert.That(result.Created, Has.Count.EqualTo(layout.Roles.Count + layout.Categories.Count + layout.Channels.Count()));
        Assert.That(result.ChannelIds.Keys, Is.EquivalentTo(layout.Channels.Select(c => c.Key)));
        Assert.That(server.Channels.Single(c => c.Name == "events").CategoryId, Is.EqualTo(server.GetId("Info")));
    }

    /// <summary>
    /// Tests that the setup can be repeated without creating anything twice.
    /// </summary>
    [Test]
    public async Task SetupIsIdempotentAsync()
    {
        var server = new FakeDiscordServer();
        var layout = DiscordServerLayout.LoadDefault();
        var first = await this._provisioner.ProvisionAsync(server, layout).ConfigureAwait(false);
        var createdCount = server.CreatedCount;

        var second = await this._provisioner.ProvisionAsync(server, layout).ConfigureAwait(false);

        Assert.That(server.CreatedCount, Is.EqualTo(createdCount));
        Assert.That(second.Created, Is.Empty);
        Assert.That(second.Adopted, Is.EquivalentTo(first.Created));
        Assert.That(second.ChannelIds, Is.EquivalentTo(first.ChannelIds));
    }

    /// <summary>
    /// Tests that existing channels are adopted by their name, case-insensitive, even when they are in another category.
    /// </summary>
    [Test]
    public async Task ExistingChannelsAreAdoptedAsync()
    {
        var server = new FakeDiscordServer();
        var otherCategoryId = server.AddCategory("Other");
        var eventsId = server.AddChannel("Events", otherCategoryId);
        var generalId = server.AddChannel("general", null);

        var result = await this._provisioner.ProvisionAsync(server, DiscordServerLayout.LoadDefault()).ConfigureAwait(false);

        Assert.That(result.ChannelIds["events"], Is.EqualTo(eventsId));
        Assert.That(result.ChannelIds["general"], Is.EqualTo(generalId));
        Assert.That(result.Adopted, Is.EquivalentTo(new[] { "#events", "#general" }));
        Assert.That(server.Channels.Count(c => string.Equals(c.Name, "events", StringComparison.OrdinalIgnoreCase)), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a failure is reported, and that the setup continues with the rest.
    /// </summary>
    [Test]
    public async Task FailureIsReportedAndSetupContinuesAsync()
    {
        var server = new FakeDiscordServer { FailingName = "Staff" };

        var result = await this._provisioner.ProvisionAsync(server, DiscordServerLayout.LoadDefault()).ConfigureAwait(false);

        Assert.That(result.Failed, Is.EqualTo(new[] { ("Staff", "Missing Permissions") }));
        Assert.That(result.ChannelIds.Keys, Does.Contain("world-news"));
        Assert.That(result.ChannelIds.Keys, Does.Not.Contain("staff-alerts"));
    }

    /// <summary>
    /// Tests the permissions: Read-only channels can only be written by the bot, and the staff category can only be seen by the game masters and the bot.
    /// </summary>
    [Test]
    public async Task PermissionsAreAppliedAsync()
    {
        var server = new FakeDiscordServer();
        var result = await this._provisioner.ProvisionAsync(server, DiscordServerLayout.LoadDefault()).ConfigureAwait(false);
        var gmRoleId = server.Roles.Single(r => r.Name == "GM").Id;
        const DiscordChannelAccess viewAndSend = DiscordChannelAccess.View | DiscordChannelAccess.Send;

        Assert.That(server.Overwrites[result.ChannelIds["events"]], Is.EqualTo(new DiscordPermissionOverwrite[]
        {
            new(server.EveryoneRoleId, true, DiscordChannelAccess.None, DiscordChannelAccess.Send),
            new(server.BotUserId, false, viewAndSend, DiscordChannelAccess.None),
        }));
        Assert.That(server.Overwrites.ContainsKey(result.ChannelIds["general"]), Is.False);
        Assert.That(server.Overwrites[server.GetId("Staff")], Is.EqualTo(new DiscordPermissionOverwrite[]
        {
            new(server.EveryoneRoleId, true, DiscordChannelAccess.None, DiscordChannelAccess.View),
            new(gmRoleId, true, viewAndSend, DiscordChannelAccess.None),
            new(server.BotUserId, false, viewAndSend, DiscordChannelAccess.None),
        }));
        Assert.That(server.Overwrites[result.ChannelIds["staff-alerts"]], Is.EqualTo(new DiscordPermissionOverwrite[]
        {
            new(server.EveryoneRoleId, true, DiscordChannelAccess.None, viewAndSend),
            new(gmRoleId, true, DiscordChannelAccess.View, DiscordChannelAccess.None),
            new(server.BotUserId, false, viewAndSend, DiscordChannelAccess.None),
        }));
    }

    /// <summary>
    /// Tests that the channels of the layout are found by their name.
    /// </summary>
    [Test]
    public void ChannelsAreFoundByName()
    {
        var server = new FakeDiscordServer();
        var infoId = server.AddCategory("INFO");
        var statusId = server.AddChannel("server-status", infoId);
        server.AddChannel("server-status", null);
        var newsId = server.AddChannel("world-news", null);

        var channels = DiscordServerProvisioner.FindChannels(DiscordServerLayout.LoadDefault(), server.GetChannels());

        Assert.That(channels, Is.EquivalentTo(new Dictionary<string, ulong> { { "server-status", statusId }, { "world-news", newsId } }));
    }
}
