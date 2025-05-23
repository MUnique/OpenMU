// <copyright file="FixDefenseCalcsPlugIn075.cs" company="MUnique">
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
[Guid("683A8F8F-EFE9-4EF4-B536-21048E195A87")]
public class FixDefenseCalcsPlugIn075 : FixDefenseCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDefenseCalcs075;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}