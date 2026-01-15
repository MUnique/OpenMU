// -----------------------------------------------------------------------
// <copyright file="MediumManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for middle health potions.
/// </summary>
[Guid("EC1F3DDF-5AF1-455C-AE0C-11A8018AE7D4")]
[PlugIn]
[Display(Name = nameof(PlugInResources.MediumManaPotionConsumeHandler_Name), Description = nameof(PlugInResources.MediumManaPotionConsumeHandler_Description), ResourceType = typeof(PlugInResources))]
public class MediumManaPotionConsumeHandler : ManaPotionConsumeHandler
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.MediumManaPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 2;
}