// <copyright file="ItemDurabilityRefactorPlugIn095d.cs" company="MUnique">
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
[Guid("B6E1C4A8-2F9D-47C3-A5B7-1E8D6F2A9C40")]
public class ItemDurabilityRefactorPlugIn095D : ItemDurabilityRefactorPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ItemDurabilityRefactor095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}
