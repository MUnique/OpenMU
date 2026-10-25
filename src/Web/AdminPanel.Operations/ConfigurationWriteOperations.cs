// <copyright file="ConfigurationWriteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Previews and applies bounded configuration patches using the existing persistence notification path.</summary>
/// <param name="persistence">The persistence provider.</param>
/// <param name="configurations">The active configuration source.</param>
/// <param name="executor">The audit and authorization executor.</param>
/// <param name="confirmations">The shared preview coordinator.</param>
/// <param name="settings">The write safety policy.</param>
/// <param name="audit">The durable history.</param>
/// <param name="plugins">The shared plugin change service.</param>
public sealed class ConfigurationWriteOperations(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations, AdminOperationExecutor executor, AdminChangeConfirmation confirmations, AdminWriteSettings settings, IAdminAuditRepository audit, PluginWriteOperations plugins)
{
    /// <summary>Merge-patches scalar fields or explicit references of one configuration object.</summary>
    /// <param name="type">The configuration type.</param>
    /// <param name="id">The object identifier.</param>
    /// <param name="patch">A bounded JSON merge patch.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview or applied change, including before and after values.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public ValueTask<JsonObject> UpdateAsync(string type, Guid id, string patch, bool apply, string? confirmationToken, CancellationToken cancellationToken)
        => this.ChangeAsync("config_update", "update", type, id, null, null, null, patch, apply, confirmationToken, cancellationToken);

    /// <summary>Creates a new child in an aggregate collection; the identifier is assigned by the server.</summary>
    /// <param name="parentType">The parent type.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="collection">The aggregate collection field.</param>
    /// <param name="values">Initial scalar and reference values.</param>
    /// <param name="apply">Whether to apply the preview.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview or applied creation.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public ValueTask<JsonObject> CreateAsync(string parentType, Guid parentId, string collection, string values, bool apply, string? confirmationToken, CancellationToken cancellationToken)
        => this.ChangeAsync("config_create", "create", string.Empty, Guid.Empty, parentType, parentId, collection, values, apply, confirmationToken, cancellationToken);

    /// <summary>Deletes an unreferenced leaf child from its aggregate collection.</summary>
    /// <param name="parentType">The owning parent type.</param>
    /// <param name="parentId">The owning parent identifier.</param>
    /// <param name="collection">The owning aggregate collection.</param>
    /// <param name="id">The child identifier.</param>
    /// <param name="apply">Whether to apply the preview.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview or applied deletion.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public ValueTask<JsonObject> DeleteAsync(string parentType, Guid parentId, string collection, Guid id, bool apply, string? confirmationToken, CancellationToken cancellationToken)
        => this.ChangeAsync("config_delete", "delete", string.Empty, id, parentType, parentId, collection, "{}", apply, confirmationToken, cancellationToken);

    /// <summary>Previews restoration of an audited update's before-values, refusing intervening changes.</summary>
    /// <param name="auditId">The successful configuration update to reverse.</param>
    /// <param name="apply">Whether to apply the reversal.</param>
    /// <param name="confirmationToken">The reversal preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview or applied reversal.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public async ValueTask<JsonObject> RevertAsync(Guid auditId, bool apply, string? confirmationToken, CancellationToken cancellationToken)
    {
        await AdminChangeExecution.RequireAsync(executor, settings, AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
        var entry = await audit.GetAsync(auditId, cancellationToken).ConfigureAwait(false);
        if (entry is not { Outcome: "Succeeded" } || entry.Operation is not ("config_update" or "config_create" or "config_delete" or "plugins_set_active" or "plugins_update_configuration" or "rates_set" or "drops_update" or "spawns_update" or "audit_revert"))
        {
            throw new AdminOperationException("Only successful configuration changes can be reverted. Operational actions cannot be reverted.");
        }

        var change = JsonNode.Parse(entry.ResultJson)?["change"] as JsonObject;
        if (change?["kind"]?.GetValue<string>() == "moneyRate")
        {
            return await this.UpdateMoneyRateAsync(change["before"]!.GetValue<float>(), apply, confirmationToken, cancellationToken, change["after"]!.GetValue<float>(), auditId, change["stateIdentity"]?.GetValue<string>()).ConfigureAwait(false);
        }

        if (change?["kind"]?.GetValue<string>() == "plugin")
        {
            return await plugins.RevertFromAuditAsync(entry, apply, confirmationToken, cancellationToken).ConfigureAwait(false);
        }

        if (change is null || change["type"]?.GetValue<string>() is not { } type || !Guid.TryParse(change["id"]?.GetValue<string>(), out var id))
        {
            throw new AdminOperationException("This audit entry has no complete reversible configuration diff.");
        }

        var before = change["before"] as JsonObject;
        var after = change["after"] as JsonObject;
        var kind = change["kind"]?.GetValue<string>();
        var parentType = change["parentType"]?.GetValue<string>();
        var parentId = Guid.TryParse(change["parentId"]?.GetValue<string>(), out var owner) ? owner : (Guid?)null;
        var collection = change["collection"]?.GetValue<string>();
        if (kind == "create" && after is not null)
        {
            return await this.ChangeAsync("audit_revert", "delete", type, id, parentType, parentId, collection, "{}", apply, confirmationToken, cancellationToken, after, auditId).ConfigureAwait(false);
        }

        if (kind == "delete" && before is not null)
        {
            return await this.ChangeAsync("audit_revert", "create", type, id, parentType, parentId, collection, before.ToJsonString(), apply, confirmationToken, cancellationToken, null, auditId).ConfigureAwait(false);
        }

        if (kind != "update" || before is null || after is null)
        {
            throw new AdminOperationException("This entry cannot be reverted.");
        }

        return await this.ChangeAsync("audit_revert", "update", type, id, null, null, null, before.ToJsonString(), apply, confirmationToken, cancellationToken, after, auditId, change["afterFingerprint"]?.GetValue<string>()).ConfigureAwait(false);
    }

    /// <summary>Runs task-oriented updates with their own operation name and confirmation binding.</summary>
    /// <param name="operation">The invariant operation name.</param>
    /// <param name="type">The configuration model type.</param>
    /// <param name="id">The persistent object identifier.</param>
    /// <param name="patch">The JSON merge patch.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="token">The single-use confirmation token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal ValueTask<JsonObject> UpdateTaskAsync(string operation, string type, Guid id, string patch, bool apply, string? token, CancellationToken cancellationToken)
        => this.ChangeAsync(operation, "update", type, id, null, null, null, patch, apply, token, cancellationToken);

    /// <summary>Changes the immutable configured money amount attribute through its assignment seam.</summary>
    /// <param name="value">The finite nonnegative money multiplier.</param>
    /// <param name="apply">Whether to apply the preview.</param>
    /// <param name="token">The confirmation token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="expected">The expected current value when reverting.</param>
    /// <param name="revertedAuditId">The original audit identifier for a reversal.</param>
    /// <param name="expectedIdentity">The expected attribute identity and aggregation mode for a reversal.</param>
    /// <returns>The preview or applied money rate.</returns>
    internal async ValueTask<JsonObject> UpdateMoneyRateAsync(float value, bool apply, string? token, CancellationToken cancellationToken, float? expected = null, Guid? revertedAuditId = null, string? expectedIdentity = null)
    {
        var actor = await AdminChangeExecution.RequireAsync(executor, settings, AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
        if (!float.IsFinite(value) || value < 0)
        {
            throw new AdminOperationException("Use a finite nonnegative money amount multiplier.", AdminOperationErrorKind.InvalidInput);
        }

        using var gate = apply ? await confirmations.Gate.LockAsync(cancellationToken).ConfigureAwait(false) : null;
        using var context = persistence.CreateNewContext();
        if (apply && context is IConfigurationEditContext editContext)
        {
            await editContext.BeginConfigurationEditAsync(cancellationToken).ConfigureAwait(false);
        }

        var id = (await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).GetId();
        var config = await context.GetByIdAsync<GameConfiguration>(id, cancellationToken).ConfigureAwait(false)
            ?? throw new AdminOperationException("The active configuration could not be loaded.");
        var matches = config.GlobalBaseAttributeValues.Where(a => a.Definition.Id == Stats.MoneyAmountRate.Id).ToArray();
        if (matches.Length != 1 || matches[0] is not IAssignable assignable)
        {
            throw new AdminOperationException("Expected one configured global MoneyAmountRate attribute. Configure it in the panel first.");
        }

        var attribute = matches[0];
        var stateIdentity = $"{attribute.GetId():D}:{attribute.AggregateType}";
        if ((expected is { } previous && attribute.Value != previous) || (expectedIdentity is not null && expectedIdentity != stateIdentity))
        {
            throw new AdminOperationException("The money rate changed after the audited operation. Revert refused.");
        }

        var candidate = new ConstValueAttribute(value, attribute.Definition, attribute.AggregateType);
        var change = new JsonObject { ["kind"] = "moneyRate", ["id"] = attribute.GetId().ToString(), ["stateIdentity"] = stateIdentity, ["before"] = attribute.Value, ["after"] = value };
        return await AdminChangeExecution.RunAsync(
            executor,
            confirmations,
            actor,
            AdminRoles.Administrator,
            revertedAuditId is null ? "rates_set" : "audit_revert",
            new { value, revertedAuditId },
            change,
            apply,
            token,
            async () =>
            {
                assignable.AssignValuesOf(candidate, config);
                await AdminChangeExecution.SaveAsync(context, cancellationToken).ConfigureAwait(false);
                await configurations.InvalidateAsync().ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
    }

    private static void ValidateReferences(GameConfiguration config, IReadOnlyDictionary<PropertyInfo, object?> values)
    {
        foreach (var (property, value) in values.Where(p => p.Value is not null && !ConfigurationPatch.IsScalar(p.Key.PropertyType)))
        {
            var type = ConfigurationPatch.IsReferenceCollection(property) ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;
            var allowed = GameConfigurationHelper.Enumerables[type](config).Cast<object>().Select(o => o.GetId()).ToHashSet();
            var references = value is List<object> list ? list : [value!];
            if (references.Any(o => !allowed.Contains(o.GetId())))
            {
                throw new AdminOperationException("References must belong to the active game configuration.", AdminOperationErrorKind.InvalidInput);
            }
        }
    }

    private static string Fingerprint(JsonObject? value, object? item, Type type)
    {
        if (value is null)
        {
            return Convert.ToHexString(SHA256.HashData("null"u8));
        }

        var state = value.DeepClone().AsObject();
        foreach (var property in type.GetProperties().Where(p => p.CanRead && p.GetIndexParameters().Length == 0).OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            var field = item is null ? null : property.GetValue(item);
            if (field is byte[] bytes)
            {
                state["$binary:" + property.Name] = Convert.ToHexString(SHA256.HashData(bytes));
            }
            else if (property.IsDefined(typeof(MemberOfAggregateAttribute)) && field is IEnumerable children)
            {
                state["$children:" + property.Name] = new JsonArray(children.Cast<object>().Select(c => c.GetId().ToString()).Order(StringComparer.Ordinal).Select(id => (JsonNode)JsonValue.Create(id)!).ToArray());
            }
            else
            {
                // Scalar/reference fields are projected already; other shapes are outside the editable surface.
            }
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(state.ToJsonString())));
    }

    private static JsonObject SelectFields(JsonObject snapshot, IEnumerable<PropertyInfo> fields)
    {
        var result = new JsonObject();
        foreach (var field in fields.OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            result[field.Name] = snapshot[field.Name]?.DeepClone();
        }

        return result;
    }

    private static object Candidate(object? item, Type type, Dictionary<PropertyInfo, object?> values)
    {
        var candidate = Activator.CreateInstance(type) ?? throw new AdminOperationException("This configuration type cannot be constructed.");
        if (item is not null)
        {
            foreach (var property in type.GetProperties().Where(p => p.CanRead && p.SetMethod?.IsPublic == true && p.GetIndexParameters().Length == 0))
            {
                property.SetValue(candidate, property.GetValue(item));
            }
        }

        foreach (var (property, value) in values.Where(v => !ConfigurationPatch.IsReferenceCollection(v.Key)))
        {
            property.SetValue(candidate, value);
        }

        ConfigurationPatch.Validate(candidate);
        return candidate;
    }

    private static object Find(GameConfiguration config, Type type, Guid id)
        => GameConfigurationHelper.Enumerables[type](config).Cast<object>().FirstOrDefault(o => o.GetId() == id)
           ?? throw new AdminOperationException("The configuration object is not part of the active game configuration.", AdminOperationErrorKind.InvalidInput);

    private static (PropertyInfo Property, Type ChildType) Collection(Type type, string name)
    {
        var property = type.GetProperty(name);
        if (property is null || !property.IsDefined(typeof(MemberOfAggregateAttribute)) || !property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
        {
            throw new AdminOperationException("Choose an aggregate ICollection field from config_describe.", AdminOperationErrorKind.InvalidInput);
        }

        var childType = property.PropertyType.GetGenericArguments()[0];
        ConfigurationPatch.Resolve(childType.Name);
        return (property, childType);
    }

    private static void CheckDeletion(GameConfiguration config, object item, object parent, PropertyInfo owningCollection, Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new AdminOperationException("This immutable configuration type has no safe reconstruction path. Use the panel to manage it.");
        }

        var restorable = ConfigurationPatch.Fields(type).Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        if (type.GetProperties().Any(p => p.SetMethod?.IsPublic == true && p.Name != "Id" && !restorable.Contains(p.Name) && p.GetValue(item) is not null))
        {
            throw new AdminOperationException("The object contains fields outside the safe snapshot. Use the panel to manage it; deleting it here would lose data needed for reversal.");
        }

        if (type.GetProperties().Where(p => p.IsDefined(typeof(MemberOfAggregateAttribute))).Any(p => p.GetValue(item) is IEnumerable children ? children.Cast<object>().Any() : p.GetValue(item) is not null))
        {
            throw new AdminOperationException("The object owns child objects. Remove them in separate reviewed steps before deleting it.");
        }

        foreach (var (sourceType, enumerate) in GameConfigurationHelper.Enumerables)
        {
            foreach (var source in enumerate(config).Cast<object>())
            {
                foreach (var property in sourceType.GetProperties().Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && !ConfigurationPatch.IsScalar(p.PropertyType)))
                {
                    if ((ReferenceEquals(source, parent) && property.Name == owningCollection.Name) || ReferenceEquals(source, item))
                    {
                        continue;
                    }

                    var value = property.GetValue(source);
                    if (ReferenceEquals(value, item) || (value is IEnumerable references && references.Cast<object>().Any(v => ReferenceEquals(v, item))))
                    {
                        throw new AdminOperationException("The object is still referenced by another configuration object. Remove the reference before deleting it.");
                    }
                }
            }
        }
    }

    private async ValueTask<JsonObject> ChangeAsync(string operation, string mode, string typeName, Guid id, string? parentType, Guid? parentId, string? collection, string patch, bool apply, string? token, CancellationToken cancellationToken, JsonObject? expected = null, Guid? auditId = null, string? expectedFingerprint = null)
    {
        var actor = await AdminChangeExecution.RequireAsync(executor, settings, AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
        using var gate = apply ? await confirmations.Gate.LockAsync(cancellationToken).ConfigureAwait(false) : null;
        using var context = persistence.CreateNewContext();
        if (apply && context is IConfigurationEditContext editContext)
        {
            await editContext.BeginConfigurationEditAsync(cancellationToken).ConfigureAwait(false);
        }

        var configId = (await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).GetId();
        var config = await context.GetByIdAsync<GameConfiguration>(configId, cancellationToken).ConfigureAwait(false)
            ?? throw new AdminOperationException("The active game configuration could not be loaded.");
        object? parent = null;
        PropertyInfo? collectionProperty = null;
        Type type;
        if (parentType is not null && parentId is { } ownerId && collection is not null)
        {
            var ownerType = ConfigurationPatch.Resolve(parentType);
            parent = Find(config, ownerType, ownerId);
            (collectionProperty, type) = Collection(ownerType, collection);
        }
        else
        {
            type = ConfigurationPatch.Resolve(typeName);
        }

        if (mode == "create" && id != Guid.Empty && await context.GetByIdAsync(id, type, cancellationToken).ConfigureAwait(false) is not null)
        {
            throw new AdminOperationException("The deleted object's identifier already exists. Revert refused.");
        }

        var item = mode == "create" ? null : type == typeof(GameServerDefinition)
            ? await context.GetByIdAsync<GameServerDefinition>(id, cancellationToken).ConfigureAwait(false) : Find(config, type, id);
        if ((mode != "create" && item is null) || (item is GameServerDefinition server && server.GameConfiguration?.GetId() != configId))
        {
            throw new AdminOperationException("The object is not part of the active game configuration.", AdminOperationErrorKind.InvalidInput);
        }

        if (mode == "delete" && (collectionProperty?.GetValue(parent) is not IEnumerable children || !children.Cast<object>().Contains(item)))
        {
            throw new AdminOperationException($"The object is not a child of the specified {nameof(collection)}.", AdminOperationErrorKind.InvalidInput);
        }

        var before = item is null ? null : ConfigurationPatch.Snapshot(item, type);
        if ((expected is not null && expected.Any(pair => !JsonNode.DeepEquals(before?[pair.Key], pair.Value)))
            || (expectedFingerprint is not null && expectedFingerprint != Fingerprint(before, item, type)))
        {
            throw new AdminOperationException("The object changed after the recorded change. Revert refused; inspect the current configuration.");
        }

        var values = mode == "delete" ? new Dictionary<PropertyInfo, object?>() : await ConfigurationPatch.ConvertAsync(type, ConfigurationPatch.Parse(patch), context, cancellationToken).ConfigureAwait(false);
        ValidateReferences(config, values);
        var after = mode == "delete" ? null : ConfigurationPatch.Snapshot(Candidate(item, type, values), type);
        if (after is not null)
        {
            foreach (var property in ConfigurationPatch.Fields(type).Where(ConfigurationPatch.IsReferenceCollection))
            {
                after[property.Name] = values.TryGetValue(property, out var proposed)
                    ? new JsonArray(((List<object>)proposed!).OrderBy(v => v.GetId()).Select(v => (JsonNode)new JsonObject { ["$ref"] = v.GetId().ToString() }).ToArray())
                    : before?[property.Name]?.DeepClone() ?? new JsonArray();
            }
        }

        if (mode == "delete")
        {
            CheckDeletion(config, item!, parent!, collectionProperty!, type);
        }

        var change = new JsonObject
        {
            ["kind"] = mode, ["type"] = type.Name, ["id"] = id.ToString(),
            ["parentType"] = parentType, ["parentId"] = parentId?.ToString(), ["collection"] = collection,
            ["beforeFingerprint"] = Fingerprint(before, item, type), ["afterFingerprint"] = Fingerprint(after, item, type),
            ["before"] = mode == "update" ? SelectFields(before!, values.Keys) : before,
            ["after"] = mode == "update" ? SelectFields(after!, values.Keys) : after,
        };
        if (change.ToJsonString().Length > 6500)
        {
            throw new AdminOperationException("The configuration diff is too large for durable audit. Use a smaller object.", AdminOperationErrorKind.InvalidInput);
        }

        return await AdminChangeExecution.RunAsync(
            executor,
            confirmations,
            actor,
            AdminRoles.Administrator,
            operation,
            new { mode, typeName, id, parentType, parentId, collection, patch, auditId },
            change,
            apply,
            token,
            async () =>
            {
                if (mode == "delete")
                {
                    collectionProperty!.PropertyType.GetMethod("Remove")!.Invoke(collectionProperty.GetValue(parent), [item]);
                    var deletion = (ValueTask<bool>)typeof(IContext).GetMethod(nameof(IContext.DeleteAsync))!.MakeGenericMethod(type).Invoke(context, [item])!;
                    if (!await deletion.ConfigureAwait(false))
                    {
                        throw new AdminOperationException("The object could not be deleted.");
                    }
                }
                else
                {
                    var target = item ?? (id == Guid.Empty ? context.CreateNew(type) : context.CreateNewWithId(type, id));
                    ConfigurationPatch.Apply(target, values);

                    if (item is null)
                    {
                        collectionProperty!.PropertyType.GetMethod("Add")!.Invoke(collectionProperty.GetValue(parent), [target]);
                        change["id"] = target.GetId().ToString();
                    }
                }

                await AdminChangeExecution.SaveAsync(context, cancellationToken).ConfigureAwait(false);
                await configurations.InvalidateAsync().ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
    }
}
