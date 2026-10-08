// <copyright file="ReferenceResolvingTypeInfoResolverTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Tests for the <see cref="ReferenceResolvingTypeInfoResolver"/>, which provides the json metadata
/// of the types which are read by the generated reference resolving converters.
/// </summary>
[TestFixture]
public class ReferenceResolvingTypeInfoResolverTests
{
    /// <summary>
    /// Tests that the game configuration can be deserialized into the basic model without the reflection based resolver,
    /// with the same result as with the reflection based resolver.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task BasicModelIsDeserializedWithoutReflectionResolverAsync()
    {
        var json = await CreateGameConfigurationJsonAsync().ConfigureAwait(false);

        var generated = Deserialize<BasicModel.GameConfiguration>(new GeneratedOnlyJsonObjectDeserializer(), json);
        var reflected = Deserialize<BasicModel.GameConfiguration>(new ReflectionResolverJsonObjectDeserializer(), json);

        Assert.That(generated.Items, Is.Not.Empty);
        Assert.That(await generated.ToJsonAsync().ConfigureAwait(false), Is.EqualTo(await reflected.ToJsonAsync().ConfigureAwait(false)));
    }

    /// <summary>
    /// Tests that the game configuration is deserialized into the entity framework model in the same way as
    /// with the reflection based resolver.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task EntityFrameworkModelIsDeserializedEquallyAsync()
    {
        var json = await CreateGameConfigurationJsonAsync().ConfigureAwait(false);

        var generated = Deserialize<EntityFramework.Model.GameConfiguration>(new JsonObjectDeserializer(), json);
        var reflected = Deserialize<EntityFramework.Model.GameConfiguration>(new ReflectionResolverJsonObjectDeserializer(), json);

        Assert.That(await generated.ToJsonAsync().ConfigureAwait(false), Is.EqualTo(await reflected.ToJsonAsync().ConfigureAwait(false)));
    }

    /// <summary>
    /// Tests that the resolver prefers the converters of the options over the built-in converters.
    /// </summary>
    [Test]
    public void ConverterOfOptionsIsPreferred()
    {
        // The generated code of the basic model registers byte arrays.
        RuntimeHelpers.RunModuleConstructor(typeof(BasicModel.GameConfiguration).Module.ModuleHandle);
        var converter = new EntityFramework.Json.BinaryAsHexJsonConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var typeInfo = ReferenceResolvingTypeInfoResolver.Instance.GetTypeInfo(typeof(byte[]), options);

        Assert.That(typeInfo?.Converter, Is.SameAs(converter));
    }

    /// <summary>
    /// Tests that the resolver returns <c>null</c> for types which aren't registered, so that it can be combined with other resolvers.
    /// </summary>
    [Test]
    public void UnknownTypeIsNotResolved()
    {
        Assert.That(ReferenceResolvingTypeInfoResolver.Instance.GetTypeInfo(typeof(ReferenceResolvingTypeInfoResolverTests), new JsonSerializerOptions()), Is.Null);
    }

    private static T Deserialize<T>(JsonObjectDeserializer deserializer, string json)
        where T : class
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return deserializer.Deserialize<T>(stream, new IdReferenceHandler()) ?? throw new InvalidOperationException("Deserialization returned null.");
    }

    private static async ValueTask<string> CreateGameConfigurationJsonAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var gameConfiguration = (BasicModel.GameConfiguration)(await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        return await gameConfiguration.ToJsonAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// A deserializer which doesn't use the reflection based <see cref="DefaultJsonTypeInfoResolver"/>.
    /// </summary>
    private sealed class GeneratedOnlyJsonObjectDeserializer : JsonObjectDeserializer
    {
        /// <inheritdoc />
        protected override void BeforeDeserialize(JsonSerializerOptions options)
        {
            base.BeforeDeserialize(options);
            options.TypeInfoResolverChain.Remove(options.TypeInfoResolverChain.OfType<DefaultJsonTypeInfoResolver>().Single());
        }
    }

    /// <summary>
    /// A deserializer which only uses the reflection based <see cref="DefaultJsonTypeInfoResolver"/>.
    /// </summary>
    private sealed class ReflectionResolverJsonObjectDeserializer : JsonObjectDeserializer
    {
        /// <inheritdoc />
        protected override void BeforeDeserialize(JsonSerializerOptions options)
        {
            base.BeforeDeserialize(options);
            options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
        }
    }
}
