// <copyright file="TrapAttributeHolderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests the <see cref="TrapAttributeHolder"/>.
/// </summary>
[TestFixture]
public class TrapAttributeHolderTest
{
    /// <summary>
    /// Tests that traps can be created concurrently, e.g. when several mini games start at the same time.
    /// The attributes of the trap definitions are cached, and a cache which isn't thread safe could get corrupted.
    /// </summary>
    [Test]
    public async Task TrapsCanBeCreatedConcurrentlyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var definitions = Enumerable.Range(0, 50).Select(i =>
        {
            var definition = new MonsterDefinition { Id = Guid.NewGuid(), Number = (short)i, ObjectKind = NpcObjectKind.Trap };
            definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = i });
            return definition;
        }).ToList();

        var traps = new Trap[definitions.Count * 20];
        Parallel.For(0, traps.Length, i =>
        {
            var definition = definitions[i % definitions.Count];
            var spawnArea = new MonsterSpawnArea { MonsterDefinition = definition, GameMap = map!.Definition, Quantity = 1 };
            traps[i] = new Trap(spawnArea, definition, map, new Mock<INpcIntelligence>().Object);
        });

        Assert.That(traps.Select(trap => trap.Attributes[Stats.Level]), Is.EqualTo(traps.Select(trap => (float)trap.Definition.Number)));
    }
}
