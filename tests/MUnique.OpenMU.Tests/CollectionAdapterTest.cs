// <copyright file="CollectionAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests for the <see cref="CollectionAdapter{TClass,TEfCore}"/>.
/// </summary>
[TestFixture]
public class CollectionAdapterTest
{
    /// <summary>
    /// Tests that the adapter contains an item which isn't of the persistent type, when it's equal
    /// to an item of the collection. This is the case for the attribute definitions of <see cref="Stats"/>,
    /// which are checked by configuration updates before adding them.
    /// </summary>
    [Test]
    public void ContainsEqualItemOfNonPersistentType()
    {
        var adapter = CreateAdapterWith(Stats.IsResting);

        Assert.That(adapter.Contains(Stats.IsResting), Is.True);
    }

    /// <summary>
    /// Tests that the adapter doesn't contain an item which isn't of the persistent type, when no
    /// item of the collection is equal to it.
    /// </summary>
    [Test]
    public void DoesNotContainUnequalItemOfNonPersistentType()
    {
        var adapter = CreateAdapterWith(Stats.IsResting);

        Assert.That(adapter.Contains(Stats.IsInSafezone), Is.False);
    }

    private static CollectionAdapter<AttributeDefinition, Persistence.EntityFramework.Model.AttributeDefinition> CreateAdapterWith(AttributeDefinition attribute)
    {
        var rawCollection = new List<Persistence.EntityFramework.Model.AttributeDefinition>
        {
            new(attribute.Id, attribute.Designation, attribute.Description),
        };

        return new CollectionAdapter<AttributeDefinition, Persistence.EntityFramework.Model.AttributeDefinition>(rawCollection);
    }
}
