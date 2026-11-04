// <copyright file="ItemStorageCloningTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for cloning item storage with virtual value properties.
/// </summary>
[TestFixture]
public class ItemStorageCloningTest
{
    /// <summary>
    /// Cloning must copy money directly, including zero and negative values.
    /// </summary>
    /// <param name="money">The stored money.</param>
    [TestCase(0)]
    [TestCase(10_000)]
    [TestCase(-1)]
    public void CloneCopiesMoney(int money)
    {
        var storage = new ItemStorage { Money = money };

        var clone = storage.Clone(new GameConfiguration());

        Assert.That(clone, Is.Not.SameAs(storage));
        Assert.That(clone.Money, Is.EqualTo(money));

        clone.Money = 123;

        Assert.That(storage.Money, Is.EqualTo(money));
    }
}
