// <copyright file="ConfigurationNameSources.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Registers the resources of the built-in configuration names as sources of <see cref="LocalizedString"/>s,
/// so that their <see cref="LocalizedString.SourceKey"/>s (e.g. "MapNames/Lorencia") can be resolved,
/// even if no data initialization was executed in the current process.
/// </summary>
public static class ConfigurationNameSources
{
    /// <summary>
    /// Registers the resources. It's called automatically when this assembly is loaded; calling it again has no effect.
    /// </summary>
    public static void Register()
    {
        RegisterIfNeeded(nameof(CharacterClassNames), CharacterClassNames.ResourceManager);
        RegisterIfNeeded(nameof(MapNames), MapNames.ResourceManager);
        RegisterIfNeeded(nameof(MerchantNames), MerchantNames.ResourceManager);
        RegisterIfNeeded(nameof(MonsterNames), MonsterNames.ResourceManager);
        RegisterIfNeeded(nameof(ItemNames), ItemNames.ResourceManager);
        RegisterIfNeeded(nameof(MiniGameNames), MiniGameNames.ResourceManager);
        RegisterIfNeeded(nameof(MiniGameDescriptions), MiniGameDescriptions.ResourceManager);
        RegisterIfNeeded(nameof(SkillNames), SkillNames.ResourceManager);
        RegisterIfNeeded(nameof(ItemOptionNames), ItemOptionNames.ResourceManager);
        RegisterIfNeeded(nameof(ItemOptionTypeNames), ItemOptionTypeNames.ResourceManager);
        RegisterIfNeeded(nameof(ItemOptionDescriptions), ItemOptionDescriptions.ResourceManager);
        RegisterIfNeeded(nameof(ItemSetNames), ItemSetNames.ResourceManager);
        RegisterIfNeeded(nameof(ArmorSetNames), ArmorSetNames.ResourceManager);
    }

    /// <summary>
    /// Registers the resources when the assembly is loaded.
    /// </summary>
#pragma warning disable CA2255 // The source keys must be resolvable as soon as the assembly is loaded, before any initialization is executed.
    [ModuleInitializer]
    internal static void RegisterOnLoad()
#pragma warning restore CA2255
    {
        Register();
    }

    private static void RegisterIfNeeded(string name, System.Resources.ResourceManager resourceManager)
    {
        if (!LocalizedStringResources.TryGetName(resourceManager, out _))
        {
            LocalizedStringResources.Register(name, resourceManager);
        }
    }
}
