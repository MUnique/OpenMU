// -----------------------------------------------------------------------
// <copyright file="ShieldPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Consume handler for shield potions, which recover the <see cref="Stats.CurrentShield"/>.
/// </summary>
public abstract class ShieldPotionConsumeHandler : RecoverConsumeHandler, IItemConsumeHandler
{
    /// <inheritdoc/>
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumShield;

    /// <inheritdoc/>
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentShield;
}