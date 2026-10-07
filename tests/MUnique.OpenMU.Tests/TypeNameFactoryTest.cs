// <copyright file="TypeNameFactoryTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the factories which create objects by their type name, and which are implemented by a code generator.
/// They have to create the same objects as <see cref="Type.GetType(string)"/> and <see cref="Activator.CreateInstance(Type, object[])"/>
/// did before.
/// </summary>
[TestFixture]
public class TypeNameFactoryTest
{
    /// <summary>
    /// Tests that each item crafting handler is created by its full name, and that the settings are passed to the
    /// constructor if, and only if, the handler is derived from <see cref="SimpleItemCraftingHandler"/>.
    /// </summary>
    [Test]
    public void ItemCraftingHandlersAreCreated()
    {
        var handlerTypes = typeof(IItemCraftingHandler).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(IItemCraftingHandler).IsAssignableFrom(type) && type.BaseType == typeof(SimpleItemCraftingHandler))
            .ToList();
        Assert.That(handlerTypes, Is.Not.Empty);

        foreach (var type in handlerTypes)
        {
            // Before, the settings were passed to the constructor of the types which are derived from SimpleItemCraftingHandler.
            Assert.That(type.GetConstructor([typeof(SimpleCraftingSettings)]), Is.Not.Null, type.FullName);
            Assert.That(ItemCraftingHandlerFactory.Create(type.FullName!, new SimpleCraftingSettings()), Is.TypeOf(type));
        }

        var otherTypes = typeof(IItemCraftingHandler).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(IItemCraftingHandler).IsAssignableFrom(type) && type.BaseType != typeof(SimpleItemCraftingHandler) && type != typeof(SimpleItemCraftingHandler))
            .ToList();
        foreach (var type in otherTypes)
        {
            // Before, the parameterless constructor was called.
            Assert.That(type.GetConstructor(Type.EmptyTypes), Is.Not.Null, type.FullName);
            Assert.That(type.GetConstructor([typeof(SimpleCraftingSettings)]), Is.Null, type.FullName);
            Assert.That(ItemCraftingHandlerFactory.Create(type.FullName!, null), Is.TypeOf(type));
        }
    }

    /// <summary>
    /// Tests that each npc intelligence is created by its full name, and that the map is passed to the
    /// constructor if, and only if, a constructor has a parameter of type <see cref="GameMap"/>.
    /// </summary>
    [Test]
    public void NpcIntelligencesAreCreated()
    {
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), 8);
        var intelligenceTypes = typeof(INpcIntelligence).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType && typeof(INpcIntelligence).IsAssignableFrom(type) && type.IsPublic)
            .ToList();
        Assert.That(intelligenceTypes, Is.Not.Empty);

        foreach (var type in intelligenceTypes)
        {
            var needsMap = type.GetConstructors().Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(GameMap)));
            var expectedConstructor = needsMap ? type.GetConstructor([typeof(GameMap)]) : type.GetConstructor(Type.EmptyTypes);
            if (expectedConstructor is null)
            {
                // It couldn't be created before, either.
                continue;
            }

            Assert.That(NpcIntelligenceFactory.Create(type.FullName!, map), Is.TypeOf(type), type.FullName);
        }
    }

    /// <summary>
    /// Tests that the factories return <c>null</c> for unknown type names, so that the callers fall back to <see cref="Type.GetType(string)"/>.
    /// </summary>
    [Test]
    public void UnknownTypeNamesReturnNull()
    {
        Assert.That(ItemCraftingHandlerFactory.Create("Unknown.Type", null), Is.Null);
        Assert.That(NpcIntelligenceFactory.Create(typeof(BasicMonsterIntelligence).AssemblyQualifiedName!, new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), 8)), Is.Null);
    }

    /// <summary>
    /// Tests that all type names of the season 6 data are known by the factories.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task TypeNamesOfInitialDataAreKnownAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        await new Persistence.Initialization.VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), 8);

        var intelligenceNames = configuration.Monsters.Select(m => m.IntelligenceTypeName).OfType<string>().Where(n => n.Length > 0).Distinct().ToList();
        var craftingNames = configuration.Monsters.SelectMany(m => m.ItemCraftings).Select(c => c.ItemCraftingHandlerClassName).OfType<string>().Where(n => n.Length > 0).Distinct().ToList();
        Assert.That(intelligenceNames, Is.Not.Empty);
        Assert.That(craftingNames, Is.Not.Empty);

        Assert.That(intelligenceNames.Where(name => NpcIntelligenceFactory.Create(name, map) is null), Is.Empty);
        Assert.That(craftingNames.Where(name => ItemCraftingHandlerFactory.Create(name, new SimpleCraftingSettings()) is null), Is.Empty);
    }
}
