// <copyright file="SkillListTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using Moq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using NUnit.Framework;

    /// <summary>
    /// Tests the skill list.
    /// </summary>
    [TestFixture]
    public class SkillListTest
    {
        private const ushort LearnedSkillId = 10;
        private const ushort NonLearnedSkillId = 999;
        private const ushort ItemSkillId = 1;

        /// <summary>
        /// Tests if the created skill list contains a skill that was learned by the character before.
        /// </summary>
        [Test]
        public void LearnedSkill()
        {
            var player = TestHelper.CreatePlayer();
            player.SelectedCharacter!.LearnedSkills.Add(this.CreateSkillEntry(LearnedSkillId));
            var skillList = new SkillList(player);
            Assert.That(skillList.ContainsSkill(LearnedSkillId), Is.True);
        }

        /// <summary>
        /// Tests if skills of equipped items are getting added to the skill list.
        /// </summary>
        [Test]
        public void ItemSkill()
        {
            var player = TestHelper.CreatePlayer();
            var item = this.CreateItemWithSkill();
            item.Durability = 1;
            player.Inventory!.AddItem(0, item);
            var skillList = new SkillList(player);
            Assert.That(skillList.ContainsSkill(ItemSkillId), Is.True);
        }

        /// <summary>
        /// Tests if the skill of an item that gets equipped afterwards, is getting added to the skill list.
        /// </summary>
        [Test]
        public void ItemSkillAddedLater()
        {
            var player = TestHelper.CreatePlayer();
            var skillList = player.SkillList as SkillList;
            player.Inventory!.AddItem(0, this.CreateItemWithSkill());

            Assert.That(skillList!.ContainsSkill(ItemSkillId), Is.True);
        }

        /// <summary>
        /// Tests the removal of item skills.
        /// </summary>
        [Test]
        public void ItemSkillRemoved()
        {
            var player = TestHelper.CreatePlayer();
            var item = this.CreateItemWithSkill();
            item.Durability = 1;
            player.Inventory!.AddItem(0, item);
            var skillList = new SkillList(player);
            Assert.That(skillList.RemoveItemSkill(item.Definition!.Skill!.Number.ToUnsigned()), Is.True);
            Assert.That(skillList.ContainsSkill(ItemSkillId), Is.False);
        }

        /// <summary>
        /// Tests if the skill list does not contain non-learned skills.
        /// </summary>
        [Test]
        public void NonLearnedSkill()
        {
            var player = TestHelper.CreatePlayer();
            Assert.That(player.SkillList!.ContainsSkill(NonLearnedSkillId), Is.False);
        }

        private Item CreateItemWithSkill()
        {
            var definition = new Mock<ItemDefinition>();
            definition.SetupAllProperties();
            definition.Object.Skill = new OpenMU.DataModel.Configuration.Skill
            {
                Number = ItemSkillId.ToSigned(),
            };

            definition.Object.Height = 1;
            definition.Object.Width = 1;
            definition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());

            var item = new Item
            {
                HasSkill = true,
                Definition = definition.Object,
            };
            return item;
        }

        private SkillEntry CreateSkillEntry(ushort skillId)
        {
            var skillEntry = new SkillEntry { Skill = new OpenMU.DataModel.Configuration.Skill { Number = skillId.ToSigned() } };
            return skillEntry;
        }
    }
}
