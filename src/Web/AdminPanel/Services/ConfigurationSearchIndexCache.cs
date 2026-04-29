// <copyright file="ConfigurationSearchIndexCache.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Caches configuration search entries for fast header search navigation.
/// </summary>
public class ConfigurationSearchIndexCache
{
    private const int MaximumTraversalDepth = 20;

    private static readonly Type[] SupportedEditableTypes = GameConfigurationHelper.Enumerables.Keys.OrderByDescending(GetInheritanceDepth).ToArray();
    private static readonly ConcurrentDictionary<Type, Type?> EditableTypeByRuntimeType = new();
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> SearchablePropertiesCache = new();

    private readonly IMigratableDatabaseContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _configDataSource;
    private readonly ILogger<ConfigurationSearchIndexCache> _logger;
    private readonly SemaphoreSlim _loadingLock = new(1, 1);

    private bool _isLoaded;
    private IReadOnlyList<ConfigurationSearchEntry> _entries = Array.Empty<ConfigurationSearchEntry>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationSearchIndexCache"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="configDataSource">The configuration data source.</param>
    /// <param name="logger">The logger.</param>
    public ConfigurationSearchIndexCache(
        IMigratableDatabaseContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> configDataSource,
        ILogger<ConfigurationSearchIndexCache> logger)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._configDataSource = configDataSource;
        this._logger = logger;
    }

    /// <summary>
    /// Gets a value indicating whether the cache was loaded at least once.
    /// </summary>
    public bool IsLoaded => this._isLoaded;

    /// <summary>
    /// Gets the cached entries.
    /// </summary>
    public IReadOnlyList<ConfigurationSearchEntry> Entries => this._entries;

    /// <summary>
    /// Ensures the cache is populated.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task.</returns>
    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (this._isLoaded)
        {
            return;
        }

        await this._loadingLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (this._isLoaded)
            {
                return;
            }

            this._entries = await this.LoadEntriesAsync(cancellationToken).ConfigureAwait(false);
            this._isLoaded = true;
        }
        finally
        {
            this._loadingLock.Release();
        }
    }

    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    public void Invalidate()
    {
        this._entries = Array.Empty<ConfigurationSearchEntry>();
        this._isLoaded = false;
    }

    private static IReadOnlyList<ConfigurationSearchEntry> BuildSearchIndex(GameConfiguration gameConfiguration, Guid gameConfigurationId)
    {
        var fullTypeName = typeof(GameConfiguration).FullName;
        if (fullTypeName is null)
        {
            return Array.Empty<ConfigurationSearchEntry>();
        }

        var result = new List<ConfigurationSearchEntry>(2048);
        var uniqueKeys = new HashSet<string>(StringComparer.Ordinal);
        var rootPath = typeof(GameConfiguration).GetTypeCaption();
        var rootUrl = $"/edit-config/{fullTypeName}/{gameConfigurationId}";
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        AddSearchEntry(
            result,
            uniqueKeys,
            rootPath,
            rootPath,
            rootUrl,
            typeof(GameConfiguration).Name);
        TraverseObject(gameConfiguration, rootUrl, rootPath, visited, 0, result, uniqueKeys);
        return result;
    }

    private static void TraverseObject(
        object current,
        string currentUrl,
        string currentPath,
        HashSet<object> visited,
        int depth,
        List<ConfigurationSearchEntry> result,
        HashSet<string> uniqueKeys)
    {
        if (depth > MaximumTraversalDepth || !visited.Add(current))
        {
            return;
        }

        var type = current.GetType();
        foreach (var property in GetSearchableProperties(type))
        {
            var propertyCaption = GetPropertyCaption(type, property);
            var propertyPath = $"{currentPath} > {propertyCaption}";
            var propertyUrl = AppendSearchParameter(currentUrl, property.Name);

            AddSearchEntry(
                result,
                uniqueKeys,
                propertyCaption,
                propertyPath,
                propertyUrl,
                property.Name,
                property.PropertyType.Name,
                type.Name);

            var propertyValue = GetPropertyValue(property, current);
            if (propertyValue is null || propertyValue is byte[])
            {
                continue;
            }

            var valueType = propertyValue.GetType();
            if (IsSimpleType(valueType))
            {
                continue;
            }

            if (propertyValue is IEnumerable enumerable and not string)
            {
                TraverseCollection(enumerable, propertyPath, propertyUrl, visited, depth + 1, result, uniqueKeys);
                continue;
            }

            if (valueType.IsValueType)
            {
                continue;
            }

            var childUrl = GetEditUrlForObject(propertyValue) ?? propertyUrl;
            var childTypeCaption = valueType.GetTypeCaption();
            AddSearchEntry(
                result,
                uniqueKeys,
                childTypeCaption,
                propertyPath,
                childUrl,
                property.Name,
                valueType.Name);
            TraverseObject(propertyValue, childUrl, propertyPath, visited, depth + 1, result, uniqueKeys);
        }
    }

    private static void TraverseCollection(
        IEnumerable collection,
        string propertyPath,
        string propertyUrl,
        HashSet<object> visited,
        int depth,
        List<ConfigurationSearchEntry> result,
        HashSet<string> uniqueKeys)
    {
        var index = 0;
        foreach (var item in collection.Cast<object?>())
        {
            if (item is null)
            {
                index++;
                continue;
            }

            var itemType = item.GetType();
            var itemCaption = GetItemCaption(item, index);
            var itemPath = $"{propertyPath} > {itemCaption}";
            var itemUrl = GetEditUrlForObject(item) ?? propertyUrl;

            AddSearchEntry(
                result,
                uniqueKeys,
                itemCaption,
                itemPath,
                itemUrl,
                itemType.Name);

            if (!IsSimpleType(itemType) && !itemType.IsValueType)
            {
                TraverseObject(item, itemUrl, itemPath, visited, depth + 1, result, uniqueKeys);
            }

            index++;
        }
    }

    private static void AddSearchEntry(
        List<ConfigurationSearchEntry> result,
        HashSet<string> uniqueKeys,
        string caption,
        string path,
        string url,
        params string[] aliases)
    {
        var key = $"{path}|{url}";
        if (!uniqueKeys.Add(key))
        {
            return;
        }

        var haystack = string.Join(' ', aliases.Prepend(caption).Append(path));
        result.Add(new ConfigurationSearchEntry(caption, path, url, Normalize(haystack), Normalize(caption)));
    }

    private static string GetPropertyCaption(Type type, PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DisplayAttribute>()?.GetName()
            ?? type.GetPropertyCaption(propertyInfo.Name);
    }

    private static IReadOnlyList<PropertyInfo> GetSearchableProperties(Type type)
    {
        return SearchablePropertiesCache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(p => p.GetCustomAttribute<TransientAttribute>() is null)
                .Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true)
                .Where(p => !p.Name.StartsWith("Raw", StringComparison.Ordinal))
                .Where(p => !p.Name.StartsWith("Joined", StringComparison.Ordinal))
                .Where(p => !p.GetIndexParameters().Any())
                .ToList());
    }

    private static string GetItemCaption(object item, int index)
    {
        var typeCaption = item.GetType().GetTypeCaption();
        var name = item.GetName();
        if (!string.IsNullOrWhiteSpace(name))
        {
            return $"{typeCaption}: {name}";
        }

        var id = item.GetId();
        return id == Guid.Empty
            ? $"{typeCaption} #{index + 1}"
            : $"{typeCaption}: {id}";
    }

    private static object? GetPropertyValue(PropertyInfo propertyInfo, object instance)
    {
        try
        {
            return propertyInfo.GetValue(instance);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
               || type.IsEnum
               || type.IsValueType
               || type == typeof(string)
               || type == typeof(Guid)
               || type == typeof(Uri)
               || type == typeof(LocalizedString);
    }

    private static string? GetEditUrlForObject(object item)
    {
        if (item is MUnique.OpenMU.PlugIns.PlugInConfiguration plugInConfiguration)
        {
            return $"/plugins?id={plugInConfiguration.GetId()}";
        }

        var runtimeType = item.GetType();
        var editableType = ResolveEditableType(runtimeType);
        var fullTypeName = editableType?.FullName;
        if (fullTypeName is null)
        {
            return null;
        }

        var id = item.GetId();
        return id != Guid.Empty ? $"/edit-config/{fullTypeName}/{id}" : null;
    }

    private static Type? ResolveEditableType(Type runtimeType)
    {
        return EditableTypeByRuntimeType.GetOrAdd(runtimeType, type =>
        {
            var editableType = SupportedEditableTypes.FirstOrDefault(candidate => candidate.IsAssignableFrom(type));
            if (editableType is not null)
            {
                return editableType;
            }

            return EnumerateTypeAndBaseTypes(type)
                .FirstOrDefault(t =>
                    !t.Assembly.IsDynamic
                    && t.GetProperty(nameof(IIdentifiable.Id), BindingFlags.Instance | BindingFlags.Public) is not null);
        });
    }

    private static IEnumerable<Type> EnumerateTypeAndBaseTypes(Type type)
    {
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            yield return current;
        }
    }

    private static int GetInheritanceDepth(Type type)
    {
        var depth = 0;
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            depth++;
        }

        return depth;
    }

    private static string AppendSearchParameter(string url, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return url;
        }

        var separator = url.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        return $"{url}{separator}search={Uri.EscapeDataString(searchTerm)}";
    }

    private static string Normalize(string value)
    {
        return string.Join(
                ' ',
                value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToUpperInvariant();
    }

    private async Task<IReadOnlyList<ConfigurationSearchEntry>> LoadEntriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            if (!await this._persistenceContextProvider.CanConnectToDatabaseAsync(token).ConfigureAwait(false)
                || !await this._persistenceContextProvider.DatabaseExistsAsync(token).ConfigureAwait(false))
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            using var context = this._persistenceContextProvider.CreateNewConfigurationContext();
            var gameConfigurationId = await context.GetDefaultGameConfigurationIdAsync(token).ConfigureAwait(false);
            if (gameConfigurationId is not { } id || id == Guid.Empty)
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            var gameConfiguration = await this._configDataSource.GetOwnerAsync(id, token).ConfigureAwait(false);
            return BuildSearchIndex(gameConfiguration, id);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Could not load the configuration search index.");
            return Array.Empty<ConfigurationSearchEntry>();
        }
    }
}
