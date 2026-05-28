// <copyright file="PlugInConfigurationChangeApplier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Text.Json;

/// <summary>
/// Applies configuration changes to a local <see cref="PlugInManager" />.
/// </summary>
public static class PlugInConfigurationChangeApplier
{
    /// <summary>
    /// Applies a changed configuration to the local <see cref="PlugInManager" />, if it is a plugin configuration.
    /// </summary>
    /// <param name="plugInManager">The plugin manager.</param>
    /// <param name="type">The changed configuration type.</param>
    /// <param name="id">The changed configuration identifier.</param>
    /// <param name="configuration">The changed configuration.</param>
    /// <returns><c>True</c>, if the change was applied; Otherwise, <c>false</c>.</returns>
    public static bool ApplyChangedConfiguration(this PlugInManager plugInManager, Type type, Guid id, object? configuration)
    {
        if (!TryGetPlugInConfiguration(type, configuration, out var plugInConfiguration))
        {
            return false;
        }

        var plugInTypeId = plugInConfiguration.TypeId == Guid.Empty
            ? id
            : plugInConfiguration.TypeId;
        plugInManager.ApplyChangedPlugInConfiguration(plugInTypeId, plugInConfiguration);
        return true;
    }

    /// <summary>
    /// Applies a removed configuration to the local <see cref="PlugInManager" />, if it is a plugin configuration.
    /// </summary>
    /// <param name="plugInManager">The plugin manager.</param>
    /// <param name="type">The removed configuration type.</param>
    /// <param name="id">The removed configuration identifier.</param>
    /// <returns><c>True</c>, if the change was applied; Otherwise, <c>false</c>.</returns>
    public static bool ApplyRemovedConfiguration(this PlugInManager plugInManager, Type type, Guid id)
    {
        if (!type.IsAssignableTo(typeof(PlugInConfiguration)))
        {
            return false;
        }

        plugInManager.DeactivatePlugIn(id);
        return true;
    }

    private static void ApplyChangedPlugInConfiguration(this PlugInManager plugInManager, Guid id, PlugInConfiguration plugInConfiguration)
    {
        var currentlyActive = plugInManager.IsPlugInActive(id);
        if (currentlyActive && !plugInConfiguration.IsActive)
        {
            plugInManager.DeactivatePlugIn(id);
        }
        else if (!currentlyActive && plugInConfiguration.IsActive)
        {
            plugInManager.ActivatePlugIn(id);
        }
        else
        {
            plugInManager.ConfigurePlugIn(id, plugInConfiguration);
        }
    }

    private static bool TryGetPlugInConfiguration(Type type, object? configuration, out PlugInConfiguration plugInConfiguration)
    {
        if (configuration is PlugInConfiguration typedConfiguration)
        {
            plugInConfiguration = typedConfiguration;
            return true;
        }

        if (type.IsAssignableTo(typeof(PlugInConfiguration)) && configuration is JsonElement jsonElement)
        {
            var deserialized = jsonElement.Deserialize<PlugInConfiguration>();
            if (deserialized is not null)
            {
                plugInConfiguration = deserialized;
                return true;
            }
        }

        plugInConfiguration = null!;
        return false;
    }
}
