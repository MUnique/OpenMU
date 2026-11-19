// <copyright file="AddSocketCountTablesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the tables of the number of sockets of dropped socket items and assigns them to the socket items,
/// so that one socket is common and the maximum is rare, instead of a uniform selection.
/// It also raises the maximum number of sockets of the socket items to five, which only the tables restrict for drops.
/// </summary>
/// <remarks>
/// It's optional, because it changes how often socket items drop with more sockets.
/// Socket items which already have a socket count table keep it.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSocketCountTablesUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSocketCountTablesUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6A761545-2497-45A3-9E8B-F73A2B27D186")]
public class AddSocketCountTablesUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.AddSocketCountTablesUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.AddSocketCountTablesUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var (oneHandedTable, twoHandedTable) = SocketCountTables.GetOrCreate(context, gameConfiguration);
        foreach (var socketItem in gameConfiguration.Items.Where(item => item.MaximumSockets > 0 && item.SocketCountTable is null))
        {
            socketItem.MaximumSockets = Math.Max(socketItem.MaximumSockets, SocketCountTables.MaximumSocketCount);
            socketItem.SocketCountTable = SocketCountTables.IsTwoHandedWeapon(socketItem) ? twoHandedTable : oneHandedTable;
        }

        return ValueTask.CompletedTask;
    }
}
