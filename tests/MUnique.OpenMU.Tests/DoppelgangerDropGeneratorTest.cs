// <copyright file="DoppelgangerDropGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using NUnit.Framework;

/// <summary>
/// Tests for the <see cref="DoppelgangerDropGenerator"/>.
/// </summary>
[TestFixture]
public class DoppelgangerDropGeneratorTest
{
    private const short InterimChestNumber = 541;

    /// <summary>
    /// Tests that a suppressed drop only affects the next interim chest, and not other monsters.
    /// </summary>
    [Test]
    public async Task SuppressesOnlyNextInterimChestDropAsync()
    {
        var item = new Mock<Item>().Object;
        var innerGenerator = new Mock<IDropGenerator>();
        innerGenerator
            .Setup(g => g.GenerateItemDropsAsync(It.IsAny<MonsterDefinition>(), It.IsAny<int>(), It.IsAny<Player>()))
            .Returns(ValueTask.FromResult<(IEnumerable<Item> Items, uint? Money)>((new[] { item }, null)));
        var generator = new DoppelgangerDropGenerator(innerGenerator.Object, InterimChestNumber);
        var chest = new MonsterDefinition { Number = InterimChestNumber };
        var otherMonster = new MonsterDefinition { Number = 533 };
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        generator.SuppressNextInterimChestDrop();

        Assert.That((await generator.GenerateItemDropsAsync(otherMonster, 0, player).ConfigureAwait(false)).Items, Is.Not.Empty);
        Assert.That((await generator.GenerateItemDropsAsync(chest, 0, player).ConfigureAwait(false)).Items, Is.Empty);
        Assert.That((await generator.GenerateItemDropsAsync(chest, 0, player).ConfigureAwait(false)).Items, Is.Not.Empty);
    }
}
