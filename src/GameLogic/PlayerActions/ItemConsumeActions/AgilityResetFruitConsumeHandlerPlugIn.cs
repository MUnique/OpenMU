// <copyright file="AgilityResetFruitConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the reset fruit of agility.
/// </summary>
[Guid("9FEE1625-ECCA-4749-8ED9-342C04C5CE3A")]
[PlugIn]
[Display(Name = nameof(PlugInResources.AgilityResetFruitConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.AgilityResetFruitConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class AgilityResetFruitConsumeHandlerPlugIn : ResetFruitConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.AgilityResetFruit;

    /// <inheritdoc />
    protected override AttributeDefinition StatAttribute => Stats.BaseAgility;
}
