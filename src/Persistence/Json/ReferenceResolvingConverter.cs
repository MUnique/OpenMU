// <copyright file="ReferenceResolvingConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A json converter which is able to resolve (circular) references.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
public class ReferenceResolvingConverter<T> : JsonConverter<T>
    where T : class, IIdentifiable, new()
{
    private static readonly Dictionary<string, (Type PropertyType, Action<T, object>? Setter, Action<T, object>? Adder)> PropertyHandlers;

    private readonly Type[] _ignoredTypes;

    static ReferenceResolvingConverter()
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            .ToList();
        PropertyHandlers = properties
            .Select(x => new
            {
                Property = x,
                CollectionInterface = !x.CanWrite && x.PropertyType.IsGenericType ? DetermineCollectionInterface(x) : null,
            })
            .Select(x =>
            {
                var tParam = Expression.Parameter(typeof(T));
                var objParam = Expression.Parameter(typeof(object));
                Action<T, object>? setter = null;
                Action<T, object>? adder = null;
                Type? propertyType = null;
                if (x.Property.CanWrite)
                {
                    propertyType = x.Property.PropertyType;
                    setter = Expression.Lambda<Action<T, object>>(
                            Expression.Assign(
                                Expression.Property(tParam, x.Property),
                                Expression.Convert(objParam, propertyType)),
                            tParam,
                            objParam)
                        .Compile();
                }
                else if (x.CollectionInterface != null && (x.Property.Name.StartsWith("Raw") || x.Property.Name.StartsWith("Joined")))
                {
                    propertyType = x.CollectionInterface.GetGenericArguments()[0];
                    adder = Expression.Lambda<Action<T, object>>(
                            Expression.Call(
                                Expression.Property(tParam, x.Property),
                                x.CollectionInterface.GetMethod("Add")!,
                                Expression.Convert(objParam, propertyType)),
                            tParam,
                            objParam)
                        .Compile();
                }
                else
                {
                    // not supported property, ignore...
                }

                return (
                    Name: x.Property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? x.Property.Name,
                    Setter: setter,
                    Adder: adder,
                    PropertyType: propertyType);
            })
            .Where(x => x.PropertyType is not null)
            .ToDictionary(x => x.Name, x => (x.PropertyType!, x.Setter, x.Adder), StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceResolvingConverter{T}"/> class.
    /// </summary>
    /// <param name="ignoredTypes">The ignored types.</param>
    public ReferenceResolvingConverter(Type[] ignoredTypes)
    {
        this._ignoredTypes = ignoredTypes;
    }

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
                    reader.Skip();
                }
                else if (propertyName is "$ref" or "$id"
                         && JsonSerializer.Deserialize<string>(ref reader, options) is { } referenceId)
                {
                    item ??= ResolveObjectReference(options, referenceId);
                }
                else if (PropertyHandlers.TryGetValue(propertyName, out var handler)
                         && !this._ignoredTypes.Contains(handler.PropertyType) && !this._ignoredTypes.Contains(handler.PropertyType!.BaseType))
                {
                    ReadProperty(ref reader, options, item, handler);
                }
                else
                {
                    reader.Skip();
                }
            }
        }

        return item!;
    }

    private static void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, T? item, (Type PropertyType, Action<T, object>? Setter, Action<T, object>? Adder) handler)
    {
        _ = item ?? throw new InvalidOperationException("Item must be set here already. Is $id missing?");

        if (!reader.Read())
        {
            throw new JsonException($"Bad JSON");
        }

        if (handler.Setter != null)
        {
            if (JsonSerializer.Deserialize(ref reader, handler.PropertyType, options) is { } value)
            {
                handler.Setter(item, value);
            }
        }
        else
        {
            if (reader.TokenType == JsonTokenType.StartArray)
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

                    if (JsonSerializer.Deserialize(ref reader, handler.PropertyType, options) is { } collectionItem)
                    {
                        handler.Adder!(item, collectionItem);
                    }
                }
            }
            else
            {
                reader.Skip();
            }
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

    private static Type? DetermineCollectionInterface(PropertyInfo x)
    {
        return x.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? x.PropertyType
            : x.PropertyType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }
}