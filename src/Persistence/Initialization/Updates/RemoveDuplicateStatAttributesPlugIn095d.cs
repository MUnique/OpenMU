// <copyright file="RemoveDuplicateStatAttributesPlugIn095d.cs" company="MUnique">
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
[Guid("D1F4C7A8-3B62-4E05-8A9C-5C1B6E3F7D24")]
public class RemoveDuplicateStatAttributesPlugIn095D : RemoveDuplicateStatAttributesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RemoveDuplicateStatAttributes095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
