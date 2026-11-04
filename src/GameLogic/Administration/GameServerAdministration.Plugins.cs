// <copyright file="GameServerAdministration.Plugins.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx;

/// <summary>The plugin contribution channel of live game server administration.</summary>
public sealed partial class GameServerAdministration
{
    private static readonly ConditionalWeakTable<IGameContext, AsyncLock> PluginGates = new();

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<AdminPluginOperationInfo>> GetAdminOperationsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<AdminPluginOperationInfo> result = context.PlugInManager.GetActivePlugInsOf<IAdminOperationPlugIn>()
            .Where(IsValidOperation).OrderBy(p => p.Key, StringComparer.Ordinal).Take(100).Select(p => p.Operation with { InputSchema = p.Operation.InputSchema.DeepClone().AsObject() }).ToArray();
        return ValueTask.FromResult(result);
    }

    /// <inheritdoc />
    public async ValueTask<JsonObject> ReadAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
    {
        var plugin = this.ResolvePlugin(plugInId, name);
        if (!plugin.Operation.ReadOnly)
        {
            throw new ArgumentException("A write operation cannot be invoked through the read channel.", nameof(name));
        }

        return CheckSize(await plugin.ExecuteAsync(context, ParseArguments(arguments), null, cancellationToken).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async ValueTask<JsonObject> PreviewAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
    {
        var plugin = this.ResolvePlugin(plugInId, name);
        if (plugin.Operation.ReadOnly)
        {
            throw new ArgumentException("Use the read channel for a read-only operation.", nameof(name));
        }

        return CheckSize(await plugin.PreviewAsync(context, ParseArguments(arguments), cancellationToken).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async ValueTask<JsonObject> ExecuteAdminOperationAsync(Guid plugInId, string name, string arguments, JsonObject expectedPreview, CancellationToken cancellationToken)
    {
        using var gate = await PluginGates.GetValue(context, _ => new AsyncLock()).LockAsync(cancellationToken).ConfigureAwait(false);
        var plugin = this.ResolvePlugin(plugInId, name);
        if (plugin.Operation.ReadOnly)
        {
            throw new ArgumentException("The operation metadata changed. Request a new preview.", nameof(name));
        }

        var input = ParseArguments(arguments);
        var preview = CheckSize(await plugin.PreviewAsync(context, input.DeepClone().AsObject(), cancellationToken).ConfigureAwait(false));
        if (!JsonNode.DeepEquals(preview, expectedPreview))
        {
            throw new ArgumentException("The plugin state changed. Request a new preview.", nameof(expectedPreview));
        }

        return CheckSize(await plugin.ExecuteAsync(context, input, expectedPreview.DeepClone().AsObject(), cancellationToken).ConfigureAwait(false));
    }

    private static JsonObject ParseArguments(string arguments)
    {
        if (arguments.Length > 16384)
        {
            throw new ArgumentException("Plugin input exceeds 16 KiB.", nameof(arguments));
        }

        try
        {
            return JsonNode.Parse(arguments, documentOptions: new JsonDocumentOptions { MaxDepth = 16 }) as JsonObject
                ?? throw new ArgumentException("Plugin input must be a JSON object.", nameof(arguments));
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Plugin input must be a JSON object of depth at most 16.", nameof(arguments), exception);
        }
    }

    private static JsonObject CheckSize(JsonObject result)
        => result.ToJsonString().Length <= 16384 ? result
            : throw new InvalidOperationException("The plugin result exceeds 16 KiB; inspect state before retrying a write.");

    private static bool IsValidOperation(IAdminOperationPlugIn plugin)
    {
        var info = plugin.Operation;
        return info.PlugInId == plugin.GetType().GUID && info.PlugInId != Guid.Empty && info.Name == plugin.Key
            && info.Name.Length is >= 3 and <= 100 && info.Name.All(c => c is >= 'a' and <= 'z' or '_')
            && info.Description.Length is > 0 and <= 2000 && info.InputSchema.ToJsonString().Length <= 16384
            && info.InputSchema["type"]?.ToString() == "object"
            && info.MinimumRole is "Viewer" or "Operator" or "Administrator"
            && (info.ReadOnly || info.MinimumRole is "Operator" or "Administrator");
    }

    private IAdminOperationPlugIn ResolvePlugin(Guid id, string name)
        => context.PlugInManager.GetActivePlugInsOf<IAdminOperationPlugIn>().SingleOrDefault(p => p.GetType().GUID == id && p.Key == name && IsValidOperation(p))
            ?? throw new ArgumentException("The operation is inactive, invalid or unavailable. Refresh plugin_operations_list.", nameof(name));
}
