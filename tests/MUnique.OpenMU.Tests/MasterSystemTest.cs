// <copyright file="MasterSystemTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;

/// <summary>
/// Tests the master level system.
/// </summary>
[TestFixture]
public class MasterSystemTest
{
    private readonly int _skillIdRank1 = 1;
    private readonly int _skillIdRank2 = 2;
    private readonly int _skillIdRank3 = 3;

    private Player _player = null!;
    private Skill _skillRank1 = null!;
    private Skill _skillRank2 = null!;
    private Skill _skillRank3 = null!;
    private AddMasterPointAction _addAction = null!;

    /// <summary>
    /// Setups the test data.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._player = TestHelper.CreatePlayer();
        var context = this._player.GameContext;
        this._skillRank1 = this.CreateSkill(1, 1, 1, null, this._player.SelectedCharacter!.CharacterClass!);
        this._skillRank2 = this.CreateSkill(2, 2, 1, null, this._player.SelectedCharacter!.CharacterClass!);
        this._skillRank3 = this.CreateSkill((short)this._skillIdRank3, 3, 1, null, this._player.SelectedCharacter!.CharacterClass!);
        this._skillRank3.MasterDefinition!.MinimumLevel = 10;
        context.Configuration.Skills.Add(this._skillRank1);
        context.Configuration.Skills.Add(this._skillRank2);
        context.Configuration.Skills.Add(this._skillRank3);
        this._addAction = new AddMasterPointAction();
    }

    /// <summary>
    /// Tests if the adding of master points failes because of insufficient level up points.
    /// </summary>
    [Test]
    public void FailedInsufficientLevelUpPoints()
    {
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter!.LearnedSkills, Is.Empty);
    }

    /// <summary>
    /// Tests if the adding of master points succeeds.
    /// </summary>
    [Test]
    public void Succeeded()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 1;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Is.Not.Empty);
    }

    /// <summary>
    /// Tests if the adding of master points fails because of an insufficient reached skill rank.
    /// </summary>
    [Test]
    public void RankNotSufficient()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 1;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Is.Empty);
    }

    /// <summary>
    /// Tests if the adding of master points fails because the skill of the previous rank does not have the required level 10.
    /// </summary>
    [Test]
    public void PreviousRankTooLowLevel()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 9;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(1));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.First().Skill, Is.SameAs(this._skillRank1));
    }

    /// <summary>
    /// Tests if the adding of master points succeeds because the skill of the previous rank has the required level 10.
    /// </summary>
    [Test]
    public void PreviousRankEnoughLevels()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 10;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(2));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank1), Is.True);
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank2), Is.True);
    }

    /// <summary>
    /// Tests if the adding of master points succeeds when a skill has a minium level of 10 and the character has enough master points.
    /// </summary>
    [Test]
    public void MinimumLevel10WithEnoughPoints()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 10;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        this._player.SelectedCharacter.LearnedSkills.Last().Level = 10;
        this._player.SelectedCharacter.MasterLevelUpPoints = 10;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank3);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(3));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank3), Is.True);
    }

    /// <summary>
    /// Tests if the adding of master points succeeds when a skill has a minimum level of 10 and the character has not enough master points.
    /// </summary>
    [Test]
    public void MinimumLevel10WithoutEnoughPoints()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 10;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        this._player.SelectedCharacter.LearnedSkills.Last().Level = 10;
        this._player.SelectedCharacter.MasterLevelUpPoints = 9;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank3);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(2));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank3), Is.False);
    }

    /// <summary>
    /// Tests if adding a point to a new skill results in the skill having level 1.
    /// </summary>
    [Test]
    public void AddedSkillGotLevel()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 1;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests if adding a point to a skill increases its level by one.
    /// </summary>
    [Test]
    public void AddLevelToLearnedSkill()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests if adding a point to a new skill fails because the required skill is not learned yet.
    /// </summary>
    [Test]
    public void RequiredSkillNotLearned()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 10;

        this._skillRank2.MasterDefinition!.RequiredMasterSkills.Add(new Skill());
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(1));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank2), Is.False);
    }

    /// <summary>
    /// Tests if adding a point to a new skill not fails because the required skill has been learned.
    /// </summary>
    [Test]
    public void RequiredSkillLearned()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 2;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 10;
        this._skillRank2.MasterDefinition!.RequiredMasterSkills.Add(this._skillRank1);
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(2));
        Assert.That(this._player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this._skillRank2), Is.True);
    }

    /// <summary>
    /// Tests if adding master points decreases the available master points.
    /// </summary>
    [Test]
    public void MasterLevelUpPointDecreasedWhenLearned()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 1;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if a failed adding of master points does not decrease the available master points.
    /// </summary>
    [Test]
    public void MasterLevelUpPointNotDecreasedWhenNotLearned()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 1;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank2);
        Assert.That(this._player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests if the adding of master points fails when the maximum level (20) of a skill has been reached.
    /// </summary>
    [Test]
    public void MasterLevelMaximumReached()
    {
        this._player.SelectedCharacter!.MasterLevelUpPoints = 3;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._player.SelectedCharacter.LearnedSkills.First().Level = 19;
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        this._addAction.AddMasterPointAsync(this._player, (ushort)this._skillIdRank1);
        Assert.That(this._player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(20));
        Assert.That(this._player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(1));
    }

    private Skill CreateSkill(short id, byte rank, byte rootId, Skill? requiredSkill, CharacterClass charClass)
    {
        var masterDef = new Mock<MasterSkillDefinition>();
        masterDef.SetupAllProperties();
        masterDef.Object.Rank = rank;
        masterDef.Object.MaximumLevel = 20;
        masterDef.Object.MinimumLevel = 1;
        masterDef.Object.Root = new MasterSkillRoot { Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, rootId) };
        masterDef.Setup(m => m.RequiredMasterSkills).Returns(new List<Skill>());
        if (requiredSkill != null)
        {
            masterDef.Object.RequiredMasterSkills.Add(requiredSkill);
        }

        var skill = new Mock<Skill>();
        skill.SetupAllProperties();
        skill.Object.Number = id;
        skill.Setup(s => s.QualifiedCharacters).Returns(new List<CharacterClass>());
        skill.Object.QualifiedCharacters.Add(charClass);
        skill.Object.MasterDefinition = masterDef.Object;

        return skill.Object;
    }
}