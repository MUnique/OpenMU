// <copyright file="AddConfigurationNameTranslationsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds configuration name translations for 075 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddConfigurationNameTranslations_Name), Description = nameof(PlugInResources.AddConfigurationNameTranslations_Description), ResourceType = typeof(PlugInResources))]
[Guid("d4498a46-f81e-4eae-b7a9-fedbe6bf3589")]
public class AddConfigurationNameTranslationsPlugIn075 : AddConfigurationNameTranslationsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
