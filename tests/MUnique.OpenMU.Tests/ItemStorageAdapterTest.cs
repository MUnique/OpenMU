// <copyright file="ItemStorageAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for <see cref="ItemStorageAdapter"/>.
/// </summary>
[TestFixture]
public class ItemStorageAdapterTest
{
    /// <summary>
    /// Money must be forwarded to the wrapped storage. Trade backup/restore goes through
    /// <c>Inventory.ItemStorage.Money</c>, which is an adapter instance.
    /// </summary>
    [Test]
    public void MoneyIsForwardedToActualStorage()
    {
        var actual = new ItemStorage { Money = 10_000 };
        var adapter = new ItemStorageAdapter(actual, 0, 64);

        Assert.That(adapter.Money, Is.EqualTo(10_000));

        adapter.Money = 7_000;

        Assert.That(actual.Money, Is.EqualTo(7_000));
        Assert.That(adapter.Money, Is.EqualTo(7_000));
    }

    /// <summary>
    /// Simulates trade cancel: backup at trade open, deduct escrowed zen, restore from backup.
    /// </summary>
    [Test]
    public void BackupAndRestoreKeepsEscrowedMoney()
    {
        var actual = new ItemStorage { Money = 10_000 };
        var adapter = new ItemStorageAdapter(actual, 0, 64);

        var backup = new BackupItemStorage(adapter);
        adapter.Money -= 3_000;

        Assert.That(actual.Money, Is.EqualTo(7_000));
        Assert.That(backup.Money, Is.EqualTo(10_000));

        adapter.Money = backup.Money;

        Assert.That(actual.Money, Is.EqualTo(10_000));
    }
}
