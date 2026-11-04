// <copyright file="ReferenceResolvingConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A json converter which is able to resolve (circular) references.
/// It determines the properties to read by reflection.
/// </summary>
/// <remarks>
/// It's used for types which don't have a generated converter, see <see cref="ReferenceResolvingConverterRegistry"/>.
/// </remarks>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
public class ReferenceResolvingConverter<T> : ReferenceResolvingConverterBase<T>
    where T : class, IIdentifiable, new()
{
    private static readonly (Type PropertyType, Action<T, object>? Setter, Action<T, object>? Adder)[] PropertyHandlers;

    private static readonly ReferenceResolvingProperty[] PropertyDescriptions;

    static ReferenceResolvingConverter()
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            .ToList();
        var handlers = properties
            .Select(x => new
            {
                Property = x,
                CollectionInterface = x.PropertyType.IsGenericType ? DetermineCollectionInterface(x) : null,
            })
            .Select(x =>
            {
                var tParam = Expression.Parameter(typeof(T));
                var objParam = Expression.Parameter(typeof(object));
                Action<T, object>? setter = null;
                Action<T, object>? adder = null;
                Type? propertyType = null;
                var jsonPropertyName = x.Property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? x.Property.Name;
                if (x.Property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
                {
                    // ignore ...
                }
                else if (x.CollectionInterface != null && x.Property.Name.StartsWith("Raw"))
                {
                    propertyType = x.CollectionInterface.GetGenericArguments()[0];

                    var collectionExpr = Expression.Convert(Expression.Property(tParam, x.Property), x.CollectionInterface);
                    var itemExpr = Expression.Convert(objParam, propertyType);
                    var containsCall = Expression.Call(collectionExpr, x.CollectionInterface.GetMethod("Contains")!, itemExpr);
                    var addCall = Expression.Call(collectionExpr, x.CollectionInterface.GetMethod("Add")!, itemExpr);

                    adder = Expression.Lambda<Action<T, object>>(
                            Expression.IfThen(Expression.Not(containsCall), addCall),
                            tParam,
                            objParam)
                        .Compile();
                }
                else if (x.CollectionInterface != null && x.Property.Name.StartsWith("Joined"))
                {
                    var basePropertyName = x.Property.Name.Substring("Joined".Length);
                    var baseCollectionProperty = properties.First(p => p.Name == basePropertyName);
                    var baseType = baseCollectionProperty.PropertyType.GetGenericArguments()[0];

                    propertyType = x.CollectionInterface.GetGenericArguments()[0];

                    propertyType = propertyType.GetProperties().First(p => p.PropertyType.BaseType == baseType).PropertyType;
                    jsonPropertyName = basePropertyName;

                    var baseCollectionInterface = DetermineCollectionInterface(baseCollectionProperty)!;
                    var collectionExpr = Expression.Convert(Expression.Property(tParam, baseCollectionProperty), baseCollectionInterface);
                    var itemExpr = Expression.Convert(objParam, propertyType);
                    var containsCall = Expression.Call(collectionExpr, baseCollectionInterface.GetMethod("Contains")!, itemExpr);
                    var addCall = Expression.Call(collectionExpr, baseCollectionInterface.GetMethod("Add")!, itemExpr);

                    adder = Expression.Lambda<Action<T, object>>(
                            Expression.IfThen(Expression.Not(containsCall), addCall),
                            tParam,
                            objParam)
                        .Compile();
                }
                else if (x.Property.CanWrite && properties.All(p => p.Name != "Joined" + x.Property.Name))
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
                else if (x.CollectionInterface != null
                         && properties.All(p => p.Name != "Raw" + x.Property.Name && p.Name != "Joined" + x.Property.Name))
                {
                    // A collection without a public setter and without a "Raw" or "Joined" counterpart
                    // which would hold its data (e.g. ItemSlotType.ItemSlots).
                    // We can't assign a new collection, so we add the items to the existing one.
                    propertyType = x.CollectionInterface.GetGenericArguments()[0];

                    var collectionExpr = Expression.Convert(Expression.Property(tParam, x.Property), x.CollectionInterface);
                    var itemExpr = Expression.Convert(objParam, propertyType);
                    var addCall = Expression.Call(collectionExpr, x.CollectionInterface.GetMethod("Add")!, itemExpr);

                    adder = Expression.Lambda<Action<T, object>>(addCall, tParam, objParam).Compile();
                }
                else if (x.Property.GetSetMethod(nonPublic: true) is not null
                         && properties.All(p => p.Name != "Joined" + x.Property.Name))
                {
                    // A property with a non-public setter (e.g. ConstValueAttribute.Value).
                    // A compiled expression isn't allowed to call it, so we set it by reflection.
                    propertyType = x.Property.PropertyType;
                    var property = x.Property;
                    setter = (target, value) => property.SetValue(target, value);
                }
                else
                {
                    // not supported property, ignore...
                }

                return (
                    Name: jsonPropertyName,
                    Setter: setter,
                    Adder: adder,
                    PropertyType: propertyType);
            })
            .Where(x => x.PropertyType is not null)
            .ToArray();
        PropertyHandlers = handlers.Select(x => (x.PropertyType!, x.Setter, x.Adder)).ToArray();
        PropertyDescriptions = handlers.Select(x => new ReferenceResolvingProperty(x.Name, x.PropertyType!, x.Setter is null)).ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceResolvingConverter{T}"/> class.
    /// </summary>
    /// <param name="ignoredTypes">The ignored types.</param>
    public ReferenceResolvingConverter(Type[] ignoredTypes)
        : base(PropertyDescriptions, ignoredTypes)
    {
    }

    /// <inheritdoc />
    protected override void ReadPropertyValue(int index, ref Utf8JsonReader reader, T target, JsonSerializerOptions options)
    {
        var handler = PropertyHandlers[index];
        if (JsonSerializer.Deserialize(ref reader, handler.PropertyType, options) is { } value)
        {
            handler.Setter!(target, value);
        }
    }

    /// <inheritdoc />
    protected override void ReadCollectionItem(int index, ref Utf8JsonReader reader, T target, JsonSerializerOptions options)
    {
        var handler = PropertyHandlers[index];
        if (JsonSerializer.Deserialize(ref reader, handler.PropertyType, options) is { } collectionItem)
        {
            handler.Adder!(target, collectionItem);
        }
    }

    private static Type? DetermineCollectionInterface(PropertyInfo x)
    {
        return x.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? x.PropertyType
            : x.PropertyType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }
}