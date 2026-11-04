// <copyright file="MoveServerPortsOutOfDynamicRangePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update moves the ports of the game servers and the chat server out of the dynamic port range
/// for Season 6.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MoveServerPortsOutOfDynamicRangePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MoveServerPortsOutOfDynamicRangePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("7EDE79F1-213C-4DC3-8846-824B3B8D1470")]
public class MoveServerPortsOutOfDynamicRangePlugInSeason6 : MoveServerPortsOutOfDynamicRangePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
