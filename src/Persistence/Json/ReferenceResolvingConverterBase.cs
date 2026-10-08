// <copyright file="ReferenceResolvingConverterBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// The base class of json converters which are able to resolve (circular) references.
/// </summary>
/// <remarks>
/// It reads the json object and resolves its <c>$id</c> and <c>$ref</c> properties.
/// The values of the other properties are read and applied by the derived class.
/// </remarks>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
public abstract class ReferenceResolvingConverterBase<T> : JsonConverter<T>
    where T : class, IIdentifiable, new()
{
    private readonly Dictionary<string, int> _propertyIndexes;

    private readonly bool[] _isIgnored;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceResolvingConverterBase{T}"/> class.
    /// </summary>
    /// <param name="properties">The properties which are read.</param>
    /// <param name="ignoredTypes">The ignored types. Properties of these types (or with these base types) are skipped.</param>
    protected ReferenceResolvingConverterBase(IReadOnlyList<ReferenceResolvingProperty> properties, Type[] ignoredTypes)
    {
        this.Properties = properties;
        this._propertyIndexes = new Dictionary<string, int>(properties.Count, StringComparer.InvariantCultureIgnoreCase);
        this._isIgnored = new bool[properties.Count];
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            this._propertyIndexes.Add(property.Name, i);
            this._isIgnored[i] = ignoredTypes.Contains(property.PropertyType) || ignoredTypes.Contains(property.PropertyType.BaseType);
        }
    }

    /// <summary>
    /// Gets the properties which are read by this converter.
    /// </summary>
    public IReadOnlyList<ReferenceResolvingProperty> Properties { get; }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException($"Don't use {nameof(ReferenceResolvingConverterFactory)} for writing.");
    }

    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        T? item = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                if (propertyName == null)
                {
                    SkipValue(ref reader);
                }
                else if (propertyName is "$ref" or "$id"
                         && ReadValue<string>(ref reader, options) is { } referenceId)
                {
                    item ??= ResolveObjectReference(options, referenceId);
                }
                else if (this._propertyIndexes.TryGetValue(propertyName, out var index)
                         && !this._isIgnored[index])
                {
                    this.ReadProperty(ref reader, options, item, index);
                }
                else
                {
                    SkipValue(ref reader);
                }
            }
        }

        return item!;
    }

    /// <summary>
    /// Reads a value of the specified type with the metadata of the serializer options.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="reader">The reader, positioned at the value.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The read value.</returns>
    protected static TValue? ReadValue<TValue>(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (options.TypeInfoResolver is null)
        {
            PopulateDefaultResolver(options);
        }

        return JsonSerializer.Deserialize(ref reader, (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue)));
    }

    /// <summary>
    /// Reads the value of a property, which is not a collection, and applies it to the target.
    /// </summary>
    /// <param name="index">The index of the property in <see cref="Properties"/>.</param>
    /// <param name="reader">The reader, positioned at the value.</param>
    /// <param name="target">The target object.</param>
    /// <param name="options">The serializer options.</param>
    protected abstract void ReadPropertyValue(int index, ref Utf8JsonReader reader, T target, JsonSerializerOptions options);

    /// <summary>
    /// Reads an item of a collection property and adds it to the target.
    /// </summary>
    /// <param name="index">The index of the property in <see cref="Properties"/>.</param>
    /// <param name="reader">The reader, positioned at the item.</param>
    /// <param name="target">The target object.</param>
    /// <param name="options">The serializer options.</param>
    protected abstract void ReadCollectionItem(int index, ref Utf8JsonReader reader, T target, JsonSerializerOptions options);

    /// <summary>
    /// Populates the default resolver of the serializer options, like the <see cref="JsonSerializer"/> does
    /// when it gets options without a <see cref="JsonSerializerOptions.TypeInfoResolver"/>.
    /// This is only required when the converter is called directly, without the serializer.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The serializer uses the reflection based resolver in this case, too.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The serializer uses the reflection based resolver in this case, too.")]
    private static void PopulateDefaultResolver(JsonSerializerOptions options)
    {
        options.MakeReadOnly(populateMissingResolver: true);
    }

    /// <summary>
    /// Skips the current value - or the value of the property the reader is positioned on - in a way
    /// which is safe for chunked (streaming) deserialization. <see cref="Utf8JsonReader.Skip"/> must not
    /// be used here: it throws "Cannot skip tokens on partial JSON" when the buffer is not final,
    /// while <see cref="Utf8JsonReader.TrySkip"/> handles that case.
    /// </summary>
    /// <param name="reader">The reader.</param>
    private static void SkipValue(ref Utf8JsonReader reader)
    {
        if (!reader.TrySkip())
        {
            throw new JsonException("Incomplete JSON: could not skip the value.");
        }
    }

    /// <summary>
    /// Resolves the object reference by the reference handler of the serializer.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    /// <param name="id">The identifier of the object.</param>
    /// <returns>The resolved object.</returns>
    /// <remarks>
    /// The idea here is, to handle $ref and $id the same;
    /// If it's a $ref, there are two cases:
    ///   - the object was read before: it's all logical, we use the previously read object.
    ///   - the object wasn't read before (e.g. circular reference):
    ///       We already create the object as a placeholder, so that when $id comes along, it can be filled.
    /// If it's an $id, there are also two cases:
    ///   - the object was created before by a $ref: all fine, we take and fill it
    ///   - the object wasn't created before: we create a new one.
    /// This implies, that any object should have $id as first property.
    /// </remarks>
    private static T? ResolveObjectReference(JsonSerializerOptions serializer, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("id must be provided; it's null or empty.", nameof(id));
        }

        T? resolvedObject;
        if (serializer.ReferenceHandler is IIdReferenceHandler { Current: { } resolver })
        {
            resolvedObject = (T?)resolver.ResolveReference(id);
        }
        else
        {
            throw new UnreachableException("This should never happen!");
        }

        if (resolvedObject is null)
        {
            resolvedObject = new()
            {
                Id = new Guid(id),
            };
            resolver.AddReference(id, resolvedObject);
        }

        return resolvedObject;
    }

    private void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, T? item, int index)
    {
        var target = item ?? throw new InvalidOperationException("Item must be set here already. Is $id missing?");

        if (!reader.Read())
        {
            throw new JsonException($"Bad JSON");
        }

        if (!this.Properties[index].IsCollection)
        {
            this.ReadPropertyValue(index, ref reader, target, options);
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            this.ReadCollection(ref reader, options, target, index);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            // When the json was written with a reference handler, collections are wrapped
            // into an object which holds the "$id" of the collection and its "$values".
            this.ReadWrappedCollection(ref reader, options, target, index);
        }
        else
        {
            SkipValue(ref reader);
        }
    }

    private void ReadWrappedCollection(ref Utf8JsonReader reader, JsonSerializerOptions options, T item, int index)
    {
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                SkipValue(ref reader);
                continue;
            }

            var isValues = reader.ValueTextEquals("$values"u8);
            if (!reader.Read())
            {
                throw new JsonException("Bad JSON");
            }

            if (isValues && reader.TokenType == JsonTokenType.StartArray)
            {
                this.ReadCollection(ref reader, options, item, index);
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    private void ReadCollection(ref Utf8JsonReader reader, JsonSerializerOptions options, T item, int index)
    {
        while (true)
        {
            if (!reader.Read())
            {
                throw new JsonException($"Bad JSON");
            }

            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            this.ReadCollectionItem(index, ref reader, item, options);
        }
    }
}
