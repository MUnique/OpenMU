// <copyright file="DiscordServerLayoutTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.IO;
using System.Text;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// Tests for the <see cref="DiscordServerLayout"/>.
/// </summary>
[TestFixture]
public class DiscordServerLayoutTest
{
    /// <summary>
    /// Tests that the default layout is valid and has a channel for each category of notifications and for the status.
    /// </summary>
    [Test]
    public void DefaultLayoutIsComplete()
    {
        var layout = DiscordServerLayout.LoadDefault();

        Assert.That(layout.Validate(), Is.Empty);
        Assert.That(layout.Channels.SelectMany(c => c.Notifications).Distinct(), Is.EquivalentTo(Enum.GetValues<DiscordChannelCategory>()));
        Assert.That(layout.Channels.Count(c => c.ShowsStatus), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a layout with a duplicate key and an unknown role isn't loaded.
    /// </summary>
    [Test]
    public void InvalidLayoutIsRejected()
    {
        const string json = """
            {
              "categories": [
                { "key": "a", "name": "A", "visibleTo": [ "gm" ], "channels": [ { "key": "x", "name": "x" }, { "key": "X", "name": "y" } ] }
              ]
            }
            """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var exception = Assert.Throws<InvalidDataException>(() => DiscordServerLayout.Load(stream));

        Assert.That(exception!.Message, Does.Contain("unknown role 'gm'").And.Contain("'X' isn't unique"));
    }
}
