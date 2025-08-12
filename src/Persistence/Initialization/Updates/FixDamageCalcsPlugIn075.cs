// <copyright file="FixDamageCalcsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes character stats, magic effects, items, and options related to damage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("42B1582B-667F-4098-A339-DDA8560157E3")]
public class FixDamageCalcsPlugIn075 : FixDamageCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDamageCalcs075;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.UpdateWeaponItems(context, gameConfiguration);
    }
}