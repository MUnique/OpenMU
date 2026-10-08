// <copyright file="ConfigurationCacheRefreshTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Tests for refreshing the caches of the configuration types, which are used to resolve
/// the references of loaded accounts to configuration objects.
/// </summary>
[TestFixture]
public class ConfigurationCacheRefreshTests
{
    /// <summary>
    /// Tests that objects which are added to or removed from the cached configuration at runtime,
    /// e.g. in the admin panel, are resolved after a refresh.
    /// Previously, the cache was built once, so references to added objects couldn't be resolved.
    /// </summary>
    [Test]
    public async Task AddedAndRemovedObjectsAreConsideredAfterRefreshAsync()
    {
        var existingItem = new ItemDefinition { Id = Guid.NewGuid() };
        var addedItem = new ItemDefinition { Id = Guid.NewGuid() };
        var configuration = new GameConfiguration();
        configuration.RawItems.Add(existingItem);

        var repositoryProvider = new CacheAwareRepositoryProvider(NullLoggerFactory.Instance, null);
        using var context = new CachingEntityFrameworkContext(
            new EntityDataContext { CurrentGameConfiguration = configuration },
            repositoryProvider,
            null,
            NullLogger<CachingEntityFrameworkContext>.Instance);
        using var usage = repositoryProvider.ContextStack.UseContext(context);
        var repository = repositoryProvider.GetRepository<DataModel.Configuration.Items.ItemDefinition>()!;
        repositoryProvider.EnsureCachesForCurrentGameConfiguration();
        Assert.That(repositoryProvider.ConfigurationReferences.ResolveReference(existingItem.Id.ToString()), Is.SameAs(existingItem));

        configuration.RawItems.Add(addedItem);
        configuration.RawItems.Remove(existingItem);
        repositoryProvider.RefreshConfigurationCaches();

        Assert.That(repositoryProvider.ConfigurationReferences.ResolveReference(addedItem.Id.ToString()), Is.SameAs(addedItem));
        Assert.That(repositoryProvider.ConfigurationReferences.ResolveReference(existingItem.Id.ToString()), Is.Null);
        Assert.That(await repository.GetByIdAsync(addedItem.Id).ConfigureAwait(false), Is.SameAs(addedItem));
        Assert.That(await repository.GetByIdAsync(existingItem.Id).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that the configuration objects of a reset cache are not resolved anymore.
    /// Previously, the resolver was a singleton which kept them.
    /// </summary>
    [Test]
    public void ResetCacheDiscardsTheConfigurationReferences()
    {
        var item = new ItemDefinition { Id = Guid.NewGuid() };
        var configuration = new GameConfiguration();
        configuration.RawItems.Add(item);

        var repositoryProvider = new CacheAwareRepositoryProvider(NullLoggerFactory.Instance, null);
        using var context = new CachingEntityFrameworkContext(
            new EntityDataContext { CurrentGameConfiguration = configuration },
            repositoryProvider,
            null,
            NullLogger<CachingEntityFrameworkContext>.Instance);
        using var usage = repositoryProvider.ContextStack.UseContext(context);
        _ = repositoryProvider.GetRepository<DataModel.Configuration.Items.ItemDefinition>();
        repositoryProvider.EnsureCachesForCurrentGameConfiguration();
        Assert.That(repositoryProvider.ConfigurationReferences.ResolveReference(item.Id.ToString()), Is.SameAs(item));

        repositoryProvider.ResetCache();

        Assert.That(repositoryProvider.ConfigurationReferences.ResolveReference(item.Id.ToString()), Is.Null);
    }
}
