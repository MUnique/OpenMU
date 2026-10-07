// <copyright file="CharacterCardDeliveryPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Delivers a character card of the cash shop by unlocking its character class for the account.
/// </summary>
/// <remarks>
/// A card can't be used when the class is already unlocked, e.g. by a plugin which unlocks it at a
/// certain level; it stays in the storage then.
/// </remarks>
public abstract class CharacterCardDeliveryPlugInBase : ICashShopProductDeliveryPlugIn
{
    private readonly byte _classNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterCardDeliveryPlugInBase"/> class.
    /// </summary>
    /// <param name="card">The item of the character card.</param>
    /// <param name="classNumber">The number of the character class which the card unlocks.</param>
    protected CharacterCardDeliveryPlugInBase(ItemIdentifier card, byte classNumber)
    {
        this.Key = card;
        this._classNumber = classNumber;
    }

    /// <inheritdoc />
    public ItemIdentifier Key { get; }

    /// <inheritdoc />
    public bool CanDeliver(CashShopProduct product) => product.Duration == TimeSpan.Zero;

    /// <inheritdoc />
    public ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product)
    {
        if (player.Account is not { } account
            || account.UnlockedCharacterClasses.Any(c => c.Number == this._classNumber))
        {
            return ValueTask.FromResult(CashShopUseResult.CannotUse);
        }

        if (this.GetCharacterClass(player) is not { } characterClass)
        {
            player.Logger.LogWarning("The character class {classNumber} of the character card {card} isn't configured.", this._classNumber, this.Key);
            return ValueTask.FromResult(CashShopUseResult.CannotUse);
        }

        account.UnlockedCharacterClasses.Add(characterClass);
        return ValueTask.FromResult(CashShopUseResult.Success);
    }

    /// <inheritdoc />
    public ValueTask DeliveredAsync(Player player, CashShopProduct product)
    {
        var className = this.GetCharacterClass(player)?.Name.GetTranslation(player.Culture);
        return player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CashShopCharacterClassUnlocked), className);
    }

    private CharacterClass? GetCharacterClass(Player player)
    {
        return player.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.Number == this._classNumber);
    }
}
