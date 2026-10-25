// <copyright file="ItemStatProfileIndexesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests that the database keeps the item stat profiles unique where the item formulas look them up:
/// a profile by its name within a configuration, and the terms and requirement formulas of a profile by their attribute.
/// </summary>
/// <remarks>
/// The test doesn't require a database, the context is just created to get its model.
/// </remarks>
[TestFixture]
public class ItemStatProfileIndexesTest
{
    /// <summary>
    /// Tests that the entity type has a unique index on the given properties.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="properties">The properties of the index, in their order.</param>
    [TestCase(typeof(Persistence.EntityFramework.Model.ItemStatProfile), "GameConfigurationId", "Name")]
    [TestCase(typeof(Persistence.EntityFramework.Model.ItemStatTerms), "ItemStatProfileId", "AttributeId")]
    [TestCase(typeof(Persistence.EntityFramework.Model.ItemRequirementFormula), "ItemStatProfileId", "AttributeId")]
    public void HasUniqueIndex(Type entityType, params string[] properties)
    {
        using var context = new EntityDataContext();
        var uniqueIndexes = context.Model.FindEntityType(entityType)!.GetIndexes()
            .Where(index => index.IsUnique)
            .Select(index => string.Join(", ", index.Properties.Select(property => property.Name)));

        Assert.That(uniqueIndexes, Does.Contain(string.Join(", ", properties)));
    }
}
