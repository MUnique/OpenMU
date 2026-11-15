// <copyright file="EnergyResetFruitConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the reset fruit of energy.
/// </summary>
[Guid("076C0B3A-3722-41EC-B2BF-1791F2FC41B7")]
[PlugIn]
[Display(Name = nameof(PlugInResources.EnergyResetFruitConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.EnergyResetFruitConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class EnergyResetFruitConsumeHandlerPlugIn : ResetFruitConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.EnergyResetFruit;

    /// <inheritdoc />
    protected override AttributeDefinition StatAttribute => Stats.BaseEnergy;
}
