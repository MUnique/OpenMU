// <copyright file="FixDefenseCalcsPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes character stats, magic effects, and options related to defense.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("C6945ADC-0313-47AC-AFAE-61D0544C8935")]
public class FixDefenseCalcsPlugIn095D : FixDefenseCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDefenseCalcs095d;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}