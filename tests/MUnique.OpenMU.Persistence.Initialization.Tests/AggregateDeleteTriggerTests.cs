// <copyright file="AggregateDeleteTriggerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Tests for the generated <see cref="AggregateDeleteTriggers"/>.
/// </summary>
[TestFixture]
internal class AggregateDeleteTriggerTests
{
    /// <summary>
    /// Tests that the currently generated script is applied by one of the migrations.
    /// </summary>
    /// <remarks>
    /// When this test fails, a member of an aggregate was added to (or removed from) the model:
    /// add a migration which applies the current <see cref="AggregateDeleteTriggers.CreateScript"/>,
    /// so that the triggers of an existing database are updated, too. The script is copied into the
    /// migration on purpose - a migration must keep applying what it applied when it was added.
    /// </remarks>
    [Test]
    public void GeneratedScriptIsAppliedByAMigration()
    {
        var expectedScript = NormalizeLineEndings(AggregateDeleteTriggers.CreateScript);
        var appliedScripts = typeof(AggregateDeleteTriggers).Assembly.GetTypes()
            .Where(type => typeof(Migration).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => (Migration)Activator.CreateInstance(type)!)
            .SelectMany(migration => migration.UpOperations)
            .OfType<SqlOperation>()
            .Select(operation => NormalizeLineEndings(operation.Sql))
            .ToList();

        Assert.That(
            appliedScripts,
            Has.Some.EqualTo(expectedScript),
            "No migration applies the currently generated trigger script. Please add one.");
    }

    /// <summary>
    /// Tests that the script contains the triggers of the item storages, which are the ones which
    /// would leave the most rows behind: a storage keeps every item which lies in it.
    /// </summary>
    [Test]
    public void ItemStoragesAreCoveredByTheScript()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AggregateDeleteTriggers.CreateScript, Does.Contain("\"trg_Character_DeleteInventory\""));
            Assert.That(AggregateDeleteTriggers.CreateScript, Does.Contain("\"trg_Account_DeleteVault\""));
        });
    }

    private static string NormalizeLineEndings(string value)
    {
        return value.Replace("\r\n", "\n");
    }
}
