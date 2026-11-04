// <copyright file="StandardizeEventEnterDurationPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update raises the entrance duration of all events with a shared entrance to the five minutes which every event entrance announces, for Season 6 databases.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.StandardizeEventEnterDurationPlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.StandardizeEventEnterDurationPlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("0B9DAFCB-8DE4-437C-8CC3-BDE44229955D")]
public class StandardizeEventEnterDurationPlugInSeason6 : StandardizeEventEnterDurationPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
