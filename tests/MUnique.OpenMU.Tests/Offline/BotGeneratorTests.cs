// <copyright file="BotGeneratorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests for <see cref="BotGenerator"/> queue planning.
/// </summary>
[TestFixture]
public class BotGeneratorTests
{
    /// <summary>
    /// Tests that grown elf slots reach the target spread over the queue
    /// instead of converting a prefix of it.
    /// </summary>
    [Test]
    public void GrowElfSlots_ReachesTargetWithoutPrefixBias()
    {
        for (var run = 0; run < 3; run++)
        {
            var queue = BalancedQueue(50);
            var elf = new CharacterClass { Number = 8 };
            var before = queue.ToList();
            var initialElves = before
                .Select((_, n) => n)
                .Where(n => before[n].Number == 8)
                .ToHashSet();

            BotGenerator.GrowElfSlots(queue, elf, 12);

            var slots = queue.ToList();
            Assert.That(slots, Has.Count.EqualTo(50));
            Assert.That(slots.Count(c => c.Number == 8), Is.EqualTo(15));

            var converted = slots
                .Select((_, n) => n)
                .Where(n => slots[n].Number == 8 && !initialElves.Contains(n))
                .OrderBy(n => n)
                .ToList();
            Assert.That(converted, Has.Count.EqualTo(8));

            // The old code always converted the first 8 non-elf slots.
            Assert.That(converted, Is.Not.EqualTo(new[] { 0, 1, 3, 4, 5, 6, 7, 8 }));
        }
    }

    /// <summary>
    /// Tests that the queue stays untouched when it already holds enough elves.
    /// </summary>
    [Test]
    public void GrowElfSlots_KeepsQueueWhenAlreadyEnough()
    {
        var queue = BalancedQueue(50);
        var before = queue.ToList();

        BotGenerator.GrowElfSlots(queue, new CharacterClass { Number = 8 }, 0);

        Assert.That(queue.ToList(), Is.EqualTo(before));
    }

    private static Queue<CharacterClass> BalancedQueue(int total)
    {
        var numbers = new byte[] { 0, 4, 8, 12, 16, 20, 24 };
        var queue = new Queue<CharacterClass>();
        for (var n = 0; n < total; n++)
        {
            queue.Enqueue(new CharacterClass { Number = numbers[n % numbers.Length] });
        }

        return queue;
    }
}
