// <copyright file="BackupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Implementation of <see cref="IBackupService"/> which uses the available repositories
/// and does not depend on a specific persistence backend.
/// </summary>
public class BackupService : IBackupService
{
    private static readonly (string Prefix, Type DataModelType, Type BasicModelType)[] EntryTypeInfos =
    [
        ("GameConfiguration_", typeof(GameConfiguration), typeof(BasicModel.GameConfiguration)),
        ("ChatServerDefinition_", typeof(ChatServerDefinition), typeof(BasicModel.ChatServerDefinition)),
        ("ConnectServerDefinition_", typeof(ConnectServerDefinition), typeof(BasicModel.ConnectServerDefinition)),
        ("GameServerDefinition_", typeof(GameServerDefinition), typeof(BasicModel.GameServerDefinition)),
        ("Account_", typeof(Account), typeof(BasicModel.Account)),
    ];

    private readonly IPersistenceContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupService"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    public BackupService(IPersistenceContextProvider contextProvider)
    {
        this._contextProvider = contextProvider;
    }

    /// <inheritdoc />
    public async Task CreateBackupAsync(Stream outputStream, CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

        // A single shared reference handler ensures cross-type references are written as $ref.
        var sharedHandler = new IdReferenceHandler();

        // Use a single context so the context stack is set up correctly for all repository calls.
        using var context = this._contextProvider.CreateNewContext();

        // Export in dependency order: configuration first so that accounts can reference config objects.
        await ExportAsync<GameConfiguration, BasicModel.GameConfiguration>(archive, "GameConfiguration_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<ChatServerDefinition, BasicModel.ChatServerDefinition>(archive, "ChatServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<ConnectServerDefinition, BasicModel.ConnectServerDefinition>(archive, "ConnectServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<GameServerDefinition, BasicModel.GameServerDefinition>(archive, "GameServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<Account, BasicModel.Account>(archive, "Account_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task RestoreBackupAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true);

        // A single shared handler accumulates deserialized objects so cross-file $ref references resolve correctly.
        var sharedHandler = new IdReferenceHandler();
        var createdObjects = new Dictionary<Guid, object>();

        // Sort entries so GameConfiguration is processed first (other types reference its sub-objects).
        var orderedEntries = archive.Entries
            .OrderBy(e => GetTypeOrder(e.Name))
            .ThenBy(e => e.Name)
            .ToList();

        using var context = this._contextProvider.CreateNewContext();
        using (context.SuspendChangeNotifications())
        {
            foreach (var entry in orderedEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var typeInfo = GetTypeInfoForEntry(entry.Name);
                if (typeInfo is null)
                {
                    continue;
                }

                await using var entryStream = entry.Open();
                var basicModelObj = await DeserializeAsync(entryStream, typeInfo.Value.BasicModelType, sharedHandler, cancellationToken).ConfigureAwait(false);
                if (basicModelObj is null)
                {
                    continue;
                }

                this.GetOrCreateObject(context, basicModelObj, createdObjects);
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task ExportAsync<TData, TBasic>(
        ZipArchive archive,
        string filePrefix,
        IContext context,
        IdReferenceHandler sharedHandler,
        CancellationToken cancellationToken)
        where TData : class
        where TBasic : class
    {
        var items = await context.GetAsync<TData>(cancellationToken).ConfigureAwait(false);
        var serializer = new JsonObjectSerializer();
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item is not IConvertibleTo<TBasic> convertible)
            {
                continue;
            }

            if (item is not IIdentifiable identifiable)
            {
                continue;
            }

            var basicModel = convertible.Convert();
            var entryName = $"{filePrefix}{identifiable.Id}.json";
            var entry = archive.CreateEntry(entryName);
            await using var stream = entry.Open();
            await serializer.SerializeAsync(basicModel, stream, sharedHandler, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task<object?> DeserializeAsync(
        Stream stream,
        Type basicModelType,
        IdReferenceHandler referenceHandler,
        CancellationToken cancellationToken)
    {
        // Read to memory first because ZipArchive entry streams don't support seeking.
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
        ms.Position = 0;

        var deserializer = new JsonObjectDeserializer();

        if (basicModelType == typeof(BasicModel.GameConfiguration))
        {
            return deserializer.Deserialize<BasicModel.GameConfiguration>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.ChatServerDefinition))
        {
            return deserializer.Deserialize<BasicModel.ChatServerDefinition>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.ConnectServerDefinition))
        {
            return deserializer.Deserialize<BasicModel.ConnectServerDefinition>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.GameServerDefinition))
        {
            return deserializer.Deserialize<BasicModel.GameServerDefinition>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.Account))
        {
            return deserializer.Deserialize<BasicModel.Account>(ms, referenceHandler);
        }

        throw new ArgumentException($"Unsupported backup entry type: {basicModelType}", nameof(basicModelType));
    }

    private static int GetTypeOrder(string entryName)
    {
        for (var i = 0; i < EntryTypeInfos.Length; i++)
        {
            if (entryName.StartsWith(EntryTypeInfos[i].Prefix, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return EntryTypeInfos.Length;
    }

    private static (string Prefix, Type DataModelType, Type BasicModelType)? GetTypeInfoForEntry(string entryName)
    {
        foreach (var typeInfo in EntryTypeInfos)
        {
            if (entryName.StartsWith(typeInfo.Prefix, StringComparison.Ordinal))
            {
                return typeInfo;
            }
        }

        return null;
    }

    private object GetOrCreateObject(IContext context, object basicModelObj, Dictionary<Guid, object> createdObjects)
    {
        if (basicModelObj is IIdentifiable identifiable && createdObjects.TryGetValue(identifiable.Id, out var existing))
        {
            return existing;
        }

        var dataModelBaseType = FindDataModelBaseType(basicModelObj.GetType());
        var newObj = context.CreateNew(dataModelBaseType);

        if (basicModelObj is IIdentifiable id2)
        {
            createdObjects[id2.Id] = newObj;
            SetId(newObj, id2.Id);
        }

        this.CopyBaseTypeProperties(basicModelObj, newObj, dataModelBaseType, context, createdObjects);
        this.CopyRawCollectionProperties(basicModelObj, newObj, context, createdObjects);

        return newObj;
    }

    private static Type FindDataModelBaseType(Type basicModelType)
    {
        var current = basicModelType.BaseType;
        while (current != null && current != typeof(object))
        {
            if (current.Assembly != basicModelType.Assembly
                && current.Assembly != typeof(object).Assembly)
            {
                return current;
            }

            current = current.BaseType;
        }

        return basicModelType;
    }

    private static void SetId(object obj, Guid id)
    {
        var idProp = obj.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        idProp?.SetValue(obj, id);
    }

    private void CopyBaseTypeProperties(
        object source,
        object target,
        Type baseType,
        IContext context,
        Dictionary<Guid, object> createdObjects)
    {
        var properties = baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var prop in properties)
        {
            if (!prop.CanRead || IsCollectionType(prop.PropertyType))
            {
                continue;
            }

            object? value;
            try
            {
                value = prop.GetValue(source);
            }
            catch
            {
                continue;
            }

            if (value is null)
            {
                continue;
            }

            var targetProp = FindWritableProperty(target.GetType(), prop.Name);
            if (targetProp is null)
            {
                continue;
            }

            if (value is IIdentifiable)
            {
                var child = this.GetOrCreateObject(context, value, createdObjects);
                try
                {
                    targetProp.SetValue(target, child);
                }
                catch
                {
                    // Ignore type incompatibilities.
                }
            }
            else
            {
                try
                {
                    targetProp.SetValue(target, value);
                }
                catch
                {
                    // Ignore.
                }
            }
        }

        // Recurse into MUnique parent base types for inherited properties.
        if (baseType.BaseType is { } parentBase
            && parentBase != typeof(object)
            && parentBase.Namespace?.StartsWith("MUnique", StringComparison.Ordinal) is true)
        {
            this.CopyBaseTypeProperties(source, target, parentBase, context, createdObjects);
        }
    }

    private void CopyRawCollectionProperties(
        object source,
        object target,
        IContext context,
        Dictionary<Guid, object> createdObjects)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        var rawCollectionProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name.StartsWith("Raw", StringComparison.Ordinal)
                        && IsCollectionType(p.PropertyType)
                        && p.CanRead);

        foreach (var rawProp in rawCollectionProps)
        {
            object? sourceCollection;
            try
            {
                sourceCollection = rawProp.GetValue(source);
            }
            catch
            {
                continue;
            }

            if (sourceCollection is not System.Collections.IEnumerable sourceEnumerable)
            {
                continue;
            }

            var targetProp = targetType.GetProperty(
                rawProp.Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (targetProp is null)
            {
                continue;
            }

            object? targetCollection;
            try
            {
                targetCollection = targetProp.GetValue(target);
            }
            catch
            {
                continue;
            }

            if (targetCollection is null)
            {
                continue;
            }

            var addMethod = targetCollection.GetType().GetMethod("Add");
            if (addMethod is null)
            {
                continue;
            }

            foreach (var item in sourceEnumerable)
            {
                if (item is null)
                {
                    continue;
                }

                try
                {
                    if (item is IIdentifiable)
                    {
                        var createdItem = this.GetOrCreateObject(context, item, createdObjects);
                        addMethod.Invoke(targetCollection, new[] { createdItem });
                    }
                    else
                    {
                        addMethod.Invoke(targetCollection, new[] { item });
                    }
                }
                catch
                {
                    // Ignore individual item errors.
                }
            }
        }
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string) || type.IsArray)
        {
            return false;
        }

        return type.IsGenericType
            && (type.GetGenericTypeDefinition() == typeof(ICollection<>)
                || type.GetGenericTypeDefinition() == typeof(IList<>)
                || type.GetGenericTypeDefinition() == typeof(List<>));
    }

    private static PropertyInfo? FindWritableProperty(Type type, string propertyName)
    {
        var prop = type.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        return prop?.GetSetMethod(nonPublic: true) is not null ? prop : null;
    }
}
