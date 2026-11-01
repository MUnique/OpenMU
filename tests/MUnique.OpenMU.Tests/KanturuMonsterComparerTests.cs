// <copyright file="KanturuMonsterComparerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for <see cref="KanturuMonsterComparer"/>.
/// </summary>
[TestFixture]
public class KanturuMonsterComparerTests
{
    /// <summary>
    /// Tests that definitions with the same number are considered the same monster.
    /// </summary>
    [Test]
    public void IsSameMonster_SameNumberDifferentInstances_ReturnsTrue()
    {
        var first = new MonsterDefinition { Number = 362 };
        var second = new MonsterDefinition { Number = 362 };

        Assert.That(KanturuMonsterComparer.IsSameMonster(first, second), Is.True);
    }

    /// <summary>
    /// Tests that definitions with different numbers are considered different monsters.
    /// </summary>
    [Test]
    public void IsSameMonster_DifferentNumbers_ReturnsFalse()
    {
        var first = new MonsterDefinition { Number = 362 };
        var second = new MonsterDefinition { Number = 363 };

        Assert.That(KanturuMonsterComparer.IsSameMonster(first, second), Is.False);
    }

    /// <summary>
    /// Tests that null definitions never match.
    /// </summary>
    [Test]
    public void IsSameMonster_Null_ReturnsFalse()
    {
        Assert.That(KanturuMonsterComparer.IsSameMonster(null, new MonsterDefinition { Number = 362 }), Is.False);
        Assert.That(KanturuMonsterComparer.IsSameMonster(new MonsterDefinition { Number = 362 }, null), Is.False);
        Assert.That(KanturuMonsterComparer.IsSameMonster(null, null), Is.False);
    }

    /// <summary>
    /// Tests that a configured monster counts towards the kill target.
    /// </summary>
    [Test]
    public void IsCountedMonster_KillCounts_ReturnsTrue()
    {
        var phase = new KanturuPhaseDefinition
        {
            CountedMonsters = new List<MonsterDefinition> { new() { Number = 362 } },
        };

        Assert.That(KanturuMonsterComparer.IsCountedMonster(new MonsterDefinition { Number = 362 }, phase), Is.True);
    }

    /// <summary>
    /// Tests that other monsters don't count towards the kill target.
    /// </summary>
    [Test]
    public void IsCountedMonster_OtherMonster_ReturnsFalse()
    {
        var phase = new KanturuPhaseDefinition
        {
            CountedMonsters = new List<MonsterDefinition> { new() { Number = 362 } },
        };

        Assert.That(KanturuMonsterComparer.IsCountedMonster(new MonsterDefinition { Number = 999 }, phase), Is.False);
    }

    /// <summary>
    /// Tests that no kill counts without a current phase.
    /// </summary>
    [Test]
    public void IsCountedMonster_NullPhase_ReturnsFalse()
    {
        Assert.That(KanturuMonsterComparer.IsCountedMonster(new MonsterDefinition { Number = 362 }, null), Is.False);
    }
}
