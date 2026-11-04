// <copyright file="MasterSkillIndexTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Tests the index of master skills on the client's master skill tree (<see cref="MasterSkillExtensions.GetMasterSkillIndex"/>),
/// which has to match <c>MasterSkillTreeData.bmd</c> of the client.
/// </summary>
[TestFixture]
public class MasterSkillIndexTests
{
    private const byte DuelMasterNumber = 13;

    /// <summary>
    /// The Duel Master's Ice Strengthener (489) sits at index 55 of the client's tree (rank 5, column 2 of the middle root).
    /// Index 53 is an empty slot, so a learned level was not shown on the tree after entering the game again.
    /// </summary>
    [Test]
    public void DuelMasterIceStrengthenerIsAtIndex55()
    {
        var skill = new Skill { Number = 489, MasterDefinition = new MasterSkillDefinition() };
        var duelMaster = new CharacterClass { Number = DuelMasterNumber };

        Assert.That(skill.GetMasterSkillIndex(duelMaster), Is.EqualTo(55));
    }
}
