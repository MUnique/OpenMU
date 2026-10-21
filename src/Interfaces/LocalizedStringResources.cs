// <copyright file="LocalizedStringResources.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Resources;

/// <summary>
/// Registry of resources which can be the source of a <see cref="LocalizedString"/>.
/// A <see cref="LocalizedString.SourceKey"/> has the format "{source name}/{resource key}", e.g. "MapNames/Lorencia".
/// Only registered sources are resolved, so a source key stored in the database can never
/// load arbitrary types or resources.
/// </summary>
public static class LocalizedStringResources
{
    private static readonly ConcurrentDictionary<string, ResourceManager> ResourceManagersByName = new(StringComparer.Ordinal);

    private static readonly ConcurrentDictionary<ResourceManager, string> NamesByResourceManager = new();

    /// <summary>
    /// Gets the separator between the source name and the resource key.
    /// </summary>
    public static char KeySeparator => '/';

    /// <summary>
    /// Registers a resource source.
    /// </summary>
    /// <param name="name">The unique name of the source, e.g. "MapNames".</param>
    /// <param name="resourceManager">The resource manager.</param>
    /// <exception cref="ArgumentException">The name is invalid.</exception>
    /// <exception cref="InvalidOperationException">The name is already registered for another resource manager.</exception>
    public static void Register(string name, ResourceManager resourceManager)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(resourceManager);
        if (name.Contains(KeySeparator, StringComparison.Ordinal) || name.Contains('|', StringComparison.Ordinal))
        {
            throw new ArgumentException($"The source name must not contain '{KeySeparator}' or '|'.", nameof(name));
        }

        var registered = ResourceManagersByName.GetOrAdd(name, resourceManager);
        if (!ReferenceEquals(registered, resourceManager))
        {
            throw new InvalidOperationException($"The localization source name '{name}' is already registered for another resource.");
        }

        NamesByResourceManager.TryAdd(resourceManager, name);
    }

    /// <summary>
    /// Tries to get the registered name of a resource manager.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="name">The registered name.</param>
    /// <returns><see langword="true"/>, if the resource manager is registered.</returns>
    public static bool TryGetName(ResourceManager resourceManager, [NotNullWhen(true)] out string? name)
    {
        return NamesByResourceManager.TryGetValue(resourceManager, out name);
    }

    /// <summary>
    /// Creates the source key for an entry of a registered resource.
    /// </summary>
    /// <param name="sourceName">The registered source name.</param>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>The source key.</returns>
    public static string CreateSourceKey(string sourceName, string resourceKey)
    {
        return sourceName + KeySeparator + resourceKey;
    }

    /// <summary>
    /// Tries to resolve a source key to a registered resource manager and its resource key.
    /// </summary>
    /// <param name="sourceKey">The source key, e.g. "MapNames/Lorencia".</param>
    /// <param name="resourceManager">The resource manager of the source.</param>
    /// <param name="resourceKey">The resource key within the source.</param>
    /// <returns><see langword="true"/>, if the source is registered.</returns>
    public static bool TryResolve(string? sourceKey, [NotNullWhen(true)] out ResourceManager? resourceManager, [NotNullWhen(true)] out string? resourceKey)
    {
        resourceManager = null;
        resourceKey = null;
        var separatorIndex = sourceKey?.IndexOf(KeySeparator, StringComparison.Ordinal) ?? -1;
        if (separatorIndex <= 0 || separatorIndex == sourceKey!.Length - 1)
        {
            return false;
        }

        if (!ResourceManagersByName.TryGetValue(sourceKey[..separatorIndex], out resourceManager))
        {
            return false;
        }

        resourceKey = sourceKey[(separatorIndex + 1)..];
        return true;
    }
}
