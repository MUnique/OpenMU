// <copyright file="ItemDurabilityRefactorPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update resets some game configuration values which are used for item durability reduction.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("4D8A2F91-C7E3-46B5-9A1D-8F2C6E7B3A50")]
public class ItemDurabilityRefactorPlugIn075 : ItemDurabilityRefactorPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ItemDurabilityRefactor075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}
