// <copyright file="NonCachingRepositoryProviderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests for the <see cref="NonCachingRepositoryProvider"/>.
/// </summary>
[TestFixture]
public class NonCachingRepositoryProviderTests
{
    /// <summary>
    /// Tests that the initialization provides a repository for every type of the model,
    /// without failing to register one which is already registered (see issue #1192).
    /// </summary>
    [Test]
    public void ProvidesRepositoriesForAllModelTypes()
    {
        var provider = CreateProvider();

        var typesWithoutRepository = EntityDataContext.CompleteModel.GetEntityTypes()
            .Select(t => t.ClrType)
            .Where(t => provider.GetRepository(t) is null)
            .ToList();

        Assert.That(typesWithoutRepository, Is.Empty);
        Assert.That(provider.GetRepository(typeof(Account)), Is.InstanceOf<AccountRepository>());
        Assert.That(provider.GetRepository(typeof(LetterBody)), Is.InstanceOf<LetterBodyRepository>());
        Assert.That(provider.GetRepository(typeof(DataModel.Configuration.GameConfiguration)), Is.InstanceOf<GameConfigurationRepository>());
    }

    /// <summary>
    /// Tests that concurrent first accesses, e.g. of players logging in right after the start
    /// of the server, don't fail with an already registered repository.
    /// </summary>
    [Test]
    public async Task ConcurrentFirstAccessesSucceedAsync()
    {
        var provider = CreateProvider();
        using var start = new ManualResetEventSlim();
        var accesses = Enumerable.Range(0, 8)
            .Select(_ => Task.Run(() =>
            {
                start.Wait();
                return provider.GetRepository(typeof(Account));
            }))
            .ToList();

        start.Set();
        var repositories = await Task.WhenAll(accesses).ConfigureAwait(false);

        Assert.That(repositories, Has.All.InstanceOf<AccountRepository>());
    }

    /// <summary>
    /// Tests that concurrent first accesses through the shared <see cref="CacheAwareRepositoryProvider"/>
    /// of the <see cref="PersistenceContextProvider"/> succeed. In the distributed deployment, the game server
    /// host accesses it from its startup and from the incoming requests of the web server at the same time.
    /// </summary>
    [Test]
    public async Task ConcurrentFirstAccessesThroughCacheAwareProviderSucceedAsync()
    {
        var provider = new CacheAwareRepositoryProvider(NullLoggerFactory.Instance, null);
        using var start = new ManualResetEventSlim();
        var accesses = Enumerable.Range(0, 8)
            .Select(_ => Task.Run(() =>
            {
                start.Wait();
                return provider.GetRepository(typeof(Account));
            }))
            .ToList();

        start.Set();
        var repositories = await Task.WhenAll(accesses).ConfigureAwait(false);

        Assert.That(repositories, Has.All.InstanceOf<AccountRepository>());
    }

    private static NonCachingRepositoryProvider CreateProvider()
    {
        return new NonCachingRepositoryProvider(NullLoggerFactory.Instance, null, null, new ContextStack());
    }
}
