// <copyright file="BotHuntingGroundTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the way a bot picks the spot it hunts on.
/// </summary>
[TestFixture]
public class BotHuntingGroundTest
{
    /// <summary>
    /// A hunting ground on the far side of the map stays a realistic choice. Bots enter a map through the
    /// same gate and rate every ground from that one spot, so a steep distance penalty made all of them
    /// queue up on the few spawns next to the entrance.
    /// </summary>
    [Test]
    public void DistantGroundKeepsARealisticShare()
    {
        var atTheGate = new MonsterSpawnArea { X1 = 200, X2 = 200, Y1 = 200, Y2 = 200, Quantity = 1 };
        var acrossTheMap = new MonsterSpawnArea { X1 = 33, X2 = 33, Y1 = 95, Y2 = 95, Quantity = 1 };
        var gate = new Point(222, 211);

        var near = BotNavigator.GroundWeight(atTheGate, gate);
        var far = BotNavigator.GroundWeight(acrossTheMap, gate);

        Assert.That(near, Is.GreaterThan(far), "closer grounds are still preferred");
        Assert.That(near, Is.LessThan(far * 10), "but not to the point where the rest of the map never gets picked");
    }

    /// <summary>
    /// The number of monsters an area holds still counts.
    /// </summary>
    [Test]
    public void DenserGroundOutweighsThinnerOneAtTheSameDistance()
    {
        var thin = new MonsterSpawnArea { X1 = 100, X2 = 100, Y1 = 100, Y2 = 100, Quantity = 1 };
        var dense = new MonsterSpawnArea { X1 = 100, X2 = 100, Y1 = 100, Y2 = 100, Quantity = 20 };
        var from = new Point(150, 150);

        Assert.That(BotNavigator.GroundWeight(dense, from), Is.GreaterThan(BotNavigator.GroundWeight(thin, from)));
    }
}
