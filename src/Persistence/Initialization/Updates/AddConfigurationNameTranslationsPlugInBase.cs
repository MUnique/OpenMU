// <copyright file="AddConfigurationNameTranslationsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Adds configuration name translations to existing configurations without changing identifiers.
/// </summary>
public abstract class AddConfigurationNameTranslationsPlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.ResourceManager.GetString(nameof(PlugInResources.AddConfigurationNameTranslations_Name), System.Globalization.CultureInfo.InvariantCulture)!;

    /// <inheritdoc />
    public override string Description => PlugInResources.ResourceManager.GetString(nameof(PlugInResources.AddConfigurationNameTranslations_Description), System.Globalization.CultureInfo.InvariantCulture)!;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ConfigurationNameTranslations.Apply(gameConfiguration);
        return default;
    }
}
