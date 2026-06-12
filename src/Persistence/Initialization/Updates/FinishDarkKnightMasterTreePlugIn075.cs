// <copyright file="FinishDarkKnightMasterTreePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update completes the dark knight master tree skills and effects. It also fixes the double wield damage calculations.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("B8F3E2C1-4D5A-6F78-9B0C-2E7D1A3F5B6C")]
public class FinishDarkKnightMasterTreePlugIn075 : FinishDarkKnightMasterTreePlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishDarkKnightMasterTree075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}
