// <copyright file="PartyColorHelperTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.OnlineAccounts;

using MUnique.OpenMU.Web.AdminPanel.Components;

/// <summary>
/// Tests for the stable party badge colors.
/// </summary>
[TestFixture]
public class PartyColorHelperTests
{
    /// <summary>
    /// The color is a fixed function of the master name (FNV-1a hue), so it survives admin panel restarts.
    /// </summary>
    /// <param name="partyMaster">The party master name.</param>
    /// <param name="expected">The expected badge color.</param>
    [TestCase("Master", "hsl(19, 45%, 45%)")]
    [TestCase("Alpha", "hsl(147, 45%, 45%)")]
    [TestCase("Beta", "hsl(223, 45%, 45%)")]
    public void GetPartyColor_ReturnsStableColor(string partyMaster, string expected)
    {
        Assert.That(PartyColorHelper.GetPartyColor(partyMaster), Is.EqualTo(expected));
    }

    /// <summary>
    /// Casing doesn't change the color: all members share the badge regardless of name casing.
    /// </summary>
    [Test]
    public void GetPartyColor_IsCaseInsensitive()
    {
        Assert.That(PartyColorHelper.GetPartyColor("master"), Is.EqualTo(PartyColorHelper.GetPartyColor("MASTER")));
    }
}
