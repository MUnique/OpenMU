// <copyright file="BackupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Implementation of <see cref="IBackupService"/> which uses the available repositories
/// and does not depend on a specific persistence backend.
/// </summary>
public class BackupService : IBackupService
{
    /// <summary>
    /// The file name prefixes of the backup entries and the type of the data which they contain.
    /// The order defines in which order the entries are exported and restored - the configuration
    /// comes first, because the accounts reference its objects.
    /// </summary>
    private static readonly (string Prefix, Type BasicModelType)[] EntryTypeInfos =
    [
        ("AdminUser_", typeof(AdminAuth.AdminUser)),
        ("GameConfiguration_", typeof(BasicModel.GameConfiguration)),
        ("SystemConfiguration_", typeof(BasicModel.SystemConfiguration)),
        ("ChatServerDefinition_", typeof(BasicModel.ChatServerDefinition)),
        ("ConnectServerDefinition_", typeof(BasicModel.ConnectServerDefinition)),
        ("GameServerDefinition_", typeof(BasicModel.GameServerDefinition)),
        ("ConfigurationUpdate_", typeof(BasicModel.ConfigurationUpdate)),
        ("ConfigurationUpdateState_", typeof(BasicModel.ConfigurationUpdateState)),
        ("Account_", typeof(BasicModel.Account)),
        ("CastleSiegeData_", typeof(BasicModel.CastleSiegeData)),
    ];

    private readonly IPersistenceContextProvider _contextProvider;
    private readonly IAdminUserRepository _adminUserRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupService"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    /// <param name="adminUserRepository">The admin user repository.</param>
    public BackupService(IPersistenceContextProvider contextProvider, IAdminUserRepository adminUserRepository)
    {
        this._contextProvider = contextProvider;
        this._adminUserRepository = adminUserRepository;
    }

    /// <inheritdoc />
    public Task CreateBackupAsync(Stream outputStream, CancellationToken cancellationToken = default)
    {
        return this.CreateBackupAsync(outputStream, BackupOptions.Default, cancellationToken);
    }

    /// <inheritdoc />
    public async Task CreateBackupAsync(Stream outputStream, BackupOptions options, CancellationToken cancellationToken = default)
    {
        await using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

        // A single shared reference handler ensures cross-type references are written as $ref.
        var sharedHandler = new IdReferenceHandler();

        // Use a single context so the context stack is set up correctly for all repository calls.
        using var context = this._contextProvider.CreateNewContext();

        // First the auth users
        await this.ExportAdminUsersAsync(archive, sharedHandler, cancellationToken).ConfigureAwait(false);

        // Export in dependency order: configuration first so that accounts can reference config objects.
        await ExportAsync<GameConfiguration, BasicModel.GameConfiguration>(archive, "GameConfiguration_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<SystemConfiguration, BasicModel.SystemConfiguration>(archive, "SystemConfiguration_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<ChatServerDefinition, BasicModel.ChatServerDefinition>(archive, "ChatServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<ConnectServerDefinition, BasicModel.ConnectServerDefinition>(archive, "ConnectServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<GameServerDefinition, BasicModel.GameServerDefinition>(archive, "GameServerDefinition_", context, sharedHandler, cancellationToken).ConfigureAwait(false);

        // The applied updates are exported, too. Otherwise, they would be applied again on the restored data.
        await ExportAsync<ConfigurationUpdate, BasicModel.ConfigurationUpdate>(archive, "ConfigurationUpdate_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        await ExportAsync<ConfigurationUpdateState, BasicModel.ConfigurationUpdateState>(archive, "ConfigurationUpdateState_", context, sharedHandler, cancellationToken).ConfigureAwait(false);

        if (options.IncludeAccounts)
        {
            await ExportAsync<Account, BasicModel.Account>(archive, "Account_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
        }

        await ExportAsync<CastleSiegeData, BasicModel.CastleSiegeData>(archive, "CastleSiegeData_", context, sharedHandler, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public bool ContainsRestorableData(Stream inputStream)
    {
        var previousPosition = inputStream.Position;
        try
        {
            using var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true);
            return archive.Entries.Any(entry => GetTypeInfoForEntry(entry.Name) is not null);
        }
        catch (InvalidDataException)
        {
            return false;
        }
        finally
        {
            inputStream.Position = previousPosition;
        }
    }

    /// <inheritdoc />
    public virtual async Task RestoreBackupAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        await using var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true);

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

                await using var entryStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
                var basicModelObj = await DeserializeAsync(entryStream, typeInfo.Value.BasicModelType, sharedHandler, cancellationToken).ConfigureAwait(false);
                if (basicModelObj is null)
                {
                    continue;
                }

                if (basicModelObj is AdminUser adminUser)
                {
                    await this._adminUserRepository.AddAsync(adminUser, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                // The root object is created and saved before it's filled with its data.
                // Otherwise, a persistence layer with foreign keys (entity framework) would run into a
                // circular dependency, because objects below the root reference the root object again
                // (e.g. GameConfiguration -> GameMapDefinition -> CastleSiegeConfiguration -> GameConfiguration).
                // The data initialization does the same for the game configuration.
                var rootObject = this.CreateObject(context, basicModelObj, createdObjects);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                this.FillObject(context, basicModelObj, rootObject, createdObjects);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task ExportAdminUsersAsync(
        ZipArchive archive,
        IdReferenceHandler sharedHandler,
        CancellationToken cancellationToken)
    {
        var items = await this._adminUserRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var serializer = new JsonObjectSerializer();
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entryName = $"AdminUser_{item.Id}.json";
            var entry = archive.CreateEntry(entryName);
            await using var stream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
            await serializer.SerializeAsync(item, stream, sharedHandler, cancellationToken).ConfigureAwait(false);
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
            TBasic targetItem;
            if (item is TBasic basicItem)
            {
                targetItem = basicItem;
            }
            else if (item is IConvertibleTo<TBasic> convertible)
            {
                targetItem = convertible.Convert();
            }
            else
            {
                continue;
            }

            if (item is not IIdentifiable identifiable)
            {
                continue;
            }

            var basicModel = targetItem;
            var entryName = $"{filePrefix}{identifiable.Id}.json";
            var entry = archive.CreateEntry(entryName);
            await using var stream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
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

        if (basicModelType == typeof(BasicModel.SystemConfiguration))
        {
            return deserializer.Deserialize<BasicModel.SystemConfiguration>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.ConfigurationUpdate))
        {
            return deserializer.Deserialize<BasicModel.ConfigurationUpdate>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.ConfigurationUpdateState))
        {
            return deserializer.Deserialize<BasicModel.ConfigurationUpdateState>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.Account))
        {
            return deserializer.Deserialize<BasicModel.Account>(ms, referenceHandler);
        }

        if (basicModelType == typeof(BasicModel.CastleSiegeData))
        {
            return deserializer.Deserialize<BasicModel.CastleSiegeData>(ms, referenceHandler);
        }

        if (basicModelType == typeof(AdminUser))
        {
            return deserializer.Deserialize<AdminUser>(ms, referenceHandler);
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

    private static (string Prefix, Type BasicModelType)? GetTypeInfoForEntry(string entryName)
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

    /// <summary>
    /// Determines whether the given property just holds run-time information which is not persisted.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns><c>true</c>, if the property is marked with the <see cref="TransientAttribute"/>; otherwise, <c>false</c>.</returns>
    private static bool IsTransient(PropertyInfo property)
    {
        return property.GetCustomAttribute<TransientAttribute>() is not null;
    }

    /// <summary>
    /// Determines the Add-method of the <see cref="ICollection{T}"/>-interface which is implemented by the given collection type.
    /// We use the interface method, because the implementing type may define additional Add-methods.
    /// </summary>
    /// <param name="collectionType">The type of the collection.</param>
    /// <returns>The Add-method, if the type implements <see cref="ICollection{T}"/>; otherwise, <c>null</c>.</returns>
    private static MethodInfo? FindCollectionAddMethod(Type collectionType)
    {
        var collectionInterface = collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? collectionType
            : collectionType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

        return collectionInterface?.GetMethod("Add");
    }

    private static PropertyInfo? FindCollectionProperty(Type type, string propertyName)
    {
        var property = type.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        return property is not null
               && property.CanRead
               && property.GetIndexParameters().Length == 0
               && IsCollectionType(property.PropertyType)
            ? property
            : null;
    }

    private static PropertyInfo? FindWritableProperty(Type type, string propertyName)
    {
        var prop = type.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        return prop?.GetSetMethod(nonPublic: true) is not null ? prop : null;
    }

    private object GetOrCreateObject(IContext context, object basicModelObj, Dictionary<Guid, object> createdObjects)
    {
        if (basicModelObj is IIdentifiable identifiable && createdObjects.TryGetValue(identifiable.Id, out var existing))
        {
            return existing;
        }

        var newObj = this.CreateObject(context, basicModelObj, createdObjects);
        this.FillObject(context, basicModelObj, newObj, createdObjects);

        return newObj;
    }

    /// <summary>
    /// Creates the persistent object for the given object of the backup, without copying its data yet.
    /// </summary>
    /// <param name="context">The context on which the object is created.</param>
    /// <param name="basicModelObj">The object of the backup.</param>
    /// <param name="createdObjects">The objects which have been created so far, by their identifier.</param>
    /// <returns>The created object.</returns>
    private object CreateObject(IContext context, object basicModelObj, Dictionary<Guid, object> createdObjects)
    {
        var newObj = context.CreateNew(FindDataModelBaseType(basicModelObj.GetType()));
        if (basicModelObj is IIdentifiable identifiable)
        {
            createdObjects[identifiable.Id] = newObj;
            SetId(newObj, identifiable.Id);
        }

        return newObj;
    }

    /// <summary>
    /// Copies the data of the given object of the backup to the created persistent object.
    /// Referenced objects are created on the way, if they don't exist yet.
    /// </summary>
    /// <param name="context">The context on which referenced objects are created.</param>
    /// <param name="basicModelObj">The object of the backup.</param>
    /// <param name="target">The created object which gets the data.</param>
    /// <param name="createdObjects">The objects which have been created so far, by their identifier.</param>
    private void FillObject(IContext context, object basicModelObj, object target, Dictionary<Guid, object> createdObjects)
    {
        this.CopyProperties(basicModelObj, target, FindDataModelBaseType(basicModelObj.GetType()), context, createdObjects);
        this.CopyRawCollectionProperties(basicModelObj, target, context, createdObjects);
    }

    private void CopyProperties(
        object source,
        object target,
        Type baseType,
        IContext context,
        Dictionary<Guid, object> createdObjects)
    {
        var properties = baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var prop in properties)
        {
            if (!prop.CanRead
                || prop.GetIndexParameters().Length > 0
                || IsTransient(prop))
            {
                continue;
            }

            if (IsCollectionType(prop.PropertyType))
            {
                // Collections which have a "Raw" counterpart on the source are copied by CopyRawCollectionProperties.
                // The other ones (e.g. collections of value types, like ItemSlotType.ItemSlots) are copied here.
                if (FindCollectionProperty(source.GetType(), "Raw" + prop.Name) is null)
                {
                    this.CopyCollection(source, target, prop.Name, prop.Name, context, createdObjects);
                }

                continue;
            }

            if (prop.GetValue(source) is not { } value)
            {
                continue;
            }

            if (FindWritableProperty(target.GetType(), prop.Name) is not { } targetProp)
            {
                continue;
            }

            var targetValue = value is IIdentifiable
                ? this.GetOrCreateObject(context, value, createdObjects)
                : value;

            if (!targetProp.PropertyType.IsInstanceOfType(targetValue))
            {
                throw new InvalidOperationException(
                    $"Can't restore '{baseType.Name}.{prop.Name}': a value of type '{targetValue.GetType()}' can't be assigned to a property of type '{targetProp.PropertyType}'.");
            }

            targetProp.SetValue(target, targetValue);
        }

        // Recurse into MUnique parent base types for inherited properties.
        if (baseType.BaseType is { } parentBase
            && parentBase != typeof(object)
            && parentBase.Namespace?.StartsWith("MUnique", StringComparison.Ordinal) is true)
        {
            this.CopyProperties(source, target, parentBase, context, createdObjects);
        }
    }

    private void CopyRawCollectionProperties(
        object source,
        object target,
        IContext context,
        Dictionary<Guid, object> createdObjects)
    {
        var rawCollectionProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name.StartsWith("Raw", StringComparison.Ordinal)
                        && IsCollectionType(p.PropertyType)
                        && p.CanRead
                        && p.GetIndexParameters().Length == 0);

        foreach (var rawProp in rawCollectionProps)
        {
            this.CopyCollection(source, target, rawProp.Name, rawProp.Name["Raw".Length..], context, createdObjects);
        }
    }

    /// <summary>
    /// Copies the items of a collection of the backup object into the corresponding collection of the created object.
    /// </summary>
    /// <param name="source">The object of the backup.</param>
    /// <param name="target">The created object.</param>
    /// <param name="sourcePropertyName">The name of the collection property of the <paramref name="source"/>.</param>
    /// <param name="fallbackTargetPropertyName">
    /// The name of the collection property which is used, if the target has no property with the name of the source property.
    /// Many-to-many relations don't have a "Raw" collection on the entity framework model - it holds the join entities
    /// in a "Joined" collection instead. Adding the items to the collection of the data model creates these join entities.
    /// </param>
    /// <param name="context">The context on which referenced objects are created.</param>
    /// <param name="createdObjects">The objects which have been created so far, by their identifier.</param>
    private void CopyCollection(
        object source,
        object target,
        string sourcePropertyName,
        string fallbackTargetPropertyName,
        IContext context,
        Dictionary<Guid, object> createdObjects)
    {
        if (FindCollectionProperty(source.GetType(), sourcePropertyName)?.GetValue(source) is not System.Collections.IEnumerable sourceItems)
        {
            return;
        }

        var targetProp = FindCollectionProperty(target.GetType(), sourcePropertyName)
                         ?? FindCollectionProperty(target.GetType(), fallbackTargetPropertyName);
        if (targetProp?.GetValue(target) is not { } targetCollection)
        {
            return;
        }

        var addMethod = FindCollectionAddMethod(targetProp.PropertyType)
                        ?? throw new InvalidOperationException($"Can't restore '{source.GetType().Name}.{sourcePropertyName}': the target collection '{targetProp.PropertyType}' has no Add-method.");

        foreach (var item in sourceItems)
        {
            if (item is null)
            {
                continue;
            }

            var targetItem = item is IIdentifiable
                ? this.GetOrCreateObject(context, item, createdObjects)
                : item;

            addMethod.Invoke(targetCollection, [targetItem]);
        }
    }
}
