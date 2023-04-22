// <copyright file="ComplexPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Abstract consume handler for complex potions which combines a <see cref="HealthPotionConsumeHandlerPlugIn"/> and a <see cref="ShieldPotionConsumeHandlerPlugIn"/>.
/// </summary>
public abstract class ComplexPotionConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    private readonly HealthPotionConsumeHandlerPlugIn _healthPotionConsumeHandlerPlugIn;
    private readonly ShieldPotionConsumeHandlerPlugIn _shieldPotionConsumeHandlerPlugIn;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexPotionConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="healthPotionConsumeHandlerPlugIn">The health potion consume handler.</param>
    /// <param name="shieldPotionConsumeHandlerPlugIn">The shield potion consume handler.</param>
    protected ComplexPotionConsumeHandlerPlugIn(HealthPotionConsumeHandlerPlugIn healthPotionConsumeHandlerPlugIn, ShieldPotionConsumeHandlerPlugIn shieldPotionConsumeHandlerPlugIn)
    {
        this._healthPotionConsumeHandlerPlugIn = healthPotionConsumeHandlerPlugIn ?? throw new ArgumentNullException(nameof(healthPotionConsumeHandlerPlugIn));
        this._shieldPotionConsumeHandlerPlugIn = shieldPotionConsumeHandlerPlugIn ?? throw new ArgumentNullException(nameof(shieldPotionConsumeHandlerPlugIn));
    }

    /// <inheritdoc />
    protected override bool CheckPreconditions(Player player, Item item)
    {
        return base.CheckPreconditions(player, item)
               && player.PotionCooldownUntil <= DateTime.UtcNow;
    }

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            await this._healthPotionConsumeHandlerPlugIn.RecoverAsync(player, item).ConfigureAwait(false);
            await this._shieldPotionConsumeHandlerPlugIn.RecoverAsync(player, item).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}