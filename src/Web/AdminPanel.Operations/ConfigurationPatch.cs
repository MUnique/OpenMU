// <copyright file="ConfigurationPatch.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>Restricts configuration patches to known model fields and explicit references.</summary>
internal static class ConfigurationPatch
{
    /// <summary>The invariant scalar and enum JSON representation.</summary>
    internal static readonly JsonSerializerOptions Options = new() { Converters = { new JsonStringEnumConverter(), new LocalizedStringJsonConverter() } };
    private static readonly string[] SecretNames = ["password", "secret", "token", "credential", "keyhash", "authorization", "cookie", "webhook", "connectionstring", "source", "assembly"];

    /// <summary>Resolves model types from the same catalog as configuration reads.</summary>
    /// <param name="name">The invariant model type name.</param>
    internal static Type Resolve(string name) => name == nameof(GameConfiguration) ? typeof(GameConfiguration) : name == nameof(GameServerDefinition) ? typeof(GameServerDefinition)
        : GameConfigurationHelper.Enumerables.Keys.FirstOrDefault(t => t.Name == name && t != typeof(PlugInConfiguration))
          ?? throw new AdminOperationException("Unknown or excluded configuration type. Use config_types.", AdminOperationErrorKind.InvalidInput);

    /// <summary>Parses a bounded merge patch without allowing duplicate field names.</summary>
    /// <param name="json">The json.</param>
    internal static JsonObject Parse(string json)
    {
        if (json.Length > 6000)
        {
            throw new AdminOperationException("The patch exceeds 6000 characters. Split the change.", AdminOperationErrorKind.InvalidInput);
        }

        try
        {
            return JsonNode.Parse(json, documentOptions: new JsonDocumentOptions { MaxDepth = 8 }) as JsonObject
                ?? throw new AdminOperationException("Supply a JSON object merge patch.", AdminOperationErrorKind.InvalidInput);
        }
        catch (JsonException)
        {
            throw new AdminOperationException("Supply a valid JSON object merge patch.", AdminOperationErrorKind.InvalidInput);
        }
    }

    /// <summary>Returns supported scalar and reference fields, excluding identities and credentials.</summary>
    /// <param name="type">The configuration model type.</param>
    /// <param name="pluginSettings">Whether plugin metadata, rather than legacy model name exclusions, defines sensitive fields.</param>
    internal static IEnumerable<PropertyInfo> Fields(Type type, bool pluginSettings = false) => type.GetProperties().Where(p => p.GetIndexParameters().Length == 0
        && p.CanRead && (p.SetMethod?.IsPublic == true || IsReferenceCollection(p)) && p.Name != "Id" && !p.IsDefined(typeof(SecretAttribute)) && (pluginSettings || !SecretNames.Any(n => p.Name.Contains(n, StringComparison.OrdinalIgnoreCase)))
        && (IsScalar(p.PropertyType) || IsReference(p.PropertyType) || IsReferenceCollection(p))).OrderBy(p => p.Name, StringComparer.Ordinal);

    /// <summary>Projects safe scalar values and reference identifiers, never full entity graphs.</summary>
    /// <param name="item">The configuration object.</param>
    /// <param name="type">The configuration model type.</param>
    internal static JsonObject Snapshot(object item, Type type)
    {
        var result = new JsonObject();
        foreach (var property in Fields(type))
        {
            var value = property.GetValue(item);
            result[property.Name] = value is null ? null : IsReferenceCollection(property) && value is IEnumerable collection
                ? new JsonArray(collection.Cast<object>().OrderBy(v => v.GetId()).Select(v => (JsonNode)new JsonObject { ["$ref"] = v.GetId().ToString() }).ToArray()) : IsReference(property.PropertyType)
                ? new JsonObject { ["$ref"] = value.GetId().ToString() }
                : JsonSerializer.SerializeToNode(value, property.PropertyType, Options);
        }

        return result;
    }

    /// <summary>Validates all conversions and references before modifying any entity.</summary>
    /// <param name="type">The configuration model type.</param>
    /// <param name="patch">The JSON merge patch.</param>
    /// <param name="context">The persistence context used for reference resolution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="pluginSettings">Whether to use plugin secret metadata when selecting patchable fields.</param>
    internal static async ValueTask<Dictionary<PropertyInfo, object?>> ConvertAsync(Type type, JsonObject patch, IContext context, CancellationToken cancellationToken, bool pluginSettings = false)
    {
        if (patch.Count is < 1 or > 50)
        {
            throw new AdminOperationException("A patch must contain 1-50 fields.", AdminOperationErrorKind.InvalidInput);
        }

        var fields = Fields(type, pluginSettings).ToDictionary(p => p.Name, StringComparer.Ordinal);
        var result = new Dictionary<PropertyInfo, object?>();
        foreach (var (name, value) in patch)
        {
            if (!fields.TryGetValue(name, out var property))
            {
                throw new AdminOperationException("Unknown, immutable, collection or excluded field. Patch child objects separately; use explicit $ref objects for references.", AdminOperationErrorKind.InvalidInput);
            }

            object? converted;
            try
            {
                if (value is null)
                {
                    if (IsReferenceCollection(property) || (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) is null && property.PropertyType != typeof(LocalizedString)))
                    {
                        throw new JsonException();
                    }

                    converted = property.PropertyType == typeof(LocalizedString) ? (object)default(LocalizedString) : null;
                }
                else if (IsReferenceCollection(property))
                {
                    if (value is not JsonArray references || references.Count > 100)
                    {
                        throw new JsonException();
                    }

                    var elementType = property.PropertyType.GetGenericArguments()[0];
                    var resolved = new List<object>();
                    foreach (var node in references)
                    {
                        if (node is not JsonObject reference || reference.Count != 1 || !Guid.TryParse(reference["$ref"]?.GetValue<string>(), out var id))
                        {
                            throw new JsonException();
                        }

                        var target = await context.GetByIdAsync(id, elementType, cancellationToken).ConfigureAwait(false)
                            ?? throw new AdminOperationException("A referenced collection member does not exist.", AdminOperationErrorKind.InvalidInput);
                        if (resolved.Any(o => o.GetId() == id))
                        {
                            throw new JsonException();
                        }

                        resolved.Add(target);
                    }

                    converted = resolved;
                }
                else if (IsReference(property.PropertyType))
                {
                    if (value is not JsonObject reference || reference.Count != 1 || reference["$ref"] is not JsonValue idNode || !Guid.TryParse(idNode.GetValue<string>(), out var id))
                    {
                        throw new JsonException();
                    }

                    converted = await context.GetByIdAsync(id, property.PropertyType, cancellationToken).ConfigureAwait(false)
                        ?? throw new AdminOperationException("A referenced configuration object does not exist.", AdminOperationErrorKind.InvalidInput);
                }
                else
                {
                    if (property.PropertyType == typeof(LocalizedString) && (value is not JsonValue textNode || !textNode.TryGetValue<string>(out _)))
                    {
                        throw new JsonException();
                    }

                    converted = value.Deserialize(property.PropertyType, Options);
                    var scalarType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    if (scalarType.IsEnum && (converted is null || !Enum.IsDefined(scalarType, converted)))
                    {
                        throw new JsonException();
                    }

                    if ((converted is float f && !float.IsFinite(f)) || (converted is double d && !double.IsFinite(d)) || (converted is string text && text.Length > 4096) || (converted is LocalizedString localized && localized.Value?.Length > 4096))
                    {
                        throw new JsonException();
                    }
                }
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException or OverflowException)
            {
                throw new AdminOperationException("A field value does not match its type. Use config_describe and invariant enum names.", AdminOperationErrorKind.InvalidInput);
            }

            result.Add(property, converted);
        }

        return result;
    }

    /// <summary>Validates annotations on a detached candidate, so previews never dirty cached entities.</summary>
    /// <param name="candidate">The detached candidate object.</param>
    internal static void Validate(object candidate)
    {
        if (!Validator.TryValidateObject(candidate, new ValidationContext(candidate), new List<ValidationResult>(), true))
        {
            throw new AdminOperationException($"The {nameof(candidate)} violates model validation rules. Check required references and field ranges.", AdminOperationErrorKind.InvalidInput);
        }

        if ((candidate is GameConfiguration game && (game.ExperienceRate < 0 || game.MasterExperienceRate < 0))
            || (candidate is GameServerDefinition server && server.ExperienceRate < 0))
        {
            throw new AdminOperationException("Experience multipliers must be nonnegative.", AdminOperationErrorKind.InvalidInput);
        }

        if (candidate is MonsterSpawnArea spawn && spawn.Quantity < 0)
        {
            throw new AdminOperationException("Spawn quantity must be nonnegative.", AdminOperationErrorKind.InvalidInput);
        }

        if (candidate is DropItemGroup drop && (!double.IsFinite(drop.Chance) || drop.Chance is < 0 or > 1))
        {
            throw new AdminOperationException("Drop chance must be between zero and one.", AdminOperationErrorKind.InvalidInput);
        }
    }

    /// <summary>Determines whether the model field can be safely represented in a patch.</summary>
    /// <param name="type">The configuration model type.</param>
    internal static bool IsScalar(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(LocalizedString);
    }

    /// <summary>Applies validated values using public scalar setters and public collection methods.</summary>
    /// <param name="item">The configuration object.</param>
    /// <param name="values">The validated field values.</param>
    internal static void Apply(object item, IReadOnlyDictionary<PropertyInfo, object?> values)
    {
        foreach (var (property, value) in values)
        {
            if (IsReferenceCollection(property))
            {
                var collection = property.GetValue(item) ?? throw new AdminOperationException("The reference collection is not initialized.");
                property.PropertyType.GetMethod("Clear")!.Invoke(collection, null);
                foreach (var reference in (List<object>)value!)
                {
                    property.PropertyType.GetMethod("Add")!.Invoke(collection, [reference]);
                }
            }
            else
            {
                property.SetValue(item, value);
            }
        }
    }

    /// <summary>Determines whether the model field can be safely represented in a patch.</summary>
    /// <param name="property">The configuration property.</param>
    internal static bool IsReferenceCollection(PropertyInfo property) => property.PropertyType.IsGenericType
        && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
        && !property.IsDefined(typeof(MUnique.OpenMU.DataModel.Composition.MemberOfAggregateAttribute))
        && IsReference(property.PropertyType.GetGenericArguments()[0]);

    private static bool IsReference(Type type) => GameConfigurationHelper.Enumerables.ContainsKey(type) && type != typeof(PlugInConfiguration);
}
