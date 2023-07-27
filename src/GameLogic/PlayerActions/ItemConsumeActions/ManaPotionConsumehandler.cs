// -----------------------------------------------------------------------
// <copyright file="ManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Consume handler for potions which refills the players attribute <see cref="Stats.CurrentMana"/>.
/// </summary>
public abstract class ManaPotionConsumeHandler : RecoverConsumeHandlerPlugIn.ManaHealthConsumeHandlerPlugIn, IItemConsumeHandlerPlugIn
{
    /// <inheritdoc/>
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumMana;

    /// <inheritdoc/>
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentMana;

    /// <inheritdoc />
    protected override async ValueTask OnAfterRecoverAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
    }
}