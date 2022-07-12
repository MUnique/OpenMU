// <copyright file="ComplexPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Abstract consume handler for complex potions which combines a <see cref="HealthPotionConsumeHandler"/> and a <see cref="ShieldPotionConsumeHandler"/>.
/// </summary>
public class ComplexPotionConsumeHandler : BaseConsumeHandler
{
    private readonly HealthPotionConsumeHandler _healthPotionConsumeHandler;
    private readonly ShieldPotionConsumeHandler _shieldPotionConsumeHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexPotionConsumeHandler"/> class.
    /// </summary>
    /// <param name="healthPotionConsumeHandler">The health potion consume handler.</param>
    /// <param name="shieldPotionConsumeHandler">The shield potion consume handler.</param>
    protected ComplexPotionConsumeHandler(HealthPotionConsumeHandler healthPotionConsumeHandler, ShieldPotionConsumeHandler shieldPotionConsumeHandler)
    {
        this._healthPotionConsumeHandler = healthPotionConsumeHandler ?? throw new ArgumentNullException(nameof(healthPotionConsumeHandler));
        this._shieldPotionConsumeHandler = shieldPotionConsumeHandler ?? throw new ArgumentNullException(nameof(shieldPotionConsumeHandler));
    }

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage))
        {
            this._healthPotionConsumeHandler.Recover(player);
            this._shieldPotionConsumeHandler.Recover(player);
            return true;
        }

        return false;
    }
}