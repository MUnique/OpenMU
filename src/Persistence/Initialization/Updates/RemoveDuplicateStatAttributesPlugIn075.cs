// <copyright file="RemoveDuplicateStatAttributesPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update removes stat attributes which are defined more than once for a character class.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("6B0A9D2C-6E4F-4C1B-9B5A-2E7F8D4C0A31")]
public class RemoveDuplicateStatAttributesPlugIn075 : RemoveDuplicateStatAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RemoveDuplicateStatAttributes075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
