// <copyright file="MonsterLevelsViewPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Tests which monster levels the <see cref="MonsterLevelsViewPlugIn"/> sends.
/// </summary>
[TestFixture]
public class MonsterLevelsViewPlugInTest
{
    /// <summary>
    /// Tests that only monsters with a level are sent, ordered by their number,
    /// and that NPCs, guards etc. are left out.
    /// </summary>
    [Test]
    public void OnlyMonstersWithALevelAreSent()
    {
        var monsters = new List<MonsterDefinition>
        {
            new TestMonsterDefinition(7, NpcObjectKind.Monster, 34),
            new TestMonsterDefinition(0, NpcObjectKind.Monster, 4),
            new TestMonsterDefinition(1, NpcObjectKind.Monster, null),
            new TestMonsterDefinition(249, NpcObjectKind.Guard, 90),
            new TestMonsterDefinition(250, NpcObjectKind.PassiveNpc, 10),
        };
        var configuration = new Mock<GameConfiguration>();
        configuration.Setup(c => c.Monsters).Returns(monsters);

        var levels = MonsterLevelsViewPlugIn.GetMonsterLevels(configuration.Object).ToList();

        Assert.That(levels, Is.EqualTo(new List<(ushort, ushort)> { (0, 4), (7, 34) }));
    }

    private sealed class TestMonsterDefinition : MonsterDefinition
    {
        public TestMonsterDefinition(short number, NpcObjectKind kind, float? level)
        {
            this.Number = number;
            this.ObjectKind = kind;
            this.Attributes = new List<MonsterAttribute>();
            if (level is { } value)
            {
                this.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = value });
            }
        }
    }
}
