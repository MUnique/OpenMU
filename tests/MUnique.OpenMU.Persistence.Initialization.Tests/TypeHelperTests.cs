// <copyright file="TypeHelperTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="TypeHelper"/>.
/// </summary>
[TestFixture]
public class TypeHelperTests
{
    /// <summary>
    /// Tests that the persistent types of different persistence models don't get mixed up,
    /// e.g. when the admin panel (Entity Framework) executes a data initialization in memory.
    /// </summary>
    [Test]
    public void PersistentTypesOfDifferentAssembliesAreSeparated()
    {
        var entityFrameworkAssembly = typeof(EntityFramework.Model.GameConfiguration).Assembly;
        var basicModelAssembly = typeof(BasicModel.GameConfiguration).Assembly;

        var entityFrameworkObject = entityFrameworkAssembly.CreateNew<PlugInConfiguration>();
        var basicModelObject = basicModelAssembly.CreateNew<PlugInConfiguration>();

        Assert.That(entityFrameworkObject, Is.InstanceOf<EntityFramework.Model.PlugInConfiguration>());
        Assert.That(basicModelObject, Is.InstanceOf<BasicModel.PlugInConfiguration>());
    }

    /// <summary>
    /// Tests that a data initialization can be executed in memory after Entity Framework types were created.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task InMemoryInitializationAfterEntityFrameworkTypesAsync()
    {
        _ = typeof(EntityFramework.Model.GameConfiguration).Assembly.CreateNew<PlugInConfiguration>();

        var provider = new InMemoryPersistenceContextProvider();
        await new Version075.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using var context = provider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        Assert.That(configuration.PlugInConfigurations, Is.Not.Empty);
    }

    /// <summary>
    /// Tests the linking of built-in captions in a process which already uses Entity Framework types, like the admin panel.
    /// The data initialization is executed in memory to get the reference configuration.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task LinkBuiltInCaptionsAfterEntityFrameworkTypesAsync()
    {
        _ = typeof(EntityFramework.Model.GameConfiguration).Assembly.CreateNew<PlugInConfiguration>();
        var databaseProvider = new InMemoryPersistenceContextProvider();
        await new Version075.DataInitialization(databaseProvider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        var serviceContainer = new System.ComponentModel.Design.ServiceContainer();
        serviceContainer.AddService(typeof(IPersistenceContextProvider), databaseProvider);
        serviceContainer.AddService(typeof(Microsoft.Extensions.Logging.ILoggerFactory), NullLoggerFactory.Instance);
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, serviceContainer, null);
        plugInManager.DiscoverAndRegisterPlugInsOf<IDataInitializationPlugIn>();
        var service = new ConfigurationCaptionService(databaseProvider, plugInManager, NullLoggerFactory.Instance);
        var linkedInFreshConfiguration = (await service.CompareAsync().ConfigureAwait(false)).LinkedCaptions;
        Assert.That(linkedInFreshConfiguration, Is.Positive);

        // Simulates a database which was created before the captions had source keys.
        using (var context = databaseProvider.CreateNewContext())
        {
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            foreach (var caption in LocalizedCaption.FindAll(configuration))
            {
                caption.SetValue(caption.Value.WithSourceKey(null));
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That((await service.CompareAsync().ConfigureAwait(false)).LinkedCaptions, Is.Zero);

        var steps = new List<CaptionLinkStep>();
        var (linked, skipped) = await service.LinkBuiltInCaptionsAsync(new SynchronousProgress<CaptionLinkStep>(steps.Add)).ConfigureAwait(false);

        Assert.That(steps, Is.EqualTo(Enum.GetValues<CaptionLinkStep>()));
        Assert.That(skipped, Is.Zero);
        Assert.That(linked, Is.EqualTo(linkedInFreshConfiguration));
        var comparison = await service.CompareAsync().ConfigureAwait(false);
        Assert.That(comparison.LinkedCaptions, Is.EqualTo(linkedInFreshConfiguration));
        Assert.That(comparison.Changes, Is.Empty);
        Assert.That(comparison.UnresolvedSourceKeys, Is.Empty);
    }

    private sealed class SynchronousProgress<T>(Action<T> handler) : IProgress<T>
    {
        public void Report(T value) => handler(value);
    }
}
