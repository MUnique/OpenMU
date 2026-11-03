// <copyright file="StrengthResetFruitConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the reset fruit of strength.
/// </summary>
[Guid("7E4D3D06-FBCC-49EA-BAE4-4768DBE5CB41")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StrengthResetFruitConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.StrengthResetFruitConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class StrengthResetFruitConsumeHandlerPlugIn : ResetFruitConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.StrengthResetFruit;

    /// <inheritdoc />
    protected override AttributeDefinition StatAttribute => Stats.BaseStrength;
}
