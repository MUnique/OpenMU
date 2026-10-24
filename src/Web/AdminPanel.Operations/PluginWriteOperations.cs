// <copyright file="PluginWriteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.PlugIns;

/// <summary>Changes installed plugins with preview, audit and strict custom-field validation.</summary>
/// <param name="persistence">The persistence provider.</param>
/// <param name="configurations">The active configuration source.</param>
/// <param name="plugins">The installed plugin catalog.</param>
/// <param name="executor">The audit executor.</param>
/// <param name="confirmations">The confirmation coordinator.</param>
/// <param name="settings">The write policy.</param>
public sealed class PluginWriteOperations(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations, PlugInManager plugins, AdminOperationExecutor executor, AdminChangeConfirmation confirmations, AdminWriteSettings settings)
{
    /// <summary>Previews or changes an installed plugin's active state.</summary>
    /// <param name="pluginId">The plugin type identifier from plugins_list.</param>
    /// <param name="active">The requested active state.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview or applied result.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public ValueTask<JsonObject> SetActiveAsync(Guid pluginId, bool active, bool apply, string? confirmationToken, CancellationToken cancellationToken)
        => this.ChangeAsync(pluginId, active, null, apply, confirmationToken, cancellationToken);

    /// <summary>Merge-patches safe scalar custom settings; secret fields, source and external assemblies are excluded.</summary>
    /// <param name="pluginId">The installed plugin type identifier.</param>
    /// <param name="patch">The JSON merge patch with exact custom configuration property names.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A safe diff or applied result, never the full custom configuration.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    public ValueTask<JsonObject> UpdateConfigurationAsync(Guid pluginId, string patch, bool apply, string? confirmationToken, CancellationToken cancellationToken)
        => this.ChangeAsync(pluginId, null, patch, apply, confirmationToken, cancellationToken);

    /// <summary>Saves the panel's typed graphical custom editor without recording sensitive settings.</summary>
    /// <param name="pluginId">The installed plugin type identifier.</param>
    /// <param name="configuration">The typed editor model, including nested or sensitive fields.</param>
    /// <param name="serializedConfiguration">The editor's serialized configuration with its existing reference handling.</param>
    /// <param name="apply">Whether to apply the confirmed editor submission.</param>
    /// <param name="confirmationToken">The preview token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe preview or result, without custom field values.</returns>
    [AdminOperation(AdminRole.Administrator, ReadOnly = false)]
    [NotExposedToAgents("The graphical editor includes nested settings and credentials. Agents use plugins_update_configuration with a restricted scalar patch.")]
    public ValueTask<JsonObject> ReplaceFromPanelAsync(Guid pluginId, object configuration, string serializedConfiguration, bool apply, string? confirmationToken, CancellationToken cancellationToken)
    {
        var type = plugins.KnownPlugInTypes.SingleOrDefault(t => t.GUID == pluginId)?.GetCustomConfigurationType();
        if (type is null || !type.IsInstanceOfType(configuration) || serializedConfiguration.Length > 65536)
        {
            throw new AdminOperationException("The editor model does not match the installed plugin or exceeds the size limit.", AdminOperationErrorKind.InvalidInput);
        }

        ConfigurationPatch.Validate(configuration);
        return this.ChangeAsync(pluginId, null, null, apply, confirmationToken, cancellationToken, panelConfiguration: serializedConfiguration);
    }

    /// <summary>Reverses a safe plugin diff under the same preview and state checks.</summary>
    /// <param name="entry">The durable audit entry to reverse.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="token">The single-use confirmation token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal ValueTask<JsonObject> RevertFromAuditAsync(AdminAuditEntry entry, bool apply, string? token, CancellationToken cancellationToken)
    {
        var change = JsonNode.Parse(entry.ResultJson)?["change"] as JsonObject;
        if (change?["revertAvailable"]?.GetValue<bool>() is not true || change["before"] is not JsonObject before || !Guid.TryParse(change["pluginId"]?.GetValue<string>(), out var id)
            || change["afterStateFingerprint"]?.GetValue<string>() is not { } expected)
        {
            throw new AdminOperationException("The audit entry has no complete reversible plugin diff.");
        }

        var patch = before["settings"] is JsonObject fields ? fields.ToJsonString() : null;
        return this.ChangeAsync(id, patch is null ? before["active"]!.GetValue<bool>() : null, patch, apply, token, cancellationToken, "audit_revert", expected, entry.Id);
    }

    private static string Fingerprint(bool active, string? configuration)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { active, configuration }))));

    private async ValueTask<JsonObject> ChangeAsync(Guid pluginId, bool? active, string? patch, bool apply, string? token, CancellationToken cancellationToken, string? operationOverride = null, string? expectedFingerprint = null, Guid? revertedAuditId = null, string? panelConfiguration = null)
    {
        var actor = await AdminChangeExecution.RequireAsync(executor, settings, AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
        if (panelConfiguration is not null && actor.Frontend != "panel")
        {
            throw new AdminOperationException("The graphical configuration editor requires an authenticated panel circuit.", AdminOperationErrorKind.Forbidden);
        }

        using var gate = apply ? await confirmations.Gate.LockAsync(cancellationToken).ConfigureAwait(false) : null;
        var pluginType = plugins.KnownPlugInTypes.FirstOrDefault(t => t.GUID == pluginId)
            ?? throw new AdminOperationException("Plugin not found in the installed plugin catalog.", AdminOperationErrorKind.InvalidInput);
        var configId = (await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).GetId();
        using var context = persistence.CreateNewContext();
        if (apply && context is IConfigurationEditContext editContext)
        {
            await editContext.BeginConfigurationEditAsync(cancellationToken).ConfigureAwait(false);
        }

        var config = await context.GetByIdAsync<GameConfiguration>(configId, cancellationToken).ConfigureAwait(false)
            ?? throw new AdminOperationException("The active configuration could not be loaded.");
        var matches = config.PlugInConfigurations.Where(p => p.TypeId == pluginId).ToArray();
        if (matches.Length != 1)
        {
            throw new AdminOperationException("Expected one installed plugin configuration. Initialize the plugin in the panel first.");
        }

        var item = matches[0];
        string? newJson = panelConfiguration;
        var before = new JsonObject { ["active"] = item.IsActive };
        var after = new JsonObject { ["active"] = active ?? item.IsActive };
        var stringChange = false;
        if (patch is not null)
        {
            var type = pluginType.GetCustomConfigurationType()
                ?? throw new AdminOperationException("This plugin has no custom configuration.", AdminOperationErrorKind.InvalidInput);
            var requested = ConfigurationPatch.Parse(patch);
            if (requested.Any(p => ConfigurationPatch.Fields(type, pluginSettings: true).All(f => f.Name != p.Key || !ConfigurationPatch.IsScalar(f.PropertyType))))
            {
                throw new AdminOperationException("Only non-secret scalar plugin settings are supported. Use the panel for nested or sensitive configuration.", AdminOperationErrorKind.InvalidInput);
            }

            JsonObject current;
            try
            {
                current = item.CustomConfiguration is null ? JsonSerializer.SerializeToNode(Activator.CreateInstance(type), type, ConfigurationPatch.Options) as JsonObject ?? new JsonObject()
                    : JsonNode.Parse(item.CustomConfiguration) as JsonObject ?? throw new JsonException();
            }
            catch (JsonException)
            {
                throw new AdminOperationException("The existing custom configuration is invalid. Repair it in the panel first.");
            }

            var values = await ConfigurationPatch.ConvertAsync(type, requested, context, cancellationToken, pluginSettings: true).ConfigureAwait(false);
            var changedBefore = new JsonObject();
            var changedAfter = new JsonObject();
            foreach (var (property, value) in values)
            {
                if (property.PropertyType == typeof(string))
                {
                    // Even unmarked third-party strings may contain credentials. Never expose or retain their values.
                    stringChange = true;
                    changedBefore[property.Name] = new JsonObject { ["hidden"] = true };
                    changedAfter[property.Name] = new JsonObject { ["changed"] = true };
                }
                else
                {
                    changedBefore[property.Name] = current.ContainsKey(property.Name) ? current[property.Name]?.DeepClone()
                        : JsonSerializer.SerializeToNode(property.GetValue(current.Deserialize(type, ConfigurationPatch.Options)), property.PropertyType, ConfigurationPatch.Options);
                    changedAfter[property.Name] = JsonSerializer.SerializeToNode(value, property.PropertyType, ConfigurationPatch.Options);
                }

                current[property.Name] = JsonSerializer.SerializeToNode(value, property.PropertyType, ConfigurationPatch.Options);
            }

            try
            {
                ConfigurationPatch.Validate(current.Deserialize(type, ConfigurationPatch.Options) ?? throw new JsonException());
            }
            catch (JsonException)
            {
                throw new AdminOperationException("The proposed custom configuration does not match the plugin's configuration type.", AdminOperationErrorKind.InvalidInput);
            }

            before["settings"] = changedBefore;
            after["settings"] = changedAfter;
            newJson = current.ToJsonString();
        }

        // Bind all existing custom settings without exposing possibly secret values in a preview or audit.
        var fingerprint = Fingerprint(item.IsActive, item.CustomConfiguration);
        if (expectedFingerprint is not null && expectedFingerprint != fingerprint)
        {
            throw new AdminOperationException("The plugin changed after the audited operation. Revert refused.");
        }

        var change = new JsonObject
        {
            ["kind"] = panelConfiguration is null ? "plugin" : "pluginPanel",
            ["revertAvailable"] = panelConfiguration is null && !stringChange, ["pluginId"] = pluginId.ToString(), ["before"] = before, ["after"] = after,
            ["stateFingerprint"] = fingerprint, ["afterStateFingerprint"] = Fingerprint(active ?? item.IsActive, patch is null && panelConfiguration is null ? item.CustomConfiguration : newJson),
        };
        return await AdminChangeExecution.RunAsync(
            executor,
            confirmations,
            actor,
            AdminRoles.Administrator,
            operationOverride ?? (patch is null && panelConfiguration is null ? "plugins_set_active" : "plugins_update_configuration"),
            new { pluginId, active, patchFingerprint = patch is null ? null : Fingerprint(false, patch), revertedAuditId },
            change,
            apply,
            token,
            async () =>
            {
                if (active is { } requestedActive)
                {
                    item.IsActive = requestedActive;
                }
                else
                {
                    item.CustomConfiguration = newJson;
                }

                await AdminChangeExecution.SaveAsync(context, cancellationToken).ConfigureAwait(false);
                await configurations.InvalidateAsync().ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
    }
}
