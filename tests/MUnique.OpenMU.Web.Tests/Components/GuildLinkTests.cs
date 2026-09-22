// <copyright file="GuildLinkTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using MUnique.OpenMU.Web.Shared.Components;

/// <summary>
/// Tests for the <see cref="GuildLink"/> and <see cref="AllianceLink"/> components.
/// </summary>
[TestFixture]
public class GuildLinkTests
{
    private BunitContext _context = null!;

    /// <summary>
    /// Setups the test objects.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._context = new BunitContext();
    }

    /// <summary>
    /// Tears down the test objects.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// A known guild renders a link to its detail page.
    /// </summary>
    [Test]
    public void KnownGuild_RendersLink()
    {
        var guildId = Guid.NewGuid();

        var cut = this._context.Render<GuildLink>(parameters => parameters
            .Add(p => p.GuildId, guildId)
            .Add(p => p.GuildName, "Knights"));

        var link = cut.Find("a");
        Assert.That(link.GetAttribute("href"), Is.EqualTo($"guild/{guildId}"));
        Assert.That(link.TextContent, Is.EqualTo("Knights"));
    }

    /// <summary>
    /// Without an identifier the name renders as plain text without a link.
    /// </summary>
    [Test]
    public void UnknownGuild_RendersPlainText()
    {
        var cut = this._context.Render<GuildLink>(parameters => parameters
            .Add(p => p.GuildName, "Knights")
            .Add(p => p.EmptyText, "—"));

        Assert.That(cut.FindAll("a"), Is.Empty);
        Assert.That(cut.Markup, Does.Contain("Knights"));
    }

    /// <summary>
    /// Without a name the fallback text renders.
    /// </summary>
    [Test]
    public void MissingName_RendersEmptyText()
    {
        var cut = this._context.Render<AllianceLink>(parameters => parameters
            .Add(p => p.EmptyText, "none"));

        Assert.That(cut.FindAll("a"), Is.Empty);
        Assert.That(cut.Markup, Does.Contain("none"));
    }

    /// <summary>
    /// A known alliance renders a link to its alliance page.
    /// </summary>
    [Test]
    public void KnownAlliance_RendersLink()
    {
        var allianceId = Guid.NewGuid();

        var cut = this._context.Render<AllianceLink>(parameters => parameters
            .Add(p => p.AllianceId, allianceId)
            .Add(p => p.AllianceName, "Masters"));

        var link = cut.Find("a");
        Assert.That(link.GetAttribute("href"), Is.EqualTo($"alliance/{allianceId}"));
        Assert.That(link.TextContent, Is.EqualTo("Masters"));
    }

    /// <summary>
    /// Without an identifier the alliance name renders as plain text without a link.
    /// </summary>
    [Test]
    public void UnknownAlliance_RendersPlainText()
    {
        var cut = this._context.Render<AllianceLink>(parameters => parameters
            .Add(p => p.EmptyText, "—"));

        Assert.That(cut.FindAll("a"), Is.Empty);
        Assert.That(cut.Markup, Does.Contain("—"));
    }
}
