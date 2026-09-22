// <copyright file="ConnectionListTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="ConnectionList"/>.
/// </summary>
[TestFixture]
public class ConnectionListTests
{
    /// <summary>
    /// Tests if a hint is shown when there are no connections.
    /// </summary>
    [Test]
    public void ShowsHintWhenThereAreNoConnections()
    {
        using var context = new BunitContext();

        var component = context.Render<ConnectionList>(parameters => parameters
            .Add(list => list.Connections, []));

        Assert.That(component.Markup, Does.Contain("No connections found"));
    }

    /// <summary>
    /// Tests if the connections are grouped by the description of their server.
    /// </summary>
    [Test]
    public void ConnectionsAreGroupedByServerDescription()
    {
        using var context = new BunitContext();
        IReadOnlyList<ICapturedConnectionInfo> connections =
        [
            new TestConnectionInfo("Game Server 0", 0) { CharacterName = "TestCharacter" },
            new TestConnectionInfo("Connect Server", 65536),
        ];

        var component = context.Render<ConnectionList>(parameters => parameters
            .Add(list => list.Connections, connections));

        var groups = component.FindAll(".accordion-button").Select(element => element.TextContent.Trim()).ToList();
        Assert.That(groups, Has.Count.EqualTo(2));
        Assert.That(groups[0], Does.Contain("Game Server 0"));
        Assert.That(groups[1], Does.Contain("Connect Server"));
        Assert.That(component.FindAll(".list-group-item"), Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests if the connections are filtered by the entered search term.
    /// </summary>
    [Test]
    public void ConnectionsAreFilteredBySearchTerm()
    {
        using var context = new BunitContext();
        IReadOnlyList<ICapturedConnectionInfo> connections =
        [
            new TestConnectionInfo { CharacterName = "Alpha" },
            new TestConnectionInfo { CharacterName = "Beta" },
        ];

        var component = context.Render<ConnectionList>(parameters => parameters
            .Add(list => list.Connections, connections));
        component.Find("input[type=text]").Input("alph");

        var entries = component.FindAll(".list-group-item");
        Assert.That(entries, Has.Count.EqualTo(1));
        Assert.That(entries[0].TextContent, Does.Contain("Alpha"));
    }

    /// <summary>
    /// Tests if the selection of a connection is reported.
    /// </summary>
    [Test]
    public void SelectionIsReported()
    {
        using var context = new BunitContext();
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        ICapturedConnectionInfo? selected = null;

        var component = context.Render<ConnectionList>(parameters => parameters
            .Add(list => list.Connections, [connection])
            .Add(list => list.OnSelect, info => selected = info));
        component.Find(".list-group-item").Click();

        Assert.That(selected, Is.EqualTo(connection));
    }

    /// <summary>
    /// Tests if a click on the disconnect button doesn't select the connection.
    /// </summary>
    [Test]
    public void DisconnectDoesNotSelectTheConnection()
    {
        using var context = new BunitContext();
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        ICapturedConnectionInfo? selected = null;
        ICapturedConnectionInfo? disconnected = null;

        var component = context.Render<ConnectionList>(parameters => parameters
            .Add(list => list.Connections, [connection])
            .Add(list => list.OnSelect, info => selected = info)
            .Add(list => list.OnDisconnect, info => disconnected = info));
        component.Find(".list-group-item .oi-account-logout").Click();

        Assert.That(disconnected, Is.EqualTo(connection));
        Assert.That(selected, Is.Null);
    }
}
