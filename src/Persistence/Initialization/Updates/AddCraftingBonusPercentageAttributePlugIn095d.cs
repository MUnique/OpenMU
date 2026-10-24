// <copyright file="AddCraftingBonusPercentageAttributePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the crafting bonus percentage attribute for version 0.95d.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9C6D1732-EA73-497C-9CD1-029ECEC644F4")]
public class AddCraftingBonusPercentageAttributePlugIn095d : AddCraftingBonusPercentageAttributePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
