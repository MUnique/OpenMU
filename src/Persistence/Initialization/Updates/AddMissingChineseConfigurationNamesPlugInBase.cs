// <copyright file="AddMissingChineseConfigurationNamesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Adds Chinese configuration names to existing configurations without changing identifiers.
/// </summary>
public abstract class AddMissingChineseConfigurationNamesPlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.ResourceManager.GetString(nameof(PlugInResources.AddMissingChineseConfigurationNames_Name), System.Globalization.CultureInfo.InvariantCulture)!;

    /// <inheritdoc />
    public override string Description => PlugInResources.ResourceManager.GetString(nameof(PlugInResources.AddMissingChineseConfigurationNames_Description), System.Globalization.CultureInfo.InvariantCulture)!;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        CharacterClasses.ChineseCharacterClassNames.AddMissingTranslations(gameConfiguration);
        ChineseMerchantNames.AddMissingTranslations(gameConfiguration);
        ChineseMonsterNames.AddMissingTranslations(gameConfiguration);
        ChineseMapNames.AddMissingTranslations(gameConfiguration);
        return default;
    }
}
