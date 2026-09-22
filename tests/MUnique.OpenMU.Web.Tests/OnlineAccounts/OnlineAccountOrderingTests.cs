// <copyright file="OnlineAccountOrderingTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.OnlineAccounts;

using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the shared party-grouped ordering of the online-accounts tables.
/// </summary>
[TestFixture]
public class OnlineAccountOrderingTests
{
    private static readonly DateTime TestTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Partied players come first and party members stay contiguous, ordered by master then login.
    /// </summary>
    [Test]
    public void PartiedAccountsComeFirst_MembersStayContiguous()
    {
        var accounts = new List<LoggedInAccount>
        {
            new("zSolo", 1),
            new("bMember", 1, "CharB", "Master", 2),
            new("aMember", 1, "CharA", "Master", 2),
            new("aSolo", 1),
        };

        var result = accounts.OrderPartyGrouped().Select(a => a.LoginName).ToList();

        Assert.That(result, Is.EqualTo(new[] { "aMember", "bMember", "aSolo", "zSolo" }));
    }

    /// <summary>
    /// Multiple parties are grouped by their master.
    /// </summary>
    [Test]
    public void MultipleParties_GroupedByMaster()
    {
        var accounts = new List<OfflineAccount>
        {
            new("solo", 1, TestTimestamp),
            new("m2b", 1, TestTimestamp, "C2", "ZMaster", 2),
            new("m1a", 1, TestTimestamp, "C1", "AMaster", 2),
            new("m2a", 1, TestTimestamp, "C3", "ZMaster", 2),
        };

        var result = accounts.OrderPartyGrouped().Select(a => a.LoginName).ToList();

        Assert.That(result, Is.EqualTo(new[] { "m1a", "m2a", "m2b", "solo" }));
    }

    /// <summary>
    /// Ordering is case-insensitive.
    /// </summary>
    [Test]
    public void Ordering_IsCaseInsensitive()
    {
        var accounts = new List<BotAccount>
        {
            new("Bravo", 1, "Char", TestTimestamp),
            new("alpha", 1, "Char", TestTimestamp),
        };

        var result = accounts.OrderPartyGrouped().Select(a => a.LoginName).ToList();

        Assert.That(result, Is.EqualTo(new[] { "alpha", "Bravo" }));
    }
}
