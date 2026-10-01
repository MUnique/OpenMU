// <copyright file="AddConfigurationNameTranslationsPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds configuration name translations for Season6 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddConfigurationNameTranslations_Name), Description = nameof(PlugInResources.AddConfigurationNameTranslations_Description), ResourceType = typeof(PlugInResources))]
[Guid("77967be7-1f43-4162-8faf-6a2cb83de0fa")]
public class AddConfigurationNameTranslationsPlugInSeason6 : AddConfigurationNameTranslationsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
