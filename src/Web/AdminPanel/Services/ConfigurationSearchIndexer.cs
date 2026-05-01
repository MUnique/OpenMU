// <copyright file="ConfigurationSearchIndexer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Handles the heavy lifting of building the configuration search index.
/// </summary>
internal sealed class ConfigurationSearchIndexer
{
    private const int MaximumTraversalDepth = 5;

    private static readonly Type[] SupportedEditableTypes = GameConfigurationHelper.Enumerables.Keys.OrderByDescending(GetInheritanceDepth).ToArray();

    private static readonly ConcurrentDictionary<Type, Type?> EditableTypeByRuntimeType = new();
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> SearchablePropertiesCache = new();
    private static readonly ConcurrentDictionary<(Type, PropertyInfo), string> PropertyCaptionCache = new();
    private static readonly ConcurrentDictionary<Type, string> TypeCaptionCache = new();
    private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object?>> GetterCache = new();

    private readonly StringBuilder _pathBuilder = new(512);
    private readonly StringBuilder _haystackBuilder = new(1024);
    private readonly string _rootPath;
    private readonly string _rootUrl;
    private readonly int _maxDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationSearchIndexer"/> class.
    /// </summary>
    /// <param name="rootPath">The root path.</param>
    /// <param name="rootUrl">The root URL.</param>
    /// <param name="sharedVisited">The shared visited dictionary.</param>
    /// <param name="maxDepth">The maximum depth.</param>
    private ConfigurationSearchIndexer(string rootPath, string rootUrl, ConcurrentDictionary<object, byte>? sharedVisited = null, int maxDepth = MaximumTraversalDepth)
    {
        this._rootPath = rootPath;
        this._rootUrl = rootUrl;
        this.Visited = sharedVisited ?? new ConcurrentDictionary<object, byte>(ReferenceEqualityComparer.Instance);
        this._maxDepth = maxDepth;
        this.Entries = new List<ConfigurationSearchEntry>(8192);
        this.CollectionProperties = GetSearchableProperties(typeof(GameConfiguration))
            .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string))
            .ToList();
        this.ScalarProperties = GetSearchableProperties(typeof(GameConfiguration))
            .Where(p => !typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
            .ToList();
    }

    /// <summary>
    /// Gets the shared visited dictionary.
    /// </summary>
    public ConcurrentDictionary<object, byte> Visited { get; }

    /// <summary>
    /// Gets the collected entries.
    /// </summary>
    public List<ConfigurationSearchEntry> Entries { get; }

    /// <summary>
    /// Gets the collection properties to traverse.
    /// </summary>
    public IReadOnlyList<PropertyInfo> CollectionProperties { get; }

    /// <summary>
    /// Gets the scalar properties (non-collection).
    /// </summary>
    public IReadOnlyList<PropertyInfo> ScalarProperties { get; }

    /// <summary>
    /// Builds the search index for the specified configuration.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="gameConfigurationId">The game configuration identifier.</param>
    /// <returns>The collected search entries.</returns>
    public static async Task<IReadOnlyList<ConfigurationSearchEntry>> BuildSearchIndexAsync(GameConfiguration gameConfiguration, Guid gameConfigurationId)
    {
        var fullTypeName = typeof(GameConfiguration).FullName;
        if (fullTypeName is null)
        {
            return Array.Empty<ConfigurationSearchEntry>();
        }

        var rootPath = GetCachedTypeCaption(typeof(GameConfiguration));
        var rootUrl = $"/edit-config/{fullTypeName}/{gameConfigurationId}";

        var mainIndexer = new ConfigurationSearchIndexer(rootPath, rootUrl);
        mainIndexer.Visited.TryAdd(gameConfiguration, 0);

        // Add root entry
        mainIndexer.AddEntry(rootPath, rootPath, typeof(GameConfiguration).Name);

        // Process scalar properties (shallow, no traversal)
        foreach (var property in mainIndexer.ScalarProperties)
        {
            var caption = GetPropertyCaption(typeof(GameConfiguration), property);
            mainIndexer.AddEntry(caption, rootPath, property.Name, property.PropertyType.Name, typeof(GameConfiguration).Name);
        }

        // Process collections in parallel
        var collectionTasks = mainIndexer.CollectionProperties.Select(property => Task.Run(() =>
        {
            var value = GetPropertyValue(property, gameConfiguration) as IEnumerable;
            if (value is null)
            {
                return (List<ConfigurationSearchEntry>?)null;
            }

            var propertyCaption = GetPropertyCaption(typeof(GameConfiguration), property);
            var propertyPath = $"{rootPath} > {propertyCaption}";
            var localIndexer = new ConfigurationSearchIndexer(propertyPath, rootUrl, mainIndexer.Visited);

            localIndexer.TraverseCollection(value, 1);
            return (List<ConfigurationSearchEntry>?)localIndexer.Entries;
        })).ToList();

        var taskResults = await Task.WhenAll(collectionTasks).ConfigureAwait(false);

        foreach (var results in taskResults)
        {
            if (results is not null)
            {
                mainIndexer.Entries.AddRange(results);
            }
        }

        return mainIndexer.Entries;
    }

    private static string GetPropertyCaption(Type type, PropertyInfo propertyInfo)
    {
        return PropertyCaptionCache.GetOrAdd((type, propertyInfo), _ =>
            propertyInfo.GetCustomAttribute<DisplayAttribute>()?.GetName()
            ?? type.GetPropertyCaption(propertyInfo.Name));
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
        var typeCaption = GetCachedTypeCaption(item.GetType());
        var name = item.GetName();
        if (!string.IsNullOrWhiteSpace(name))
        {
            return $"{typeCaption}: {name}";
        }

        var id = item.GetId();
        return id == Guid.Empty
            ? $"{typeCaption}: #{index + 1}"
            : $"{typeCaption}: {id}";
    }

    private static string GetCachedTypeCaption(Type type)
    {
        return TypeCaptionCache.GetOrAdd(type, t => t.GetTypeCaption());
    }

    private static object? GetPropertyValue(PropertyInfo propertyInfo, object instance)
    {
        var getter = GetterCache.GetOrAdd(propertyInfo, CreateGetter);
        return getter(instance);
    }

    private static Func<object, object?> CreateGetter(PropertyInfo propertyInfo)
    {
        var getter = propertyInfo.GetGetMethod();
        if (getter is null)
        {
            return _ => null;
        }

        if (!propertyInfo.DeclaringType!.IsValueType && !propertyInfo.PropertyType.IsValueType)
        {
            try
            {
                return (Func<object, object?>)Delegate.CreateDelegate(typeof(Func<object, object?>), null, getter);
            }
            catch
            {
                // Fall through to GetValue
            }
        }

        return instance => propertyInfo.GetValue(instance);
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

    private static string AppendSearchParameter(string url, string searchTerm, bool hasParameters)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return url;
        }

        var separator = hasParameters ? "&" : "?";
        return $"{url}{separator}search={Uri.EscapeDataString(searchTerm)}";
    }

    private void AddEntry(string caption, string path, params string[] aliases)
    {
        this.AddEntry(caption, path, string.Empty, aliases);
    }

    private void AddEntry(string caption, string path, string url, params string[] aliases)
    {
        this._haystackBuilder.Clear();
        this._haystackBuilder.Append(caption);
        foreach (var alias in aliases)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                this._haystackBuilder.Append(' ');
                this._haystackBuilder.Append(alias);
            }
        }

        this._haystackBuilder.Append(' ');
        this._haystackBuilder.Append(path);

        this.Entries.Add(new ConfigurationSearchEntry(caption, path, url, this._haystackBuilder.ToString(), caption));
    }

    private void TraverseCollection(IEnumerable collection, int depth)
    {
        var index = 0;
        foreach (var item in collection)
        {
            if (item is null)
            {
                index++;
                continue;
            }

            var itemType = item.GetType();
            var itemCaption = GetItemCaption(item, index);
            var itemPath = this.BuildPath(this._rootPath, itemCaption);
            var itemUrl = GetEditUrlForObject(item) ?? this._rootUrl;

            this.AddEntry(itemCaption, itemPath, itemUrl, itemType.Name);

            if (!IsSimpleType(itemType) && !itemType.IsValueType)
            {
                this.TraverseObject(item, itemUrl, itemPath, depth + 1);
            }

            index++;
        }
    }

    private void TraverseObject(object current, string currentUrl, string currentPath, int depth)
    {
        if (depth > this._maxDepth || !this.Visited.TryAdd(current, 0))
        {
            return;
        }

        var type = current.GetType();
        var hasParams = currentUrl.Contains('?', StringComparison.Ordinal);

        foreach (var property in GetSearchableProperties(type))
        {
            var propertyCaption = GetPropertyCaption(type, property);
            var propertyPath = this.BuildPath(currentPath, propertyCaption);
            var propertyValue = GetPropertyValue(property, current);
            var valueType = propertyValue?.GetType();

            var isNavigable = propertyValue is not null
                && !IsSimpleType(valueType!)
                && (propertyValue is IEnumerable || !valueType!.IsValueType);

            var propertyUrl = isNavigable
                ? AppendSearchParameter(currentUrl, property.Name, hasParams)
                : string.Empty;

            this.AddEntry(
                propertyCaption,
                propertyPath,
                propertyUrl,
                property.Name,
                property.PropertyType.Name,
                type.Name);

            if (propertyValue is null || propertyValue is byte[] || !isNavigable)
            {
                continue;
            }

            if (propertyValue is IEnumerable enumerable and not string)
            {
                this.TraverseCollection(enumerable, depth + 1);
                continue;
            }

            if (valueType!.IsValueType)
            {
                continue;
            }

            var childUrl = GetEditUrlForObject(propertyValue) ?? propertyUrl;
            var childTypeCaption = GetCachedTypeCaption(valueType);
            this.AddEntry(childTypeCaption, propertyPath, childUrl, property.Name, valueType.Name);
            this.TraverseObject(propertyValue, childUrl, propertyPath, depth + 1);
        }
    }

    private string BuildPath(string parent, string child)
    {
        this._pathBuilder.Clear();
        this._pathBuilder.Append(parent);
        this._pathBuilder.Append(" > ");
        this._pathBuilder.Append(child);
        return this._pathBuilder.ToString();
    }
}
