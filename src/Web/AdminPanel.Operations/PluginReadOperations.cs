// <copyright file="PluginReadOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.PlugIns;

/// <summary>Reads plugin metadata without exposing custom configuration or source.</summary>
public sealed class PluginReadOperations(PlugInManager plugins)
{
    /// <summary>Lists known plugins with their active state and extension points.</summary>
    /// <param name="text">The optional name filter.</param>
    /// <param name="offset">The number of matches to skip.</param>
    /// <param name="limit">The maximum number of matches.</param>
    /// <returns>A bounded page of plugin metadata.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public JsonObject List(string text, int offset, int limit)
    {
        AdminReadResult.ValidatePage(offset, limit);
        var matches = plugins.KnownPlugInTypes.Select(t =>
        {
            var display = t.GetCustomAttributes(typeof(DisplayAttribute), inherit: true).OfType<DisplayAttribute>().FirstOrDefault();
            return new
            {
                id = t.GUID,
                name = display?.GetName() ?? t.Name,
                description = display?.GetDescription(),
                active = plugins.IsPlugInActive(t),
                pluginPoints = t.GetInterfaces().Where(i => i.IsDefined(typeof(PlugInPointAttribute), inherit: true)).Select(i => i.Name).ToArray(),
                hasCustomConfiguration = t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISupportCustomConfiguration<>)),
            };
        }).Where(p => p.name.Contains(text, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.name).ThenBy(p => p.id).ToList();
        return AdminReadResult.Page(matches.Skip(offset).Take(limit), matches.Count, offset, limit);
    }
}
