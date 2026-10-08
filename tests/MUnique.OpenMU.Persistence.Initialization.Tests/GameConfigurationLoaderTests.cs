// <copyright file="GameConfigurationLoaderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Loading;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="GameConfigurationLoader"/>.
/// </summary>
[TestFixture]
public class GameConfigurationLoaderTests
{
    /// <summary>
    /// Tests that the <see cref="GameConfigurationLoader"/> loads the same object graph as the <see cref="GameConfigurationJsonObjectLoader"/>.
    /// The order of the items of many-to-many collections isn't compared, because the json query doesn't define it, either.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task LoadsSameGraphAsJsonQueryAsync()
    {
        JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());
        await using var context = new EntityDataContext();
        await context.Database.OpenConnectionAsync().ConfigureAwait(false);
        var id = await context.Set<EntityFramework.Model.GameConfiguration>().AsNoTracking().Select(c => c.Id).FirstAsync().ConfigureAwait(false);

        var expected = await new GameConfigurationJsonObjectLoader().LoadObjectAsync<EntityFramework.Model.GameConfiguration>(id, context).ConfigureAwait(false);
        var actual = (await new GameConfigurationLoader().LoadAsync(context.Database.GetDbConnection(), default).ConfigureAwait(false)).Single(c => c.Id == id);

        var differences = new System.Collections.Generic.List<string>();
        var visited = new HashSet<Guid>();
        Compare(expected!, actual, "GameConfiguration", visited, differences);
        Assert.That(differences, Is.Empty);
        Assert.That(visited, Has.Count.GreaterThan(1000));
    }

    private static void Compare(object expected, object actual, string path, HashSet<Guid> visited, System.Collections.Generic.List<string> differences)
    {
        if (expected is not IIdentifiable identifiable || !visited.Add(identifiable.Id))
        {
            return;
        }

        if (expected.GetType() != actual.GetType())
        {
            differences.Add($"{path}: type {expected.GetType()} != {actual.GetType()}");
            return;
        }

        foreach (var property in expected.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0 && p.GetMethod?.IsPublic == true))
        {
            var expectedValue = property.GetValue(expected);
            var actualValue = property.GetValue(actual);
            var propertyPath = $"{path}.{property.Name}";
            switch (expectedValue)
            {
                case IIdentifiable expectedObject when actualValue is IIdentifiable actualObject:
                    if (expectedObject.Id != actualObject.Id)
                    {
                        differences.Add($"{propertyPath}: {expectedObject.Id} != {actualObject.Id}");
                    }
                    else
                    {
                        Compare(expectedObject, actualObject, propertyPath, visited, differences);
                    }

                    break;
                case byte[] expectedBytes:
                    if (actualValue is not byte[] actualBytes || !expectedBytes.SequenceEqual(actualBytes))
                    {
                        differences.Add($"{propertyPath}: byte arrays differ");
                    }

                    break;
                case IEnumerable expectedItems when expectedValue is not string && actualValue is IEnumerable actualItems:
                    CompareCollections(expectedItems.Cast<object>().ToList(), actualItems.Cast<object>().ToList(), propertyPath, visited, differences);
                    break;
                default:
                    // Other objects, e.g. the SimpleElement of a PowerUpDefinitionValue, are compared by their string.
                    if (!Equals(expectedValue, actualValue)
                        && !(expectedValue?.GetType() is { IsClass: true } && expectedValue.ToString() == actualValue?.ToString()))
                    {
                        differences.Add($"{propertyPath}: {expectedValue} != {actualValue}");
                    }

                    break;
            }
        }
    }

    private static void CompareCollections(System.Collections.Generic.List<object> expected, System.Collections.Generic.List<object> actual, string path, HashSet<Guid> visited, System.Collections.Generic.List<string> differences)
    {
        if (expected.Count != actual.Count)
        {
            differences.Add($"{path}: count {expected.Count} != {actual.Count}");
            return;
        }

        var actualById = actual.OfType<IIdentifiable>().GroupBy(item => item.Id).ToDictionary(group => group.Key, group => (object)group.First());
        foreach (var expectedItem in expected)
        {
            if (expectedItem is not IIdentifiable expectedObject)
            {
                // E.g. the entities of many-to-many relationships, which are compared by their keys.
                var description = Describe(expectedItem);
                if (!actual.Any(item => Describe(item) == description))
                {
                    differences.Add($"{path}: {description} is missing");
                }

                continue;
            }

            if (!actualById.TryGetValue(expectedObject.Id, out var actualItem))
            {
                differences.Add($"{path}: {expectedObject.Id} is missing");
                continue;
            }

            Compare(expectedObject, actualItem, $"{path}[{expectedObject.Id}]", visited, differences);
        }
    }

    private static string Describe(object item)
    {
        var keys = item.GetType().GetProperties().Where(p => p.PropertyType == typeof(Guid)).Select(p => $"{p.Name}={p.GetValue(item)}").ToList();
        return keys.Count > 0 ? string.Join(", ", keys) : item.ToString() ?? string.Empty;
    }
}
