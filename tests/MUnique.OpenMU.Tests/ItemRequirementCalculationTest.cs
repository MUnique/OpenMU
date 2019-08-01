// <copyright file="ItemRequirementCalculationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.BasicModel;
    using NUnit.Framework;
    using IncreasableItemOption = MUnique.OpenMU.Persistence.BasicModel.IncreasableItemOption;
    using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;

    /// <summary>
    /// Unit tests for <see cref="ItemExtensions.GetRequirement"/>.
    /// </summary>
    [TestFixture]
    public class ItemRequirementCalculationTest
    {
        /// <summary>
        /// Tests requirement calculation for the 'Vine Helm'.
        /// </summary>
        /// <param name="itemLevel">The item level.</param>
        /// <param name="requiredStrength">The required strength.</param>
        /// <param name="requiredAgility">The required agility.</param>
        [TestCase(0, 25, 30)]
        [TestCase(1, 28, 36)]
        [TestCase(2, 30, 41)]
        [TestCase(3, 33, 47)]
        [TestCase(4, 36, 52)]
        [TestCase(5, 38, 57)]
        [TestCase(6, 41, 63)]
        [TestCase(7, 44, 68)]
        [TestCase(8, 47, 74)]
        [TestCase(9, 49, 79)]
        [TestCase(10, 52, 84)]
        [TestCase(11, 55, 90)]
        [TestCase(12, 57, 95)]
        [TestCase(13, 60, 101)]
        [TestCase(14, 63, 106)]
        [TestCase(15, 65, 111)]
        public void VineHelm(byte itemLevel, int requiredStrength, int requiredAgility)
        {
            var item = new Item();
            item.Level = itemLevel;
            item.Definition = new ItemDefinition();
            item.Definition.DropLevel = 6;
            item.Definition.Group = 7;
            var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 30 };
            var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 60 };

            var strengthValue = item.GetRequirement(strengthRequirement);
            var agilityValue = item.GetRequirement(agilityRequirement);

            Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
            Assert.That(strengthValue.Item2, Is.EqualTo(requiredStrength));

            Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
            Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
        }

        /// <summary>
        /// Tests if item options add 4 strength each.
        /// </summary>
        [Test]
        public void OptionAdds4Strength()
        {
            var item = new Item();
            item.Definition = new ItemDefinition();
            item.Definition.DropLevel = 6;
            item.Definition.Group = 7;
            item.ItemOptions.Add(new ItemOptionLink { Level = 2, ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Option } });
            var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 30 };
            var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 60 };

            var strengthValue = item.GetRequirement(strengthRequirement);
            var agilityValue = item.GetRequirement(agilityRequirement);

            Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
            Assert.That(strengthValue.Item2, Is.EqualTo(33));

            Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
            Assert.That(agilityValue.Item2, Is.EqualTo(30));
        }

        /// <summary>
        /// Tests requirement calculation for the 'Sunlight Armor'.
        /// </summary>
        /// <param name="itemLevel">The item level.</param>
        /// <param name="requiredStrength">The required strength.</param>
        /// <param name="requiredAgility">The required agility.</param>
        [TestCase(0, 293, 90)]
        [TestCase(1, 299, 92)]
        [TestCase(2, 304, 93)]
        [TestCase(3, 310, 94)]
        [TestCase(4, 315, 96)]
        [TestCase(5, 321, 97)]
        [TestCase(6, 326, 99)]
        [TestCase(7, 332, 100)]
        [TestCase(8, 338, 102)]
        [TestCase(9, 343, 103)]
        [TestCase(10, 349, 104)]
        [TestCase(11, 354, 106)]
        [TestCase(12, 360, 107)]
        [TestCase(13, 365, 109)]
        [TestCase(14, 371, 110)]
        [TestCase(15, 377, 112)]
        public void SunlightArmor(byte itemLevel, int requiredStrength, int requiredAgility)
        {
            var item = new Item();
            item.Level = itemLevel;
            item.Definition = new ItemDefinition();
            item.Definition.DropLevel = 147;
            item.Definition.Group = 8;
            var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 62 };
            var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 16 };

            var strengthValue = item.GetRequirement(strengthRequirement);
            var agilityValue = item.GetRequirement(agilityRequirement);

            Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
            Assert.That(strengthValue.Item2, Is.EqualTo(requiredStrength));

            Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
            Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
        }

        /// <summary>
        /// Tests the requirement calculation of the 'Book of Neil'.
        /// Energy requirement calculation of summoner books are different from other items, so a unit test makes sense here.
        /// </summary>
        /// <param name="itemLevel">The item level.</param>
        /// <param name="requiredEnergy">The required energy.</param>
        /// <param name="requiredAgility">The required agility.</param>
        [TestCase(0, 317, 64)]
        [TestCase(1, 322, 66)]
        [TestCase(2, 327, 68)]
        [TestCase(3, 332, 71)]
        [TestCase(4, 337, 73)]
        [TestCase(5, 342, 75)]
        [TestCase(6, 347, 77)]
        [TestCase(7, 352, 80)]
        [TestCase(8, 357, 82)]
        [TestCase(9, 362, 84)]
        [TestCase(10, 367, 86)]
        [TestCase(11, 372, 89)]
        [TestCase(12, 377, 91)]
        [TestCase(13, 382, 93)]
        [TestCase(14, 387, 95)]
        [TestCase(15, 392, 98)]
        public void BookOfNeil(byte itemLevel, int requiredEnergy, int requiredAgility)
        {
            var item = new Item();
            item.Level = itemLevel;
            item.Definition = new ItemDefinition();
            item.Definition.Skill = new Skill();
            item.Definition.DropLevel = 59;
            item.Definition.Group = 5;
            var energyRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 168 };
            var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 25 };

            var energyValue = item.GetRequirement(energyRequirement);
            var agilityValue = item.GetRequirement(agilityRequirement);

            Assert.That(energyValue.Item1, Is.EqualTo(Stats.TotalEnergy));
            Assert.That(energyValue.Item2, Is.EqualTo(requiredEnergy));

            Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
            Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
        }
    }
}
