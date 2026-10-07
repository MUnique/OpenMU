// <copyright file="CompiledModelTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;

/// <summary>
/// Tests that the compiled models in <c>Persistence/EntityFramework/CompiledModels</c> are up to date,
/// i.e. that they're equal to the models which are built by the <c>OnModelCreating</c> methods of the contexts.
/// If one of these tests fails, the compiled models need to be generated again, see the
/// <c>Readme.md</c> in the <c>CompiledModels</c> folder.
/// </summary>
/// <remarks>
/// The tests don't require a database, the contexts are just created to get their models.
/// </remarks>
[TestFixture]
public class CompiledModelTests
{
    /// <summary>
    /// Tests that the context uses its compiled model instead of building the model at runtime.
    /// </summary>
    /// <param name="contextFactory">The factory of the context.</param>
    /// <param name="compiledModel">The expected compiled model.</param>
    [TestCaseSource(nameof(Contexts))]
    public void ContextUsesCompiledModel(Func<DbContext> contextFactory, IModel compiledModel)
    {
        using var context = contextFactory();
        Assert.That(context.Model, Is.SameAs(compiledModel));
    }

    /// <summary>
    /// Tests that the compiled model has the same structure as the model which is built at runtime:
    /// entity types, properties, keys, foreign keys, navigations and indexes.
    /// </summary>
    /// <remarks>
    /// The debug strings of the models can't be compared, because they differ in the order of the navigations
    /// and in the annotations, as compiled models just contain the annotations which are required at runtime.
    /// The relevant annotations for the database are compared in <see cref="CompiledModelHasEqualRelationalModel"/>.
    /// </remarks>
    /// <param name="contextFactory">The factory of the context.</param>
    /// <param name="compiledModel">The compiled model.</param>
    [TestCaseSource(nameof(Contexts))]
    public void CompiledModelHasEqualStructure(Func<DbContext> contextFactory, IModel compiledModel)
    {
        using var context = contextFactory();
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;

        Assert.That(DescribeStructure(compiledModel), Is.EqualTo(DescribeStructure(designTimeModel)));
    }

    /// <summary>
    /// Tests that the compiled model has the same relational model as the model which is built at runtime,
    /// i.e. the same tables, columns, constraints and indexes.
    /// </summary>
    /// <param name="contextFactory">The factory of the context.</param>
    /// <param name="compiledModel">The compiled model.</param>
    [TestCaseSource(nameof(Contexts))]
    public void CompiledModelHasEqualRelationalModel(Func<DbContext> contextFactory, IModel compiledModel)
    {
        using var context = contextFactory();
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;

        Assert.That(
            compiledModel.GetRelationalModel().ToDebugString(MetadataDebugStringOptions.LongDefault),
            Is.EqualTo(designTimeModel.GetRelationalModel().ToDebugString(MetadataDebugStringOptions.LongDefault)));
    }

    /// <summary>
    /// Tests that the json queries, which are built based on the model metadata, are the same for the
    /// compiled model and the model which is built at runtime.
    /// </summary>
    [Test]
    public void JsonQueriesAreEqual()
    {
        using var context = new EntityDataContext();
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;

        foreach (var (builder, clrType) in new (JsonQueryBuilder Builder, Type ClrType)[]
                 {
                     (new GameConfigurationJsonQueryBuilder(), typeof(Persistence.EntityFramework.Model.GameConfiguration)),
                     (new JsonQueryBuilder(), typeof(Persistence.EntityFramework.Model.Account)),
                 })
        {
            Assert.That(
                builder.BuildJsonQueryForEntity(context.Model.FindEntityType(clrType)!),
                Is.EqualTo(builder.BuildJsonQueryForEntity(designTimeModel.FindEntityType(clrType)!)),
                clrType.Name);
        }
    }

    private static IReadOnlyList<string> DescribeStructure(IModel model)
    {
        static string Names(IEnumerable<IReadOnlyProperty> properties) => string.Join(", ", properties.Select(p => p.Name));

        return model.GetEntityTypes()
            .SelectMany(entityType => new[] { $"{entityType.Name} : {entityType.BaseType?.Name}" }
                .Concat(entityType.GetDeclaredProperties().Select(p =>
                    $"{entityType.Name}.{p.Name}: {p.ClrType}, nullable: {p.IsNullable}, max length: {p.GetMaxLength()}, generated: {p.ValueGenerated}, generator: {GetValueGeneratorType(p)}"))
                .Concat(entityType.GetDeclaredKeys().Select(k =>
                    $"{entityType.Name} key ({Names(k.Properties)}), primary: {k.IsPrimaryKey()}"))
                .Concat(entityType.GetDeclaredForeignKeys().Select(fk =>
                    $"{entityType.Name} foreign key ({Names(fk.Properties)}) -> {fk.PrincipalEntityType.Name} ({Names(fk.PrincipalKey.Properties)}), required: {fk.IsRequired}, unique: {fk.IsUnique}, delete: {fk.DeleteBehavior}, ownership: {fk.IsOwnership}"))
                .Concat(entityType.GetDeclaredNavigations().Select(n =>
                    $"{entityType.Name}.{n.Name} -> {n.TargetEntityType.Name}, collection: {n.IsCollection}, on dependent: {n.IsOnDependent}"))
                .Concat(entityType.GetDeclaredSkipNavigations().Select(n =>
                    $"{entityType.Name}.{n.Name} -> {n.TargetEntityType.Name} (skip)"))
                .Concat(entityType.GetDeclaredIndexes().Select(i =>
                    $"{entityType.Name} index ({Names(i.Properties)}), unique: {i.IsUnique}")))
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Gets the type of the value generator of a property.
    /// </summary>
    /// <remarks>
    /// The model which is built at runtime keeps the type of the value generator factory, while the compiled model
    /// keeps a delegate of the factory. So we compare the type of the value generator which they create.
    /// </remarks>
    private static Type? GetValueGeneratorType(IProperty property)
    {
        if (property.GetValueGeneratorFactory() is { } factory)
        {
            return factory(property, property.DeclaringType).GetType();
        }

        if (property.FindAnnotation(CoreAnnotationNames.ValueGeneratorFactoryType)?.Value is Type factoryType
            && Activator.CreateInstance(factoryType) is ValueGeneratorFactory typeBasedFactory)
        {
            return typeBasedFactory.Create(property, property.DeclaringType).GetType();
        }

        return null;
    }

    private static IEnumerable<TestCaseData> Contexts()
    {
        yield return new TestCaseData(new Func<DbContext>(() => new EntityDataContext()), Persistence.EntityFramework.CompiledModels.ForEntityDataContext.EntityDataContextModel.Instance)
            .SetArgDisplayNames(nameof(EntityDataContext));
        yield return new TestCaseData(new Func<DbContext>(() => new ConfigurationContext()), Persistence.EntityFramework.CompiledModels.ForEntityDataContext.EntityDataContextModel.Instance)
            .SetArgDisplayNames(nameof(ConfigurationContext));
        yield return new TestCaseData(new Func<DbContext>(() => new AccountContext()), Persistence.EntityFramework.CompiledModels.ForAccountContext.AccountContextModel.Instance)
            .SetArgDisplayNames(nameof(AccountContext));
        yield return new TestCaseData(new Func<DbContext>(() => new TradeContext()), Persistence.EntityFramework.CompiledModels.ForTradeContext.TradeContextModel.Instance)
            .SetArgDisplayNames(nameof(TradeContext));
    }
}
