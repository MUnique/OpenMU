// <copyright file="SelfDefensePlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Tests for <see cref="SelfDefensePlugIn"/>.
/// </summary>
[TestFixture]
public class SelfDefensePlugInTest
{
    /// <summary>
    /// Ensures that hitting an own summon doesn't start self-defense.
    /// </summary>
    [Test]
    public async ValueTask OwnSummonHitDoesNotStartSelfDefenseAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);

        var summonMock = new Mock<IAttackable>();
        var summonable = summonMock.As<ISummonable>();
        summonable.Setup(s => s.SummonedBy).Returns(player);
        summonable.Setup(s => s.Definition).Returns(new MonsterDefinition());

        var plugIn = new SelfDefensePlugIn();
        plugIn.AttackableGotHit(summonMock.Object, player, new HitInfo(1, 0, DamageAttributes.Undefined));

        Assert.That(player.GameContext.SelfDefenseState, Is.Empty);
    }
}