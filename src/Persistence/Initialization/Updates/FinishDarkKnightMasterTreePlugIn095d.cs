// <copyright file="FinishDarkKnightMasterTreePlugIn095d.cs" company="MUnique">
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
[Guid("D4F7A9C2-1B3E-56D8-9F0A-7C2E4B1D5A8F")]
public class FinishDarkKnightMasterTreePlugIn095D : FinishDarkKnightMasterTreePlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishDarkKnightMasterTree095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
