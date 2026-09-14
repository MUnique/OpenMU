// <copyright file="BotStarterGearEquipperTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests the starter gear outfitting: a class-appropriate weapon at the profile's item level, a full
/// armor set with per-piece qualification, and the starting potion stacks.
/// </summary>
[TestFixture]
public class BotStarterGearEquipperTest
{
    private const byte StarterItemLevel = 3;

    /// <summary>
    /// Tests that a weapon of the class's fighting style is equipped at the starter item level.
    /// </summary>
    [Test]
    public void EquipWeapon_EquipsClassWeaponAtStarterLevel()
    {
        var (equipper, inventory, sword) = CreateEquipper(out _);

        equipper.EquipWeapon();

        var equipped = inventory.Items.SingleOrDefault(i => i.ItemSlot == InventoryConstants.LeftHandSlot);
        Assert.Multiple(() =>
        {
            Assert.That(equipped, Is.Not.Null);
            Assert.That(equipped!.Definition, Is.SameAs(sword));
            Assert.That(equipped.Level, Is.EqualTo(StarterItemLevel));
        });
    }

    /// <summary>
    /// Tests that a full armor set is equipped when the class is qualified for every piece.
    /// </summary>
    [Test]
    public void EquipArmorSet_EquipsFullQualifiedSet()
    {
        var (equipper, inventory, _) = CreateEquipper(out _);

        equipper.EquipArmorSet();

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.HelmSlot), Is.EqualTo(1));
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.ArmorSlot), Is.EqualTo(1));
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.PantsSlot), Is.EqualTo(1));
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.GlovesSlot), Is.EqualTo(1));
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.BootsSlot), Is.EqualTo(1));
            Assert.That(inventory.Items.Where(i => i.ItemSlot != InventoryConstants.LeftHandSlot).Select(i => i.Level), Is.All.EqualTo(StarterItemLevel));
        });
    }

    /// <summary>
    /// Tests that a piece the class is not qualified for is skipped while the rest of the set is equipped.
    /// </summary>
    [Test]
    public void EquipArmorSet_SkipsUnqualifiedPiece()
    {
        var (equipper, inventory, _) = CreateEquipper(out var definitions);
        definitions.OfType<TestGearItemDefinition>().First(d => d.Group == 10).QualifiedCharacters.Clear();

        equipper.EquipArmorSet();

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items.Count, Is.EqualTo(4));
            Assert.That(inventory.Items.Any(i => i.ItemSlot == InventoryConstants.GlovesSlot), Is.False);
        });
    }

    /// <summary>
    /// Tests that the starting potion stacks land in the first backpack slots.
    /// </summary>
    [Test]
    public void AddPotions_AddsTwoStacks()
    {
        var (equipper, inventory, _) = CreateEquipper(out _);

        equipper.AddPotions();

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.EquippableSlotsCount), Is.EqualTo(1));
            Assert.That(inventory.Items.Count(i => i.ItemSlot == InventoryConstants.EquippableSlotsCount + 1), Is.EqualTo(1));
        });
    }

    private static (BotStarterGearEquipper Equipper, ItemStorage Inventory, TestGearItemDefinition Sword) CreateEquipper(out List<ItemDefinition> definitions)
    {
        var characterClass = new TestCharacterClass();

        var characterMock = new Mock<Character>();
        characterMock.SetupAllProperties();
        characterMock.Setup(c => c.CharacterClass).Returns(characterClass);
        var inventoryItems = new List<Item>();
        var inventoryMock = new Mock<ItemStorage>();
        inventoryMock.SetupAllProperties();
        inventoryMock.Setup(i => i.Items).Returns(inventoryItems);
        characterMock.Setup(c => c.Inventory).Returns(inventoryMock.Object);

        var sword = new TestGearItemDefinition { Group = 0, Number = 0, DropLevel = 5, Durability = 10 };
        sword.QualifiedCharacters.Add(characterClass);

        // A staff the class could wield but its build rejects, at a lower drop level than the sword:
        // preference must win over availability.
        var staff = new TestGearItemDefinition { Group = 5, Number = 0, DropLevel = 1, Durability = 10 };
        staff.QualifiedCharacters.Add(characterClass);

        definitions = new List<ItemDefinition> { sword, staff };
        foreach (var (group, number) in new[] { (7, 5), (8, 5), (9, 5), (10, 5), (11, 5) })
        {
            var piece = new TestGearItemDefinition { Group = (byte)group, Number = (byte)number, DropLevel = 5, Durability = 10 };
            piece.QualifiedCharacters.Add(characterClass);
            definitions.Add(piece);
        }

        foreach (var (group, number) in new[] { (14, 3), (14, 6) })
        {
            definitions.Add(new TestGearItemDefinition { Group = (byte)group, Number = (byte)number });
        }

        var configMock = new Mock<GameConfiguration>();
        configMock.Setup(c => c.Items).Returns(definitions);

        var contextMock = new Mock<IPlayerContext>();
        contextMock.Setup(m => m.CreateNew<Item>(It.IsAny<object[]>())).Returns(() => new Item());

        var equipper = new BotStarterGearEquipper(contextMock.Object, configMock.Object, characterMock.Object, StarterItemLevel);
        return (equipper, inventoryMock.Object, sword);
    }

    private sealed class TestGearItemDefinition : ItemDefinition
    {
        public TestGearItemDefinition()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }

    private sealed class TestCharacterClass : CharacterClass
    {
        public TestCharacterClass()
        {
            this.Number = 4;
            this.StatAttributes = new List<StatAttributeDefinition>
            {
                new(Stats.BaseStrength, 30, true),
                new(Stats.BaseAgility, 15, true),
                new(Stats.BaseEnergy, 10, true),
            };
        }
    }
}
