// <copyright file="BotServerPartitionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests how the bot population is split over the game servers of a deployment
/// (<see cref="BotServerPartition"/>): bots count towards the player limit of their server, so a server
/// must never animate more of them than its reserved share allows - and the servers must agree on who
/// animates whom without asking each other.
/// </summary>
[TestFixture]
public class BotServerPartitionTest
{
    /// <summary>
    /// A single game server animates the accounts which fit into its share; the rest stays offline
    /// instead of filling the server up, because the remaining capacity belongs to the players.
    /// </summary>
    [Test]
    public void SingleServerTakesWhatFits()
    {
        var (partition, assigned) = BotServerPartition.Split([(0, 120)], 0, 220);

        Assert.That(partition.FirstAccount, Is.EqualTo(1));
        Assert.That(partition.AccountCount, Is.EqualTo(120));
        Assert.That(partition.IsGenerator, Is.True);
        Assert.That(assigned, Is.EqualTo(120));
    }

    /// <summary>
    /// The scenario the split is made for: one server was crowded, a second one is added, and the
    /// population spreads over BOTH of them - a player who picks the new server meets bots there, too.
    /// </summary>
    [Test]
    public void PopulationSpreadsOverBothServers()
    {
        List<(byte ServerId, int Capacity)> capacities = [(0, 120), (1, 120)];

        var (first, assigned) = BotServerPartition.Split(capacities, 0, 220);
        var (second, _) = BotServerPartition.Split(capacities, 1, 220);

        Assert.That(assigned, Is.EqualTo(220));
        Assert.That(first.AccountCount, Is.EqualTo(110));
        Assert.That(second.AccountCount, Is.EqualTo(110));
        Assert.That(second.FirstAccount, Is.EqualTo(111));
    }

    /// <summary>
    /// The invariant which protects the characters: every account is animated by exactly one server.
    /// Two servers animating one account is the cross-context situation which corrupts it.
    /// </summary>
    /// <param name="requestedAccounts">The configured number of bot accounts.</param>
    [TestCase(1)]
    [TestCase(7)]
    [TestCase(220)]
    [TestCase(1000)]
    public void EveryAccountIsAnimatedExactlyOnce(int requestedAccounts)
    {
        // Deliberately uneven capacities, so the rounding of the shares is exercised.
        List<(byte ServerId, int Capacity)> capacities = [(0, 37), (1, 90), (2, 113)];
        var partitions = capacities
            .Select(server => BotServerPartition.Split(capacities, server.ServerId, requestedAccounts))
            .ToList();
        var assigned = partitions[0].AssignedAccounts;

        Assert.That(partitions.Sum(p => p.Partition.AccountCount), Is.EqualTo(assigned));
        for (var account = 1; account <= assigned; account++)
        {
            Assert.That(partitions.Count(p => p.Partition.Owns(account)), Is.EqualTo(1), $"account {account}");
        }
    }

    /// <summary>
    /// Exactly one server generates the population, so the accounts - and their unique character names -
    /// are never created twice at the same time.
    /// </summary>
    [Test]
    public void OnlyTheFirstServerGenerates()
    {
        List<(byte ServerId, int Capacity)> capacities = [(0, 50), (1, 50), (2, 50)];

        Assert.That(BotServerPartition.Split(capacities, 0, 150).Partition.IsGenerator, Is.True);
        Assert.That(BotServerPartition.Split(capacities, 1, 150).Partition.IsGenerator, Is.False);
        Assert.That(BotServerPartition.Split(capacities, 2, 150).Partition.IsGenerator, Is.False);
    }

    /// <summary>
    /// The servers may have different player limits; the shares follow their capacity.
    /// </summary>
    [Test]
    public void SharesFollowTheServerCapacity()
    {
        List<(byte ServerId, int Capacity)> capacities = [(0, 30), (1, 90)];

        var (small, _) = BotServerPartition.Split(capacities, 0, 120);
        var (big, _) = BotServerPartition.Split(capacities, 1, 120);

        Assert.That(small.AccountCount, Is.EqualTo(30));
        Assert.That(big.AccountCount, Is.EqualTo(90));
        Assert.That(big.FirstAccount, Is.EqualTo(31));
    }

    /// <summary>
    /// A server without any bot capacity (its player limit is reserved for players entirely) animates
    /// nothing, and the other servers still cover the whole population.
    /// </summary>
    [Test]
    public void ServerWithoutCapacityAnimatesNothing()
    {
        List<(byte ServerId, int Capacity)> capacities = [(0, 100), (1, 0)];

        var (empty, assigned) = BotServerPartition.Split(capacities, 1, 60);

        Assert.That(empty.AccountCount, Is.EqualTo(0));
        Assert.That(empty.IsGenerator, Is.False);
        Assert.That(empty.Owns(1), Is.False);
        Assert.That(assigned, Is.EqualTo(60));
        Assert.That(BotServerPartition.Split(capacities, 0, 60).Partition.AccountCount, Is.EqualTo(60));
    }
}
