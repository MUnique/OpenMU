// <copyright file="GeneratedReferenceResolvingConverterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Tests for the reference resolving json converters which are generated at compile time.
/// They have to behave exactly like the <see cref="ReferenceResolvingConverter{T}"/>, which uses reflection.
/// </summary>
[TestFixture]
public class GeneratedReferenceResolvingConverterTests
{
    private static readonly ReferenceResolvingConverterFactory Factory = new();

    /// <summary>
    /// Gets the persistent types of the basic model and of the entity framework model.
    /// </summary>
    private static IEnumerable<Type> PersistentTypes => new[] { typeof(BasicModel.GameConfiguration).Assembly, typeof(EntityFramework.Model.GameConfiguration).Assembly }
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsAbstract && type.IsClass && type.GetConstructor(Type.EmptyTypes) is not null && Factory.CanConvert(type))
        .OrderBy(type => type.FullName, StringComparer.Ordinal);

    /// <summary>
    /// Tests that a converter was generated for each persistent type.
    /// </summary>
    /// <param name="type">The persistent type.</param>
    [TestCaseSource(nameof(PersistentTypes))]
    public void ConverterIsGenerated(Type type)
    {
        var converter = Factory.CreateConverter(type, new JsonSerializerOptions());

        Assert.That(converter.GetType().IsGenericType, Is.False, $"No converter was generated for {type}, so it's converted by {converter.GetType()}.");
    }

    /// <summary>
    /// Tests that the generated converter reads the same properties as the <see cref="ReferenceResolvingConverter{T}"/>.
    /// </summary>
    /// <param name="type">The persistent type.</param>
    [TestCaseSource(nameof(PersistentTypes))]
    public void GeneratedConverterReadsSameProperties(Type type)
    {
        var generated = GetProperties(Factory.CreateConverter(type, new JsonSerializerOptions()));
        var reflected = GetProperties(CreateReflectionConverter(type));

        Assert.That(generated, Is.EqualTo(reflected));
    }

    /// <summary>
    /// Tests that the game configuration is deserialized into the basic model by the generated converters
    /// in the same way as by the <see cref="ReferenceResolvingConverter{T}"/>.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task BasicModelIsDeserializedEquallyAsync()
    {
        var json = await this.CreateGameConfigurationJsonAsync().ConfigureAwait(false);

        var generated = Deserialize<BasicModel.GameConfiguration>(new JsonObjectDeserializer(), json);
        var reflected = Deserialize<BasicModel.GameConfiguration>(new ReflectionJsonObjectDeserializer(), json);

        Assert.That(generated.Items, Is.Not.Empty);
        Assert.That(await generated.ToJsonAsync().ConfigureAwait(false), Is.EqualTo(await reflected.ToJsonAsync().ConfigureAwait(false)));
    }

    /// <summary>
    /// Tests that the game configuration is deserialized into the entity framework model by the generated converters
    /// in the same way as by the <see cref="ReferenceResolvingConverter{T}"/>.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task EntityFrameworkModelIsDeserializedEquallyAsync()
    {
        var json = await this.CreateGameConfigurationJsonAsync().ConfigureAwait(false);

        var generated = Deserialize<EntityFramework.Model.GameConfiguration>(new JsonObjectDeserializer(), json);
        var reflected = Deserialize<EntityFramework.Model.GameConfiguration>(new ReflectionJsonObjectDeserializer(), json);

        Assert.That(await generated.ToJsonAsync().ConfigureAwait(false), Is.EqualTo(await reflected.ToJsonAsync().ConfigureAwait(false)));
    }

    private static T Deserialize<T>(JsonObjectDeserializer deserializer, string json)
        where T : class
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return deserializer.Deserialize<T>(stream, new IdReferenceHandler()) ?? throw new InvalidOperationException("Deserialization returned null.");
    }

    private static JsonConverter CreateReflectionConverter(Type type)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(ReferenceResolvingConverter<>).MakeGenericType(type), new object[] { Array.Empty<Type>() })!;
    }

    private static List<ReferenceResolvingProperty> GetProperties(JsonConverter converter)
    {
        var properties = (IReadOnlyList<ReferenceResolvingProperty>)converter.GetType()
            .GetProperty(nameof(ReferenceResolvingConverter<BasicModel.GameConfiguration>.Properties), BindingFlags.Public | BindingFlags.Instance)!
            .GetValue(converter)!;
        return properties.OrderBy(p => p.Name, StringComparer.Ordinal).ToList();
    }

    private async ValueTask<string> CreateGameConfigurationJsonAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var gameConfiguration = (BasicModel.GameConfiguration)(await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        return await gameConfiguration.ToJsonAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// A deserializer which uses the <see cref="ReferenceResolvingConverter{T}"/> for all types.
    /// </summary>
    private sealed class ReflectionJsonObjectDeserializer : JsonObjectDeserializer
    {
        /// <inheritdoc />
        protected override void BeforeDeserialize(JsonSerializerOptions options)
        {
            base.BeforeDeserialize(options);
            options.Converters.Insert(0, new ReflectionConverterFactory(options.Converters.OfType<ReferenceResolvingConverterFactory>().Single()));
        }
    }

    /// <summary>
    /// A converter factory which creates <see cref="ReferenceResolvingConverter{T}"/> for all types of the <see cref="ReferenceResolvingConverterFactory"/>.
    /// </summary>
    private sealed class ReflectionConverterFactory : JsonConverterFactory
    {
        private readonly ReferenceResolvingConverterFactory _factory;

        public ReflectionConverterFactory(ReferenceResolvingConverterFactory factory)
        {
            this._factory = factory;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) => this._factory.CanConvert(typeToConvert);

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(ReferenceResolvingConverter<>).MakeGenericType(typeToConvert), new object[] { this._factory.IgnoredTypes })!;
        }
    }
}
