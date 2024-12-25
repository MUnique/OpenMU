// <copyright file="FixChaosMixesUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the Chaos Weapon crafting settings.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("04A5F236-117F-422A-8C38-28D09DE911D7")]
public class FixChaosMixesUpdatePlugIn075 : FixChaosMixesPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixChaosMixes075;
}