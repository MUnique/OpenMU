// <copyright file="AppearanceSerializer097Test.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Tests the <see cref="AppearanceSerializer097"/>.
/// </summary>
[TestFixture]
public class AppearanceSerializer097Test
{
    private const byte DarkKnightNumber = 4;
    private const byte BladeKnightNumber = 6;

    /// <summary>
    /// Tests that a new dark knight with a small axe is serialized correctly.
    /// </summary>
    [Test]
    public void NewDarkKnightWithSmallAxe()
    {
        var data = Serialize(DarkKnightNumber, CreateItem(InventoryConstants.LeftHandSlot, 1, 0));

        var expected = new byte[] { 0x20, 0x20, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xF8, 0x00 };
        Assert.That(data, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a first wing is set at index 5.
    /// </summary>
    [Test]
    public void FirstWing()
    {
        var data = Serialize(DarkKnightNumber, CreateItem(InventoryConstants.WingsSlot, 12, 2));

        Assert.That(data[5] & 0b1100, Is.EqualTo(2 << 2));
        Assert.That(data[9] & 0b111, Is.Zero);
    }

    /// <summary>
    /// Tests that a second wing is set at index 9, while index 5 says that there is no first wing.
    /// </summary>
    /// <param name="wingNumber">The number of the second wing.</param>
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    public void SecondWing(short wingNumber)
    {
        var data = Serialize(BladeKnightNumber, CreateItem(InventoryConstants.WingsSlot, 12, wingNumber));

        Assert.That(data[5] & 0b1100, Is.EqualTo(0b1100));
        Assert.That(data[9] & 0b111, Is.EqualTo(wingNumber - 2));
    }

    /// <summary>
    /// Tests that the dinorant is flagged at index 10 instead of index 9, which is used by the second wings.
    /// </summary>
    [Test]
    public void Dinorant()
    {
        var data = Serialize(BladeKnightNumber, CreateItem(InventoryConstants.PetSlot, 13, 3), CreateItem(InventoryConstants.WingsSlot, 12, 6));

        Assert.That(data[5] & 0b11, Is.EqualTo(0b11));
        Assert.That(data[10] & 0x01, Is.EqualTo(0x01));
        Assert.That(data[9] & 0b111, Is.EqualTo(4));
    }

    /// <summary>
    /// Tests that an excellent armor with a number of 16 and higher is serialized with its fifth bit at index 9.
    /// </summary>
    [Test]
    public void ExcellentArmorWithHighNumber()
    {
        var armor = CreateItem(InventoryConstants.ArmorSlot, 8, 17);
        armor.VisibleOptions.Add(ItemOptionTypes.Excellent);

        var data = Serialize(BladeKnightNumber, armor);

        Assert.That(data[0], Is.EqualTo(0x30));
        Assert.That(data[3] & 0x0F, Is.EqualTo(17 & 0x0F));
        Assert.That(data[9] & 0x40, Is.EqualTo(0x40));
        Assert.That(data[10] & 0x40, Is.EqualTo(0x40));
    }

    /// <summary>
    /// Tests that excellent weapons are flagged at index 10.
    /// </summary>
    [Test]
    public void ExcellentWeapons()
    {
        var leftHand = CreateItem(InventoryConstants.LeftHandSlot, 0, 5);
        leftHand.VisibleOptions.Add(ItemOptionTypes.Excellent);
        var rightHand = CreateItem(InventoryConstants.RightHandSlot, 0, 5);
        rightHand.VisibleOptions.Add(ItemOptionTypes.Excellent);

        var data = Serialize(DarkKnightNumber, leftHand, rightHand);

        Assert.That(data[10], Is.EqualTo(0x04 | 0x02));
    }

    private static byte[] Serialize(byte characterClassNumber, params ItemAppearance[] items)
    {
        var serializer = new AppearanceSerializer097();
        var appearanceData = new Mock<IAppearanceData>();
        appearanceData.Setup(a => a.CharacterClass).Returns(new CharacterClass { Number = characterClassNumber });
        appearanceData.Setup(a => a.EquippedItems).Returns(items);
        var data = new byte[serializer.NeededSpace];
        serializer.WriteAppearanceData(data, appearanceData.Object, false);
        return data;
    }

    private static ItemAppearance CreateItem(byte slot, byte group, short number)
    {
        return new TestItemAppearance
        {
            ItemSlot = slot,
            Definition = new ItemDefinition { Group = group, Number = number },
        };
    }

    /// <summary>
    /// An item appearance with an initialized collection of visible options.
    /// </summary>
    private sealed class TestItemAppearance : ItemAppearance
    {
        /// <inheritdoc />
        public override ICollection<ItemOptionType> VisibleOptions => base.VisibleOptions ??= new List<ItemOptionType>();
    }
}
