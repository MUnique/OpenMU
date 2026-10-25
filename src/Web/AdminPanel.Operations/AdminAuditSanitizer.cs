// <copyright file="AdminAuditSanitizer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using MUnique.OpenMU.PlugIns;

/// <summary>Removes credential fields from already explicit audit projections as defense in depth.</summary>
internal static class AdminAuditSanitizer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { ExcludeSecrets } }, Converters = { new JsonStringEnumConverter() } };
    private static readonly string[] SecretNames = ["password", "secret", "token", "securitycode", "keyhash", "authorization", "cookie", "credential", "recoverycode", "apikey", "privatekey"];

    /// <summary>Serializes the explicit projection and redacts credential fields recursively.</summary>
    /// <param name="value">The explicit argument or result projection.</param>
    /// <returns>Sanitized JSON.</returns>
    internal static string Serialize(object? value)
    {
        var node = JsonSerializer.SerializeToNode(value, Options);
        Redact(node);
        return node?.ToJsonString() ?? "null";
    }

    private static void ExcludeSecrets(JsonTypeInfo type)
    {
        foreach (var property in type.Properties.Where(p => p.AttributeProvider?.IsDefined(typeof(SecretAttribute), true) is true))
        {
            property.ShouldSerialize = (_, _) => false;
        }
    }

    private static void Redact(JsonNode? node)
    {
        if (node is JsonObject item)
        {
            foreach (var name in item.Select(p => p.Key).ToArray())
            {
                if (SecretNames.Any(secret => name.Contains(secret, StringComparison.OrdinalIgnoreCase))
                    && (item[name] is not JsonValue scalar || scalar.TryGetValue<string>(out _) || SecretNames.Contains(name, StringComparer.OrdinalIgnoreCase)))
                {
                    item[name] = "[redacted]";
                }
                else
                {
                    Redact(item[name]);
                }
            }
        }
        else if (node is JsonArray array)
        {
            foreach (var child in array)
            {
                Redact(child);
            }
        }
        else
        {
            // Scalar values in explicit projections have no credential fields to traverse.
        }
    }
}
