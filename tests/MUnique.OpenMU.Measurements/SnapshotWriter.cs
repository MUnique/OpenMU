// <copyright file="SnapshotWriter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Writes canonical JSON snapshots of the loaded game configuration and accounts.
/// </summary>
/// <remarks>
/// The database doesn't guarantee the order of collection items, so the snapshots are canonicalized:
/// array items are sorted by their id (or reference id). This way, two snapshots of the same data
/// are byte-wise equal, and the output of a new loader can be compared with the output of the current one.
/// </remarks>
internal static class SnapshotWriter
{
    /// <summary>
    /// Writes the snapshots into the specified folder.
    /// </summary>
    /// <param name="folder">The target folder.</param>
    /// <param name="loginNames">The login names of the accounts.</param>
    public static async ValueTask WriteAsync(string folder, IEnumerable<string> loginNames)
    {
        Directory.CreateDirectory(folder);
        var provider = new PersistenceContextProvider(NullLoggerFactory.Instance, null);

        using var configurationContext = provider.CreateNewConfigurationContext();
        var id = await configurationContext.GetDefaultGameConfigurationIdAsync(default).ConfigureAwait(false)
                 ?? throw new InvalidOperationException("No game configuration found. Run the 'init' mode first.");
        var configuration = await configurationContext.GetByIdAsync<DataModel.Configuration.GameConfiguration>(id).ConfigureAwait(false)
                            ?? throw new InvalidOperationException("Game configuration couldn't be loaded.");
        await WriteSnapshotAsync(Path.Combine(folder, "GameConfiguration.json"), ((IConvertibleTo<Persistence.BasicModel.GameConfiguration>)configuration).Convert()).ConfigureAwait(false);

        foreach (var loginName in loginNames)
        {
            using var playerContext = provider.CreateNewPlayerContext(configuration);
            var account = await playerContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false)
                          ?? throw new InvalidOperationException($"Account {loginName} not found.");
            await WriteSnapshotAsync(Path.Combine(folder, $"Account_{loginName}.json"), ((IConvertibleTo<Persistence.BasicModel.Account>)account).Convert()).ConfigureAwait(false);
        }
    }

    private static async ValueTask WriteSnapshotAsync<T>(string path, T obj)
    {
        var json = await obj.ToJsonAsync().ConfigureAwait(false);
        var canonical = Canonicalize(JsonNode.Parse(json))!.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, canonical).ConfigureAwait(false);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        Console.WriteLine($"{path}: {canonical.Length / 1024.0 / 1024.0:0.00} MB, SHA256 {hash}");
    }

    private static JsonNode? Canonicalize(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                var result = new JsonObject();
                foreach (var (key, value) in jsonObject.ToList())
                {
                    jsonObject.Remove(key);
                    result[key] = Canonicalize(value);
                }

                return result;
            case JsonArray array:
                var items = array.Select(Canonicalize).ToList();
                array.Clear();
                var sorted = new JsonArray();
                foreach (var item in items.OrderBy(GetSortKey, StringComparer.Ordinal))
                {
                    sorted.Add(item);
                }

                return sorted;
            default:
                return node?.DeepClone();
        }
    }

    private static string GetSortKey(JsonNode? node)
    {
        if (node is JsonObject jsonObject)
        {
            if (jsonObject["$id"] is JsonValue id)
            {
                return "id:" + id;
            }

            if (jsonObject["$ref"] is JsonValue reference)
            {
                return "ref:" + reference;
            }
        }

        return node?.ToJsonString() ?? string.Empty;
    }
}
