// <copyright file="FixMiniGameZenDropsUpdatePlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update configures the Devil Square maps of version 0.95d to drop no zen.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Name), Description = nameof(PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FB4DD55E-68F4-4295-929F-35E4926C8353")]
public class FixMiniGameZenDropsUpdatePlugIn095D : FixMiniGameZenDropsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    protected override IReadOnlyCollection<short> MapNumbersWithoutZen => [Version095d.Maps.DevilSquare1.Number];
}
