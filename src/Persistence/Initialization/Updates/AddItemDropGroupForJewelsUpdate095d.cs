// <copyright file="AddItemDropGroupForJewelsUpdate095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update creates a specific item drop group for jewels with a default chance of 5%.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("D21056E6-E912-416B-A076-3C2D17DA517B")]
public class AddItemDropGroupForJewelsUpdate095D : AddItemDropGroupForJewelsUpdate075
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddItemDropGroupForJewels095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.AddJewelToItemDrop(gameConfiguration, 4, null, "Jewel of Life");
    }
}