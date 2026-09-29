// <copyright file="PartyDisplayTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.OnlineAccounts;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the party display normalization.
/// </summary>
[TestFixture]
public class PartyDisplayTests
{
    /// <summary>
    /// Without a party there is nothing to display.
    /// </summary>
    [Test]
    public void From_NullParty_ReturnsEmpty()
    {
        var (master, size) = PartyDisplay.From(null);

        Assert.That(master, Is.Null);
        Assert.That(size, Is.EqualTo(0));
    }

    /// <summary>
    /// A party without a master (e.g. momentarily during transitions) renders as solo.
    /// </summary>
    [Test]
    public void From_PartyWithoutMaster_ReturnsEmpty()
    {
        var partyManager = new PartyManager(5, new NullLogger<Party>());
        var party = new Party(partyManager, 5, new NullLogger<Party>());

        var (master, size) = PartyDisplay.From(party);

        Assert.That(master, Is.Null);
        Assert.That(size, Is.EqualTo(0));
    }
}
