// <copyright file="MasterSystemTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using NUnit.Framework;

    /// <summary>
    /// Tests the master level system.
    /// </summary>
    [TestFixture]
    public class MasterSystemTest
    {
        private readonly int skillIdRank0 = 1;
        private readonly int skillIdRank1 = 2;
        private readonly int skillIdRank2 = 3;

        private IGameContext context;
        private Player player;
        private Skill skillRank0;
        private Skill skillRank1;
        private Skill skillRank2;
        private AddMasterPointAction addAction;

        /// <summary>
        /// Setups the test data.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.player = TestHelper.GetPlayer();
            this.context = this.player.GameContext;
            this.skillRank0 = this.CreateSkill(1, 0, 1, null, this.player.SelectedCharacter.CharacterClass);
            this.skillRank1 = this.CreateSkill(2, 1, 1, null, this.player.SelectedCharacter.CharacterClass);
            this.skillRank2 = this.CreateSkill((short)this.skillIdRank2, 2, 1, null, this.player.SelectedCharacter.CharacterClass);
            this.context.Configuration.Skills.Add(this.skillRank0);
            this.context.Configuration.Skills.Add(this.skillRank1);
            this.context.Configuration.Skills.Add(this.skillRank2);
            this.addAction = new AddMasterPointAction(this.context);
        }

        /// <summary>
        /// Tests if the adding of master points failes because of insufficient level up points.
        /// </summary>
        [Test]
        public void FailedInsufficientLevelUpPoints()
        {
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Is.Empty);
        }

        /// <summary>
        /// Tests if the adding of master points succeeds.
        /// </summary>
        [Test]
        public void Succeeded()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 1;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Is.Not.Empty);
        }

        /// <summary>
        /// Tests if the adding of master points failes because of an insufficient reached skill rank.
        /// </summary>
        [Test]
        public void RankNotSufficient()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 1;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Is.Empty);
        }

        /// <summary>
        /// Tests if the adding of master points failes because the skill of the previous rank does not have the required level 10.
        /// </summary>
        [Test]
        public void PreviousRankTooLowLevel()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 2;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.player.SelectedCharacter.LearnedSkills.First().Level = 9;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(1));
            Assert.That(this.player.SelectedCharacter.LearnedSkills.First().Skill, Is.SameAs(this.skillRank0));
        }

        /// <summary>
        /// Tests if the adding of master points succeeds because the skill of the previous rank has the required level 10.
        /// </summary>
        [Test]
        public void PreviousRankEnoughLevels()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 2;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.player.SelectedCharacter.LearnedSkills.First().Level = 10;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(2));
            Assert.That(this.player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this.skillRank0), Is.True);
            Assert.That(this.player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this.skillRank1), Is.True);
        }

        /// <summary>
        /// Tests if adding a point to a new skill results in the skill having level 1.
        /// </summary>
        [Test]
        public void AddedSkillGotLevel()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 1;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests if adding a point to a skill increases its level by one.
        /// </summary>
        [Test]
        public void AddLevelToLearnedSkill()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 2;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests if adding a point to a new skill fails because the required skill is not learned yet.
        /// </summary>
        [Test]
        public void RequiredSkillNotLearned()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 2;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.player.SelectedCharacter.LearnedSkills.First().Level = 10;

            this.skillRank1.MasterDefinitions.First().RequiredMasterSkills.Add(new Skill());
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(1));
            Assert.That(this.player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this.skillRank1), Is.False);
        }

        /// <summary>
        /// Tests if adding a point to a new skill not fails because the required skill has been learned.
        /// </summary>
        [Test]
        public void RequiredSkillLearned()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 2;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.player.SelectedCharacter.LearnedSkills.First().Level = 10;
            this.skillRank1.MasterDefinitions.First().RequiredMasterSkills.Add(this.skillRank0);
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.LearnedSkills, Has.Count.EqualTo(2));
            Assert.That(this.player.SelectedCharacter.LearnedSkills.Any(l => l.Skill == this.skillRank1), Is.True);
        }

        /// <summary>
        /// Tests if adding master points decreases the available master points.
        /// </summary>
        [Test]
        public void MasterLevelUpPointDecreasedWhenLearned()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 1;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests if a failed adding of master points does not decrease the available master points.
        /// </summary>
        [Test]
        public void MasterLevelUpPointNotDecreasedWhenNotLearned()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 1;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank1);
            Assert.That(this.player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests if the adding of master points fails when the maximum level (20) of a skill has been reached.
        /// </summary>
        [Test]
        public void MasterLevelMaximumReached()
        {
            this.player.SelectedCharacter.MasterLevelUpPoints = 3;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.player.SelectedCharacter.LearnedSkills.First().Level = 19;
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            this.addAction.AddMasterPoint(this.player, (ushort)this.skillIdRank0);
            Assert.That(this.player.SelectedCharacter.LearnedSkills.First().Level, Is.EqualTo(20));
            Assert.That(this.player.SelectedCharacter.MasterLevelUpPoints, Is.EqualTo(1));
        }

        private Skill CreateSkill(short id, byte rank, byte rootId, Skill requiredSkill, CharacterClass charClass)
        {
            var masterDef = new Mock<MasterSkillDefinition>();
            masterDef.SetupAllProperties();
            masterDef.Object.CharacterClass = charClass;
            masterDef.Object.Rank = rank;
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
            skill.Setup(s => s.MasterDefinitions).Returns(new List<MasterSkillDefinition>());
            skill.Object.QualifiedCharacters.Add(charClass);
            skill.Object.MasterDefinitions.Add(masterDef.Object);

            return skill.Object;
        }
    }
}
