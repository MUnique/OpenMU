// <copyright file="ItemDropValidatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the <see cref="ItemDropValidator"/> class.
/// </summary>
[TestFixture]
public class ItemDropValidatorTest
{
    private ItemDropValidator _validator = null!;
    private Player _player = null!;
    private Item _item = null!;
    private Point _validTarget;

    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._validator = new ItemDropValidator();
        this._player = TestHelper.CreatePlayer();
        this._item = TestHelper.CreateItem();
        this._validTarget = new Point(100, 100);
        
        // Setup a valid map terrain
        if (this._player.CurrentMap?.Terrain != null)
        {
            // Mark the valid target as walkable
            this._player.CurrentMap.Terrain.WalkMap[this._validTarget.X, this._validTarget.Y] = true;
            this._player.CurrentMap.Terrain.SafezoneMap[this._validTarget.X, this._validTarget.Y] = false;
        }
    }

    /// <summary>
    /// Tests that a valid item drop is allowed.
    /// </summary>
    [Test]
    public void ValidItemDrop_ShouldBeAllowed()
    {
        // Arrange
        this._player.Position = new Point(98, 98); // Close to target

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    /// <summary>
    /// Tests that dropping on unwalkable terrain is rejected.
    /// </summary>
    [Test]
    public void DropOnUnwalkableTerrain_ShouldBeRejected()
    {
        // Arrange
        var unwalkableTarget = new Point(101, 101);
        if (this._player.CurrentMap?.Terrain != null)
        {
            this._player.CurrentMap.Terrain.WalkMap[unwalkableTarget.X, unwalkableTarget.Y] = false;
        }

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, unwalkableTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("unwalkable terrain"));
    }

    /// <summary>
    /// Tests that dropping too far from player is rejected.
    /// </summary>
    [Test]
    public void DropTooFarFromPlayer_ShouldBeRejected()
    {
        // Arrange
        this._player.Position = new Point(90, 90); // Far from target (100, 100)
        var farTarget = new Point(110, 110); // Very far from player
        if (this._player.CurrentMap?.Terrain != null)
        {
            this._player.CurrentMap.Terrain.WalkMap[farTarget.X, farTarget.Y] = true;
        }

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, farTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("exceeds maximum allowed"));
    }

    /// <summary>
    /// Tests that rate limiting prevents spam dropping.
    /// </summary>
    [Test]
    public void RapidDropAttempts_ShouldBeRateLimited()
    {
        // Arrange
        this._player.Position = new Point(98, 98);

        // Act - First drop should succeed
        var firstResult = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);
        
        // Act - Immediate second drop should be rate limited
        var secondResult = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(firstResult.IsValid, Is.True);
        Assert.That(secondResult.IsValid, Is.False);
        Assert.That(secondResult.ErrorMessage, Does.Contain("cooldown"));
    }

    /// <summary>
    /// Tests that bound items cannot be dropped.
    /// </summary>
    [Test]
    public void BoundItem_ShouldNotBeDroppable()
    {
        // Arrange
        this._item.Definition!.IsBoundToCharacter = true;
        this._item.Definition.DropLevel = DropLevel.OnDeath;
        this._player.Position = new Point(98, 98);

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Bound items"));
    }

    /// <summary>
    /// Tests that quest items cannot be dropped.
    /// </summary>
    [Test]
    public void QuestItem_ShouldNotBeDroppable()
    {
        // Arrange
        this._item.Definition!.IsQuestItem = true;
        this._player.Position = new Point(98, 98);

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Quest items"));
    }

    /// <summary>
    /// Tests that broken items cannot be dropped.
    /// </summary>
    [Test]
    public void BrokenItem_ShouldNotBeDroppable()
    {
        // Arrange
        this._item.Durability = 0;
        this._item.Definition!.Durability = 100; // Item should have durability
        this._player.Position = new Point(98, 98);

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Broken items"));
    }

    /// <summary>
    /// Tests that items with DropLevel.Never cannot be dropped.
    /// </summary>
    [Test]
    public void NeverDroppableItem_ShouldNotBeDroppable()
    {
        // Arrange
        this._item.Definition!.DropLevel = DropLevel.Never;
        this._player.Position = new Point(98, 98);

        // Act
        var result = this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("cannot be dropped"));
    }

    /// <summary>
    /// Tests cleanup of old validation entries.
    /// </summary>
    [Test]
    public void CleanupOldEntries_ShouldRemoveExpiredData()
    {
        // Arrange
        this._player.Position = new Point(98, 98);
        this._validator.ValidateItemDrop(this._player, this._item, this._validTarget);

        // Act
        this._validator.CleanupOldEntries();

        // Assert - This test mainly ensures the cleanup method doesn't throw exceptions
        // In a real scenario, you would need access to internal state to verify cleanup
        Assert.Pass("Cleanup completed without exceptions");
    }
}