// <copyright file="GeneratedPersistentTypeRegistryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Reflection;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the persistent type registries which are generated at compile time.
/// The <see cref="TypeHelper"/> has to return the same types with them as by searching the assemblies.
/// </summary>
[TestFixture]
public class GeneratedPersistentTypeRegistryTests
{
    /// <summary>
    /// Gets the assemblies of the persistence models.
    /// </summary>
    private static IEnumerable<Assembly> ModelAssemblies =>
    [
        typeof(BasicModel.GameConfiguration).Assembly,
        typeof(EntityFramework.Model.GameConfiguration).Assembly,
    ];

    /// <summary>
    /// Tests that the assembly has a generated registry.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    [TestCaseSource(nameof(ModelAssemblies))]
    public void RegistryIsGenerated(Assembly assembly)
    {
        Assert.That(assembly.GetCustomAttribute<PersistentTypeRegistryAttribute>(), Is.Not.Null);
    }

    /// <summary>
    /// Tests that the persistent type of each base type is the same as the first type of the assembly with this base type.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    [TestCaseSource(nameof(ModelAssemblies))]
    public void PersistentTypesOfBaseTypesAreEqual(Assembly assembly)
    {
        var baseTypes = assembly.GetTypes()
            .Select(type => type.BaseType)
            .OfType<Type>()
            .Where(baseType => baseType.Assembly != assembly && baseType.Namespace?.StartsWith("MUnique.OpenMU.", StringComparison.Ordinal) == true)
            .Distinct()
            .ToList();
        var registeredBaseTypes = 0;
        foreach (var baseType in baseTypes)
        {
            var expected = assembly.GetTypes().First(type => type.BaseType == baseType);
            Assert.That(assembly.GetPersistentTypeOf(baseType), Is.EqualTo(expected), baseType.FullName);
            if (assembly.FindPersistentType(baseType) is { } persistentType)
            {
                // Base types which are not part of the persistence model, e.g. of the repository providers, are not registered.
                Assert.That(persistentType.Type, Is.EqualTo(expected), baseType.FullName);
                registeredBaseTypes++;
            }
        }

        Assert.That(registeredBaseTypes, Is.GreaterThan(90));
    }

    /// <summary>
    /// Tests that the registry contains each entity type of the entity framework model, so that
    /// their repositories are created without reflection.
    /// </summary>
    [Test]
    public void RegistryContainsAllEntityTypes()
    {
        var assembly = typeof(EntityFramework.Model.GameConfiguration).Assembly;
        var entityTypes = EntityFramework.EntityDataContext.CompleteModel.GetEntityTypes()
            .Select(entityType => entityType.ClrType)
            .Where(type => type.Assembly == assembly)
            .Distinct()
            .ToList();

        Assert.That(entityTypes.Where(type => assembly.FindPersistentType(type) is null), Is.Empty);
    }

    /// <summary>
    /// Tests that <see cref="TypeHelper.CreateNew{TBase}"/> creates instances of the same types as by reflection.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    [TestCaseSource(nameof(ModelAssemblies))]
    public void InstancesAreCreated(Assembly assembly)
    {
        var persistentTypes = assembly.GetTypes()
            .Where(type => assembly.FindPersistentType(type) is not null && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) is { IsPublic: true })
            .ToList();
        Assert.That(persistentTypes, Is.Not.Empty);

        foreach (var type in persistentTypes)
        {
            Assert.That(assembly.CreateNew(type), Is.TypeOf(type));
            if (type.BaseType is { } baseType && baseType.Assembly != assembly && assembly.GetPersistentTypeOf(baseType) == type)
            {
                Assert.That(assembly.CreateNew(baseType), Is.TypeOf(type));
            }
        }
    }

    /// <summary>
    /// Tests that the in-memory repositories are created for the persistent types.
    /// </summary>
    [Test]
    public void MemoryRepositoriesAreCreated()
    {
        var provider = new InMemoryRepositoryProvider();
        var repository = provider.GetRepository(typeof(DataModel.Configuration.GameConfiguration));

        Assert.That(repository, Is.TypeOf<MemoryRepository<BasicModel.GameConfiguration>>());
    }
}
